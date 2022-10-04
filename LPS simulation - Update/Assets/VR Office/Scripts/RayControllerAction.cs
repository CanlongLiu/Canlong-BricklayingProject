using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ChiliGames.VROffice
{
    public class RayControllerAction : MonoBehaviour
    {
        public ActionBasedController leftTeleportRay;
        public ActionBasedController rightTeleportRay;
        public float activationThreshold = 0.1f;
        public XRRayInteractor leftInteractorRay;
        public XRRayInteractor rightInteractorRay;
        public bool leftTeleportEnabled { get; set; } = true;
        public bool rightTeleportEnabled { get; set; } = true;
        private bool leftButtonPressedLastFrame = false;
        private bool rightButtonPressedLastFrame = false;
        public bool isLeftInteractorRayHovering;
        public bool isRightInteractorRayHovering;
        public GameObject leftTeleportReticle;
        public GameObject rightTeleportReticle;

        void Start()
        {
            InitializeTeleportRay(leftTeleportRay);
            InitializeTeleportRay(rightTeleportRay);
        }

        void InitializeTeleportRay(ActionBasedController teleportRay)
        {
            if (!teleportRay) { return; }

            teleportRay.gameObject.SetActive(false);
        }

        void Update()
        {
            Vector3 pos = new Vector3();
            Vector3 norm = new Vector3();
            int index = 0;
            bool validTarget = false;
            if (leftTeleportRay)
            {
                isLeftInteractorRayHovering = leftInteractorRay.TryGetHitInfo(out pos, out norm, out index, out validTarget);
                leftTeleportRay.gameObject.SetActive(!isLeftInteractorRayHovering && leftTeleportEnabled && leftButtonPressedLastFrame);
                leftTeleportReticle.gameObject.SetActive(!isLeftInteractorRayHovering && leftTeleportEnabled && leftButtonPressedLastFrame);
            }

            if (rightTeleportRay)
            {
                isRightInteractorRayHovering = rightInteractorRay.TryGetHitInfo(out pos, out norm, out index, out validTarget);
                rightTeleportRay.gameObject.SetActive(!isRightInteractorRayHovering && rightTeleportEnabled && rightButtonPressedLastFrame);
                rightTeleportReticle.gameObject.SetActive(!isLeftInteractorRayHovering && leftTeleportEnabled && leftButtonPressedLastFrame);
            }

            ManageTeleportRay(leftTeleportRay, ref leftButtonPressedLastFrame, leftTeleportReticle, leftTeleportEnabled);
            ManageTeleportRay(rightTeleportRay, ref rightButtonPressedLastFrame, rightTeleportReticle, rightTeleportEnabled);
        }

        void ManageTeleportRay(ActionBasedController teleportRay, ref bool buttonPressedLastFrame, GameObject teleportReticle, bool teleportEnabled)
        {
            if (!teleportRay) { return; }

            // get the state of the teleport button
            bool isPressed = false;

            float input = teleportRay.activateAction.action.ReadValue<float>();
            if (input > activationThreshold)
            {
                isPressed = true;
            }
            else
            {
                isPressed = false;
            }

            bool buttonJustPressed = isPressed && !buttonPressedLastFrame;
            bool buttonJustReleased = !isPressed && buttonPressedLastFrame;

            if (buttonJustPressed && teleportEnabled && !isLeftInteractorRayHovering && !isRightInteractorRayHovering)
            {
                teleportRay.gameObject.SetActive(true);
                // this stops the reticle from appearing by the player's feet for 1 frame every time the teleport ray was activated
                teleportReticle.SetActive(false);
            }
            else if (buttonJustReleased)
            {
                // if we disable this object this frame, then the teleport won't work, so do it next frame
                SetActiveNextFrame(teleportRay.gameObject, false);
            }

            buttonPressedLastFrame = isPressed;
        }

        public void SetActiveNextFrame(GameObject gameObject, bool active)
        {
            StartCoroutine(SetActiveNextFrameHelper(gameObject, active));
        }

        IEnumerator SetActiveNextFrameHelper(GameObject gameObject, bool active)
        {
            yield return null;
            gameObject.SetActive(active);
        }
    }
}