// Custom Action by DumbGameDev
// www.dumbgamedev.com

using HutongGames.PlayMaker;

namespace DGD
{
    [ActionCategory("Unity XR Input")]
    [Tooltip("Get Grip Button Pressed State. True for pressed, false for not pressed.")]
    public class GetGripButtonState : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(XRControllerInput))]
        [Tooltip("Gameobject with the XR Controller Input script")]
        public FsmOwnerDefault inputGameObject;

        [ActionSection("Output")]
        [Tooltip("True when primary axis button is pressed down, false when unpressed")]
        [UIHint(UIHint.Variable)]
        public FsmBool gripState;

        [ActionSection("Options")]
        public FsmBool everyFrame;

        XRControllerInput input;

        public override void Reset()
        {
            inputGameObject = null;
            everyFrame = false;
            gripState = false;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(inputGameObject);
            input = go.GetComponent<XRControllerInput>();
            if (go == null || input == null)
            {
                return;
            }

            GetValue();

            if (!everyFrame.Value)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            if (everyFrame.Value)
            {
                GetValue();
            }
        }

        void GetValue()
        {
            var go = Fsm.GetOwnerDefaultTarget(inputGameObject);
            if (go == null || input == null)
            {
                return;
            }

            gripState.Value = input.gripButton;
        }
    }
}