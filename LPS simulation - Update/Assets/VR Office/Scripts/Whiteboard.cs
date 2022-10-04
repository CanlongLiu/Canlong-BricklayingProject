using System.Linq;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

namespace ChiliGames.VROffice
{
    public class Whiteboard : MonoBehaviourPunCallbacks
    {
        [SerializeField] private int maxTextureSize = 2048;
        [HideInInspector] public int whiteBoardSizeX;
        [HideInInspector] public int whiteBoardSizeY;
        private Texture2D texture;
        private Color32[] deleteColor;
        private new Renderer renderer;

        [SerializeField] int dequeueRate = 3;

        private Queue<MarkerPositions> markerPositionQueue;

        private Dictionary<int, MarkerData> markerIDs = new Dictionary<int, MarkerData>();

        class MarkerData
        {
            public Color32[] color;
            public int pensize;
        }

        class MarkerPositions
        {
            public int markerID;
            public int posX;
            public int posY;
        }

        void Awake()
        {
            markerPositionQueue = new Queue<MarkerPositions>();

            //Dertermine whiteboard resolution from size.
            if (transform.localScale.x > transform.localScale.y)
            {
                float ratio = transform.localScale.x / transform.localScale.y;
                whiteBoardSizeX = maxTextureSize;
                whiteBoardSizeY = (int)(maxTextureSize / ratio);
            }
            else
            {
                float ratio = transform.localScale.y / transform.localScale.x;
                whiteBoardSizeY = maxTextureSize;
                whiteBoardSizeX = (int)(maxTextureSize / ratio);
            }

            //create whiteboard texture
            renderer = GetComponent<Renderer>();
            texture = new Texture2D(whiteBoardSizeX, whiteBoardSizeY, TextureFormat.RGB24, false);

            //Apply whiteboard texture
            renderer.material.mainTexture = texture;

            //Create white delete color
            deleteColor = Enumerable.Repeat(new Color32(255, 255, 255, 255),
                whiteBoardSizeX * whiteBoardSizeY).ToArray();

            //Set the whiteboard to white on awake
            RPC_ClearWhiteboard();
        }

        private void Start()
        {
            //Find and store references to every marker in scene for performance
            var markers = FindObjectsOfType<Marker>();
            foreach (var item in markers)
            {
                PhotonView pv = item.GetComponent<PhotonView>();
                markerIDs.Add(pv.ViewID, new MarkerData { pensize = item.penSize });
                markerIDs[pv.ViewID].color = CreateColorArray(new Color(item.color.r, item.color.g, item.color.b), pv.ViewID);
            }
        }

        private void Update()
        {
            for (int i = 0; i < dequeueRate; i++)
            {
                if (markerPositionQueue.Count > 0)
                {
                    var item = markerPositionQueue.Dequeue();
                    DrawAtPosition(item.markerID, item.posX, item.posY);
                }
                else
                {
                    return;
                }
            }
        }

        public void AddToQueue(int id, int _posX, int _posY)
        {
            Debug.Log("Add to queue");
            markerPositionQueue.Enqueue(new MarkerPositions { markerID = id, posX = _posX, posY = _posY });
        }

        private void DrawAtPosition(int id, int posX, int posY)
        {
            texture.SetPixels32(posX, posY, markerIDs[id].pensize, markerIDs[id].pensize, markerIDs[id].color);
            texture.Apply();
        }

        //To clear the whiteboard.
        public void ClearWhiteboard()
        {
            photonView.RPC("RPC_ClearWhiteboard", RpcTarget.AllBuffered);
        }

        public Color32[] CreateColorArray(Color32 color, int id)
        {
            return Enumerable.Repeat(new Color32(color.r, color.g, color.b, 255), markerIDs[id].pensize * markerIDs[id].pensize).ToArray();
        }

        [PunRPC]
        public void RPC_ClearWhiteboard()
        {
            texture.SetPixels32(deleteColor);
            texture.Apply();
        }
    }
}
