using UnityEngine;

public class HPUI : MonoBehaviour
{
    public PlayerController mPlayerController;
    public RectTransform canvasRect;  // Canvas �� RectTransform
    public RectTransform uiRect;      // Ѫ���� RectTransform

    private void Start()
    {
        mPlayerController = PlayerController.instance;
    }

    void Update()
    {
        if (mPlayerController == null || uiRect == null || canvasRect == null) return;

        // ���� �� ��Ļ
        Vector3 screenPos = Camera.main.WorldToScreenPoint(mPlayerController.transform.position);

        // ��Ļ �� UI �ֲ�����
        Vector2 uiPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null,       // Overlay ģʽ�� null��Camera ģʽҪ�� UI Camera
            out uiPos))
        {
            uiRect.anchoredPosition = new Vector2(uiPos.x, uiRect.anchoredPosition.y);
        }
    }
}
