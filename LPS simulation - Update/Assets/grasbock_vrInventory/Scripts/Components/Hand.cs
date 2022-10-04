using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;

namespace GVRI{
    [RequireComponent(typeof(Rigidbody))]
    public class Hand : MonoBehaviour
    {
        public XRNode node;
        public InputDevice device;

        public GameObject gameObjectInHand = null;

        bool justPressed = false;
        bool lastTriggerState = false;

        Slot slot = null;

        Rigidbody rb = null;
        Vector3 oldPos;
        Vector3 velocity = new Vector3(0,0,0);

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            oldPos = transform.position;

            Utils.AddSphereTriggerColliderIfNotAvailable(gameObject);
        }

        //its important that it is a fixed update. Update is called multiple times so "justPressed" will not always work
        void FixedUpdate()
        {
            //velocity, needed for thowing
            Vector3 newPos = transform.position;
            var delta = (newPos - oldPos);
            velocity = delta / Time.deltaTime;
            oldPos = newPos;

            if (!device.isValid)
            {//device cannot give input
                device = InputDevices.GetDeviceAtXRNode(node);
                if (!device.isValid) return; //this happens if for example the controller is turned off
            }

            bool triggerValue = false;
            if (device.TryGetFeatureValue(CommonUsages.gripButton, out triggerValue))
            {
                //was able to get input from device, so sending the result to input function to handle
                Input(triggerValue);
            }

            lastTriggerState = triggerValue;

        
        }
    
        //this is extracted for cleaner input handling
        void Input(bool triggerValue)
        {
            if (!lastTriggerState && triggerValue)
            {//just pressed
                justPressed = true;
                if (slot)
                {//a valid inventorySlot
                    if (!gameObjectInHand)
                    {//holding no gameObjects
                        ItemInfo itemInfo = slot.CoreSlot.ItemInfo;
                        PhotonView pv = GetComponent<PhotonView>();
                        int itemsRemoved = slot.CoreSlot.Remove(1); //本地remove1
                        Debug.Log("remove 1 at local " + slot);
                        pv.RPC("RemoveOne", RpcTarget.OthersBuffered, 1, true);//其它玩家remove1
                        if (itemsRemoved > 0)
                        {//sucessfully pulled an Item from InventorySlot                //添加网络
                            AttachGameObject(
                                PhotonNetwork.Instantiate(itemInfo.name, transform.position, transform.rotation)
                            );
                            Debug.Log("Successfully instantiate the Item in NetworkedHand" + itemInfo.name);
                        }
                    }

                }
            }
            else
            {
                justPressed = false;
            }

            if (lastTriggerState && !triggerValue)
            {//just released
                if (gameObjectInHand)
                {//holding a gameObject that could potentially be an item to store.
                    DropGameObjectInHand();
                }
            }
        }

        void DropGameObjectInHand() // 丢弃，打开物理效应， gameObjectInHand设置为空
        {
            gameObjectInHand.transform.parent = null;
            //reenable physics
            Rigidbody go_rb = gameObjectInHand.GetComponent<Rigidbody>();
            if (go_rb)
            {
                go_rb.isKinematic = false;
                //apply throw
                go_rb.velocity = velocity;
            }        

            gameObjectInHand = null;
        }

        void AttachGameObject(GameObject go) // 粘在手上，gameObjectInHand 设置为生成的物体， 关掉物理效应
        {
            go.transform.parent = gameObject.transform;
            gameObjectInHand = go;
            //disable physics for the time being
            Rigidbody rb = go.GetComponent<Rigidbody>();
            if (rb)
                rb.isKinematic = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            //创建一个Slot类的othersInvslot的变量， 获得碰到手的slot的slot脚本赋值于othersInvslot
            Slot othersInvSlot = other.GetComponent<Slot>();
        //如果是存在的
            if (othersInvSlot) {
                Debug.Log("inventory Slot found by enter");
                slot = othersInvSlot;
                //将这和othersInvSlot赋值于slot
            }
        }

        private void OnTriggerStay(Collider other)
        {
            Item item = other.GetComponent<Item>();
            if (item && !gameObjectInHand)
            {//its an item and there is none in this hand
                Debug.Log("its an item and there is none in this hand");
                if (justPressed)
                {
                    if (item.transform.parent != null)
                    {
                        Hand otherHand = item.transform.parent.gameObject.GetComponent<Hand>();
                        if (otherHand)
                        {
                            Debug.Log("its another hand, swap the item between hands");
                            otherHand.gameObjectInHand = null;
                        }
                        else
                        {
                            Debug.Log("its some kind of different GameObject (could be an apple on a tree or a berry in a bush)"); 
                        }
                    }
                    else
                    {
                        Debug.Log("item is not attached to anything");
                    }
                    AttachGameObject(other.gameObject);
                } 
            }
        }

        private void OnTriggerExit(Collider other)
        {

            Slot othersInvSlot = other.GetComponent<Slot>();
            if (othersInvSlot)
            {
                Debug.Log("The hand doesn't touch Slot, inventory Slot no more");
                slot = null;
            }
        }

        [PunRPC]

        public void RemoveOne(int a, bool state) // 给其他玩家减1
        {
            Debug.Log("Recieved a message to Remove 1 for remote player " + slot);
            slot.CoreSlot.Remove(a);
        }
    }
    
}