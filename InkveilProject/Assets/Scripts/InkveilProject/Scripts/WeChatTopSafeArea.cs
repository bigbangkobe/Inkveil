#if !UNITY_EDITOR && UNITY_WEBGL
using WeChatWASM;
#endif
using UnityEngine;
using UnityEngine.UI;

public class WeChatTopSafeArea : MonoBehaviour
{
    public RectTransform topBar;          // ��ק��� Top
    public int extraPadding = 8;          // ���������϶�����أ�

    void Start()
    {
        if (topBar == null) topBar = GetComponent<RectTransform>();

#if !UNITY_EDITOR && UNITY_WEBGL
        // 1) ��ȡ���Ұ�ť��״̬��
        var cap = WX.GetMenuButtonBoundingClientRect();
        var sys = WX.GetSystemInfoSync();
        var win = WX.GetWindowInfo();

        float statusBar     = (float)sys.statusBarHeight;
        float capsuleBottom = (float)cap.bottom;

        // ��־����ӡ״̬���߶ȡ����� bottom�����ڳߴ��밲ȫ��
        Debug.Log($"[WeChatTopSafeArea] statusBarHeight={statusBar}, cap.bottom={capsuleBottom}, " +
                  $"window=({win.windowWidth}x{win.windowHeight}), " +
                  $"safeArea(L{win.safeArea.left},T{win.safeArea.top},R{win.safeArea.right},B{win.safeArea.bottom})");

        float needTop = Mathf.Max(statusBar, capsuleBottom) + extraPadding;
        Debug.Log($"[WeChatTopSafeArea] needTop(max(statusBar, cap.bottom)+padding)={needTop}");

        ApplyTopPadding(Mathf.CeilToInt(needTop));
#else
        // ��΢�Ż��������豸��ȫ����������
        var safe = Screen.safeArea;
        float topInset = (Screen.height - (safe.y + safe.height));

        Debug.Log($"[WeChatTopSafeArea][Approx] Screen=({Screen.width}x{Screen.height}), " +
                  $"safeArea(x={safe.x}, y={safe.y}, w={safe.width}, h={safe.height}), " +
                  $"topInset~={topInset}, extraPadding={extraPadding}");

        ApplyTopPadding(Mathf.CeilToInt(topInset) + extraPadding);
#endif
    }

    void ApplyTopPadding(int topPx)
    {
        // ȷ����ˮƽ Stretch����ֱ����
        topBar.anchorMin = new Vector2(0f, 1f);
        topBar.anchorMax = new Vector2(1f, 1f);
        topBar.pivot = new Vector2(0.5f, 1f);

        // ��Ҫ�� sizeDelta.y�����������λ�߶ȣ���ֻ������������Ų
        var ap = topBar.anchoredPosition;
        ap.y = -topPx;                     // ������ʾ����
        topBar.anchoredPosition = ap;

        // ������ʷ���������ﲻ�Ķ� offsetMin/Max ��ֵ��ֻ�Ǹ���ȥˢ�£�
        topBar.offsetMin = new Vector2(topBar.offsetMin.x, topBar.offsetMin.y);
        topBar.offsetMax = new Vector2(topBar.offsetMax.x, topBar.offsetMax.y);

        Debug.Log($"[WeChatTopSafeArea] ApplyTopPadding -> anchoredPosition.y={topBar.anchoredPosition.y}, topPx={topPx}");
    }
}
