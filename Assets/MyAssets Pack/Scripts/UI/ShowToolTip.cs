using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//任务栏物品奖励列表中 鼠标进入显示物品详细信息 和背包中实现的功能一致 
//背包中是挂载在SlotHolder当中 需要重写一个脚本 挂载在ItemUI(任务中使用的是ItemUI)
public class ShowToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemUI currentItemUI;
    void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();
    }
    //鼠标进入
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        QuestUI.Instance.tooltip.gameObject.SetActive(true);
        QuestUI.Instance.tooltip.SetupTooltip(currentItemUI.currentItemData);
    }
    //鼠标退出
    public void OnPointerExit(PointerEventData eventData)
    {
        QuestUI.Instance.tooltip.gameObject.SetActive(false);
    }
}
