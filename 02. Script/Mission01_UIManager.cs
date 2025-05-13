using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Mission01_UIManager : MonoBehaviour
{
    [SerializeField] private Mission01_DataManager dataManager;
    public Transform MovePos; //�̵��� ��ġ
    public ClockTowerCtrl[] ClockTowerCtrl_sc; //ClockTowerCtrl
    public GameObject[] ChoiceClocks; //������ �ð� ������Ʈ��
    [Header("�̵� �ð� (��)")]
    private float moveDuration = 0.5f;

    public TextMeshProUGUI QuestionText; //���� �ؽ�Ʈ
    //���ϴ� ��ġ�� �̵��ϴ� �޼���
    public void MoveToTarget(Transform clockTower)
    {
        if (MovePos != null)
        {
            // DOTween Sequence ����
            Sequence moveAndScale = DOTween.Sequence();

            moveAndScale.Append(clockTower.DOMove(MovePos.position, moveDuration).SetEase(Ease.InOutQuad));
            moveAndScale.Join(clockTower.DOScale(new Vector3(0.4f, 0.4f, 0.4f), moveDuration).SetEase(Ease.InOutQuad)); // ���ϴ� ������ ������ ����
            moveAndScale.OnComplete(() =>
            {
                dataManager.ActiveChoiceClock(true); // ������ �ð� Ȱ��ȭ
            });
        }
    }
    public void MoveToTarget1(Transform clockTower)
    {
        if (MovePos != null)
        {
            // ��� ��ġ�� ������ ����
            clockTower.position = MovePos.position;
            clockTower.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            dataManager.ActiveChoiceClock(true); // ������ �ð� Ȱ��ȭ
        }
    }


    //���ϴ� ��ġ�� �̵��ϴ� �޼���
    public void MoveToTargetReverse(Transform clockTower)
    {
        // ���� ��ġ Vector3 ��������
        Vector3 reversePos = clockTower.GetComponent<ClockTowerCtrl>().OriginalPos;
        // ��� ��ġ�� ������ ����
        clockTower.position = reversePos;
        clockTower.localScale = new Vector3(0.2870494f, 0.2870494f, 0.2870494f);
        dataManager.ActiveChoiceClock(false); // ������ �ð� ��Ȱ��ȭ
    }

}
