using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//>单条对话
public class DialoguePiece
{
    //对话ID
    public string ID;
    //对话中包含的图片(NPC,Item)
    public Sprite image;

    [TextArea]
    //对话详情
    public string text;

    //>存放当前对话可能包含的选项
    public List<DialogueOption> optionsList = new List<DialogueOption>();

    //>存放 当前对话语句中所包含的任务 
    public QuestData_SO pieceQuestData;


}
