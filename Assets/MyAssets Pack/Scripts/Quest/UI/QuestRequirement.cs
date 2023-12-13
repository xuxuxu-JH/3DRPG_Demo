using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//>Requirement 任务需求组件
public class QuestRequirement : MonoBehaviour
{
    private Text requireName;
    //任务进展  当前数量/总数
    private Text progressNumber;

    private void Awake()
    {
        requireName = GetComponent<Text>();
        progressNumber = transform.GetChild(0).GetComponent<Text>();
    }

    //>任务未完成的情况
    //设置 同步任务需求和任务名字
    public void SetupRequirement(string name, int amount, int currentAmount)
    {
        requireName.text = name;
        progressNumber.text = currentAmount.ToString() + "/" + amount.ToString();
    }
    //>任务已完成的情况
    public void SetupRequirement(string name, bool isFinished)
    {
        if (isFinished)
        {
            requireName.text = name;
            progressNumber.text = "完成";

            requireName.color = Color.green;
            progressNumber.color = Color.green;
        }
    }
}
