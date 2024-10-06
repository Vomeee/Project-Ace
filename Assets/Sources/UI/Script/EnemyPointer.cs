using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPointer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TargettingSystem targettingSystem; // Ÿ�� ����
    [SerializeField] Image arrowImage; // ȭ��ǥ �̹���
    [SerializeField] RectTransform arrowRectTransform; // ȭ��ǥ�� RectTransform

    [SerializeField] Color transparentColor; // ȭ��ǥ�� ������ ���� ���� ����
    [SerializeField] Color originalColor; // ȭ��ǥ�� ���� ���� ����

    [SerializeField] bool isBeingShown; // ȭ��ǥ�� Ȱ��ȭ ��������
    [SerializeField] float radius = 150f; // ȭ��ǥ�� ������ ������

    void Start()
    {
        // ���� �� ȭ��ǥ�� ��Ȱ��ȭ
        arrowImage.color = transparentColor;
        isBeingShown = false;
    }

    void Update()
    {
        // Ÿ���� ���� ��ǥ�� ����Ʈ ��ǥ�� ��ȯ
        Vector3 targetViewportPos = Camera.main.WorldToViewportPoint(targettingSystem.currentTargetTransform.position);

        // ȭ���� ������� Ȯ�� (x�� y ���� 0~1 ������ ������� Ȯ��)
        bool isOutOfBounds = targetViewportPos.x < 0 || targetViewportPos.x > 1 || targetViewportPos.y < 0 || targetViewportPos.y > 1;

        if (isOutOfBounds)
        {
            if (!isBeingShown)
            {
                isBeingShown = true;
                arrowImage.color = originalColor;
            }

            // Ÿ�� ������ ī�޶��� ���� ��ǥ��� ��ȯ�Ͽ� ���
            Vector3 targetDirFromCamera = (targettingSystem.currentTargetTransform.position - Camera.main.transform.position).normalized;

            // ī�޶��� ���� ��ǥ���� Ÿ�� ������ ��� (ī�޶��� ���� ������ ���)
            Vector3 arrowDirection = Camera.main.transform.InverseTransformDirection(targetDirFromCamera);

            // ȭ��ǥ�� transform.rotation�� ī�޶� ȸ���� �ݿ��Ͽ� ����
            float angle = Mathf.Atan2(arrowDirection.x, arrowDirection.y) * Mathf.Rad2Deg;
            arrowImage.transform.rotation = Quaternion.Euler(0, -angle, -angle);

            // ȭ�� �߾��� �������� ȭ��ǥ ��ġ ���
            Vector2 arrowPosition = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad) - 10) * radius;

            // ȭ��ǥ�� RectTransform ��ġ ����
            arrowRectTransform.anchoredPosition = arrowPosition;
        }
        else
        {
            if (isBeingShown)
            {
                isBeingShown = false;
                arrowImage.color = transparentColor;
            }
        }
    }
}
