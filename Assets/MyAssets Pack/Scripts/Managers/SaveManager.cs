using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
//使用PlayerPrefs配合Json(JsonUtility) 实现数据的持久化 
//SO本质上还是一个类 
public class SaveManager : Singleton<SaveManager>
{
    //key
    string sceneName = "level";

    [Header("PlayerPostion")]
    float positionX;
    float positionY;
    float positionZ;
    private Transform playerTransform;
    private Vector3 playerV3;

    //返回之前保存的场景名
    public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            playerTransform = GameObject.Find("Player").transform;
            SavePlayerPosition(playerTransform);
            SceneController.Instance.TransitionToMainMenu();
        }
    }

    #region 保存和读取Player的数据
    //将player的so文件序列化为json key为so文件的名字 唯一的key对应唯一的json
    public void SavePlayerData()
    {
        Save(GameManager.Instance.playerCharacterData.characterData, GameManager.Instance.playerCharacterData.characterData.name);
        InventroyManager.Instance.SaveInventoryData();
    }
    //通过已知的key 从PlayerPrefs中读取json字符串 再反序列化 覆盖playerdata
    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerCharacterData.characterData, GameManager.Instance.playerCharacterData.characterData.name);
        InventroyManager.Instance.LoadInventoryData();
    }

    public void SavePlayerPosition(Transform playerTransform)
    {
        positionX = playerTransform.position.x;
        positionY = playerTransform.position.y;
        positionZ = playerTransform.position.z;

        PlayerPrefs.SetFloat("X", positionX);
        PlayerPrefs.SetFloat("Y", positionY);
        PlayerPrefs.SetFloat("Z", positionZ);
    }

    public void LoadPlayerPosition()
    {
        playerV3 = new Vector3(PlayerPrefs.GetFloat("X"), PlayerPrefs.GetFloat("Y"), PlayerPrefs.GetFloat("Z"));
        GameObject.Find("Player").transform.SetPositionAndRotation(playerV3, Quaternion.identity);
    }
    #endregion

    #region 通用保存方法
    //>保存数据
    //将类序列化为Json 再通过PlayerPrefs保存 该数据有一个唯一的key值与之对应
    public void Save(Object data, string key)
    {
        string jsonData = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(key, jsonData);
        //key不会发生变化 只保存该key->player当前所在的场景名 
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }

    //>读取数据 
    //>data 需要被写入的数据 ,string 需要读取的数据key
    //通过唯一的key从PlayerPrefs中读取 然后再覆盖 写入到目标的data中
    public void Load(Object data, string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            //反序列化 将已经在PlayerPrefs中包含的数据 写入到对应的data中
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }

    #endregion

}
