using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//>NPC Trigger  
public class DialogueTrigger : MonoBehaviour
{
    //当前的NPC的对话数据 
    public Dialogue_Data_SO DialogueCurrentData;
    //是否可以对话 默认false 
    bool canTalk = false;

    #region 开启对话 条件判断
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && DialogueCurrentData != null)
        {
            //满足对话触发条件
            canTalk = true;
        }
    }
    private void Update()
    {
        //按下鼠标右键 开启对话
        if (canTalk && Input.GetMouseButtonDown(1))
        {
            Opendialogue();
        }
    }

    #endregion

    //打开对话
    void Opendialogue()
    {
        DialogueUI_Manager.Instance.DestroyOptions();
        //传入当前NPC的对话数据
        DialogueUI_Manager.Instance.GetDialogueData(DialogueCurrentData);
        //传入对话语句 默认从第一条语句开始
        DialogueUI_Manager.Instance.UpdateMainDialogue(DialogueCurrentData.piecesList[0]);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueUI_Manager.Instance.dialoguePanel.gameObject.SetActive(false);
            canTalk = false;
        }
    }
}
