using UnityEngine;

public class TrailWidthController : MonoBehaviour
{
    public TrailRenderer trailRenderer;
    public float newWidth = 5.0f;  // ���ϴ� �β� ��

    void Start()
    {
        // Trail Renderer�� �β� ����
        trailRenderer.widthMultiplier = newWidth;
    }
}
