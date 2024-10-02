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

            // Ÿ���� ī�޶�κ��� ��� ���⿡ �ִ��� ���
            Vector3 targetDirFromCamera = targettingSystem.currentTargetTransform.position - targettingSystem.playerTransform.position;

            arrowRectTransform.up = targetDirFromCamera;
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
