using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SmallTagBlink : MonoBehaviour
{
    [SerializeField] RawImage border;
    [SerializeField] TextMeshProUGUI text;

    [SerializeField] Color transparentColor;  // ������ ����
    [SerializeField] Color warningColor;      // ��� ���� (������ �迭)
    [SerializeField] float blinkInterval = 0.25f; // �����̴� ���� (�� ����)

    private float timeSinceLastBlink = 0f;  // ���������� ������ ����� �ð�
    private bool isWarningColor = true;     // ���� ��� ������ ����� �������� ����

    private void Start()
    {
        // �ʱ� ���� ���� (��� ����)
        setColor(warningColor);
    }

    private void Update()
    {
        // ��� �ð� ���
        timeSinceLastBlink += Time.deltaTime;

        // ������ �����̱� ������ ����ߴ��� Ȯ��
        if (timeSinceLastBlink >= blinkInterval)
        {
            // ���� ��ȯ
            if (isWarningColor)
            {
                setColor(transparentColor);  // ���������
            }
            else
            {
                setColor(warningColor);      // ��� ��������
            }

            // ���� ���� ����
            isWarningColor = !isWarningColor;

            // �ð� �ʱ�ȭ
            timeSinceLastBlink = 0f;
        }
    }

    // ������ �����ϴ� �Լ�
    void setColor(Color color)
    {
        text.color = color;
        border.color = color;
    }
}
