using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//>对话选项
public class Option_UI : MonoBehaviour
{
    public Text optionText;
    //当前option
    private Button optionButton;
    //当前选项所匹配的对话
    private DialoguePiece currentPiece;

    private bool takeQuest;
    //跳转至下一条语句ID
    private string nextPieceID;

    private void Awake()
    {
        optionButton = GetComponent<Button>();
        optionButton.onClick.AddListener(OnOptionClicked);
    }

    //>获取语句中的选项数据 (语句中包含 选项的数据) 
    public void UpdateOption(DialoguePiece piece, DialogueOption pieceOption)
    {
        currentPiece = piece;
        optionText.text = pieceOption.text;

        //当前选项中的目标跳转ID
        nextPieceID = pieceOption.targetID;

        //获取 当前选项是否会接受到任务(是否勾选)
        takeQuest = pieceOption.takeQuest;
    }

    //>点击选项按钮 (语句跳转 && 接受任务)
    public void OnOptionClicked()
    {
        #region 点击接收任务 将当前piece中的任务添加到任务接受列表当中
        //当前语句包含任务
        if (currentPiece.pieceQuestData != null)
        {
            //类型转换 复制一份
            var newTask = new QuestDataManager.QuestTask
            {
                questData = Instantiate(currentPiece.pieceQuestData)
            };
            //勾选 点击当前选项会接受任务
            if (takeQuest)
            {
                //传入当前语句中的任务 查找是否已经被接受过
                if (QuestDataManager.Instance.HaveQuest(currentPiece.pieceQuestData))
                {
                    //>如果已经接受 判断任务是否完成 给予奖励
                    if (QuestDataManager.Instance.GetTask(newTask.questData).IsComplete)
                    {
                        newTask.questData.GiveRewards();
                        //>任务结束
                        QuestDataManager.Instance.GetTask(newTask.questData).IsFinished = true;
                    }
                }
                //>没有 将新任务添加到任务列表当中
                else
                {
                    //将新任务添加到Player任务列表当中
                    QuestDataManager.Instance.playertasksList.Add(newTask);
                    //>修改刚添加进列表中的任务的状态 任务开始
                    QuestDataManager.Instance.GetTask(newTask.questData).IsStarted = true;

                    //在接受新任务的时候 先检查背包中是否已经包含了任务中需求物品
                    foreach (var requireItemName in newTask.questData.RequireTargetName())
                    {
                        InventroyManager.Instance.CheckQuestItemInBag(requireItemName);
                    }
                }
            }
        }
        #endregion

        #region 点击进行对话
        //>如果不包含下一句ID 说明该选项直接结束对话 
        if (nextPieceID == "" || nextPieceID == null)
        {
            DialogueUI_Manager.Instance.dialoguePanel.SetActive(false);
            return;
        }
        //>如果包含 下一句的对话ID 进行语句跳转 
        else
        {
            //通过ID从字典中取出语句
            DialogueUI_Manager.Instance.UpdateMainDialogue(DialogueUI_Manager.Instance.currentDialogueData.pieceDic[nextPieceID]);
        }
        #endregion
    }
}
