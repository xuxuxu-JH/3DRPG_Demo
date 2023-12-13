using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//>对话数据_SO
[CreateAssetMenu(fileName = "New Talk", menuName = "Dialogue/Dialogue Data")]
public class Dialogue_Data_SO : ScriptableObject
{
    //>List存储所有的对话语句
    public List<DialoguePiece> piecesList = new List<DialoguePiece>();
    //>通过字典,将自身ID和语句相绑定
    public Dictionary<string, DialoguePiece> pieceDic = new Dictionary<string, DialoguePiece>();

#if UNITY_EDITOR
    //Ins每次修改这个类值的时候 都会执行该函数 这是一个编辑器的内置函数
    private void OnValidate()
    {
        pieceDic.Clear();
        //将自身的ID 和 语句相匹配
        foreach (var piece in piecesList)
        {
            if (!pieceDic.ContainsKey(piece.ID))
                pieceDic.Add(piece.ID, piece);
        }
    }
#endif

    public QuestData_SO GetQuest()
    {
        QuestData_SO currentQuest = null;
        foreach (var piece in piecesList)
        {
            if (piece.pieceQuestData != null)
                currentQuest = piece.pieceQuestData;
        }
        return currentQuest;
    }

}
