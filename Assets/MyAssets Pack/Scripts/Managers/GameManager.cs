using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
//>��¼������ݱ���ȫ�ֵ���,ʵ�ֹ۲���:����Player�������¼�,���/�Ƴ��۲���,֪ͨ���й۲���,������ĸ���
public class GameManager : Singleton<GameManager>
{
    public CinemachineFreeLook followCamera;
    //>����ȫ��ʹ��playerData                                             
    public CharacterData playerCharacterData;

    //>װ�����еĹ۲��� �̳���IEndGameObserver�ӿڵĵ���
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    protected override void Awake()
    {
        DontDestroyOnLoad(this);
        base.Awake();
    }
    //����������� 
    public void RigisterPlayer(CharacterData player)
    {
        //�õ�Player��CS
        playerCharacterData = player;

        //>ת������������ĸ���
        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if (followCamera != null)
        {
            //>����������ĸ������͹۲����ΪLookAtPoint
            followCamera.Follow = playerCharacterData.transform.GetChild(3);
            followCamera.LookAt = playerCharacterData.transform.GetChild(3);
        }
    }

    //���&�Ƴ��۲��ߵ��б���ȥ
    //>���е���������ʱ ������ӵ��۲����б���
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    //>�����۲����б� ֪ͨ���й۲��� 
    //PlayerController
    public void NotifyObservers()
    {
        foreach (var observer in endGameObservers)
        {
            //�۲���ִ�и��ԵĽ�������
            observer.EndNotify();
        }
    }

    //�õ�layer�ĳ�������� ������ڵ�transformλ��
    public Transform GetEntrance()
    {
        foreach (var item in FindObjectsOfType<Destination>())
        {
            if (item.destinationTag == Destination.DestinationTag.GameStart)
                Debug.Log("�ҵ���");
            return item.transform;
        }
        return null;
    }
}
