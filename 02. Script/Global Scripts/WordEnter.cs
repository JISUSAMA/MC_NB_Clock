using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordEnter : MonoBehaviour // �̰� �ܾ� �� �� �� ���� �ִ´�.
{
    [SerializeField] private Mission01_DataManager dataManager;
    [SerializeField] private Transform activeChild;

    private Clock clockComponent;
    private int hour;
    private int minute;

    public bool isin = false;

    private string AnswerStr, ObjStr;

    private void Awake()
    {
        // ������ �Ŵ��� ã��
        dataManager = GameObject.FindAnyObjectByType<Mission01_DataManager>();

        // �ڽ� ������Ʈ�� ���� ��쿡�� activeChild �ʱ�ȭ
        if (transform.childCount > 0)
        {
            activeChild = transform.GetChild(0);
            clockComponent = activeChild.GetComponent<Clock>();

            if (clockComponent != null)
            {
                hour = clockComponent.hour;
                minute = clockComponent.minutes;
            }
            else
            {
                Debug.LogWarning($"{activeChild.name}���� Clock ������Ʈ�� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}���� �ڽ��� �����ϴ�.");
        }
    }

    private void OnEnable()
    {
        if (clockComponent != null)
        {
            hour = clockComponent.hour;
            minute = clockComponent.minutes;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        CompareAnswer(other);
        if (GameObject.FindAnyObjectByType<TouchObjectDetector>().isDragging)
        {
            Debug.Log($"stay : {AnswerStr} {ObjStr}");
            if (AnswerStr == ObjStr)
            {
                Debug.Log("stay");
                isin = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CompareAnswer(other);
        if (AnswerStr != ObjStr)
        {
            Debug.Log("exit");
            isin = false;
            StartCoroutine(dataManager._CheckAnswer_Wrong());
        }
        if (other.gameObject.CompareTag(StringKeys.TOWER_TAG))
        {
            if (GameObject.FindAnyObjectByType<TouchObjectDetector>().isinOut)
            {
                if (AnswerStr == ObjStr)
                {
                    Debug.Log($"���� : {AnswerStr} {ObjStr}");
                    other.transform.GetComponent<BoxCollider>().enabled = false;
                    StartCoroutine(dataManager._CheckAnswer_Correct(AnswerStr));
                }
                else if (AnswerStr != ObjStr)
                {
                    Debug.Log($"Ʋ�� : {AnswerStr} {ObjStr}");
                    StartCoroutine(dataManager._CheckAnswer_Wrong());
                }
            }
        }

        Debug.Log("Exit isinOut : " + GameObject.FindAnyObjectByType<TouchObjectDetector>().isinOut);
    }

    private void CompareAnswer(Collider other)
    {
        Debug.Log(other.gameObject.name);

        if (other.transform.childCount > 0)
        {
            Transform answerObject = other.transform.GetChild(0);
            Clock answerClock = answerObject.GetComponent<Clock>();

            if (answerClock != null)
            {
                int answerHour = answerClock.hour;
                int answerMinute = answerClock.minutes;

                AnswerStr = $"{answerHour}_{answerMinute}";
                ObjStr = $"{hour}_{minute}";
            }
            else
            {
                Debug.LogWarning("��� ������Ʈ�� Clock ������Ʈ�� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("��� ������Ʈ�� �ڽ��� �����ϴ�.");
        }
    }
}
