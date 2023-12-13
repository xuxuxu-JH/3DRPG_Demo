using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//转换场景 淡入淡出
public class SceneFader : MonoBehaviour
{
    CanvasGroup canvasGroup;

    public float fadeInDuration;

    public float fadeOutDuration;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        DontDestroyOnLoad(gameObject);
    }
    //伴随着 别的任务一起执行的时候 通常使用协程
    public IEnumerator FadeOutAndIn()
    {
        yield return FadeOut(fadeOutDuration);
        yield return FadeIn(fadeInDuration);
    }
    //>实现淡入淡出主要是控制Canvas的alpha值
    //淡出 由透明逐渐变白
    public IEnumerator FadeOut(float time)
    {
        while (canvasGroup.alpha < 1)
        {
            //time越大 渐变的时间越长
            canvasGroup.alpha += Time.deltaTime / time;
            yield return null;
        }
    }
    //淡入 由白逐渐变为透明
    public IEnumerator FadeIn(float time)
    {
        while (canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.deltaTime / time;
            yield return null;
        }
        //每次切换场景 销毁切换物体
        Destroy(gameObject);
    }
}
