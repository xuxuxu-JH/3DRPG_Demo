using UnityEngine;
public static class ExtensionMethod
{
    private const float doThreshold = 0.5f;
    /// <summary>
    /// 拓展方法 用于Enemy判断Player是否在正面方向
    /// </summary>
    public static bool IsFacinTarget(this Transform transform, Transform target)
    {
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        float dot = Vector3.Dot(transform.forward, vectorToTarget);//点乘叉乘
        //>如果目标大于0.5 说明在面前
        return dot >= doThreshold;
    }
}
