using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class DialogueUI_Manager : Singleton<DialogueUI_Manager>
{
    [Header("Basic Elements")]
    public Image icon;
    public Text mainText;
    public Button nextButton;
    public GameObject dialoguePanel;//LayoutContronl

    [Header("Options")]
    public RectTransform optionPanel;
    public Option_UI optionPrefab;

    [Header("Data")]
    [HideInInspector]
    public Dialogue_Data_SO currentDialogueData;
    int currentIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        nextButton.gameObject.SetActive(false);
        nextButton.onClick.AddListener(ContinueDialogue);
    }

    //>NextButton 点击下一句
    void ContinueDialogue()
    {
        nextButton.gameObject.SetActive(false);
        //>继续播放列表中下一个对话语句 
        //如果越界 说明对话结束
        if (currentIndex < currentDialogueData.piecesList.Count)
        {
            //下一句
            UpdateMainDialogue(currentDialogueData.piecesList[currentIndex]);
        }
        else
        //结束对话 关闭窗口
        {
            dialoguePanel.SetActive(false);
        }
    }

    //>UI得到对话数据
    public void GetDialogueData(Dialogue_Data_SO dialogue_Data)
    {
        currentDialogueData = dialogue_Data;
        //List 语句Index
        currentIndex = 0;
    }

    //>更新对话
    public void UpdateMainDialogue(DialoguePiece piece)
    {
        //播放下一句
        currentIndex++;

        //>开启对话窗口
        dialoguePanel.SetActive(true);

        //显示对话图片
        if (piece.image != null)
        {
            icon.enabled = true;
            icon.sprite = piece.image;
        }
        else icon.enabled = false;

        //播放文本
        mainText.text = "";
        //监听对话播放 对话播放完成后 再显示Next
        mainText.DOText(piece.text, 1.5f).OnComplete(() => DoTextComplete());

        #region Next 或 Options
        void DoTextComplete()
        {
            //>如果是纯对话 激活Next 播放下一句
            if (piece.optionsList.Count == 0 && currentDialogueData.piecesList.Count > 0)
            {
                nextButton.interactable = true;
                nextButton.gameObject.SetActive(true);
                nextButton.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                nextButton.interactable = false;
                nextButton.transform.GetChild(0).gameObject.SetActive(false);
            }

            //>如果不是纯对话 创建Options
            CreatOptions(piece);
        }

        #endregion

    }
    //>创建选项
    void CreatOptions(DialoguePiece piece)
    {
        //在创建新的选项之前 先销毁上一句对话中的所有选项
        DestroyOptions();

        //>生成当前语句中包含的所有选项 每个选项都得到各自的选项数据
        for (int i = 0; i < piece.optionsList.Count; i++)
        {
            var option = Instantiate(optionPrefab, optionPanel);

            option.UpdateOption(piece, piece.optionsList[i]);
        }
    }

    public void DestroyOptions()
    {
        if (optionPanel.childCount > 0)
        {
            for (int i = 0; i < optionPanel.childCount; i++)
            {
                Destroy(optionPanel.GetChild(i).gameObject);
            }
        }
    }
}
