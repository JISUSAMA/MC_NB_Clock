using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mission01_DataManager : MonoBehaviour
{
    [SerializeField] private Mission01_UIManager uiManager;
    [Header("Mission01 정답 관련")]
    public string currentAnswer = "";
    public int currentAnswerIndex = 0; //현재 진행중인 정답 인덱스
    public List<string> choiceClocksAnswers = new List<string>(); //선택지 리스트

    [Header("Clock Tower 오브젝트 관련")]
    private GameObject nowClock; //현재 선택된 시계 오브젝트
    bool nextClock = false; //다음 시계로 넘어가는지 체크하는 변수
    bool resetClock = false; //다음 시계로 넘어가는지 체크하는 변수

    private void Start()
    {
        INFO();
        StopAllCoroutines();
        StartCoroutine(_OnStart());
    }
    public void INFO()
    {
        GameManager.instance.fadeImage.color = new Color(0, 0, 0, 1); // 페이드 아웃
        GameManager.instance.CanTouch = false; // 터치 불가능
        ActiveChoiceClock(false); // 선택지 시계 비활성화
        uiManager.QuestionText.gameObject.SetActive(false);// 질문 텍스트 비활성화
    }
    IEnumerator _OnStart()
    {
        GameManager.instance.currentStage = "mission1";
       
        while (currentAnswerIndex < uiManager.ClockTowerCtrl_sc.Length)
        {
            if (currentAnswerIndex == 0)
            {
                StartCoroutine(_Start_Mission01());
            }
            else
            {
                StartCoroutine(_NextQuiz());
            }
            yield return new WaitUntil(() => nextClock == true); // 다음 시계로 넘어가는지 체크하는 변수
        
            currentAnswerIndex += 1;
            nextClock = false; // 다음 시계로 넘어가는지 체크하는 변수 초기화
            ClearThenFadeInOut(); // 페이드 아웃
            yield return new WaitForSeconds(1f);
            GameManager.instance.npcAnimator.gameObject.SetActive(true); // npc 비활성화
            yield return new WaitUntil(() => resetClock == true);
            resetClock = false; // 시계탑 제자리로 돌아가는지 체크하는 변수 초기화
            if (currentAnswerIndex == 3) { break; }
        }

    }
    IEnumerator _OnEnd()
    {
        GameManager.instance.npcAnimator.SetTrigger("jump");
        GameManager.instance.npcAnimator.SetTrigger("nice");
        SoundManager.instance.PlaySFX("Clear");
        yield return new WaitForSeconds(1f);
        yield return CoroutineRunner.instance.RunAndWait("mission1",
            NarrationManager.instance.ShowNarrationAuto("정말 잘했어요! 덕분에 세상이 다시 움직이기 시작했어요!",StringKeys.MISSION1_AUDIO9));
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
        yield return null;
    }
    IEnumerator _Start_Mission01()
    {
        MakeAnswer(); //정답 생성
        StartCoroutine(GameManager.instance.FadeOut()); // 페이드 아웃
        yield return new WaitForSeconds(1.2f);
        NarrationManager.instance.TitleOb.SetActive(true); // 타이틀 UI 활성화
        NarrationManager.instance.TitleText.text = "시계 바늘 맞추기"; // 타이틀 텍스트 설정
        yield return new WaitForSeconds(1f);
        NarrationManager.instance.TitleOb.SetActive(false); // 타이틀 UI 활성화
        GameManager.instance.npcAnimator.SetTrigger("hi");
        yield return CoroutineRunner.instance.RunAndWait("mission1",
           NarrationManager.instance.ShowNarrationAuto("여러분 안녕하세요!",StringKeys.MISSION1_AUDIO0));
        yield return CoroutineRunner.instance.RunAndWait("mission1",
        NarrationManager.instance.ShowNarrationAuto("저기 시계탑 보이시죠?\n시계 소리 덕분에 모두가 시간을 잘 지키고 있었어요.", StringKeys.MISSION1_AUDIO1));
        yield return new WaitForSeconds(1f);
        foreach (ClockTowerCtrl clock in uiManager.ClockTowerCtrl_sc)
        {
            clock.transitionTowerClock(false); // 시계탑 시,분침 비활성화
            clock.Particle[0].gameObject.SetActive(true); // 실패 파티클 비활성화
        }
        yield return new WaitForSeconds(1f);
        GameManager.instance.npcAnimator.SetTrigger("no");
        yield return CoroutineRunner.instance.RunAndWait("mission1",
           NarrationManager.instance.ShowNarrationAuto("앗, 큰일이에요! 시계의 초침이 사라졌어요!", StringKeys.MISSION1_AUDIO2));
        yield return CoroutineRunner.instance.RunAndWait("mission1",
           NarrationManager.instance.ShowNarrationAuto("시계가 멈추니까, 마을도 전부 멈춰버렸어요.", StringKeys.MISSION1_AUDIO3));
        yield return CoroutineRunner.instance.RunAndWait("mission1",
           NarrationManager.instance.ShowNarrationAuto("이제 우리가 시계를 다시 고쳐야 해요.", StringKeys.MISSION1_AUDIO4));
        yield return CoroutineRunner.instance.RunAndWait("mission1",
           NarrationManager.instance.ShowNarrationAuto("올바른 시계를 골라서 시계탑에 옮겨주세요!", StringKeys.MISSION1_AUDIO5));
        GameManager.instance.FadeInOut();
        yield return new WaitForSeconds(1f);
        GameManager.instance.npcAnimator.gameObject.SetActive(false); // npc 비활성화
        uiManager.MoveToTarget1(uiManager.ClockTowerCtrl_sc[currentAnswerIndex].transform);
        yield return new WaitForSeconds(0.8f);
        uiManager.QuestionText.gameObject.SetActive(true);// 질문 텍스트 비활성화
        GameManager.instance.CanTouch = true; // 터치 가능
    }

    void MakeAnswer()
    {
        choiceClocksAnswers.Clear(); //정답 리스트 초기화
        // 시: 1 ~ 12
        int hour = Random.Range(1, 13);
        // 분: 0, 15, 30, 45 중 랜덤 선택
        int[] validMinutes = { 0, 15, 30, 45 };
        int minute = validMinutes[Random.Range(0, validMinutes.Length)];
        uiManager.ClockTowerCtrl_sc[currentAnswerIndex].ClockTowerSetting(hour, minute); // 시계탑 시계 시,분 설정
        currentAnswer = $"{hour}_{minute}";//9시 => 9_0
        Debug.Log("Generated Answer: " + currentAnswer);
        choiceClocksAnswers.Add(currentAnswer); //정답 리스트에 추가

        //정답 리스트에 랜덤으로 2개 더 추가
        for (int i = 0; i < 2; i++)
        {
            int randomHour = Random.Range(1, 13);
            int randomMinute = validMinutes[Random.Range(0, validMinutes.Length)];
            string randomAnswer = $"{randomHour}_{randomMinute}";
            choiceClocksAnswers.Add(randomAnswer);
        }
        //정답 리스트를 랜덤으로 섞기
        for (int i = 0; i < choiceClocksAnswers.Count; i++)
        {
            int randomIndex = Random.Range(0, choiceClocksAnswers.Count);
            string temp = choiceClocksAnswers[i];
            choiceClocksAnswers[i] = choiceClocksAnswers[randomIndex];
            choiceClocksAnswers[randomIndex] = temp;
        }
        //정답 리스트를 UI에 적용
        for (int i = 0; i < uiManager.ChoiceClocks.Length; i++)
        {
            if (uiManager.ChoiceClocks[i] != null)
            {
                Clock clock = uiManager.ChoiceClocks[i].transform.GetChild(0).GetComponent<Clock>();
                if (clock != null)
                {
                    string[] answerParts = choiceClocksAnswers[i].Split('_');
                    int hourAnswer = int.Parse(answerParts[0]);
                    int minuteAnswer = int.Parse(answerParts[1]);
                    clock.hour = hourAnswer;
                    clock.minutes = minuteAnswer;
                }
            }
        }
        uiManager.QuestionText.text = $"{hour:D2}시 {minute:D2}분을 찾아주세요!";
    }
    IEnumerator _NextQuiz()
    {
        // 다음 퀴즈로 넘어가는 코루틴
        GameManager.instance.CanTouch = false; // 터치 불가능
        MakeAnswer(); //정답 생성
        uiManager.MoveToTarget1(uiManager.ClockTowerCtrl_sc[currentAnswerIndex].transform);
        GameManager.instance.CanTouch = true; // 터치 가능
        uiManager.QuestionText.gameObject.SetActive(true);// 질문 텍스트 활성화
        GameManager.instance.npcAnimator.gameObject.SetActive(false); // npc 비활성화
        yield return null;
    }
    public IEnumerator _CheckAnswer_Correct(string AnswerStr)
    {
        uiManager.QuestionText.gameObject.SetActive(false);// 질문 텍스트 비활성화
        SoundManager.instance.PlaySFX("success01");

        NarrationManager.instance.ShowDialog();
        GameManager.instance.CanTouch = false; // 터치 불가능
        uiManager.ClockTowerCtrl_sc[currentAnswerIndex].Particle[1].gameObject.SetActive(true); // 성공 파티클 활성화
        uiManager.ClockTowerCtrl_sc[currentAnswerIndex].transitionTowerClock(true); // 시계탑 시,분침 비활성화
        yield return CoroutineRunner.instance.RunAndWait("mission1",
  NarrationManager.instance.ShowNarrationAuto("정답이에요! 시계가 다시 똑딱똑딱 움직여요!", StringKeys.MISSION1_AUDIO6));
        yield return new WaitForSeconds(1.5f);
        nextClock = true; // 다음 시계로 넘어가는지 체크하는 변수
        yield return null;
    }
    public IEnumerator _CheckAnswer_Wrong()
    {
        SoundManager.instance.PlaySFX("wrong01");
        GameManager.instance.npcAnimator.SetTrigger("no");
        ReplayFaillParticle(currentAnswerIndex);
        yield return CoroutineRunner.instance.RunAndWait("mission1",
          NarrationManager.instance.ShowNarrationAuto("조금 아쉬웠어요! 다시 한 번 해볼까요?", StringKeys.MISSION1_AUDIO7));
        yield return null;
    }
    /// 시계 선택지 활성화/비활성화 메서드
    public void ActiveChoiceClock(bool active)
    {
        foreach (GameObject clock in uiManager.ChoiceClocks)
        {
            if (clock != null)
                clock.SetActive(active);
        }
    }
    public void ReplayFaillParticle(int index)
    {
        var particle = uiManager.ClockTowerCtrl_sc[index].Particle[0];
        if (particle != null)
        {
            // 다시 실행할 수 있도록 준비
            particle.gameObject.SetActive(true);
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particle.Play(); // 정상 재생
        }
    }
    void ClearThenFadeInOut()
    {
        StopCoroutine(_ClearThenFadeInOut());
        StartCoroutine(_ClearThenFadeInOut());
    }
    IEnumerator _ClearThenFadeInOut()
    {
        GameManager.instance.CanTouch = false;
        GameManager.instance.FadeInOut();
        yield return new WaitForSeconds(1.2f);
        uiManager.MoveToTargetReverse(uiManager.ClockTowerCtrl_sc[currentAnswerIndex - 1].transform); //시계탑 제자리로 돌아가
        yield return new WaitForSeconds(1.2f);
        //미션이 마지막이 아닌경우
        if (currentAnswerIndex != 3)
        {
            GameManager.instance.npcAnimator.SetTrigger("jump");
            yield return CoroutineRunner.instance.RunAndWait("mission1",
           NarrationManager.instance.ShowNarrationAuto("좋아요! 다음 시계도 고쳐줘요!", StringKeys.MISSION1_AUDIO8));
            GameManager.instance.FadeInOut();
            yield return new WaitForSeconds(1f);
        }
        else
        {
            StartCoroutine(_OnEnd()); //미션 종료
        }

        resetClock = true; //  //시계탑 제자리로 돌아갔는지 체크하는 변수
    }
}
