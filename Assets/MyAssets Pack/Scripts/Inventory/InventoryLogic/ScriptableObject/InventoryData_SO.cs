using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory Data")]
// >背包数据库 用于存储背包中 物品数据 和 物品数量
public class InventoryData_SO : ScriptableObject
{
    //>背包数据列表 存储背包中物品的数据
    public List<InventoryItem> inventroyItemList = new List<InventoryItem>();

    [System.Serializable]
    //记录背包单个物品的数据和数量
    public class InventoryItem
    {
        //物品数据
        public ItemData_SO itemData;
        //物品数量
        public int amount;
    }

    //>将物品数据添加到背包的数据列表当中
    //遍历背包List
    //1.查找是否包含当前物品数据,如果不包含说明是新的物品,添加进背包List当中;
    //2.如果包含当前物品数据,背包中已经存在当前类型的物品,只需更新物品的数量即可
    //3.如果当前的物品不支持叠加显示,就相当于是添加一个新的物品到背包数据当中了
    public void AddItem(ItemData_SO newItemData, int amount)
    {
        //物品在背包中是否被找到
        bool found = false;
        //>如果添加的物品是可堆叠的 遍历背包列表中是否已经包含这个物品 如果包含 堆叠数量增加
        if (newItemData.stackable)
        {
            foreach (var item in inventroyItemList)
            {
                if (item.itemData == newItemData)
                {
                    item.amount += amount;
                    //在列表中被找到 下面的if语句不再执行
                    found = true;
                    break;
                }
            }
        }
        //if(newItemData.stackable == false && item.itemData != newItemData)
        //>如果是不可堆叠 或 背包中没有这类物品(数据不同,是新的物品) 就再列表中就近位置添加一个这个物品
        //第一次添加物品 执行
        for (int i = 0; i < inventroyItemList.Count; i++)
        {
            //遍历列表 在最近处找到空位置
            if (inventroyItemList[i].itemData == null && !found)
            {
                inventroyItemList[i].itemData = newItemData;
                inventroyItemList[i].amount = amount;
                break;
            }
        }
    }

}

