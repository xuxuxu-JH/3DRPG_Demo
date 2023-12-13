using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//>单个单元格
//由于格子是通用的 可以用于背包栏 物品栏 人物状态栏等 所以要进行格子类型的区分
public enum SlotType
{
    BAG,
    WEAPON,
    ARMOR,
    ACTION
}

public class SlotHolder : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public SlotType slotType;

    //> SlotHolder -> ItemUI
    //得到单元格下的UI脚本
    public ItemUI itemUI;

    //>判断当前(自身)单元格的类型 得到对应类型的背包数据库 刷新当前单元格的UI 更换武器
    public void UpdateItem()
    {
        switch (slotType)
        {
            //>背包格
            case SlotType.BAG:
                //根据类型 得到对应的数据库                
                itemUI.BagData = InventroyManager.Instance.inventoryData;
                break;
            #region 其他格
            //>(剑)
            case SlotType.WEAPON:
                itemUI.BagData = InventroyManager.Instance.equipmentData;
                //>当前装备格装备了武器 (数据不为空)
                if (itemUI.BagData.inventroyItemList[itemUI.Index].itemData != null)
                {
                    //传入当前单元格的物品数据 切换武器
                    GameManager.Instance.playerCharacterData.ChangeWeapon(itemUI.BagData.inventroyItemList[itemUI.Index].itemData);
                }//>没有武器就卸下（单元格数据位空）
                else
                {
                    GameManager.Instance.playerCharacterData.UnEquipWeapon();
                    print("卸下");
                }
                break;
            //>(盾牌)
            case SlotType.ARMOR:
                itemUI.BagData = InventroyManager.Instance.equipmentData;
                break;
            //>物品快捷栏
            case SlotType.ACTION:
                itemUI.BagData = InventroyManager.Instance.actionData;
                break;

                #endregion
        }

        //>得到背包中 对应的物品数据
        var itemBagSlotData = itemUI.BagData.inventroyItemList[itemUI.Index];
        //得到数据 更新UI
        itemUI.SetupItemUI(itemBagSlotData.itemData, itemBagSlotData.amount);
    }

    #region 鼠标操作
    public void OnPointerClick(PointerEventData eventData)
    {
        //再次点击如果是满血 不允许使用
        if (GameManager.Instance.playerCharacterData.CurrentHealth == GameManager.Instance.playerCharacterData.MaxHealth)
            return;
        //print("玩家血量已满,无法使用");
        if (eventData.clickCount % 2 == 0)//双击鼠标2次 使用一个物品
            UseItem();
        //print("使用了一次物品");
    }
    //使用一个物品
    public void UseItem()
    {
        if (itemUI.GetItemData() != null)//>如果当前单元格的物品是空的是不允许使用的
        {
            //如果双击的物品是可使用物品类型 且 当前物品在背包中还有库存
            if (itemUI.GetItemData().itemType == ItemType.Useable && itemUI.BagData.inventroyItemList[itemUI.Index].amount > 0)
            {
                //将物品的生命值加成 添加到玩家属性中
                GameManager.Instance.playerCharacterData.ApplyHealth(itemUI.GetItemData().useableData.healthPoint);

                //使用后 物品的数量也要减少一次
                itemUI.BagData.inventroyItemList[itemUI.Index].amount -= 1;

                //>如果在背包中使用了物品 任务要及时更新(防止由于使用了物品 导致任务数据的不同步)
                QuestDataManager.Instance.UpdateQuestProgress(itemUI.GetItemData().itemName, -1);
            }
        }
        //使用物品 数据改变 刷新UI
        UpdateItem();
    }

    //>鼠标指针 进入和退出 格子 显示物品详细信息
    public void OnPointerEnter(PointerEventData eventData)
    {
        //当前单元格有物品才会显示信息
        if (itemUI.GetItemData())
        {
            InventroyManager.Instance.tooltip.SetupTooltip(itemUI.GetItemData());
            InventroyManager.Instance.tooltip.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventroyManager.Instance.tooltip.gameObject.SetActive(false);
    }

    #endregion


    void OnDisable()
    {
        InventroyManager.Instance.tooltip.gameObject.SetActive(false);
    }

}