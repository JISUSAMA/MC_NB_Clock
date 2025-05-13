using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    //[Header("INTRO")]
    //[SerializeField]private IntroManager introManager;
    //[Header("Mission 01")]
    //[SerializeField] private Mission01_UIManager mission01_UI;
    //public Mission01_DataManager mission01_Data;
    [Header("NPC")]
    public Animator npcAnimator;
    public Image fadeImage;
    public float fadeDurationTime = 1f;

    public string currentStage; // ���� ��������
    public string currentAnswer_en; // ���� ����
    public bool CanTouch = false;

    public TextMeshProUGUI DebugText;
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
    /// <summary>
    /// ȭ�� ��ȯ�� ���� FadeIn, FadeOut �ڷ�ƾ   
    /// </summary>
    public IEnumerator FadeIn()
    {
        if (!fadeImage.gameObject.activeInHierarchy)
            fadeImage.gameObject.SetActive(true);

        fadeImage.DOFade(1, fadeDurationTime).SetEase(Ease.Linear);
        yield return new WaitForSeconds(fadeDurationTime + 0.4f);
        fadeImage.gameObject.SetActive(false);
    }
    public IEnumerator FadeOut()
    {
        if (!fadeImage.gameObject.activeInHierarchy)
            fadeImage.gameObject.SetActive(true);

        fadeImage.DOFade(0, fadeDurationTime).SetEase(Ease.Linear);
        yield return new WaitForSeconds(fadeDurationTime + 0.4f);
        fadeImage.gameObject.SetActive(false);
    }
    public void FadeInOut()
    {
        if (!fadeImage.gameObject.activeInHierarchy)
            fadeImage.gameObject.SetActive(true);

        StartCoroutine(FadeCoroutine());
    }
    IEnumerator FadeCoroutine()     //FadeIn -> FadeOut
    {
        StartCoroutine(FadeIn());
        yield return new WaitForSeconds(fadeDurationTime + 0.4f);   //fade time �̿ܿ� 0.4�� ���
        StartCoroutine(FadeOut());
        yield return new WaitForSeconds(fadeDurationTime);
        fadeImage.gameObject.SetActive(false);
    }

    public void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

}
/// <summary>
/// StringUtil Ŭ������ ���ڿ� ���� ��ƿ��Ƽ �޼��带 �����մϴ�.
/// </summary>
public static class StringUtil
{
    private static Dictionary<string, KeyValuePair<string, string>> koreanParticles = new Dictionary<string, KeyValuePair<string, string>>
    {
        { "��/��", new KeyValuePair<string, string>("��", "��") },
        { "��/��", new KeyValuePair<string, string>("��", "��") },
        { "��/��", new KeyValuePair<string, string>("��", "��") },
    };

    public static string KoreanParticle(string text)
    {
        foreach (var particle in koreanParticles)
        {
            text = Regex.Replace(text, $@"([\uAC00-\uD7A3]+){particle.Key}", match =>
            {
                string word = match.Groups[1].Value;
                char lastChar = word[word.Length - 1];

                bool hasFinalConsonant = (lastChar - 0xAC00) % 28 > 0;

                return word + (hasFinalConsonant ? particle.Value.Key : particle.Value.Value);
            });
        }
        return text;
    }

}
