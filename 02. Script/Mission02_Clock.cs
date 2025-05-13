using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission02_Clock : MonoBehaviour
{
    public string ClockTime;
    public Clock Clock_sc;
    void Awake()
    {
        Clock_sc = this.gameObject.transform.GetChild(0).GetComponent<Clock>(); // ��ġ ��(Vector3)�� ����
    }
    public void ClockTowerSetting(int hour, int minute)
    {
        Clock_sc.hour = hour; // �ð�ž �ð� ��
        Clock_sc.minutes = minute; // �ð�ž �ð� ��
        ClockTime = $"{hour:D2}:{minute:D2}"; // ���� ��,�� �ؽ�Ʈ ����
    }
    public IEnumerator _CheckAnswer_Correct()
    {
        SoundManager.instance.PlaySFX("success01");
        GameManager.instance.npcAnimator.SetTrigger("applaud");
        GameManager.instance.CanTouch = false; // ��ġ �Ұ���
        yield return CoroutineRunner.instance.RunAndWait("mission1",
     NarrationManager.instance.ShowNarrationAuto("�����̿���! ���� ���߾��!", StringKeys.MISSION2_AUDIO3));
        yield return new WaitForSeconds(1.5f);
        Mission02_DataManager.instance.isAnswer = true; // ����
    }
    public IEnumerator _CheckAnswer_Wrong()
    {
        SoundManager.instance.PlaySFX("wrong01");
        GameManager.instance.npcAnimator.SetTrigger("no");
        yield return CoroutineRunner.instance.RunAndWait("mission1",
          NarrationManager.instance.ShowNarrationAuto("�����ƿ�! �ٽ� �� �� �غ����?", StringKeys.MISSION2_AUDIO4));
    }
}
