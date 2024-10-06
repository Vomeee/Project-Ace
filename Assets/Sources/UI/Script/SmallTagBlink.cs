using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SmallTagBlink : MonoBehaviour
{
    [SerializeField] RawImage border;
    [SerializeField] TextMeshProUGUI text;

    [SerializeField] Color transparentColor;  // 투명한 색상
    [SerializeField] Color warningColor;      // 경고 색상 (빨간색 계열)
    [SerializeField] float blinkInterval = 0.25f; // 깜빡이는 간격 (초 단위)

    private float timeSinceLastBlink = 0f;  // 마지막으로 색상이 변경된 시간
    private bool isWarningColor = true;     // 현재 경고 색상이 적용된 상태인지 여부

    private void Start()
    {
        // 초기 색상 설정 (경고 색상)
        setColor(warningColor);
    }

    private void Update()
    {
        // 경과 시간 계산
        timeSinceLastBlink += Time.deltaTime;

        // 설정된 깜빡이기 간격이 경과했는지 확인
        if (timeSinceLastBlink >= blinkInterval)
        {
            // 색상 전환
            if (isWarningColor)
            {
                setColor(transparentColor);  // 투명색으로
            }
            else
            {
                setColor(warningColor);      // 경고 색상으로
            }

            // 색상 상태 반전
            isWarningColor = !isWarningColor;

            // 시간 초기화
            timeSinceLastBlink = 0f;
        }
    }

    // 색상을 변경하는 함수
    void setColor(Color color)
    {
        text.color = color;
        border.color = color;
    }
}
