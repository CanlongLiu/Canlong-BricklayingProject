using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using ChiliGames;
using UnityEngine.XR.Interaction.Toolkit;

namespace GVRI{
    [RequireComponent(typeof(Rigidbody))]
    public class HandIntegration : MonoBehaviour
    {
        //这个脚本只判断是不是在手上, 获得触碰的slot信息，和是否在手上的信息。对触碰的slot进行减1(本地和其他玩家),对本地的手上生成一个额外的物体,丢弃，
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
                        PhotonView pv = slot.GetComponent<PhotonView>();
                        int itemsRemoved = slot.CoreSlot.Remove(1); //本地remove1
                        Debug.Log("remove 1 at local " + slot);
                        if (pv)
                        {
                            pv.RPC("RemoveOne", RpcTarget.OthersBuffered, 1);//其它玩家在slot上remove1
                            if (itemsRemoved > 0)
                            {//sucessfully pulled an Item from InventorySlot                //添加网络生成到手上，设置为在手上
                                AttachGameObject(
                                    Instantiate(itemInfo.prefab, transform.position, transform.rotation)
                                );
                                //Debug.Log("Successfully instantiate the Item in local" + itemInfo.name);
                            }
                            if (slot.ArrayApplied)
                            {
                                //--------PlaymakerAction------------//
                                PlayMakerFSM fSM = slot.GetComponent<PlayMakerFSM>();
                                fSM.SendEvent("Remove");//update to Playmaker array
                            }
                        }
                        else
                        {
                            if (itemsRemoved > 0)
                            {//sucessfully pulled an Item from InventorySlot                //添加网络生成到手上，设置为在手上
                                AttachGameObject(
                                    Instantiate(itemInfo.prefab, transform.position, transform.rotation)
                                );
                                //Debug.Log("Successfully instantiate the Item in local" + itemInfo.name);
                            }
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
            ///reenable physics
            Rigidbody go_rb = gameObjectInHand.GetComponent<Rigidbody>();
            if (go_rb)
            {
                go_rb.isKinematic = false;
                //apply throw
                go_rb.velocity = velocity;
            }        

            gameObjectInHand = null;
        }

        private void AttachGameObject(GameObject go) 
        {
          
            go.transform.parent = gameObject.transform;
            gameObjectInHand = go;

            //disable physics for the time being
           Rigidbody rb = go.GetComponent<Rigidbody>();
            if (rb)
                rb.isKinematic = true;
        }

        private void OnTriggerEnter(Collider other) //！不用改 获得Slot
        {
            //创建一个Slot类的othersInvslot的变量， 获得碰到手的slot的slot脚本赋值于othersInvslot
            Slot othersInvSlot = other.GetComponent<Slot>();
        //如果是存在的
            if (othersInvSlot) {
                Debug.Log("inventory Slot found by enter" );
                slot = othersInvSlot;
                //将这和othersInvSlot赋值于slot
            }
        }

        private void OnTriggerStay(Collider other)//如果手碰到了其它物体A：other
        {
            Item item = other.GetComponent<Item>(); //尝试获得A的item
            //XRGrabbablePun a = other.GetComponent<XRGrabbablePun>();//!增加了一段获得offset想获得是否被attach的属性
            if (item && !gameObjectInHand) //如果物体A的item存在且手上没有东西
            {//its an item and there is none in this hand
                //Debug.Log("its an item and there is none in this hand");
                if (justPressed) //刚好按了trigger
                {
                    if (item.transform.parent != null) //如果这个物体A在手上attachTransform存在，尝试执行swap   "a.attachTransform"
                    {
                        Hand otherHand = item.transform.parent.gameObject.GetComponent<Hand>();//获得另外一个物体A手上的Hand脚本 "a.selectingInteractor.gameObject.GetComponent<Hand>()"
                        //Hand otherHand = item.transform.parent.gameObject.GetComponent<Hand>(); //获得另外一个物体A手上的Hand脚本
                        if (otherHand) //如果这个脚本存在是手
                        {
                            //Debug.Log("its another hand, swap the item between hands");
                            otherHand.gameObjectInHand = null; //将这个手上GameObjectInhand设置为空。
                        }
                        else
                        {
                            //Debug.Log("its some kind of different GameObject (could be an apple on a tree or a berry in a bush)"); 
                        }
                    }
                    else
                    {
                       // Debug.Log("item is not attached to anything");
                    }

                    //!!!这里是否需要改变 attachpoints?
                    AttachGameObject(other.gameObject);//执行将这个物体A存储到这个手上
                } 
            }
        }

        private void OnTriggerExit(Collider other) //判断是否离开，设置为Slot为空
        {

            Slot othersInvSlot = other.GetComponent<Slot>();
            if (othersInvSlot)
            {
                Debug.Log("The hand doesn't touch Slot, inventory Slot no more");
                slot = null;
            }
        }

    }
    
}