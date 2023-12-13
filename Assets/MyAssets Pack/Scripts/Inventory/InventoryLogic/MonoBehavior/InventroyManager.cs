using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static InventoryData_SO;
//InventoryCanvas 挂载在画布上 画布管理所有背包相关
//>背包管理器 处理背包数据 以及数据和UI之间相联系
public class InventroyManager : Singleton<InventroyManager>
{
    //临时变量 用于记录拖拽前的物品数据
    public DragData OriginalDragData;
    //保存原始的Holder和Holder的位置
    public class DragData
    {
        public SlotHolder originalHolder;
        public RectTransform originalParent;
    }
    [Header("Drag Canvas")]
    public Canvas dragCanvas;

    [Header("Inventory Data")]
    #region Inventory Data
    //>关联了整个背包相关的数据
    public InventoryData_SO inventoryTemplate;
    [HideInInspector]
    public InventoryData_SO inventoryData;

    public InventoryData_SO actionTemplate;
    [HideInInspector]
    public InventoryData_SO actionData;

    public InventoryData_SO equipmentTemplate;
    [HideInInspector]
    public InventoryData_SO equipmentData;
    #endregion

    [Header("Containers")]
    #region Containers
    //>关联 各个面板的ContainerUI 对象
    //背包面板
    public ContainerUI Inventroy_ContainerUI;
    //装备栏
    public ContainerUI ActionBar_ContainerUI;
    //人物属性栏
    public ContainerUI Equipment_ContainerUI;
    #endregion

    [Header("UI Panel")]
    #region UI Panel
    public GameObject bagPanel;

    public GameObject statsPanel;
    public GameObject CloseButton;

    bool isOpen = false;
    #endregion

    [Header("Stats Text")]
    #region Stats Text
    //人物状态栏 显示Player生命值和攻击力的文本
    public Text healthText;
    public Text attackText;
    #endregion

    [Header("Tooltip")]
    public ItemTooltip tooltip;

    protected override void Awake()
    {
        base.Awake();
        //复制所有的背包数据
        //如果数据不是空的 就生成一个新的数据 保证每次新建游戏的时候 数据是空的
        if (inventoryTemplate != null)
            inventoryData = Instantiate(inventoryTemplate);
        if (actionTemplate != null)
            actionData = Instantiate(actionTemplate);
        if (equipmentTemplate != null)
            equipmentData = Instantiate(equipmentTemplate);
    }

    private void Start()
    {
        //无论是转场景还是新建游戏的时候 都先加载数据
        LoadInventoryData();
        //游戏刚开始 Refresh一次UI 正确显示背包的UI
        Inventroy_ContainerUI.RefreshHolderUI();
        ActionBar_ContainerUI.RefreshHolderUI();
        Equipment_ContainerUI.RefreshHolderUI();
    }

    private void Update()
    {
        //控制面板的整体关闭
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isOpen = !isOpen;
            bagPanel.SetActive(isOpen);
            statsPanel.SetActive(isOpen);
            CloseButton.SetActive(isOpen);
        }

        UpdateStatsText(GameManager.Instance.playerCharacterData.CurrentHealth, GameManager.Instance.playerCharacterData.attackData.minDamage, GameManager.Instance.playerCharacterData.attackData.maxDamage);

    }
    #region 保存所有的背包数据
    public void SaveInventoryData()
    {
        SaveManager.Instance.Save(inventoryData, inventoryData.name);
        SaveManager.Instance.Save(actionData, actionData.name);
        SaveManager.Instance.Save(equipmentData, equipmentData.name);
    }

    public void LoadInventoryData()
    {
        SaveManager.Instance.Load(inventoryData, inventoryData.name);
        SaveManager.Instance.Load(actionData, actionData.name);
        SaveManager.Instance.Load(equipmentData, equipmentData.name);
    }

    #endregion

    //更新状态栏里的玩家信息的文本 
    public void UpdateStatsText(int health, int min, int max)
    {
        healthText.text = health.ToString();
        attackText.text = min + "-" + max;
    }

    #region 判断鼠标指针(被拖拽的物品) 是否在格子的方格范围内

    //>是否在以下三个中 其中一个UI范围内
    public bool Check_AllContainersUI(Vector3 Position)
    {
        if (CheckInActionUI(Position) || CheckInEquipmentUI(Position) || CheckInInventoryUI(Position))
            return true;
        else
            return false;
    }

    //>传入鼠标坐标 遍历背包中的30个格子 检测物品是否在这30个格子的范围内
    public bool CheckInInventoryUI(Vector3 position)
    {
        for (int i = 0; i < Inventroy_ContainerUI.slotHoldersArray.Length; i++)
        {
            //t 代表了每个单元格的位置 
            RectTransform t = Inventroy_ContainerUI.slotHoldersArray[i].transform as RectTransform;
            //检查 物品是否在这个30个(t)的范围内
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        //如果都不在 返回false
        return false;
    }
    public bool CheckInActionUI(Vector3 position)
    {
        for (int i = 0; i < ActionBar_ContainerUI.slotHoldersArray.Length; i++)
        {
            RectTransform t = ActionBar_ContainerUI.slotHoldersArray[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckInEquipmentUI(Vector3 position)
    {
        for (int i = 0; i < Equipment_ContainerUI.slotHoldersArray.Length; i++)
        {
            RectTransform t = Equipment_ContainerUI.slotHoldersArray[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }


    #endregion

    #region 检测任务物品
    //>检测 任务的目标物品 在 背包中是否已经持有 
    public void CheckQuestItemInBag(string itemQuestName)
    {
        //检查背包和快捷栏的所有物品 
        foreach (var item in inventoryData.inventroyItemList)
        {
            if (item.itemData != null)
            {
                if (item.itemData.itemName == itemQuestName)
                    QuestDataManager.Instance.UpdateQuestProgress(item.itemData.itemName, item.amount);
            }
        }

        foreach (var item in actionData.inventroyItemList)
        {
            if (item.itemData != null)
            {
                if (item.itemData.itemName == itemQuestName)
                    QuestDataManager.Instance.UpdateQuestProgress(item.itemData.itemName, item.amount);
            }
        }
    }
    #endregion

    //背包中是否有 任务需要的物品 返回背包数据
    public InventoryItem QuestItemInBag(ItemData_SO questItem)
    {
        return inventoryData.inventroyItemList.Find(i => i.itemData == questItem);
    }
    public InventoryItem QuestItemInAction(ItemData_SO questItem)
    {
        return actionData.inventroyItemList.Find(i => i.itemData == questItem);
    }
}
