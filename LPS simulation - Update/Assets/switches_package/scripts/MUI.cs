using UnityEngine;
using System.Collections;

namespace GVRMUI
{
    //mechanical user Interface
    public class MUI : MonoBehaviour
    {
        //gets called when the player begins touching the mechanical user Interface
        public virtual void BeginTouch(Transform controllerTransform) { }

        //gets called when the player keeps touching the mui
        public virtual void Touch(Transform controllerTransform) { }

        //gets called when the player stops touching the mui
        public virtual void StopTouch(Transform controllerTransform) { }

        //gets called when the player begins pressing the mui
        public virtual void BeginPress(Transform controllerTransform) { }

        //gets called when the player keeps pressing the mui
        public virtual void Press(Transform controllerTransform) { }

        //gets called when the player stops pressing the mui
        public virtual void StopPress(Transform controllerTransform) { }
    }
}

