using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mission01_DataManager : MonoBehaviour
{
    [SerializeField] private Mission01_UIManager uiManager;
    [Header("Mission01 ���� ����")]
    public string currentAnswer = "";
    public int currentAnswerIndex = 0; //���� �������� ���� �ε���
    public List<string> choiceClocksAnswers = new List<string>(); //������ ����Ʈ

    [Header("Clock Tower ������Ʈ ����")]
    private GameObject nowClock; //���� ���õ� �ð� ������Ʈ
    bool nextClock = false; //���� �ð�� �Ѿ���� üũ�ϴ� ����
    bool resetClock = false; //���� �ð�� �Ѿ���� üũ�ϴ� ����

    private void Start()
    {
        INFO();
        StopAllCoroutines();
        StartCoroutine(_OnStart());
    }
    public void INFO()
    {
        GameManager.instance.fadeImage.color = new Color(0, 0, 0, 1); // ���̵� �ƿ�
        GameManager.instance.CanTouch = false; // ��ġ �Ұ���
        ActiveChoiceClock(false); // ������ �ð� ��Ȱ��ȭ
        uiManager.QuestionText.gameObject.SetActive(false);// ���� �ؽ�Ʈ ��Ȱ��ȭ
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
            yield return new WaitUntil(() => nextClock == true); // ���� �ð�� �Ѿ���� üũ�ϴ� ����
        
            currentAnswerIndex += 1;
            nextClock = false; // ���� �ð�� �Ѿ���� üũ�ϴ� ���� �ʱ�ȭ
            ClearThenFadeInOut(); // ���̵� �ƿ�
            yield return new WaitForSeconds(1f);
            GameManager.instance.npcAnimator.gameObject.SetActive(true); // npc ��Ȱ��ȭ
            yield return new WaitUntil(() => resetClock == true);
            resetClock = false; // �ð�ž ���ڸ��� ���ư����� üũ�ϴ� ���� �ʱ�ȭ
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
            NarrationManager.instance.ShowNarrationAuto("���� ���߾��! ���п� ������ �ٽ� �����̱� �����߾��!",StringKeys.MISSION1_AUDIO9));
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
        yield return null;
    }
    IEnumerator _Start_Mission01()
    {
        MakeAnswer(); //���� ����
        StartCoroutine(GameManager.instance.FadeOut()); // ���̵� �ƿ�
        yield return new WaitForSeconds(1.2f);
        NarrationManager.instance.TitleOb.SetActive(true); // Ÿ��Ʋ UI Ȱ��ȭ
        NarrationManager.instance.TitleText.text = "�ð� �ٴ� ���߱�"; // Ÿ��Ʋ �ؽ�Ʈ ����
        yield return new WaitForSeconds(1f);
        NarrationManager.instance.TitleOb.SetActive(false); // Ÿ��Ʋ UI Ȱ��ȭ
        GameManager.instance.npcAnimator.SetTrigger("hi");
        yield return CoroutineRunner.instance.RunAndWait("mission1",
           NarrationManager.instance.ShowNarrationAuto("������ �ȳ��ϼ���!",StringKeys.MISSION1_AUDIO0));
        yield return CoroutineRunner.instance.RunAndWait("mission1",
        NarrationManager.instance.ShowNarrationAuto("���� �ð�ž ���̽���?\n�ð� �Ҹ� ���п� ��ΰ� �ð��� �� ��Ű�� �־����.", StringKeys.MISSION1_AUDIO1));
        yield return new WaitForSeconds(1f);
        foreach (ClockTowerCtrl clock in uiManager.ClockTowerCtrl_sc)
        {
            clock.transitionTowerClock(false); // �ð�ž ��,��ħ ��Ȱ��ȭ
            clock.Particle[0].gameObject.SetActive(true); // ���� ��ƼŬ ��Ȱ��ȭ
        }
        yield return new WaitForSeconds(1f);
        GameManager.instance.npcAnimator.SetTrigger("no");
        yield return CoroutineRunner.instance.RunAndWait("mission1",
           NarrationManager.instance.ShowNarrationAuto("��, ū���̿���! �ð��� ��ħ�� ��������!", StringKeys.MISSION1_AUDIO2));
        yield return CoroutineRunner.instance.RunAndWait("mission1",
           NarrationManager.instance.ShowNarrationAuto("�ð谡 ���ߴϱ�, ������ ���� ������Ⱦ��.", StringKeys.MISSION1_AUDIO3));
        yield return CoroutineRunner.instance.RunAndWait("mission1",
           NarrationManager.instance.ShowNarrationAuto("���� �츮�� �ð踦 �ٽ� ���ľ� �ؿ�.", StringKeys.MISSION1_AUDIO4));
        yield return CoroutineRunner.instance.RunAndWait("mission1",
           NarrationManager.instance.ShowNarrationAuto("�ùٸ� �ð踦 ��� �ð�ž�� �Ű��ּ���!", StringKeys.MISSION1_AUDIO5));
        GameManager.instance.FadeInOut();
        yield return new WaitForSeconds(1f);
        GameManager.instance.npcAnimator.gameObject.SetActive(false); // npc ��Ȱ��ȭ
        uiManager.MoveToTarget1(uiManager.ClockTowerCtrl_sc[currentAnswerIndex].transform);
        yield return new WaitForSeconds(0.8f);
        uiManager.QuestionText.gameObject.SetActive(true);// ���� �ؽ�Ʈ ��Ȱ��ȭ
        GameManager.instance.CanTouch = true; // ��ġ ����
    }

    void MakeAnswer()
    {
        choiceClocksAnswers.Clear(); //���� ����Ʈ �ʱ�ȭ
        // ��: 1 ~ 12
        int hour = Random.Range(1, 13);
        // ��: 0, 15, 30, 45 �� ���� ����
        int[] validMinutes = { 0, 15, 30, 45 };
        int minute = validMinutes[Random.Range(0, validMinutes.Length)];
        uiManager.ClockTowerCtrl_sc[currentAnswerIndex].ClockTowerSetting(hour, minute); // �ð�ž �ð� ��,�� ����
        currentAnswer = $"{hour}_{minute}";//9�� => 9_0
        Debug.Log("Generated Answer: " + currentAnswer);
        choiceClocksAnswers.Add(currentAnswer); //���� ����Ʈ�� �߰�

        //���� ����Ʈ�� �������� 2�� �� �߰�
        for (int i = 0; i < 2; i++)
        {
            int randomHour = Random.Range(1, 13);
            int randomMinute = validMinutes[Random.Range(0, validMinutes.Length)];
            string randomAnswer = $"{randomHour}_{randomMinute}";
            choiceClocksAnswers.Add(randomAnswer);
        }
        //���� ����Ʈ�� �������� ����
        for (int i = 0; i < choiceClocksAnswers.Count; i++)
        {
            int randomIndex = Random.Range(0, choiceClocksAnswers.Count);
            string temp = choiceClocksAnswers[i];
            choiceClocksAnswers[i] = choiceClocksAnswers[randomIndex];
            choiceClocksAnswers[randomIndex] = temp;
        }
        //���� ����Ʈ�� UI�� ����
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
        uiManager.QuestionText.text = $"{hour:D2}�� {minute:D2}���� ã���ּ���!";
    }
    IEnumerator _NextQuiz()
    {
        // ���� ����� �Ѿ�� �ڷ�ƾ
        GameManager.instance.CanTouch = false; // ��ġ �Ұ���
        MakeAnswer(); //���� ����
        uiManager.MoveToTarget1(uiManager.ClockTowerCtrl_sc[currentAnswerIndex].transform);
        GameManager.instance.CanTouch = true; // ��ġ ����
        uiManager.QuestionText.gameObject.SetActive(true);// ���� �ؽ�Ʈ Ȱ��ȭ
        GameManager.instance.npcAnimator.gameObject.SetActive(false); // npc ��Ȱ��ȭ
        yield return null;
    }
    public IEnumerator _CheckAnswer_Correct(string AnswerStr)
    {
        uiManager.QuestionText.gameObject.SetActive(false);// ���� �ؽ�Ʈ ��Ȱ��ȭ
        SoundManager.instance.PlaySFX("success01");

        NarrationManager.instance.ShowDialog();
        GameManager.instance.CanTouch = false; // ��ġ �Ұ���
        uiManager.ClockTowerCtrl_sc[currentAnswerIndex].Particle[1].gameObject.SetActive(true); // ���� ��ƼŬ Ȱ��ȭ
        uiManager.ClockTowerCtrl_sc[currentAnswerIndex].transitionTowerClock(true); // �ð�ž ��,��ħ ��Ȱ��ȭ
        yield return CoroutineRunner.instance.RunAndWait("mission1",
  NarrationManager.instance.ShowNarrationAuto("�����̿���! �ð谡 �ٽ� �ȵ��ȵ� ��������!", StringKeys.MISSION1_AUDIO6));
        yield return new WaitForSeconds(1.5f);
        nextClock = true; // ���� �ð�� �Ѿ���� üũ�ϴ� ����
        yield return null;
    }
    public IEnumerator _CheckAnswer_Wrong()
    {
        SoundManager.instance.PlaySFX("wrong01");
        GameManager.instance.npcAnimator.SetTrigger("no");
        ReplayFaillParticle(currentAnswerIndex);
        yield return CoroutineRunner.instance.RunAndWait("mission1",
          NarrationManager.instance.ShowNarrationAuto("���� �ƽ������! �ٽ� �� �� �غ����?", StringKeys.MISSION1_AUDIO7));
        yield return null;
    }
    /// �ð� ������ Ȱ��ȭ/��Ȱ��ȭ �޼���
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
            // �ٽ� ������ �� �ֵ��� �غ�
            particle.gameObject.SetActive(true);
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particle.Play(); // ���� ���
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
        uiManager.MoveToTargetReverse(uiManager.ClockTowerCtrl_sc[currentAnswerIndex - 1].transform); //�ð�ž ���ڸ��� ���ư�
        yield return new WaitForSeconds(1.2f);
        //�̼��� �������� �ƴѰ��
        if (currentAnswerIndex != 3)
        {
            GameManager.instance.npcAnimator.SetTrigger("jump");
            yield return CoroutineRunner.instance.RunAndWait("mission1",
           NarrationManager.instance.ShowNarrationAuto("���ƿ�! ���� �ð赵 �������!", StringKeys.MISSION1_AUDIO8));
            GameManager.instance.FadeInOut();
            yield return new WaitForSeconds(1f);
        }
        else
        {
            StartCoroutine(_OnEnd()); //�̼� ����
        }

        resetClock = true; //  //�ð�ž ���ڸ��� ���ư����� üũ�ϴ� ����
    }
}
