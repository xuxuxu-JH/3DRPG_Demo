using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// >该脚本用于挂载到游戏场景上的物品 用于物品拾取后
public class ItemPickup : MonoBehaviour
{
    //关联当前物品数据 Data中也包含当前物品模型的预制体
    public ItemData_SO itemData;
    //Trigger用于检测 Colider用于掉落碰撞
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //>装备并在Player身上生成武器模型 并替换Player的Attack属性值
            GameManager.Instance.playerCharacterData.EquipWeapon2(itemData);

            //>将当前拾起的物品数据 添加到背包物品数据List当中
            InventroyManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);

            //>添加数据后 刷新背包所有格子的UI
            InventroyManager.Instance.Inventroy_ContainerUI.RefreshHolderUI();

            //收集物品 更新任务
            QuestDataManager.Instance.UpdateQuestProgress(itemData.itemName, itemData.itemAmount);

            //物品被拾起后在场景中销毁
            Destroy(gameObject);

        }
    }
}
