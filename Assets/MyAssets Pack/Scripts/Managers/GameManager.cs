using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
//>记录玩家数据便于全局调用,实现观察者:监听Player的死亡事件,添加/移除观察者,通知所有观察者,摄像机的跟随
public class GameManager : Singleton<GameManager>
{
    public CinemachineFreeLook followCamera;
    //>便于全局使用playerData                                             
    public CharacterData playerCharacterData;

    //>装载所有的观察者 继承了IEndGameObserver接口的敌人
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    protected override void Awake()
    {
        DontDestroyOnLoad(this);
        base.Awake();
    }
    //监听玩家死亡 
    public void RigisterPlayer(CharacterData player)
    {
        //得到Player的CS
        playerCharacterData = player;

        //>转场景设置相机的跟随
        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if (followCamera != null)
        {
            //>设置摄像机的跟随对象和观察对象都为LookAtPoint
            followCamera.Follow = playerCharacterData.transform.GetChild(3);
            followCamera.LookAt = playerCharacterData.transform.GetChild(3);
        }
    }

    //添加&移除观察者到列表当中去
    //>所有敌人在启用时 都会添加到观察者列表当中
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    //>遍历观察者列表 通知所有观察者 
    //PlayerController
    public void NotifyObservers()
    {
        foreach (var observer in endGameObservers)
        {
            //观察者执行各自的结束表现
            observer.EndNotify();
        }
    }

    //得到layer的出生点入口 返回入口的transform位置
    public Transform GetEntrance()
    {
        foreach (var item in FindObjectsOfType<Destination>())
        {
            if (item.destinationTag == Destination.DestinationTag.GameStart)
                Debug.Log("找到了");
            return item.transform;
        }
        return null;
    }
}
