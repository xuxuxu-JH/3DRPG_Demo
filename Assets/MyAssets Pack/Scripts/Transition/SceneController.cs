using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
//>负责传送逻辑 包含同场景传送和异场景传送 异步加载
public class SceneController : Singleton<SceneController>, IEndGameObserver
{
    public GameObject playerPrefab;
    //淡入淡出
    public SceneFader sceneFaderPrefab;

    bool fadeFinished;

    GameObject player;
    NavMeshAgent plyaerAgent;

    Vector3 playerPosition;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        //将SceneController注册为观察者 实现Player死亡返回主菜单
        GameManager.Instance.AddObserver(this);
        fadeFinished = true;
    }
    #region 游戏中进行传送
    //>传送到目的地 根据传送门提供的传送类型 
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            //同场景内传送
            case TransitionPoint.TransitionType.SameScene:
                //SceneManager.GetActiveScene().name 得到当前激活的场景名 也就是当前场景
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                //异场景传送
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    //>传送场景名 传送目的地标签
    IEnumerator Transition(string sceneName, Destination.DestinationTag destinationTag)
    {
        //>转场景前 先保存数据
        SaveManager.Instance.SavePlayerData();
        InventroyManager.Instance.SaveInventoryData();
        QuestDataManager.Instance.SaveQuestData();

        //>异场景传送
        if (SceneManager.GetActiveScene().name != sceneName)//如果当前场景名和传入场景名不同 为异
        {
            //加载前场景淡入淡出
            SceneFader fade = Instantiate(sceneFaderPrefab);
            yield return StartCoroutine(fade.FadeOut(1.5f));

            //异步加载 先加载场景
            yield return SceneManager.LoadSceneAsync(sceneName);
            //在指定位置生成Player
            //yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            //>异场景转换后 需要加载player数据
            SaveManager.Instance.LoadPlayerData();
            yield return StartCoroutine(fade.FadeIn(1.5f));
            yield break;
        }
        else//>同场景传送
        {
            //得到Player的GameObject赋值 
            player = GameManager.Instance.playerCharacterData.gameObject;
            //在传送前 停止Player的Agent工作
            plyaerAgent = player.GetComponent<NavMeshAgent>();
            plyaerAgent.enabled = false;

            //得到相同Tag值的终点后 将Player设置为Destination的角度和位置
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);

            plyaerAgent.enabled = true;
            yield return null;
        }

    }

    //>得到相匹配Tag值的Destination
    private Destination GetDestination(Destination.DestinationTag destinationTag)
    {
        //>找到场景中的所有的Destion 看那个一Tag值是相匹配的
        var entrances = FindObjectsOfType<Destination>();
        for (int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].destinationTag == destinationTag)
            {
                return entrances[i];
            }
        }
        return null;
    }

    #endregion


    #region 主菜单传送
    //>NewGame
    public void Transition_ToGameStart()
    {
        StartCoroutine(LoadLevel("Game"));
    }

    //>Continue
    public void Transition_ToLoadGame()
    {
        //从PlayerPrefs中读取已经保存的场景名 传入加载对应的场景
        StartCoroutine(LoadLevel2(SaveManager.Instance.SceneName));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        //在转场景的时生成FadeCanvas
        SceneFader fade = Instantiate(sceneFaderPrefab);

        if (sceneName != "")
        {
            yield return StartCoroutine(fade.FadeOut(1.5f));
            yield return SceneManager.LoadSceneAsync(sceneName);
        }

        SaveManager.Instance.SavePlayerData();

        yield return StartCoroutine(fade.FadeIn(1.5f));
        yield break;
    }
    //Continue Button
    IEnumerator LoadLevel2(string sceneName)
    {

        //在转场景的时生成FadeCanvas
        SceneFader fade = Instantiate(sceneFaderPrefab);
        //加载场景的时白屏
        yield return StartCoroutine(fade.FadeOut(1.5f));

        if (sceneName != "")
        {
            //加载场景
            yield return SceneManager.LoadSceneAsync(sceneName);
        }
        //加载新游戏的数据
        SaveManager.Instance.LoadPlayerData();
        InventroyManager.Instance.LoadInventoryData();
        SaveManager.Instance.LoadPlayerPosition();
        //加载完后恢复透明
        yield return StartCoroutine(fade.FadeIn(1.5f));
        yield break;

    }
    #endregion


    #region Esc 返回主菜单
    public void TransitionToMainMenu()
    {
        SaveManager.Instance.SavePlayerData();
        InventroyManager.Instance.SaveInventoryData();
        QuestDataManager.Instance.SaveQuestData();
        StartCoroutine(LoadMain());
    }
    IEnumerator LoadMain()
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(1.5f));

        yield return SceneManager.LoadSceneAsync("Menu");

        yield return StartCoroutine(fade.FadeIn(1.5f));
        yield break;
    }
    #endregion

    public void EndNotify()
    {
        //通知观察者是在Update中执行的 fadeFinished保证了 只开启一次协程
        if (fadeFinished)
        {
            fadeFinished = false;
        }
    }

    public void StartLoadMain()
    {
        StartCoroutine(LoadMain());
    }
}
