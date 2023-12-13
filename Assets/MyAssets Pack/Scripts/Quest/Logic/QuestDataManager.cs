using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//>管理Player接受到的任务数据
public class QuestDataManager : Singleton<QuestDataManager>
{
    //>存放单个接受到的任务
    [System.Serializable]
    public class QuestTask
    {
        public QuestData_SO questData;
        //直接修改任务的进度
        public bool IsStarted { get { return questData.isStarted; } set { questData.isStarted = value; } }
        public bool IsComplete { get { return questData.isComplete; } set { questData.isComplete = value; } }
        public bool IsFinished { get { return questData.isFinished; } set { questData.isFinished = value; } }
    }
    //>存放Player所有 接受到的任务
    //存饭任务的List并不是SO类型 需要单独来保存
    public List<QuestTask> playertasksList = new List<QuestTask>();

    private void Start()
    {
        LoadQuestData();
    }

    #region 保存和加载任务数据
    public void LoadQuestData()
    {
        //Array 读取保存的任务数量
        var questCount = PlayerPrefs.GetInt("QuestCount");
        //读取数据 创建一个新的SO 将数据覆盖 然后添加到player任务列表当中
        for (int i = 0; i < questCount; i++)
        {
            var newQuest = ScriptableObject.CreateInstance<QuestData_SO>();
            SaveManager.Instance.Load(newQuest, "task" + i);
            playertasksList.Add(new QuestTask { questData = newQuest });
        }
    }

    public void SaveQuestData()
    {
        //保存List当中任务的数量
        PlayerPrefs.SetInt("QuestCount", playertasksList.Count);
        //保存的是QuestTask类中的questData 以及它在List当中对应的序号
        //>按照数据的需要保存数据 序号和数据 一 一 对应
        for (int i = 0; i < playertasksList.Count; i++)
        {
            SaveManager.Instance.Save(playertasksList[i].questData, "task" + i);
        }
    }

    #endregion

    //>查找当前已经被接受的任务列表中 是否包含某个任务
    public bool HaveQuest(QuestData_SO data)
    {
        if (data != null)
        {
            return playertasksList.Any(q => q.questData.questName == data.questName);
        }
        else
        {
            return false;
        }
    }

    //>得到列表中的某个任务 用于修改列表中某个任务的状态
    public QuestTask GetTask(QuestData_SO data)
    {
        return playertasksList.Find(q => q.questData.questName == data.questName);
    }

    //>更新任务进度(更新player任务列表中 任务目标的数量)
    //调用: 敌人死亡 拾取物品 使用物品 检查背包
    public void UpdateQuestProgress(string requireName, int amount)//目标名 目标数量
    {
        //遍历player的任务列表 
        foreach (var task in playertasksList)
        {
            //任务完成 直接跳过 不做检测
            if (task.IsFinished)
                continue;

            //>在player任务列表中 寻找含有此目标的任务 修改任务中 该目标的数量
            var matchTask = task.questData.questRequiresList.Find(r => r.name == requireName);
            //修改任务中 此目标的数量
            if (matchTask != null)
                matchTask.currentAmount += amount;

            //每个任务都要更新任务进度
            task.questData.CheckQuestProgress();
        }


    }
}
