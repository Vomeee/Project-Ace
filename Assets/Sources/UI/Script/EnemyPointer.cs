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

            // 타겟이 카메라로부터 어느 방향에 있는지 계산
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
