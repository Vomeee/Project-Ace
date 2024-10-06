using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPointer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TargettingSystem targettingSystem; // 타겟 정보
    [SerializeField] Image arrowImage; // 화살표 이미지
    [SerializeField] RectTransform arrowRectTransform; // 화살표의 RectTransform

    [SerializeField] Color transparentColor; // 화살표가 보이지 않을 때의 색상
    [SerializeField] Color originalColor; // 화살표가 보일 때의 색상

    [SerializeField] bool isBeingShown; // 화살표가 활성화 상태인지
    [SerializeField] float radius = 150f; // 화살표가 움직일 반지름

    void Start()
    {
        // 시작 시 화살표를 비활성화
        arrowImage.color = transparentColor;
        isBeingShown = false;
    }

    void Update()
    {
        // 타겟의 월드 좌표를 뷰포트 좌표로 변환
        Vector3 targetViewportPos = Camera.main.WorldToViewportPoint(targettingSystem.currentTargetTransform.position);

        // 화면을 벗어났는지 확인 (x와 y 값이 0~1 범위를 벗어났는지 확인)
        bool isOutOfBounds = targetViewportPos.x < 0 || targetViewportPos.x > 1 || targetViewportPos.y < 0 || targetViewportPos.y > 1;

        if (isOutOfBounds)
        {
            if (!isBeingShown)
            {
                isBeingShown = true;
                arrowImage.color = originalColor;
            }

            // 타겟 방향을 카메라의 로컬 좌표계로 변환하여 계산
            Vector3 targetDirFromCamera = (targettingSystem.currentTargetTransform.position - Camera.main.transform.position).normalized;

            // 카메라의 로컬 좌표에서 타겟 방향을 계산 (카메라의 로컬 방향을 고려)
            Vector3 arrowDirection = Camera.main.transform.InverseTransformDirection(targetDirFromCamera);

            // 화살표의 transform.rotation을 카메라 회전을 반영하여 설정
            float angle = Mathf.Atan2(arrowDirection.x, arrowDirection.y) * Mathf.Rad2Deg;
            arrowImage.transform.rotation = Quaternion.Euler(0, -angle, -angle);

            // 화면 중앙을 기준으로 화살표 위치 계산
            Vector2 arrowPosition = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad) - 10) * radius;

            // 화살표의 RectTransform 위치 설정
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
