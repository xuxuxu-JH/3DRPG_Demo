using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
/// <summary>
/// >MouseManager 主要负责鼠标的点击事件相关的逻辑  
/// </summary>
public class MouseManager : Singleton<MouseManager>
{
    //鼠标指针的变量 Ins赋值
    public Texture2D point, doorway, attack, target, arrow;

    //用于接收 鼠标点击射线返回的out参数 
    RaycastHit hitInfo;
    //>鼠标点击事件 (只注册了MoveToTarget)
    public event Action<Vector3> OnMouseClicked;
    //>敌人点击事件 ()
    public event Action<GameObject> OnEnemyClicked;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        SetCursorTexture();

        //如果鼠标在和UI进行交互 直接return
        if (InteractWithUI())
        {
            return;//如果在和UI在交互 直接返回 不会进行人物的移动
        }

        //实时进行鼠标点击判断
        MouseControl();
    }

    // >切换鼠标指针的贴图 (射线检测)
    void SetCursorTexture()
    {
        //如果在UI上显示 直接设置为默认指针
        if (InteractWithUI())
        {
            Cursor.SetCursor(arrow, Vector2.zero, CursorMode.Auto);
            return;
        }

        //将屏幕(主摄像机)上的点(鼠标)转换为射线 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //>在Update中实时发射射线 并进行检测
        if (Physics.Raycast(ray, out hitInfo))
        {
            //>hitinfo射线返回信息 根据射线射中物体的tag 改变texture 
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);//16,16 指针偏移位置
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Attackable":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Item":
                    Cursor.SetCursor(point, new Vector2(16, 16), CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }

    //在SetCursorTexture()函数中 已经通过鼠标位置发射了射线 
    //>基于这个射线击中的物体返回的tag信息 比较tag 通过不同的tag 触发对应的事件 执行对应的逻辑
    void MouseControl()
    {
        //>鼠标左键 每按下一次 基于射线返回信息 进行一次判断
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            switch (hitInfo.collider.gameObject.tag)
            {
                //允许Player移动到 地面 传送门 游戏内物品道具
                //>MoveToTarget
                case "Ground":
                case "Portal":
                case "Item":
                    OnMouseClicked?.Invoke(hitInfo.point);
                    break;
                //>MoveToAttackTarget
                case "Enemy":
                    OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
                    break;
                case "Attackable":
                    GameManager.Instance.playerCharacterData.attackData.attackRange = 2;
                    OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
                    break;
            }
        }
    }
    //>判断当前鼠标是否正在和UI进行互动 
    bool InteractWithUI()
    {
        //如果当前鼠标在和UI面板进行交互 且 在鼠标是在UI面板上的
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;
        else
            return false;
    }
}
