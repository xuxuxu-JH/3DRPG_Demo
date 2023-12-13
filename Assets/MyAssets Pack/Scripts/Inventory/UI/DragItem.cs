using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//实际上拖拽的游戏对象是ItemSlot(ItemUI) 因此他是必须有ItemUI这个组件的
[RequireComponent(typeof(ItemUI))]
//>UGUI实现拖拽必须要继承的三个接口 IBeginDragHandler, IDragHandler, IEndDragHandler 
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //临时变量 作为交换的中介
    ItemUI currentItemUI;
    //当前格子
    SlotHolder currentHolder;
    //目标格子
    SlotHolder targetHolder;
    private void Awake()
    {
        //>得到当前的ItemUI
        currentItemUI = GetComponent<ItemUI>();
        //>得到当前的格子
        currentHolder = GetComponentInParent<SlotHolder>();
    }
    //开始拖拽
    public void OnBeginDrag(PointerEventData eventData)
    {
        InventroyManager.Instance.OriginalDragData = new InventroyManager.DragData();
        //>拖拽前 保存一份原始的单元格数据(物品数据)
        //记录原始的Holder和位置
        InventroyManager.Instance.OriginalDragData.originalHolder = GetComponent<SlotHolder>();
        InventroyManager.Instance.OriginalDragData.originalParent = (RectTransform)transform.parent;

        //>将物品作为顶层画布DargCanvas 使物品在最顶层显示
        transform.SetParent(InventroyManager.Instance.dragCanvas.transform, true);// true 将图片保持是原始大小,旋转等参数
    }
    //拖拽过程中
    public void OnDrag(PointerEventData eventData)
    {
        //>拖拽过程中物品始终跟随鼠标移动
        transform.position = eventData.position;
    }
    //结束拖拽
    public void OnEndDrag(PointerEventData eventData)
    {
        //1.判断当前鼠标指针是否在UI页面上
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //2.判断拖拽物品是否在背包单元格的方格内
            if (InventroyManager.Instance.Check_AllContainersUI(eventData.position))
            {
                //>3.得到目标单元格
                if (eventData.pointerEnter.gameObject.GetComponent<SlotHolder>())
                {
                    targetHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>();
                }
                else
                {
                    //无论是否是空格 都能都到目标格的SlotHolder
                    targetHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>();
                }

                //>4.判断 目标单元格的 类型 
                switch (targetHolder.slotType)
                {
                    //背包不用判断 因为背包是容纳所有类型的物品
                    case SlotType.BAG:
                        SwapItem();
                        break;
                    case SlotType.WEAPON:
                        ////类型一致 才允许交换
                        if (currentItemUI.BagData.inventroyItemList[currentItemUI.Index].itemData.itemType == ItemType.Weapon)
                            SwapItem();
                        break;
                    case SlotType.ARMOR:
                        //类型一致 才允许交换
                        if (currentItemUI.BagData.inventroyItemList[currentItemUI.Index].itemData.itemType == ItemType.Armor)
                            SwapItem();
                        break;
                    case SlotType.ACTION:
                        //类型一致 才允许交换
                        if (currentItemUI.BagData.inventroyItemList[currentItemUI.Index].itemData.itemType == ItemType.Useable)
                            SwapItem();
                        break;
                }

                //数据发生改变 两个单元格都刷新一次
                currentHolder.UpdateItem();
                targetHolder.UpdateItem();
            }
        }

        //>取消拖拽 返回原始单元格
        transform.SetParent(InventroyManager.Instance.OriginalDragData.originalParent);
        RectTransform t = transform as RectTransform;
        t.offsetMax = -Vector2.one * 5;
        t.offsetMin = Vector2.one * 5;
    }

    //>交换两个单元格之间物品的数据 
    public void SwapItem()
    {
        //得到目标单元格内物品数据
        var targetItemData = targetHolder.itemUI.BagData.inventroyItemList[targetHolder.itemUI.Index];

        //得到当前拖拽物品的数据
        var tempItemData = currentHolder.itemUI.BagData.inventroyItemList[currentHolder.itemUI.Index];

        //比较当前拖拽物品和目标物品的数据是否相同
        bool isSameItem = tempItemData.itemData == targetItemData.itemData;

        //>如果是相同物品 且 可叠加
        if (isSameItem && targetItemData.itemData.stackable)
        {
            //更新目标格子的物品数量
            targetItemData.amount += tempItemData.amount;
            //原来格子里的物品数据就应该消除
            tempItemData.itemData = null;
            tempItemData.amount = 0;
            //完成一次叠加物品的拖拽
        }
        else//>如果是不同的物品 且 不可堆叠
        {
            //>交换单元格物品的数据 改变物品在List中对应的index的位置
            currentHolder.itemUI.BagData.inventroyItemList[currentHolder.itemUI.Index] = targetItemData;
            targetHolder.itemUI.BagData.inventroyItemList[targetHolder.itemUI.Index] = tempItemData;
        }
    }

}