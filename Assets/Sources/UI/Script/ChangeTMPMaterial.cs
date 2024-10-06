using UnityEngine;
using TMPro;

public class ChangeTMPMaterial : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro; // 텍스트 객체
    [SerializeField] private Material newMaterial; // 적용하고자 하는 새로운 머티리얼

    void Start()
    {
        // 만약 새로운 머티리얼을 적용하려면
        if (textMeshPro != null && newMaterial != null)
        {
            textMeshPro.fontSharedMaterial = newMaterial;
        }
    }
}
