using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    [SerializeField] Image fadeImage;
    [SerializeField] float fadeDuration;

    private Coroutine currentCoroutine; // 현재 실행 중인 코루틴 참조

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
        // 이전에 실행 중이던 페이드 코루틴 중지
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // 새 코루틴 시작
        currentCoroutine = StartCoroutine(Fade(targetAlpha, onComplete));
    }

    private IEnumerator Fade(float targetAlpha, System.Action onComplete)
    {
        float startAlpha = fadeImage.color.a;
        float elapsedTime = 0f;

        // 알파 값 변경
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);

            // 이미지 색상 업데이트
            Color color = fadeImage.color;
            color.a = newAlpha;
            fadeImage.color = color;

            yield return null;
        }

        // 알파 값 확정
        Color finalColor = fadeImage.color;
        finalColor.a = targetAlpha;
        fadeImage.color = finalColor;

        // 완료 시 콜백 실행
        onComplete?.Invoke();

        // 코루틴 종료
        currentCoroutine = null;
    }
}
