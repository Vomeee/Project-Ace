using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockOnUI : MonoBehaviour
{
    public Transform thisEnemyTransform; // �� ��ü�� Transform
    public Camera mainCamera; // ���� ī�޶�
    public RectTransform lockOnUI; // ���� UI�� RectTransform
    public Image lockOnUIImage; // UI�� Image ������Ʈ

    public float minDistance = 10f; // UI�� ���̱� �����ϴ� �ּ� �Ÿ�
    public float maxDistance = 100f; // UI�� �ִ� ũ�⿡ �����ϴ� �Ÿ�
    public Vector2 minSize = new Vector2(10f, 10f); // UI�� �ּ� ũ��
    public Vector2 maxSize = new Vector2(100f, 100f); // UI�� �ִ� ũ��

    public Color lockedOnColor = Color.red; // ���� ������ ���� ����
    public Color targetedColor = Color.yellow; // Ÿ���� ������ ���� ����
    private Color originalColor = Color.green; // ���� ����
    private bool isFlickering = false; // ������ ����

    [SerializeField]
    private EnemyAI enemyAI;

    void Start()
    {
        if (thisEnemyTransform != null)
        {
            enemyAI = thisEnemyTransform.GetComponent<EnemyAI>(); //�� ��ü�� EnemyAI
        }
    }

    void Update()
    {
        if (enemyAI == null || lockOnUIImage == null) return;

        // �� ��ü�� ���� ��ǥ�� ȭ�� ��ǥ�� ��ȯ
        Vector3 screenPos = mainCamera.WorldToScreenPoint(thisEnemyTransform.position);

        // ���� �÷��̾� ���� �Ÿ� ���
        float distanceToTarget = Vector3.Distance(mainCamera.transform.position, thisEnemyTransform.position);

        // ���� ī�޶��� �þ� �ȿ� �ִ��� Ȯ��
        if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
        {
            // �Ÿ� ���� �������� UI ǥ��
            if (distanceToTarget <= maxDistance)
            {
                lockOnUI.gameObject.SetActive(true);

                // �Ÿ� ������� ũ�� ����
                float t = Mathf.InverseLerp(minDistance, maxDistance, distanceToTarget);
                lockOnUI.sizeDelta = Vector2.Lerp(minSize, maxSize, t);

                // ȭ�� ��ǥ�� UI ĵ���� ��ǥ�� ��ȯ
                lockOnUI.position = screenPos;

                // ���� �� ������ ȿ�� ����
                if (enemyAI.isLockedOn)
                {
                    // ���� ����
                    lockOnUIImage.color = lockedOnColor;
                    if (isFlickering)
                    {
                        StopCoroutine(FlickerEffect());
                        isFlickering = false;
                    }
                }
                else if (enemyAI.isTargeted)
                {
                    // Ÿ���� ����
                    if (!isFlickering)
                    {
                        StartCoroutine(FlickerEffect());
                    }
                }
                else
                {
                    // Ÿ���õ��� ���� ����
                    lockOnUIImage.color = originalColor;
                    if (isFlickering)
                    {
                        StopCoroutine(FlickerEffect());
                        isFlickering = false;
                    }
                }
            }
            else
            {
                // �Ÿ� ���� ���̸� UI �����
                lockOnUI.gameObject.SetActive(false);
            }
        }
        else
        {
            // ���� ī�޶� �þ� �ۿ� ���� �� UI �����
            lockOnUI.gameObject.SetActive(false);
        }
    }

    private IEnumerator FlickerEffect()
    {
        isFlickering = true;
        while (true)
        {
            lockOnUIImage.color = targetedColor;
            yield return new WaitForSeconds(0.5f);
            lockOnUIImage.color = originalColor;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
