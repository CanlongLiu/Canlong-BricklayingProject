using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GVRI{
    [Serializable]
    public class CoreSlot
    {
        //send message whenever itemcount is changed
        public delegate void notifyNewItemCount(CoreSlot slot);
        public List<notifyNewItemCount> subscribers = new List<notifyNewItemCount>();

        [SerializeField]
        private ItemInfo _itemInfo;
        // 控制itemcount and iteminfo 的关系 增减删除 如果改变ItemInfo将Count 设置为1
        public ItemInfo ItemInfo {
            get { 
                //if(_itemInfo == null) Debug.Log("get is null "); 
                return _itemInfo; 
            }
            set {
                if (_itemInfo == value) //no changes
                    return;
                if (value == null) //no value
                {//there is no longer an item that can be associated with count
                    _itemCount = 0;
                    //Debug.Log("The iteminfo is removed and ItemCount is set to 0");
                }
                else if(_itemInfo == null) //changed from null to 1(add 1)
                {
                    _itemCount = 1;
                    //Debug.Log("The item is added and Itemcount is set to 1 when iteminfo is changed from null ");
                }
                _itemInfo = value;
                //告诉订阅者
                Debug.Log("The iteminfo is changed");
                NotifySubscribers();
            }
        }
        [SerializeField]
        private int _itemCount;
        // 第二次增加 如果改变ItemCount将Count + ItemCount ，这里Value是原来的Itemcount
        public int ItemCount { 
            get => _itemCount; 
            set {
                if (_itemCount == value) //no changes
                    return;

                _itemCount = value;
                if (_itemCount == 0) //no contents left
                {//there is no item stored in here anymore
                    _itemInfo = null;
                }else if(_itemInfo == null)
                {
                    _itemCount = 0;
                }
                //Debug.Log("The item is added by ItemCount at the second time" + value + _itemCount);
                NotifySubscribers();
            } 
        }

        // Start is called before the first frame update
        public CoreSlot()
        {
            if (_itemInfo != null && ItemCount == 0)
            {//this doesn't make sense
                Debug.LogWarning("There is a stored ItemInfo, but ItemCount is 0. Removing Items");
                _itemInfo = null;
            }
            else if (_itemInfo == null && ItemCount > 0)
            {//this doesn't make sense
                Debug.LogWarning("There is a positive ItemCount but no ItemInfo. Removing Items");
                ItemCount = 0;
            }
        }

        //add a <count> ItemInfos to the BaseSlot
        //returns true if succeeded
        public int Add(ItemInfo itemInfo, int count)
        {
            if (!itemInfo) Debug.LogError("The itemInfo is null. Cannot store");
            if(itemInfo.Equals(_itemInfo) || _itemInfo == null)
            {
                /* For capped items (future)
                if (StoredItemCount + count > maxStorage)
                {
                    uint storeCount = maxStorage - StoredItemCount;
                    StoredItemCount += storeCount;
                    return count - storeCount;
                }
                */
                
                _itemInfo = itemInfo;
                _itemCount += count;
                //Debug.Log("The item" + _itemInfo + "is added" + _itemCount + "Add method");
                NotifySubscribers();
                return count;
            }
            return 0;
        }

        //remove all items
        public void Clear()
        {
            Remove(_itemCount);
        }

        //removes <count> from BaseSlot. returns the number of items removed 
        public int Remove(int count)
        {
            if (_itemCount > 0)
            {//can pick an item because there are some stored
                int itemsRemoved;
                Debug.Log("Strart remove");
                if (_itemCount > count)
                {
                    itemsRemoved = count;
                    _itemCount -= count;
                    Debug.Log("remove 1");
                }
                else
                {//no further items left
                    itemsRemoved = _itemCount;
                    _itemCount = 0;
                    _itemInfo = null;
                    Debug.Log("no further items left");
                }
                NotifySubscribers();
                return itemsRemoved;
            }

            return 0;
        }

        //this function just loops through all delegate Functions and calls them
        void NotifySubscribers()
        {
            Debug.Log("Boardcast changes");
            //remove all nulls
            subscribers.RemoveAll(item => item == null);
            //notify subscribers
            foreach (notifyNewItemCount n in subscribers)
            {
                n(this);
            }
        }
    }
}