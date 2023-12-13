using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//���͵���ģʽ
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            return instance;
        }
    }
    //��Awake����ʵ�����������
    protected virtual void Awake()
    {
        //�����еĵ����������ظ�
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = (T)this;
        }
    }
    //��⵱ǰ�����Ƿ��Ѿ�����
    public static bool IsInitialized
    {
        get { return instance != null; }
    }

    protected virtual void OnDestory()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
