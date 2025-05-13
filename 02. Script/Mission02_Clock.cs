using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission02_Clock : MonoBehaviour
{
    public string ClockTime;
    public Clock Clock_sc;
    void Awake()
    {
        Clock_sc = this.gameObject.transform.GetChild(0).GetComponent<Clock>(); // 위치 값(Vector3)을 저장
    }
    public void ClockTowerSetting(int hour, int minute)
    {
        Clock_sc.hour = hour; // 시계탑 시계 시
        Clock_sc.minutes = minute; // 시계탑 시계 분
        ClockTime = $"{hour:D2}:{minute:D2}"; // 퀴즈 시,분 텍스트 설정
    }
    public IEnumerator _CheckAnswer_Correct()
    {
        SoundManager.instance.PlaySFX("success01");
        GameManager.instance.npcAnimator.SetTrigger("applaud");
        GameManager.instance.CanTouch = false; // 터치 불가능
        yield return CoroutineRunner.instance.RunAndWait("mission1",
     NarrationManager.instance.ShowNarrationAuto("정답이에요! 정말 잘했어요!", StringKeys.MISSION2_AUDIO3));
        yield return new WaitForSeconds(1.5f);
        Mission02_DataManager.instance.isAnswer = true; // 정답
    }
    public IEnumerator _CheckAnswer_Wrong()
    {
        SoundManager.instance.PlaySFX("wrong01");
        GameManager.instance.npcAnimator.SetTrigger("no");
        yield return CoroutineRunner.instance.RunAndWait("mission1",
          NarrationManager.instance.ShowNarrationAuto("괜찮아요! 다시 한 번 해볼까요?", StringKeys.MISSION2_AUDIO4));
    }
}
