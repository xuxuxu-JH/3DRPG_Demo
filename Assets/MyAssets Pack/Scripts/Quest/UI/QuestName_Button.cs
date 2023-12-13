using UnityEngine;
using UnityEngine.UI;
//>任务列表 任务按钮
public class QuestName_Button : MonoBehaviour
{
    public Text questNameText;
    //得到当前任务数据
    public QuestData_SO currentQuestData;
    //>得到任务描述组件
    public Text questContentText;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(UpdateQuestContent);
    }
    //>Button_OnClick
    //点击同步任务数据 切换任务显示 
    void UpdateQuestContent()
    {
        //>同步任务详情
        QuestUI.Instance.questDescirption.text = currentQuestData.description;
        //>同步任务目标
        QuestUI.Instance.SetupRequireList(currentQuestData);

        foreach (Transform item in QuestUI.Instance.rewardTransform)
        {
            Destroy(item.gameObject);
        }

        //>遍历当前任务中的任务奖励列表
        foreach (var item in currentQuestData.rewardsList)
        {
            QuestUI.Instance.SetupRewardItem(item.itemData, item.amount);
        }
    }

    //>设置Button按钮名字和任务名一致
    public void SetupNameButton(QuestData_SO questData)
    {
        //得到当前的任务数据
        currentQuestData = questData;
        //如果任务状态是已完成的 就显示 任务名+已完成
        if (questData.isComplete)
        {
            questNameText.text = questData.questName + "任务已完成";
        }
        if (questData.isFinished)
        {
            questNameText.text = questData.questName + "任务已结束";
        }
        //如果未完成 就显示任务名
        else
        {
            questNameText.text = questData.questName;
        }
    }
}
