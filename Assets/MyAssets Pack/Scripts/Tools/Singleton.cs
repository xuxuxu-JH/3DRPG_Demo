using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//泛型单列模式
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
    //在Awake里面实例化这个单例
    protected virtual void Awake()
    {
        //场景中的单例对象不能重复
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = (T)this;
        }
    }
    //检测当前单例是否已经生成
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
