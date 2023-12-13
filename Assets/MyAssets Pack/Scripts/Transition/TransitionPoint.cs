using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//>传送条件判断
public class TransitionPoint : MonoBehaviour
{
    //>传送类型 当前传送是同场景传送还是异场景传送
    public enum TransitionType
    {
        SameScene,
        DifferentScene
    }
    [Header("Transition Info")]
    public TransitionType transitionType;

    //传送指定场景
    public string sceneName;

    //传送的目的地终点的Tag
    public Destination.DestinationTag destinationTag;

    private bool canTrans;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canTrans)//防止在场景任何位置 任意点按下E都会传送
        {
            //传入终点位置 进行传送
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    //检测Player是否在传送门范围内
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = false;
        }
    }

}
