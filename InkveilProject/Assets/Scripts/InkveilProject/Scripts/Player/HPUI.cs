using UnityEngine;

public class HPUI : MonoBehaviour
{
    public PlayerController mPlayerController;
    public RectTransform canvasRect;  // Canvas 的 RectTransform
    public RectTransform uiRect;      // 血条的 RectTransform

    private void Start()
    {
        mPlayerController = PlayerController.instance;
    }

    void Update()
    {
        if (mPlayerController == null || uiRect == null || canvasRect == null) return;

        // 世界 → 屏幕
        Vector3 screenPos = Camera.main.WorldToScreenPoint(mPlayerController.transform.position);

        // 屏幕 → UI 局部坐标
        Vector2 uiPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null,       // Overlay 模式传 null；Camera 模式要传 UI Camera
            out uiPos))
        {
            uiRect.anchoredPosition = new Vector2(uiPos.x, uiRect.anchoredPosition.y);
        }
    }
}
