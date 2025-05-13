using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Mission02_UIManager : MonoBehaviour
{

    public bool isIntro = false;
    [Header("Mission UI Group")]
    public GameObject MissionGroup;
    public Image QuizImage; // 퀴즈 이미지
    public Sprite[] QuizSpriteList; // 퀴즈 이미지 리스트
    public Mission02_Clock[] ClockObjs_sc; // 시계, 터치 오브젝트들
    public TextMeshProUGUI QuizTimeText; // 퀴즈 시간 텍스트
    [Header("NPC")]
    public Vector3 NPC_originalPos;
    public Vector3 NPC_originalRot;

    void Awake()
    {
        // NPC 원래 위치와 회전값 저장
        NPC_originalPos = GameManager.instance.npcAnimator.gameObject.transform.position;
        NPC_originalRot = GameManager.instance.npcAnimator.gameObject.transform.rotation.eulerAngles;
    }
    public void IntroStart()
    {
        StopCoroutine(_IntroStart());
        StartCoroutine(_IntroStart());
    }
    IEnumerator _IntroStart()
    {
        NarrationManager.instance.TitleOb.SetActive(true);
        NarrationManager.instance.TitleText.text = "순서대로 착착!\n하루 일과 정리하기";
        yield return new WaitForSeconds(2f);
        NarrationManager.instance.TitleOb.SetActive(false);

        // 나레이션 시작
        NarrationManager.instance.ShowDialog();
        GameManager.instance.npcAnimator.SetTrigger("hi");
        yield return CoroutineRunner.instance.RunAndWait("mission2",
        NarrationManager.instance.ShowNarrationAuto("이번엔 시간과 하루 일과를 맞춰 볼 거예요!", StringKeys.MISSION2_AUDIO0));
        yield return CoroutineRunner.instance.RunAndWait("mission2",
        NarrationManager.instance.ShowNarrationAuto("장면에 어울리는 시계를 찾아 눌러 주세요!", StringKeys.MISSION2_AUDIO1));
        NarrationManager.instance.HideDialog();
        GameManager.instance.FadeInOut();
        isIntro = true;
    }
    public void MissionStart()
    {
        StopCoroutine(_MissionStart());
        StartCoroutine(_MissionStart());
    }
    IEnumerator _MissionStart()
    {
        yield return new WaitForSeconds(1f);
        GameManager.instance.npcAnimator.gameObject.transform.position = new Vector3(-2.037f, -1.95f, -5.08f);
        GameManager.instance.npcAnimator.gameObject.transform.rotation = Quaternion.Euler(0f, 147.638f, 0f);
        MissionGroup.SetActive(true); // 미션 UI 활성화
        Mission02_DataManager.instance.MissionSetting();
        yield return new WaitForSeconds(0.6f);
        // 나레이션 시작
        NarrationManager.instance.ShowDialog();
        GameManager.instance.npcAnimator.SetTrigger("hi");
        yield return CoroutineRunner.instance.RunAndWait("mission2",
        NarrationManager.instance.ShowNarrationAuto("이 장면은 몇 시일까요? 알맞은 시계를 골라 주세요!", StringKeys.MISSION2_AUDIO2));
        NarrationManager.instance.HideDialog();
        GameManager.instance.CanTouch = true;
    }
}
