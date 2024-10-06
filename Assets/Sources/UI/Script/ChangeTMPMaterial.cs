using UnityEngine;
using TMPro;

public class ChangeTMPMaterial : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro; // �ؽ�Ʈ ��ü
    [SerializeField] private Material newMaterial; // �����ϰ��� �ϴ� ���ο� ��Ƽ����

    void Start()
    {
        // ���� ���ο� ��Ƽ������ �����Ϸ���
        if (textMeshPro != null && newMaterial != null)
        {
            textMeshPro.fontSharedMaterial = newMaterial;
        }
    }
}
