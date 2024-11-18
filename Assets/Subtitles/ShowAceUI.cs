using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAceUI : MonoBehaviour
{
    public CanvasGroup canvasGroup; // CanvasGroup ����
    public float fadeDuration = 0.5f; // Fade �ð� (��)

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    public void FadeIn()
    {
        StartCoroutine(Fade(0, 1)); // Alpha�� 0���� 1�� ����
    }

    public void FadeOut()
    {
        StartCoroutine(Fade(1, 0)); // Alpha�� 1���� 0���� ����
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

        // Alpha�� 0�̸� ��ȣ�ۿ� ��Ȱ��ȭ
        //canvasGroup.interactable = endAlpha > 0;
        //canvasGroup.blocksRaycasts = endAlpha > 0;
    }
}
