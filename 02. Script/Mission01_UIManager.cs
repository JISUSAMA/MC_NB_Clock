using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Mission01_UIManager : MonoBehaviour
{
    [SerializeField] private Mission01_DataManager dataManager;
    public Transform MovePos; //이동할 위치
    public ClockTowerCtrl[] ClockTowerCtrl_sc; //ClockTowerCtrl
    public GameObject[] ChoiceClocks; //선택지 시계 오브젝트들
    [Header("이동 시간 (초)")]
    private float moveDuration = 0.5f;

    public TextMeshProUGUI QuestionText; //질문 텍스트
    //원하는 위치로 이동하는 메서드
    public void MoveToTarget(Transform clockTower)
    {
        if (MovePos != null)
        {
            // DOTween Sequence 생성
            Sequence moveAndScale = DOTween.Sequence();

            moveAndScale.Append(clockTower.DOMove(MovePos.position, moveDuration).SetEase(Ease.InOutQuad));
            moveAndScale.Join(clockTower.DOScale(new Vector3(0.4f, 0.4f, 0.4f), moveDuration).SetEase(Ease.InOutQuad)); // 원하는 스케일 값으로 변경
            moveAndScale.OnComplete(() =>
            {
                dataManager.ActiveChoiceClock(true); // 선택지 시계 활성화
            });
        }
    }
    public void MoveToTarget1(Transform clockTower)
    {
        if (MovePos != null)
        {
            // 즉시 위치와 스케일 설정
            clockTower.position = MovePos.position;
            clockTower.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            dataManager.ActiveChoiceClock(true); // 선택지 시계 활성화
        }
    }


    //원하는 위치로 이동하는 메서드
    public void MoveToTargetReverse(Transform clockTower)
    {
        // 원래 위치 Vector3 가져오기
        Vector3 reversePos = clockTower.GetComponent<ClockTowerCtrl>().OriginalPos;
        // 즉시 위치와 스케일 적용
        clockTower.position = reversePos;
        clockTower.localScale = new Vector3(0.2870494f, 0.2870494f, 0.2870494f);
        dataManager.ActiveChoiceClock(false); // 선택지 시계 비활성화
    }

}
