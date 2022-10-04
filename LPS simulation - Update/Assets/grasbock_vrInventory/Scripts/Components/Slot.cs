using ChiliGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Photon.Pun;
using HutongGames.PlayMaker;

namespace GVRI{

    public class Slot : MonoBehaviourPun
    {
        //the thing containing the data
        [SerializeField, HideInInspector]
        private CoreSlot _coreSlot = new CoreSlot();

        public CoreSlot CoreSlot
        {
            get => _coreSlot;
            set
            {
                if (_coreSlot != null) // there is a slot base slot already attached to this slot. remove its references
                    _coreSlot.subscribers.Remove(NotifySlotChange);

                _coreSlot = value;
                if (_coreSlot != null)
                {
                    _coreSlot.subscribers.Add(NotifySlotChange);
                    NotifySlotChange(_coreSlot);
                }

            }
        }

        //public float distance = 0.1f; future to get rid of other components
        public float pushForce = 1.0f;
        (GameObject, ItemInfo) preview;//执行判断逻辑的中介变量
        public float previewRotationSpeed = 1.0f;
        public float previewSizeFactor = 0.8f;
        public int maxStorage = 1;
        PhotonView pv;//设置变量pv
        public bool ArrayApplied;
        
        ~Slot()
        {
            _coreSlot.subscribers.Remove(NotifySlotChange);
        }

        void Awake()
        {
            
             pv = GetComponent<PhotonView>();// 唤醒时候获得PhotonView组件赋予pv
            if (_coreSlot == null)
                CoreSlot = new CoreSlot();
            _coreSlot.subscribers.Add(NotifySlotChange);
        }

        /// Start is called before the first frame update
        void Start()
        {
            NotifySlotChange(_coreSlot); //执行告知改变
            //如果刚进入 更新
            if (CoreSlot.ItemInfo != null && preview.Item1 == null) //如果没有gameobject且参考Coreslot有ItemInfo
            {//make a preview of the item in storage if there were none before
                //should only contain graphical elements
                preview.Item1 = CreatePreviewCopy(CoreSlot.ItemInfo.prefab); //执行生成参考Coreslot的ItemInfoprefab.传递给Item1(GameObject)

            }

            Utils.AddSphereTriggerColliderIfNotAvailable(gameObject);
        }

        void Update()
        {//slowly rotate itemPreview around
         //if (preview.Item1)
         //{
         //    preview.Item1.transform.LookAt(
         //        preview.Item1.transform.position + Vector3.up,
         //        new Vector3(previewRotationSpeed * Mathf.Cos(Time.time), 0.0f, previewRotationSpeed * Mathf.Sin(Time.time))
         //    );
         //}

            //在这里对本slot的Itemcount and ItemInfo进行同步 获得本Coreslot分身的数量和info，同步给其它用户


        }

        private void NotifySlotChange(CoreSlot s) //the coreslot attached to this slot(输入值s是_Coreslot) 直接更改iteminfo的话
        {
            ItemInfo ii = s.ItemInfo; // ii 为CoreSLOT的Iteminfo
            if (preview.Item2 != ii)  
            {
                Debug.Log("delete the previewcopy");
                Destroy(preview.Item1);//判断如果现在的item2 （第一次为空）不等于Coreslot的Iteminfo，且这个slot是master的 删除展示的gameobject
                preview.Item2 = ii; // 附着的Iteminfo赋予展示的info
                if (ii != null) //如果附着的itemInfo不是空集
                {
                    //make a preview of the item in storage if there were none before
                    //should only contain graphical elements
                    preview.Item1 = CreatePreviewCopy(ii.prefab); // 执行一次创建展示，将创建展示的GameObject赋予preview.Item1
                }
            }
        }

        /// Creates a preview of the GameObject specified in the ItemInfo 修改Instantiate to PhotonNetwork. Instantiate
        GameObject CreatePreviewCopy(GameObject original)
        {
            //添加网络
            GameObject preview = Instantiate(original, transform.position, transform.rotation);
            //Debug.Log("Create a PreviewCopy Successfully" + original.name);

            //remove all components that are not used for visuals 在这里先删除pun offset components再删除rigid body 不太好处理，我决定inorder 删除他
            var components = preview.GetComponents<Component>();

            foreach (Component comp in components)
            {
                if (comp is XRGrabbablePunOffset)
                {

                    Destroy(comp);

                }


                if (comp is Rigidbody)
                {

                    Destroy(comp);
                    //Rigidbody rb;
                    // rb = preview.GetComponent<Rigidbody>();
                    //rb.isKinematic = true; 

                }

                if (comp is MeshCollider)
                {
                    Destroy(comp);
                }

                if (comp is Item)
                {
                    Destroy(comp);
                }




                //if ( !(comp is MeshRenderer) &&
                //    !(comp is MeshFilter) &&
                //    !(comp is Transform)
                //)
                //{
                //    Destroy(comp);
                //} 
            }

            //set parent
            preview.transform.parent = transform;

            //resize a little
            preview.transform.localScale *= previewSizeFactor;

            return preview;
        }

        /// Attempt to store a GameObject and returns true if succeeded
        public bool Store(GameObject go)
        {
            if (ArrayApplied) return false;
            if (!go) return false; //gameobject does not exist
            if (CoreSlot.ItemCount >= maxStorage) return false; //gameobject stored exceeds the max storage

            Item item = go.GetComponent<Item>();
            if (!item)
            {//not an item, don't care
                return false;
            }

            if (go.transform.parent != null)
            { //this item is being held, insertion probably not intended
                return false;
            }


            ItemInfo itemInfo = item.itemInfo;
            if (CoreSlot.Add(itemInfo, 1) == 1) 
            {//sucessfully moved object into inventorySlot
                //remove the item with gameobject
                Destroy(go);
                go.SetActive(false); //disable anything till deleted (OnTriggerStay gets called multiple times before OnDestroy)
                //Debug.Log("This object is stored by me");
                
                //------------更新本地-Fsm-----------------// 为什么分开？因为本地加了1了。。。。
                //PlayMakerFSM fSM = GetComponent<PlayMakerFSM>(); //获得FSM组件赋予fSM
                //fSM.FsmVariables.GetFsmString("Name").Value = itemInfo.name;// get FSM variable and set 
                //fSM.SendEvent("Add");//update to Playmaker array to remote player,在这里我不buffer自身的增加，因为不会。。。
                //------------更新本地-Fsm------------------//
                if (pv)
                {
                    //----------------更新远程-FSM-----------//
                    pv.RPC("AddOne", RpcTarget.OthersBuffered, itemInfo.name); //  这就是为什么做这个动作的人没有更新RPC。。
                }
                
                return true;
            }
            //push the item out of the inventory
            item.push((go.transform.position - transform.position) * pushForce);
            return false;
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
        //添加网络
        private void OnTriggerEnter(Collider other)
        {
            PhotonView otherpv = other.GetComponent<PhotonView>();

            if (otherpv == null) Store(other.gameObject);
            if (otherpv != null && otherpv.IsMine)
            {
                Store(other.gameObject);
            }
            else return;

        }
        //添加网络
        private void OnTriggerStay(Collider other)
        {
            PhotonView otherpv = other.GetComponent<PhotonView>();

            if (otherpv == null) Store(other.gameObject);
            if (otherpv != null && otherpv.IsMine)
            {
                
                Store(other.gameObject);
            }
            else return;



        }


    
        [PunRPC]
        public void AddOne(string a)
        {
            //PlayMakerFSM fSM = GetComponent<PlayMakerFSM>(); //获得FSM组件赋予fSM

            if (a == "TypeA1")
            {
                //Debug.Log("RPC add one is called TypeA1");
                var b = Resources.Load("ItemInfos/TypeA1") as ItemInfo;
                CoreSlot.Add(b, 1);
                //if (ArrayApplied)
                //{
                    //fSM.FsmVariables.GetFsmString("Name").Value = "TypeA1";// Set Fsm variable
                    //fSM.SendEvent("Add");//update to Playmaker array to remote player
                //}


            }

            if (a == "TypeB1")
            {
                var b = Resources.Load("ItemInfos/TypeB1") as ItemInfo;
                CoreSlot.Add(b, 1);
                //if (ArrayApplied)
                //{
                    //fSM.FsmVariables.GetFsmString("Name").Value = "TypeB1";// Set Fsm variable
                    //fSM.SendEvent("Add");//update to Playmaker array to remote player
                //}

            }

            if (a == "TypeC")
            {
                var b = Resources.Load("ItemInfos/TypeC") as ItemInfo;
                CoreSlot.Add(b, 1);
                //if (ArrayApplied)
                //{
                    //fSM.FsmVariables.GetFsmString("Name").Value = "TypeC";// Set Fsm variable
                    //fSM.SendEvent("Add");//update to Playmaker array to remote player
                //}

            }
            if (a == "TypeD1")
            {
                var b = Resources.Load("ItemInfos/TypeD1") as ItemInfo;
                CoreSlot.Add(b, 1);
                //if (ArrayApplied)
                //{
                    //fSM.FsmVariables.GetFsmString("Name").Value = "TypeD1";// Set Fsm variable
                   //fSM.SendEvent("Add");//update to Playmaker array to remote player
                //}

            }

            if (a == "TypeE1")
            {
                var b = Resources.Load("ItemInfos/TypeE1") as ItemInfo;
                CoreSlot.Add(b, 1);
                //if (ArrayApplied)
                //{
                    //fSM.FsmVariables.GetFsmString("Name").Value = "TypeE1";// Set Fsm variable
                    //fSM.SendEvent("Add");//update to Playmaker array to remote player
                //}

            }

            if (a == "TypeF1")
            {
                var b = Resources.Load("ItemInfos/TypeF1") as ItemInfo;
                CoreSlot.Add(b, 1);
                //if (ArrayApplied)
                //{
                    //fSM.FsmVariables.GetFsmString("Name").Value = "TypeF1";// Set Fsm variable
                    //fSM.SendEvent("Add");//update to Playmaker array to remote player
                //}

            }



        }
        [PunRPC]
        public void RemoveOne(int number)
        {
            //Debug.Log("Recieved a message to Remove 1 for remote player ");
            CoreSlot.Remove(number);
            if (ArrayApplied) { 
            PlayMakerFSM fSM = GetComponent<PlayMakerFSM>(); //获得FSM组件赋予fSM
            fSM.SendEvent("Remove");//update to Playmaker array to remote player
            }

        }


        //这里因为 传输的自定义类需要注册 比较麻烦所以用[PunRPC]
        //void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        //{
        //    Debug.Log("Boardcase to remote player the Iteminfo " + CoreSlot.ItemInfo);
        //    if (stream.IsWriting)
        //    {
        //        // We own this player: send the others our data
        //        stream.SendNext(CoreSlot.ItemInfo);
        //        stream.SendNext(CoreSlot.ItemCount);
        //    }
        //    else
        //    {
        //        // Network player, receive data
        //        this.CoreSlot.ItemInfo = (ItemInfo)stream.ReceiveNext();
        //        this.CoreSlot.ItemCount = (int)stream.ReceiveNext();
        //    }
        //  }
    }
}