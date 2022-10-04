using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

namespace ChiliGames.VROffice
{
    //This class sends a Raycast from the marker and detect if it's hitting the whiteboard (tag: Finish)
    [RequireComponent(typeof(XRGrabbablePun))]
    public class Marker : MonoBehaviourPun
    {
        [SerializeField] private Transform drawingPoint;
        [SerializeField] private Renderer markerTip;
        public int penSize = 6;
        private int penSizeD2;

        public Color32 color = Color.blue;

        private Whiteboard whiteboard;
        private RaycastHit touch;
        private bool touching;
        private bool touchingLastFrame;
        private float drawingDistance = 0.015f;
        private Quaternion lastAngle;
        private XRGrabbablePun grabbable;
        private bool grabbed;
        private int currentFrame = 0;

        private float lastX;
        private float lastY;

        private int lastLerpX;
        private int lastLerpY;

        private void Awake()
        {
            grabbable = GetComponent<XRGrabbablePun>();
            penSizeD2 = penSize / 2;
        }

        private void Start()
        {
            //Subscribe to grabbed and dropped events
            grabbable.selectEntered.AddListener(MarkerGrabbed);
            grabbable.selectExited.AddListener(MarkerDropped);

            var block = new MaterialPropertyBlock();

            //Set the tip to color defined in inspector
            block.SetColor("_BaseColor", color);
            markerTip.SetPropertyBlock(block);
        }

        private void MarkerGrabbed(SelectEnterEventArgs arg0)
        {
            grabbed = true;
        }

        private void MarkerDropped(SelectExitEventArgs arg0)
        {
            grabbed = false;
        }

        void Update()
        {
            //Run raycast every 2 frames for performance
            /*currentFrame++;
            if (currentFrame % 2 != 0)
            {
                currentFrame = 0;
            }*/

            //if the marker is not in possesion of the user, or is not grabbed, we don't run update.
            if (!photonView.IsMine) return;
            if (!grabbed) return;

            //Cast a raycast to detect whiteboard.
            if (Physics.Raycast(drawingPoint.position, drawingPoint.up, out touch, drawingDistance))
            {
                //The whiteboard has the tag "Finish".
                if (touch.collider.CompareTag("Finish"))
                {
                    if (!touching)
                    {
                        touching = true;
                        //store angle so while drawing, marker doesn't rotate
                        lastAngle = transform.rotation;
                        whiteboard = touch.collider.GetComponent<Whiteboard>();
                    }
                    if (whiteboard == null) return;
                    //Save reference of marker ID, color and size the first time we touch the whiteboard
                    photonView.RPC("DrawAtPosition", RpcTarget.AllBuffered, whiteboard.photonView.ViewID, touch.textureCoord.x, touch.textureCoord.y);
                    //DrawAtPosition(whiteboard, touch.textureCoord.x, touch.textureCoord.y); //Updated-----------------------------
                }
            }
            else if (whiteboard != null)
            {
                touching = false;
                touchingLastFrame = false;
                whiteboard = null;
            }
        }
        [PunRPC] // Updated----------------------------
        //RPC sent by the Marker class so every user gets the information to draw in whiteboard.
        public void DrawAtPosition(int whiteBoardID, float _posX, float _posY) // public void DrawAtPosition(Whiteboard whiteBoard, float _posX, float _posY)
        {
            GameObject touchedWhiteboard = PhotonView.Find(whiteBoardID).gameObject;//-------
            Whiteboard whiteBoard = touchedWhiteboard.GetComponent<Whiteboard>(); //=---------

            int x = (int)((1 - _posX) * whiteBoard.whiteBoardSizeX - penSizeD2);
            int y = (int)((1 - _posY) * whiteBoard.whiteBoardSizeY - penSizeD2);

            //If last frame was not touching a marker, we don't need to lerp from last pixel coordinate to new, so we set the last coordinates to the new.
            if (!touchingLastFrame)
            {
                touchingLastFrame = true;
                whiteBoard.AddToQueue(photonView.ViewID, x, y);
            }
            else
            {
                if (lastLerpX == 0 && lastLerpY == 0)
                {
                    lastLerpX = x;
                    lastLerpY = y;
                }
                //Lerp last pixel to new pixel, so we draw a continuous line.
                for (float t = 0.1f; t < 1.00f; t += 0.1f)
                {
                    int lerpX = (int)Mathf.Lerp(lastX, (float)x, t);
                    int lerpY = (int)Mathf.Lerp(lastY, (float)y, t);

                    if (NotTooClose(penSizeD2, lerpX, lastLerpX, lerpY, lastLerpY))
                    {
                        whiteBoard.AddToQueue(photonView.ViewID, lerpX, lerpY);
                        lastLerpX = lerpX;
                        lastLerpY = lerpY;
                    }
                }

                if (NotTooClose(penSizeD2, x, (int)lastX, y, (int)lastY))
                {
                    whiteBoard.AddToQueue(photonView.ViewID, x, y);
                }
            }

            lastX = (float)x;
            lastY = (float)y;
        }

        private bool NotTooClose(int range, int x1, int x2, int y1, int y2)
        {
            var dx = x1 - x2;
            var dy = y1 - y2;
            return (dx * dx) + (dy * dy) > (range * range);
        }

        private void OnDestroy()
        {
            if (grabbable != null)
            {
                grabbable.selectEntered.RemoveListener(MarkerGrabbed);
                grabbable.selectExited.RemoveListener(MarkerDropped);
            }
        }

        private void LateUpdate()
        {
            if (!photonView.IsMine) return;

            //lock rotation of marker when touching whiteboard.
            if (touching)
            {
                transform.rotation = lastAngle;
            }
        }
    }
}