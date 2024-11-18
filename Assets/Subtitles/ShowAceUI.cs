using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAceUI : MonoBehaviour
{
    public CanvasGroup canvasGroup; // CanvasGroup 연결
    public float fadeDuration = 0.5f; // Fade 시간 (초)

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    public void FadeIn()
    {
        StartCoroutine(Fade(0, 1)); // Alpha를 0에서 1로 변경
    }

    public void FadeOut()
    {
        StartCoroutine(Fade(1, 0)); // Alpha를 1에서 0으로 변경
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        // Alpha가 0이면 상호작용 비활성화
        //canvasGroup.interactable = endAlpha > 0;
        //canvasGroup.blocksRaycasts = endAlpha > 0;
    }
}
