using UnityEngine;

public class TrailWidthController : MonoBehaviour
{
    public TrailRenderer trailRenderer;
    public float newWidth = 5.0f;  // 원하는 두께 값

    void Start()
    {
        // Trail Renderer의 두께 설정
        trailRenderer.widthMultiplier = newWidth;
    }
}
