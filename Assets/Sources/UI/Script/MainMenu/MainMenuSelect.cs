using System.Collections;
using UnityEngine;

public class MainMenuSelect : MonoBehaviour
{
    public RectTransform InitialMainMenuRectTransform; // 화면 밖에서 UI 표출 위치
    public RectTransform FinalMainMenuRectTransform; // 화면 안으로 가져온 뒤의 위치

    [SerializeField]
    private Canvas MainMenuUI;

    public float transitionDuration = 1.0f; // UI가 이동하는 데 걸리는 시간

    void Start()
    {
        // FinalMainMenuRectTransform 위치를 코드에서 직접 설정
        FinalMainMenuRectTransform.position = new Vector3(0f, 0f, -1.74f);

        // UI를 시작 위치로 설정
        MainMenuUI.GetComponent<RectTransform>().position = InitialMainMenuRectTransform.position;

        // 코루틴을 통해 UI를 화면 안으로 이동
        StartCoroutine(MoveUIToFinalPosition());
    }

    private IEnumerator MoveUIToFinalPosition()
    {
        RectTransform uiRectTransform = MainMenuUI.GetComponent<RectTransform>();
        Vector3 startPosition = InitialMainMenuRectTransform.position;
        Vector3 endPosition = FinalMainMenuRectTransform.position;

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);

            // UI의 위치를 startPosition에서 endPosition으로 부드럽게 이동
            uiRectTransform.position = Vector3.Lerp(startPosition, endPosition, t);

            yield return null; // 다음 프레임까지 대기
        }

        // 이동이 완료되면 UI를 정확히 최종 위치로 설정
        uiRectTransform.position = endPosition;
    }
}
