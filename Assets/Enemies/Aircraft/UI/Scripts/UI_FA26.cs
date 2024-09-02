using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockOnUI : MonoBehaviour
{
    public Transform target; // 적 기체의 Transform
    public Camera mainCamera; // 메인 카메라
    public RectTransform lockOnUI; // 락온 UI의 RectTransform
    public Image lockOnUIImage; // UI의 Image 컴포넌트

    public float minDistance = 10f; // UI가 보이기 시작하는 최소 거리
    public float maxDistance = 100f; // UI가 최대 크기에 도달하는 거리
    public Vector2 minSize = new Vector2(10f, 10f); // UI의 최소 크기
    public Vector2 maxSize = new Vector2(100f, 100f); // UI의 최대 크기

    public Color lockedOnColor = Color.red; // 락온 상태일 때의 색상
    public Color targetedColor = Color.yellow; // 타게팅 상태일 때의 색상
    private Color originalColor; // 원래 색상
    private bool isFlickering = false; // 깜빡임 여부

    private EnemyAI enemyAI;

    void Start()
    {
        if (lockOnUIImage != null)
        {
            originalColor = lockOnUIImage.color;
        }

        if (target != null)
        {
            enemyAI = target.GetComponent<EnemyAI>();
        }
    }

    void Update()
    {
        if (enemyAI == null || lockOnUIImage == null) return;

        // 적 기체의 월드 좌표를 화면 좌표로 변환
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        // 적과 플레이어 간의 거리 계산
        float distanceToTarget = Vector3.Distance(mainCamera.transform.position, target.position);

        // 적이 카메라의 시야 안에 있는지 확인
        if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
        {
            // 거리 범위 내에서만 UI 표시
            if (distanceToTarget <= maxDistance)
            {
                lockOnUI.gameObject.SetActive(true);

                // 거리 기반으로 크기 조정
                float t = Mathf.InverseLerp(minDistance, maxDistance, distanceToTarget);
                lockOnUI.sizeDelta = Vector2.Lerp(minSize, maxSize, t);

                // 화면 좌표를 UI 캔버스 좌표로 변환
                lockOnUI.position = screenPos;

                // 색상 및 깜빡임 효과 적용
                if (enemyAI.isLockedOn)
                {
                    // 락온 상태
                    lockOnUIImage.color = lockedOnColor;
                    if (isFlickering) StopCoroutine("FlickerEffect");
                }
                else if (enemyAI.isTargeted)
                {
                    // 타게팅 상태
                    if (!isFlickering) StartCoroutine("FlickerEffect");
                }
                else
                {
                    // 타겟팅되지 않은 상태

                    if (isFlickering) StopCoroutine("FlickerEffect");
                }
            }
            else
            {
                // 거리 범위 밖이면 UI 숨기기
                lockOnUI.gameObject.SetActive(false);
            }
        }
        else
        {
            // 적이 카메라 시야 밖에 있을 때 UI 숨기기
            lockOnUI.gameObject.SetActive(false);
        }
    }

    private IEnumerator FlickerEffect()
    {
        isFlickering = true;
        while (true)
        {
            lockOnUIImage.color = originalColor;
            yield return new WaitForSeconds(0.5f);
            lockOnUIImage.color = targetedColor;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
