using System.Collections;
using UnityEngine;

public class MainMenuSelect : MonoBehaviour
{
    public RectTransform InitialMainMenuRectTransform; // ȭ�� �ۿ��� UI ǥ�� ��ġ
    public RectTransform FinalMainMenuRectTransform; // ȭ�� ������ ������ ���� ��ġ

    [SerializeField]
    private Canvas MainMenuUI;

    public float transitionDuration = 1.0f; // UI�� �̵��ϴ� �� �ɸ��� �ð�

    void Start()
    {
        // FinalMainMenuRectTransform ��ġ�� �ڵ忡�� ���� ����
        FinalMainMenuRectTransform.position = new Vector3(0f, 0f, -1.74f);

        // UI�� ���� ��ġ�� ����
        MainMenuUI.GetComponent<RectTransform>().position = InitialMainMenuRectTransform.position;

        // �ڷ�ƾ�� ���� UI�� ȭ�� ������ �̵�
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

            // UI�� ��ġ�� startPosition���� endPosition���� �ε巴�� �̵�
            uiRectTransform.position = Vector3.Lerp(startPosition, endPosition, t);

            yield return null; // ���� �����ӱ��� ���
        }

        // �̵��� �Ϸ�Ǹ� UI�� ��Ȯ�� ���� ��ġ�� ����
        uiRectTransform.position = endPosition;
    }
}
