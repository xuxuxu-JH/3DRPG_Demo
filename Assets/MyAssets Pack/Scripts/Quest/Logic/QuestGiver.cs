using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//>NPC
//>切换NPC的对话
[RequireComponent(typeof(DialogueTrigger))]
public class QuestGiver : MonoBehaviour
{
    DialogueTrigger dialogueTrigger;
    //当前NPC的任务
    QuestData_SO currentQuestData;

    //不同状态下的对话数据
    public Dialogue_Data_SO stratDialogue;
    public Dialogue_Data_SO progressDialogue;
    public Dialogue_Data_SO completeDialogue;
    public Dialogue_Data_SO finishDialogue;

    #region 返回player中任务的状态
    public bool IsStarted
    {
        get
        {
            //在player列表中查找是否包含此项任务
            //返回player中 此项任务 修改状态
            if (QuestDataManager.Instance.HaveQuest(currentQuestData))
                return QuestDataManager.Instance.GetTask(currentQuestData).IsStarted;
            else
                return false;
        }
    }

    public bool IsComplete
    {
        get
        {
            if (QuestDataManager.Instance.HaveQuest(currentQuestData))
                return QuestDataManager.Instance.GetTask(currentQuestData).IsComplete;
            else
                return false;
        }
    }

    public bool IsFinished
    {
        get
        {
            if (QuestDataManager.Instance.HaveQuest(currentQuestData))
                return QuestDataManager.Instance.GetTask(currentQuestData).IsFinished;
            else
                return false;
        }
    }

    #endregion
    public void Awake()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
    }
    private void Start()
    {
        //刚开始时 将NPC的对话数据切换为 开始部分
        dialogueTrigger.DialogueCurrentData = stratDialogue;
        //得到当前对话下包含的任务
        currentQuestData = dialogueTrigger.DialogueCurrentData.GetQuest();
    }

    private void Update()
    {
        //>根据player当前任务的状态 实时切换NPC的对话
        if (IsStarted)
        {
            //任务完成 但是还没结束(未领取任务奖励)
            if (IsComplete)
            {
                dialogueTrigger.DialogueCurrentData = completeDialogue;
            }
            else
            {//任务进行中
                dialogueTrigger.DialogueCurrentData = progressDialogue;
            }
        }

        if (IsFinished)
        {
            dialogueTrigger.DialogueCurrentData = finishDialogue;
        }
    }
}
