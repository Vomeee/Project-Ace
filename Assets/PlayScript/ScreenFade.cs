using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    [SerializeField] Image fadeImage;
    [SerializeField] float fadeDuration;

    private Coroutine currentCoroutine; // ���� ���� ���� �ڷ�ƾ ����

    private void Start()
    {
        
    }

    public void FadeOut(System.Action onComplete = null)
    {
        StartFade(1f, onComplete);
    }

    public void FadeIn(System.Action onComplete = null)
    {
        StartFade(0f, onComplete);
    }

    private void StartFade(float targetAlpha, System.Action onComplete)
    {
        // ������ ���� ���̴� ���̵� �ڷ�ƾ ����
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // �� �ڷ�ƾ ����
        currentCoroutine = StartCoroutine(Fade(targetAlpha, onComplete));
    }

    private IEnumerator Fade(float targetAlpha, System.Action onComplete)
    {
        float startAlpha = fadeImage.color.a;
        float elapsedTime = 0f;

        // ���� �� ����
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);

            // �̹��� ���� ������Ʈ
            Color color = fadeImage.color;
            color.a = newAlpha;
            fadeImage.color = color;

            yield return null;
        }

        // ���� �� Ȯ��
        Color finalColor = fadeImage.color;
        finalColor.a = targetAlpha;
        fadeImage.color = finalColor;

        // �Ϸ� �� �ݹ� ����
        onComplete?.Invoke();

        // �ڷ�ƾ ����
        currentCoroutine = null;
    }
}
