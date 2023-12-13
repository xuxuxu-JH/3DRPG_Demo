using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InventoryData_SO;
using System.Linq;
using Unity.Mathematics;

//>单个任务数据配置
[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
public class QuestData_SO : ScriptableObject
{
    //任务名字
    public string questName;
    [TextArea]
    //任务描述
    public string description;

    //>任务的三种状态 
    //任务开始(进行中)
    public bool isStarted;
    //任务完成(未领取奖励)
    public bool isComplete;
    //任务结束(领取奖励)
    public bool isFinished;

    [System.Serializable]
    //>任务目标
    public class QuestRequires
    {
        //需要收集的目标名
        public string name;
        //需要完成的目标数量
        public int requireAmount;
        //当前目标数量
        public int currentAmount;
    }

    //>当前任务需要完成的所有目标 (完成一个任务 可能需要多个目标)
    public List<QuestRequires> questRequiresList = new List<QuestRequires>();

    //>完成任务后得到的奖励物品
    public List<InventoryItem> rewardsList = new List<InventoryItem>();

    //>检查当前任务进度
    public void CheckQuestProgress()
    {
        //当currentAmount >= requireAmount 代表完成了一个目标 放入临时的List当中
        var finishRequires = questRequiresList.Where(requires => requires.requireAmount <= requires.currentAmount);
        //当达标 和 目标 的成员数量一致 说明当前任务完成了
        if (finishRequires.Count() == questRequiresList.Count)
        {
            isComplete = true;
        };
    }

    //完成任务 给与奖励
    public void GiveRewards()
    {
        //遍历任务中的奖励列表
        foreach (var reward in rewardsList)
        {
            //> amount<0 任务需要提交物品
            if (reward.amount < 0)
            {
                int requireCount = Mathf.Abs(reward.amount);
                //如果 背包中有任务需要的物品
                if (InventroyManager.Instance.QuestItemInBag(reward.itemData) != null)
                {
                    //>如果背包中的物品数量 不够 任务中需要的物品数量
                    if (InventroyManager.Instance.QuestItemInBag(reward.itemData).amount <= requireCount)
                    {
                        //将目标数量先减少 此时requireCount还有剩余
                        requireCount -= InventroyManager.Instance.QuestItemInBag(reward.itemData).amount;
                        //背包中的数量归零
                        InventroyManager.Instance.QuestItemInBag(reward.itemData).amount = 0;
                        //如果快捷栏中还有 此物品 从快捷栏中获取 对应减少数量
                        if (InventroyManager.Instance.QuestItemInAction(reward.itemData) != null)
                            InventroyManager.Instance.QuestItemInAction(reward.itemData).amount -= requireCount;
                    }
                    else //背包的数量足够 直接从背包中扣除
                    {
                        InventroyManager.Instance.QuestItemInBag(reward.itemData).amount -= requireCount;
                    }
                }   //快捷栏中有任务需要的物品 从快捷栏中扣除
                else
                {
                    InventroyManager.Instance.QuestItemInAction(reward.itemData).amount -= requireCount;
                }
            }
            else//>reward.amount > 0 任务给予物品奖励
            {
                InventroyManager.Instance.inventoryData.AddItem(reward.itemData, reward.amount);
            }
            //添加 更新UI 
            InventroyManager.Instance.Inventroy_ContainerUI.RefreshHolderUI();
            InventroyManager.Instance.ActionBar_ContainerUI.RefreshHolderUI();
        }
    }

    //将当前任务中 包含的任务目标的 目标名 添加进列表当中 
    //返回这个列表
    public List<string> RequireTargetName()
    {
        List<string> targetNameList = new List<string>();
        foreach (var require in questRequiresList)
        {
            targetNameList.Add(require.name);
        }
        return targetNameList;
    }
}
