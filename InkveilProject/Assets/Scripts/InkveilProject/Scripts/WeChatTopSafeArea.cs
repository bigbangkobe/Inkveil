#if !UNITY_EDITOR && UNITY_WEBGL
using WeChatWASM;
#endif
using UnityEngine;
using UnityEngine.UI;

public class WeChatTopSafeArea : MonoBehaviour
{
    public RectTransform topBar;          // 拖拽你的 Top
    public int extraPadding = 8;          // 额外留点缝隙（像素）

    void Start()
    {
        if (topBar == null) topBar = GetComponent<RectTransform>();

#if !UNITY_EDITOR && UNITY_WEBGL
        // 1) 读取胶囊按钮与状态栏
        var cap = WX.GetMenuButtonBoundingClientRect();
        var sys = WX.GetSystemInfoSync();
        var win = WX.GetWindowInfo();

        float statusBar     = (float)sys.statusBarHeight;
        float capsuleBottom = (float)cap.bottom;

        // 日志：打印状态栏高度、胶囊 bottom、窗口尺寸与安全区
        Debug.Log($"[WeChatTopSafeArea] statusBarHeight={statusBar}, cap.bottom={capsuleBottom}, " +
                  $"window=({win.windowWidth}x{win.windowHeight}), " +
                  $"safeArea(L{win.safeArea.left},T{win.safeArea.top},R{win.safeArea.right},B{win.safeArea.bottom})");

        float needTop = Mathf.Max(statusBar, capsuleBottom) + extraPadding;
        Debug.Log($"[WeChatTopSafeArea] needTop(max(statusBar, cap.bottom)+padding)={needTop}");

        ApplyTopPadding(Mathf.CeilToInt(needTop));
#else
        // 非微信环境：按设备安全区做个近似
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
        // 确保：水平 Stretch，垂直贴顶
        topBar.anchorMin = new Vector2(0f, 1f);
        topBar.anchorMax = new Vector2(1f, 1f);
        topBar.pivot = new Vector2(0.5f, 1f);

        // 不要动 sizeDelta.y（就是你的栏位高度），只把它整体往下挪
        var ap = topBar.anchoredPosition;
        ap.y = -topPx;                     // 负数表示往下
        topBar.anchoredPosition = ap;

        // 避免历史残留（这里不改动 offsetMin/Max 数值，只是赋回去刷新）
        topBar.offsetMin = new Vector2(topBar.offsetMin.x, topBar.offsetMin.y);
        topBar.offsetMax = new Vector2(topBar.offsetMax.x, topBar.offsetMax.y);

        Debug.Log($"[WeChatTopSafeArea] ApplyTopPadding -> anchoredPosition.y={topBar.anchoredPosition.y}, topPx={topPx}");
    }
}
