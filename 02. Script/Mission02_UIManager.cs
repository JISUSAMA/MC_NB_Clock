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
    public Image QuizImage; // ���� �̹���
    public Sprite[] QuizSpriteList; // ���� �̹��� ����Ʈ
    public Mission02_Clock[] ClockObjs_sc; // �ð�, ��ġ ������Ʈ��
    public TextMeshProUGUI QuizTimeText; // ���� �ð� �ؽ�Ʈ
    [Header("NPC")]
    public Vector3 NPC_originalPos;
    public Vector3 NPC_originalRot;

    void Awake()
    {
        // NPC ���� ��ġ�� ȸ���� ����
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
        NarrationManager.instance.TitleText.text = "������� ����!\n�Ϸ� �ϰ� �����ϱ�";
        yield return new WaitForSeconds(2f);
        NarrationManager.instance.TitleOb.SetActive(false);

        // �����̼� ����
        NarrationManager.instance.ShowDialog();
        GameManager.instance.npcAnimator.SetTrigger("hi");
        yield return CoroutineRunner.instance.RunAndWait("mission2",
        NarrationManager.instance.ShowNarrationAuto("�̹��� �ð��� �Ϸ� �ϰ��� ���� �� �ſ���!", StringKeys.MISSION2_AUDIO0));
        yield return CoroutineRunner.instance.RunAndWait("mission2",
        NarrationManager.instance.ShowNarrationAuto("��鿡 ��︮�� �ð踦 ã�� ���� �ּ���!", StringKeys.MISSION2_AUDIO1));
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
        MissionGroup.SetActive(true); // �̼� UI Ȱ��ȭ
        Mission02_DataManager.instance.MissionSetting();
        yield return new WaitForSeconds(0.6f);
        // �����̼� ����
        NarrationManager.instance.ShowDialog();
        GameManager.instance.npcAnimator.SetTrigger("hi");
        yield return CoroutineRunner.instance.RunAndWait("mission2",
        NarrationManager.instance.ShowNarrationAuto("�� ����� �� ���ϱ��? �˸��� �ð踦 ��� �ּ���!", StringKeys.MISSION2_AUDIO2));
        NarrationManager.instance.HideDialog();
        GameManager.instance.CanTouch = true;
    }
}
