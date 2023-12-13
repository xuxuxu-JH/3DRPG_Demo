using UnityEngine;
using UnityEngine.UI;

//>管理整个Quest任务面板UI
public class QuestUI : Singleton<QuestUI>
{
    [Header("Elements")]
    //整个任务面板 开启关闭
    public GameObject questPanel;
    public ItemTooltip tooltip;
    private bool isOpen;

    [Header("Quest Name")]
    public RectTransform questListTransform;
    public QuestName_Button questName_Button;

    [Header("Text Content")]
    public Text questDescirption;

    [Header("Requirement")]
    public RectTransform requireTransform;
    public QuestRequirement requirement;

    [Header("Reward Panel")]
    public RectTransform rewardTransform;
    public ItemUI rewardUI;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isOpen = !isOpen;
            questPanel.SetActive(isOpen);
            questDescirption.text = string.Empty;
            //>显示任务按钮列表
            SetupQuestList();
            print("当前玩家的任务数量为" + QuestDataManager.Instance.playertasksList.Count);
            if (!isOpen)
                tooltip.gameObject.SetActive(false);
        }
    }
    //>显示任务按钮列表
    public void SetupQuestList()
    {
        #region 打开任务窗口 清楚所有显示 保证显示的任务是最新的
        //销毁任务按钮列表
        foreach (Transform item in questListTransform)
        {
            Destroy(item.gameObject);
        }
        //销毁任务需求列表
        foreach (Transform item in requireTransform)
        {
            Destroy(item.gameObject);
        }
        //销毁任务奖励物品列表
        foreach (Transform item in rewardTransform)
        {
            Destroy(item.gameObject);
        }
        #endregion

        //>根据player的任务列表 生成对应数量的任务按钮
        //得到player任务列表中的包含的所有任务数据 传入到生成的Button中获取
        foreach (var questTask in QuestDataManager.Instance.playertasksList)
        {
            //在List面板下生成Button
            var newTaskButton = Instantiate(questName_Button, questListTransform);
            //传入任务数据 设置按钮的名字
            newTaskButton.SetupNameButton(questTask.questData);
        }
    }

    //>点击任务按钮 显示任务目标 
    //>QuestName_Button -> OnClick
    public void SetupRequireList(QuestData_SO questData)
    {
        //每次点按 都是不同的任务 需要销毁上一个的需求显示
        foreach (Transform item in requireTransform)
        {
            Destroy(item.gameObject);
        }

        //遍历当前任务中的任务目标列表
        foreach (var require in questData.questRequiresList)
        {
            var newRequireMent = Instantiate(requirement, requireTransform);
            //如果当前任务是已完成的情况 面板中的文字显示不同
            if (questData.isFinished)
                newRequireMent.SetupRequirement(require.name, questData.isFinished);
            else
                //如果是未完成
                newRequireMent.SetupRequirement(require.name, require.requireAmount, require.currentAmount);
        }
    }

    //设置同步奖励列表
    public void SetupRewardItem(ItemData_SO itemData, int amount)
    {
        //根据任务RewardList 生成奖励格
        var item = Instantiate(rewardUI, rewardTransform);
        //同步格子显示
        item.SetupItemUI(itemData, amount);
    }

}
