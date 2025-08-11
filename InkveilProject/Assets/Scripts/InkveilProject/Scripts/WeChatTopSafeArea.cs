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

        float statusBar   = (float)sys.statusBarHeight;
        float capsuleBottom = (float)cap.bottom;

        float needTop = Mathf.Max(statusBar, capsuleBottom) + extraPadding;
        ApplyTopPadding(Mathf.CeilToInt(needTop));

#else
        // 非微信环境：按设备安全区做个近似
        var safe = Screen.safeArea;
        float topInset = (Screen.height - (safe.y + safe.height));
        ApplyTopPadding(Mathf.CeilToInt(topInset) + extraPadding);
#endif
    }

    void ApplyTopPadding(int topPx)
    {
        // 方式A：用 LayoutGroup 的 padding（如果 Top 里是布局）
        var layout = topBar.GetComponent<HorizontalOrVerticalLayoutGroup>();
        if (layout != null)
        {
            layout.padding.top = topPx;
            LayoutRebuilder.ForceRebuildLayoutImmediate(topBar);
            return;
        }

        // 方式B：直接改 RectTransform 偏移（Top-Stretch 锚点前提）
        var offMax = topBar.offsetMax; // (right, top)
        offMax.y = -topPx;             // 往下推
        topBar.offsetMax = offMax;
    }
}
