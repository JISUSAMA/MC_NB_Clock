using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mission02_DataManager : MonoBehaviour
{
    public static Mission02_DataManager instance { get; private set; }
    [SerializeField] private Mission02_UIManager uiManager;
    public int detectNum; // 퀴즈 번호
    public string currentAnswer = "";
    string[] AnswerString = { "7:15", "08:30", "09:45", "18:30", "20:45", "22:00" };
    public List<string> choiceClocksAnswers = new List<string>(); //선택지 리스트
    public bool isAnswer = false; // 정답 여부

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Initialized();
        OnStart();
    }
    public void Initialized()
    {
        GameManager.instance.currentStage = StringKeys.STAGE_MISSION2;
        uiManager.MissionGroup.SetActive(false);
        uiManager.NPC_originalPos = GameManager.instance.npcAnimator.gameObject.transform.position;
        uiManager.NPC_originalRot = GameManager.instance.npcAnimator.gameObject.transform.rotation.eulerAngles;
    }
    public void OnStart()
    {
        StopCoroutine(_OnStart());
        StartCoroutine(_OnStart());
    }
    IEnumerator _OnStart()
    {
        uiManager.IntroStart();
        yield return new WaitUntil(() => uiManager.isIntro == true);
        uiManager.MissionStart();
        yield return new WaitUntil(() => isAnswer == true);
        while (detectNum < 6)
        {
            isAnswer = false; // 정답 초기화
            detectNum++;
            yield return new WaitForSeconds(0.5f);
            if (detectNum == 6)
            {
                break;
            }
            else
            {
                yield return CoroutineRunner.instance.RunAndWait("mission1",
     NarrationManager.instance.ShowNarrationAuto("다음 장면도 준비됐죠?", StringKeys.MISSION2_AUDIO5));
                MissionSetting();
            }
            yield return new WaitUntil(() => isAnswer == true);
        }
        StartCoroutine(_OnEnd());
    }
    IEnumerator _OnEnd()
    {
        GameManager.instance.FadeInOut();
        yield return new WaitForSeconds(1f);
        GameManager.instance.npcAnimator.gameObject.transform.position = uiManager.NPC_originalPos;
        GameManager.instance.npcAnimator.gameObject.transform.rotation = Quaternion.Euler(uiManager.NPC_originalRot);
        uiManager.MissionGroup.SetActive(false);
        yield return new WaitForSeconds(1f);
        GameManager.instance.npcAnimator.SetTrigger("jump");
        GameManager.instance.npcAnimator.SetTrigger("nice");
        SoundManager.instance.PlaySFX("Clear");
        yield return CoroutineRunner.instance.RunAndWait("mission1",
 NarrationManager.instance.ShowNarrationAuto("순서대로 하루를 잘 정리했어요! 정말 멋져요!", StringKeys.MISSION2_AUDIO6));
        yield return CoroutineRunner.instance.RunAndWait("mission1",
NarrationManager.instance.ShowNarrationAuto("오늘도 너무 잘했어요~!\n그럼 다음에 또 만나요!", StringKeys.MISSION2_AUDIO7));
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0); //메인 씬으로 돌아가기
    }
    public void MissionSetting()
    {
        MakeAnswer();
        // 퀴즈 이미지 설정
        uiManager.QuizImage.sprite = uiManager.QuizSpriteList[detectNum];
        GameManager.instance.CanTouch = true; // 터치 가능
    }

    void MakeAnswer()
    {
        string[] answerTimes = AnswerString[detectNum].Split(':');
        int answerHour = int.Parse(answerTimes[0]);
        int answerMinute = int.Parse(answerTimes[1]);

        choiceClocksAnswers.Clear(); // 정답 리스트 초기화
        currentAnswer = $"{answerHour:D2}:{answerMinute:D2}"; // 정답
        choiceClocksAnswers.Add(currentAnswer); // 정답 추가

        uiManager.QuizTimeText.text = $"{answerHour:D2}:{answerMinute:D2}"; // 디지털 시계 표기

        int[] validMinutes = { 0, 15, 30, 45 };

        // 중복 방지를 위한 HashSet
        HashSet<string> usedAnswers = new HashSet<string>();
        usedAnswers.Add(currentAnswer); // 정답도 포함시켜 중복 방지
        while (choiceClocksAnswers.Count < 4)
        {
            int randomHour = Random.Range(0, 24);
            int randomMinute = validMinutes[Random.Range(0, validMinutes.Length)];

            // 중복 여부 확인 (12시간제 비교)
            bool isDuplicate = false;
            foreach (string answer in usedAnswers)
            {
                string[] parts = answer.Split(':');
                int existHour = int.Parse(parts[0]);
                int existMinute = int.Parse(parts[1]);
                if (IsDuplicate(existHour, existMinute, randomHour, randomMinute))
                {
                    isDuplicate = true;
                    break;
                }
            }

            if (!isDuplicate)
            {
                string randomAnswer = $"{randomHour:D2}:{randomMinute:D2}";
                choiceClocksAnswers.Add(randomAnswer);
                usedAnswers.Add(randomAnswer);
                Debug.Log("Generated Random Answer: " + randomAnswer);
            }
        }

        // 보기 섞기
        for (int i = 0; i < choiceClocksAnswers.Count; i++)
        {
            int randomIndex = Random.Range(0, choiceClocksAnswers.Count);
            string temp = choiceClocksAnswers[i];
            choiceClocksAnswers[i] = choiceClocksAnswers[randomIndex];
            choiceClocksAnswers[randomIndex] = temp;
        }

        // 시계 UI에 적용
        for (int i = 0; i < uiManager.ClockObjs_sc.Length; i++)
        {
            if (uiManager.ClockObjs_sc[i] != null)
            {
                Clock clock = uiManager.ClockObjs_sc[i].transform.GetChild(0).GetComponent<Clock>();
                if (clock != null)
                {
                    string[] answerParts = choiceClocksAnswers[i].Split(':');
                    int hourAnswer = int.Parse(answerParts[0]);
                    int minuteAnswer = int.Parse(answerParts[1]);
                    uiManager.ClockObjs_sc[i].ClockTowerSetting(hourAnswer, minuteAnswer); // 시계탑 시계 시,분 설정

                }
            }
        }

    }

    bool IsDuplicate(int hour1, int minute1, int hour2, int minute2)
    {
        int normalized1 = hour1 % 12 == 0 ? 12 : hour1 % 12;
        int normalized2 = hour2 % 12 == 0 ? 12 : hour2 % 12;
        return (normalized1 == normalized2 && minute1 == minute2);
    }

}
