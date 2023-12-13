using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//>���������ж�
public class TransitionPoint : MonoBehaviour
{
    //>�������� ��ǰ������ͬ�������ͻ����쳡������
    public enum TransitionType
    {
        SameScene,
        DifferentScene
    }
    [Header("Transition Info")]
    public TransitionType transitionType;

    //����ָ������
    public string sceneName;

    //���͵�Ŀ�ĵ��յ��Tag
    public Destination.DestinationTag destinationTag;

    private bool canTrans;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canTrans)//��ֹ�ڳ����κ�λ�� ����㰴��E���ᴫ��
        {
            //�����յ�λ�� ���д���
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    //���Player�Ƿ��ڴ����ŷ�Χ��
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
