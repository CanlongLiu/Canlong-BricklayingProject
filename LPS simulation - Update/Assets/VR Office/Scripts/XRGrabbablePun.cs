using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

namespace ChiliGames
{
    public class XRGrabbablePun : XRGrabInteractable
    {
        PhotonView pv;
        bool wasKinematic;
        protected override void Awake()
        {
            base.Awake();
            pv = GetComponent<PhotonView>();
            wasKinematic = GetComponent<Rigidbody>().isKinematic;
        }
        // 从OnSelectEntered(XRBaseInteractor interactor) 改为 OnSelectEntered(SelectEnterEventArgs args)
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            //当抓取的时候给其他所用用户应用运动学。
            pv.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
            base.OnSelectEntered(args);
            //var interactor = args.interactor;
            pv.RPC("SetKinematic", RpcTarget.OthersBuffered, true);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            //当抓取的时候给其他所用用户取消运动学。
            base.OnSelectExited(args);
            //var interactor = args.interactor;
            pv.RPC("SetKinematic", RpcTarget.OthersBuffered, wasKinematic);
        }

        [PunRPC]
        public void SetKinematic(bool state)
        {
            GetComponent<Rigidbody>().isKinematic = state;
        }
    }
}

