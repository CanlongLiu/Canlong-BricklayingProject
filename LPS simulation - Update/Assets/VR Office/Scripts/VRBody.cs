using UnityEngine;
using Photon.Pun;
using System.Collections;
using Photon.Realtime;
using TMPro;

namespace ChiliGames.VROffice
{
    //This script is attached to the VR body, to ensure each part is following the correct tracker. This is done only if the body is owned by the player
    //and replicated around the network with the Photon Transform View component
    public class VRBody : MonoBehaviourPunCallbacks
    {
        public TextMeshProUGUI PlayerName_Text;
        public Transform[] body;
        [SerializeField] SkinnedMeshRenderer lHand;
        [SerializeField] SkinnedMeshRenderer rHand;
        [SerializeField] SkinnedMeshRenderer bodyRenderer;

        private Color playerColor;

        PhotonView pv;

        private void Awake()
        {
            pv = GetComponent<PhotonView>();

            //Enable hand renderers if this is my avatar.
            if (pv.IsMine)
            {
                lHand.enabled = true;
                rHand.enabled = true;
                object avatarSelectionNumber;
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber))
                {
                    Debug.Log("Avatar selection number: " + (int)avatarSelectionNumber);
                    photonView.RPC("RPC_SetColor", RpcTarget.AllBuffered, (int)avatarSelectionNumber);
                }
            }

            if(PlatformManager.instance != null)
            {
                PlatformManager.instance.onSpawned.AddListener(SetColor);
            }

            if (PlayerName_Text != null)
            {
                PlayerName_Text.text = photonView.Owner.NickName;
            }
        }

        // Follow trackers only if it's our body
        void Update()
        {
            if (pv.IsMine)
            {
                for (int i = 0; i < body.Length; i++)
                {
                    body[i].position = PlatformManager.instance.vrRigParts[i].position;
                    body[i].rotation = PlatformManager.instance.vrRigParts[i].rotation;
                }
            }
        }

        [PunRPC]
        public void RPC_TeleportEffect()
        {
            StopAllCoroutines();
            StartCoroutine(TeleportEffect());
        }

        //Lerps the dissolve shader to create a teleportation effect on the avatar.
        IEnumerator TeleportEffect()
        {
            float effectDuration = 0.8f;
            for (float i = 0; i < effectDuration; i += Time.deltaTime)
            {
                bodyRenderer.material.SetFloat("_CutoffHeight", Mathf.Lerp(-1, 4, i / effectDuration));
                yield return null;
            }
        }

        //For setting different colors to each player joining the room.
        void SetColor()
        {
            object avatarSelectionNumber1;
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber1);
            
            pv.RPC("RPC_SetColor", RpcTarget.AllBuffered, (int)avatarSelectionNumber1);
        }

        [PunRPC]
        //void rpc_setcolor(int n)
        //{
        //    n++;
        //    switch (n)
        //    {
        //        case 1:
        //            playercolor = color.red;
        //            break;
        //        case 2:
        //            playercolor = color.cyan;
        //            break;
        //        case 3:
        //            playercolor = color.green;
        //            break;
        //        case 4:
        //            playercolor = color.yellow;
        //            break;
        //        case 5:
        //            playercolor = color.magenta;
        //            break;
        //        case 6:
        //            playercolor = color.blue;
        //            break;
        //        case 7:
        //            playercolor = color.lerp(color.yellow, color.red, 0.5f);
        //            break;
        //        case 8:
        //            playercolor = color.lerp(color.blue, color.red, 0.5f);
        //            break;
        //        case 9:
        //            playercolor = color.lerp(color.red, color.green, 0.5f);
        //            break;
        //        default:
        //            playercolor = color.black;
        //            break;
        //    }
        //    playercolor = color.lerp(color.white, playercolor, 0.5f);

        //    //set body and hands color.
        //    bodyrenderer.material.setcolor("_albedo", playercolor);
        //    lhand.material.setcolor("_basecolor", playercolor);
        //    rhand.material.setcolor("_basecolor", playercolor);
        //}
        void RPC_SetColor(int n)
        {
       
            if (n == 0)
            {
                playerColor = Color.red;
            }
            else if (n == 1)
            {
                playerColor = Color.blue;
            }
            else if (n == 2)
            {
                playerColor = Color.yellow;
            }


            playerColor = Color.Lerp(Color.white, playerColor, 0.5f);

            //Set body and hands color.
            bodyRenderer.material.SetColor("_Albedo", playerColor);
            lHand.material.SetColor("_BaseColor", playerColor);
            rHand.material.SetColor("_BaseColor", playerColor);
        }
    }
}
