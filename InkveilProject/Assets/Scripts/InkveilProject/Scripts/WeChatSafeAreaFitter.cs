using UnityEngine;
using UnityEngine.UI;

public class WeChatSafeAreaFitter : MonoBehaviour
{
//    public RectTransform safeRoot;     // 全屏 Stretch 的容器
//    public CanvasScaler canvasScaler;  // CanvasScaler (Scale With Screen Size)

//    // 缓存
//    private int _lw, _lh;
//    private Rect _lastSafe;

//    void Awake()
//    {
//        if (safeRoot == null)
//            safeRoot = GetComponentInParent<RectTransform>();
//        if (canvasScaler == null)
//            canvasScaler = GetComponentInParent<CanvasScaler>();
//    }

//    void Start() => Apply();

//    void Update()
//    {
//        // 分辨率或 Unity 安全区有变化则重算
//        if (Screen.width != _lw || Screen.height != _lh || Screen.safeArea != _lastSafe)
//            Apply();
//    }

//    public void Apply()
//    {
//        if (safeRoot == null || canvasScaler == null) return;

//        _lw = Screen.width; _lh = Screen.height; _lastSafe = Screen.safeArea;

//        // —— 1) Unity 自带 safeArea（圆角/刘海/Home 条，单位：物理 px）——
//        float insetTopPx = Screen.height - (Screen.safeArea.y + Screen.safeArea.height);
//        float insetBottomPx = Screen.safeArea.y;
//        float insetLeftPx = Screen.safeArea.x;
//        float insetRightPx = Screen.width - (Screen.safeArea.x + Screen.safeArea.width);

//        // —— 2) 叠加微信 SystemInfo.safeArea（逻辑 px，需要 × pixelRatio → 物理 px）——
//#if UNITY_WEBGL && !UNITY_EDITOR
//        try
//        {
//            // 按你接入的包名命名空间替换（示例：WeChatWASM）
//            var info = WeChatWASM.WX.GetSystemInfoSync();
//            if (info != null && info.safeArea != null)
//            {
//                float dpr = (float)info.pixelRatio;
//                // 微信坐标原点在左上，数值是逻辑 px
//                float topPx    = (float)info.safeArea.top    * dpr;
//                float bottomPx = ((float)info.windowHeight - (float)info.safeArea.bottom) * dpr;
//                float leftPx   = (float)info.safeArea.left   * dpr;
//                float rightPx  = ((float)info.windowWidth - (float)info.safeArea.right)  * dpr;

//                // 取更大值，避免“谁小取谁”导致被遮
//                insetTopPx    = Mathf.Max(insetTopPx,    topPx);
//                insetBottomPx = Mathf.Max(insetBottomPx, bottomPx);
//                insetLeftPx   = Mathf.Max(insetLeftPx,   leftPx);
//                insetRightPx  = Mathf.Max(insetRightPx,  rightPx);
//            }

//            // —— 3) 叠加“胶囊按钮”：以胶囊 bottom 作为顶部下缘（逻辑 px × dpr）——
//            var cap = WeChatWASM.WX.GetMenuButtonBoundingClientRect();
//            if (cap != null)
//            {
//                float dpr = (float)info.pixelRatio;
//                float capsuleBottomPx = (float)cap.bottom * dpr;
//                insetTopPx = Mathf.Max(insetTopPx, capsuleBottomPx);
//            }
//        }
//        catch { /* 非微信环境或老基础库，忽略 */ }
//#endif

//        // —— 4) 物理 px → UI 像素（匹配 CanvasScaler 规则）——
//        float topUI = DevicePxToUI(insetTopPx);
//        float bottomUI = DevicePxToUI(insetBottomPx);
//        float leftUI = DevicePxToUI(insetLeftPx);
//        float rightUI = DevicePxToUI(insetRightPx);

//        // —— 5) 应用到 safeRoot（四边内边距）——
//        // safeRoot 要求 AnchorMin(0,0) AnchorMax(1,1) sizeDelta(0,0)
//        safeRoot.offsetMin = new Vector2(leftUI, bottomUI);
//        safeRoot.offsetMax = new Vector2(-rightUI, -topUI);
//    }

//    float DevicePxToUI(float devicePx)
//    {
//        // 与 CanvasScaler 的缩放一致
//        var refRes = canvasScaler.referenceResolution;
//        float match = canvasScaler.matchWidthOrHeight;
//        float scaleX = Screen.width / refRes.x;
//        float scaleY = Screen.height / refRes.y;
//        float scale = Mathf.Lerp(scaleX, scaleY, match); // Match=0:按宽，1:按高
//        return devicePx / scale;
//    }
}
