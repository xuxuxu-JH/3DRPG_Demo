using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//>ItemSlot脚本
//ItemUI 关联格子的物品UI 物品icon和数量的显示
public class ItemUI : MonoBehaviour
{
    public Image icon = null;
    public Text amount = null;
    //得到自身的Item数据(Quest->ShowToolTip)
    public ItemData_SO currentItemData;

    //>在Sloder中 会根据不同的格子类型 得到对应的Bag数据库
    public InventoryData_SO BagData { get; set; }
    //用于获取到背包列表中的对应索引的数据 默认值是-1 在Refresh之前得到正确的赋值
    public int Index { get; set; } = -1;

    /// <summary>
    /// >传入物品数据 将物品数据中的icon和数量与背包UI匹配显示出来
    /// </summary>
    /// <param name="item">物品数据</param>
    /// <param name="itemAmount">物品数量</param>
    public void SetupItemUI(ItemData_SO item, int itemAmount)
    {
        //物品用完 清楚数据和UI显示
        if (itemAmount == 0)
        {
            BagData.inventroyItemList[Index].itemData = null;
            icon.gameObject.SetActive(false);
            return;
        }

        if (itemAmount < 0)
            item = null;

        if (item != null)
        {
            //得到自身的Item数据
            currentItemData = item;
            //更新背包中物品UI和数量的显示 将拾起的物品显示在背包UI中
            icon.sprite = item.itemIcon;
            amount.text = itemAmount.ToString();
            //icon的Image默认是关闭的 只要拿到了 就在背包中显示
            icon.gameObject.SetActive(true);
        }
        else
        {
            //如果是空 保持默认不显示
            icon.gameObject.SetActive(false);
        }
    }

    //>返回当前单元格的物品数据
    public ItemData_SO GetItemData()
    {
        return BagData.inventroyItemList[Index].itemData;
    }
}
