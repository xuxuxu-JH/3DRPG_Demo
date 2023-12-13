using System.ComponentModel.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//>Enemy生命值UI相关
public class HealthBarUI : MonoBehaviour
{
    //血量UI条预制体
    public GameObject healthUIPrefab;
    //UI显示的位置
    public Transform barPoint;
    //勾选血条是否保持常见
    public bool alwaysVisible;
    //UI消失显示时间
    public float visiableTime;
    private float timeLeft;

    //得到子物体组件Image 改变的是绿色imgae填充值
    Image healthSlider;

    //生成之后得到UI预制体的Transfrom组件
    Transform UIbar;
    //摄像机的位置
    Transform cam;
    //血量SO信息
    CharacterData currentStats;

    private void Awake()
    {
        currentStats = GetComponent<CharacterData>();

        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }
    //>启动的时候在场景中生成血条
    private void OnEnable()
    {
        cam = Camera.main.transform;
        //找到场景中所有的Canvas
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            //找到世界坐标的Canvas 游戏中只有HealthBar Canvas的渲染模式是World
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                //在Canvas下生成子物体 healthUIPrefab
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform;
                //得到绿色血条Image 
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                //默认不显示 受击后显示
                UIbar.gameObject.SetActive(alwaysVisible);
            }
        }

    }
    //>生成血条后 正确显示
    private void LateUpdate()
    {
        if (UIbar != null)
        {
            //将血条UI显示在人物头顶的正确位置 并实时跟随着敌人的位置
            UIbar.position = barPoint.position;
            //UI面对摄像机
            UIbar.forward = -cam.forward;

            if (timeLeft <= 0 && !alwaysVisible)
                UIbar.gameObject.SetActive(false);
            else
                timeLeft -= Time.deltaTime;
        }
    }

    //>Enemy每次受击时(BeAttack) 更新血条UI
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth == 0)
            Destroy(UIbar.gameObject);
        UIbar.gameObject.SetActive(true);
        timeLeft = visiableTime;
        //得到生命值百分比
        float sliderPercent = (float)currentHealth / maxHealth;
        //根据血量的百分比 改变绿色Image的填充数值
        healthSlider.fillAmount = sliderPercent;
    }
}
