using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    public bool IsIntroEnd = false;

    [SerializeField] private GameObject IntroCanvas;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI touchScreen;

    [SerializeField] private Button startButton;

    [Header("Wave Effect Variable")]
    private float amplitude = 40f;
    private float delayBetweenChars = 0.5f;
    private TMP_TextInfo textInfo;
    private Vector3[][] originalVertices;
    private float blinkDuration = 1.5f;
    private float duration = 1f;

    private void Awake()
    {
        INFO();
    }
    public void INFO()
    {
        IsIntroEnd = false;
        startButton.onClick.AddListener(OnStartButtonClicked); //버튼 클릭 시 호출되는 메서드
        IntroCanvas.SetActive(true); // 인트로 캔버스 활성화
        StartWaveAnimation(); // 타이틀 텍스트 웨이브 애니메이션
        Blink_TouchScreen(); // 터치 화면 깜빡임
        titleText.gameObject.SetActive(true); // 타이틀 텍스트 활성화
        touchScreen.gameObject.SetActive(true); // 터치 화면 활성화
    }
    private void OnStartButtonClicked()
    {
        SceneManager.LoadScene(StringKeys.MISSION1_NAME); // 버튼 클릭 시 "Mission01" 씬으로 전환
    }
    // 터치 화면 깜빡임
    private void Blink_TouchScreen()
    {
        touchScreen.DOFade(0, blinkDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);
    }
    // 타이틀 텍스트 웨이브 애니메이션
    private void StartWaveAnimation()
    {
        TMP_Text tmpText = titleText;
        tmpText.ForceMeshUpdate();
        textInfo = tmpText.textInfo;

        originalVertices = new Vector3[textInfo.meshInfo.Length][];
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            originalVertices[i] = textInfo.meshInfo[i].vertices.Clone() as Vector3[];
        }
        StartCoroutine(WaveCoroutine(tmpText));
    }
    // 타이틀 텍스트 웨이브 애니메이션
    private IEnumerator WaveCoroutine(TMP_Text tmpText)
    {
        while (true)
        {
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                AnimateSingleChar(tmpText, i);
                yield return new WaitForSeconds(delayBetweenChars);
            }
        }
    }
    // 글자 하나씩 웨이브 애니메이션
    private void AnimateSingleChar(TMP_Text tmpText, int charIndex)
    {
        int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
        Vector3[] original = originalVertices[materialIndex];

        DOTween.Kill($"CharTween_{charIndex}");

        DOTween.Sequence()
            .Append(DOVirtual.Float(0, amplitude, duration / 2f, (value) =>
            {
                Vector3 offset = new Vector3(0, value, 0);
                vertices[vertexIndex + 0] = original[vertexIndex + 0] + offset;
                vertices[vertexIndex + 1] = original[vertexIndex + 1] + offset;
                vertices[vertexIndex + 2] = original[vertexIndex + 2] + offset;
                vertices[vertexIndex + 3] = original[vertexIndex + 3] + offset;

                UpdateMesh(tmpText, materialIndex, vertices);
            }))
            .Append(DOVirtual.Float(amplitude, 0, duration / 2f, (value) =>
            {
                Vector3 offset = new Vector3(0, value, 0);
                vertices[vertexIndex + 0] = original[vertexIndex + 0] + offset;
                vertices[vertexIndex + 1] = original[vertexIndex + 1] + offset;
                vertices[vertexIndex + 2] = original[vertexIndex + 2] + offset;
                vertices[vertexIndex + 3] = original[vertexIndex + 3] + offset;

                UpdateMesh(tmpText, materialIndex, vertices);
            }))
            .SetId($"CharTween_{charIndex}");
    }
    // 메쉬 업데이트
    private void UpdateMesh(TMP_Text tmpText, int materialIndex, Vector3[] vertices)
    {
        tmpText.textInfo.meshInfo[materialIndex].mesh.vertices = vertices;
        tmpText.UpdateGeometry(tmpText.textInfo.meshInfo[materialIndex].mesh, materialIndex);
    }
}
