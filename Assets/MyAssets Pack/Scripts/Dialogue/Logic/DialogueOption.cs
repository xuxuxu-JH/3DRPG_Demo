using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
//>对话选项
public class DialogueOption
{
    //选项内容
    public string text;
    //该选项跳转的目标语句ID
    public string targetID;

    public bool takeQuest;
}
