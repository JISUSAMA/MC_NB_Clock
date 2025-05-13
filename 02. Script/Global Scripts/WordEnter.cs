using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordEnter : MonoBehaviour // 이건 단어 한 개 한 개에 넣는다.
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
        // 데이터 매니저 찾기
        dataManager = GameObject.FindAnyObjectByType<Mission01_DataManager>();

        // 자식 오브젝트가 있을 경우에만 activeChild 초기화
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
                Debug.LogWarning($"{activeChild.name}에는 Clock 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}에는 자식이 없습니다.");
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
                    Debug.Log($"정답 : {AnswerStr} {ObjStr}");
                    other.transform.GetComponent<BoxCollider>().enabled = false;
                    StartCoroutine(dataManager._CheckAnswer_Correct(AnswerStr));
                }
                else if (AnswerStr != ObjStr)
                {
                    Debug.Log($"틀림 : {AnswerStr} {ObjStr}");
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
                Debug.LogWarning("상대 오브젝트에 Clock 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("상대 오브젝트에 자식이 없습니다.");
        }
    }
}
