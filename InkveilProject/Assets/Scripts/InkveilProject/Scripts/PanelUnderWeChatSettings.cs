using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[ExecuteAlways]
public class PanelUnderWeChatSettings : MonoBehaviour
{
    //    [Header("可选：在安全区基础上再往下加一点顶边距（像素）")]
    //    [SerializeField] private float extraTopPadding = 0f;

    //    [Header("编辑器预览（非微信环境）")]
    //    [SerializeField] private bool simulateInEditor = true;
    //    [SerializeField] private float simulateTopCut = 44f;   // 顶部状态栏/刘海模拟
    //    [SerializeField] private float simulateBottomCut = 0f; // 底部手势区/横条模拟
    //    [SerializeField] private float simulateLeftCut = 0f;
    //    [SerializeField] private float simulateRightCut = 0f;

    //    private RectTransform _rt;
    //    private Vector2 _lastScreen;
    //    private Rect _lastApplied;

    //    private void Reset()
    //    {
    //        _rt = transform as RectTransform;
    //    }

    //    private void Awake()
    //    {
    //        _rt = transform as RectTransform;
    //        Apply();
    //    }

    //    private void OnEnable()
    //    {
    //        Apply();
    //    }

    //    private void Update()
    //    {
    //        Vector2 now = new Vector2(Screen.width, Screen.height);
    //        if (now != _lastScreen)
    //        {
    //            _lastScreen = now;
    //            Apply();
    //        }

    //#if UNITY_EDITOR
    //        if (simulateInEditor)
    //        {
    //            Apply();
    //        }
    //#endif
    //    }

    //    public void Apply()
    //    {
    //        if (!_rt) _rt = transform as RectTransform;
    //        if (!_rt) return;

    //        // 1) 拿到“以 Unity 左下角为原点”的安全区矩形
    //        Rect safe = GetSafeAreaRectInUnity();

    //        // 额外往下让一点顶边距（只对安全区做内缩）
    //        if (extraTopPadding > 0f)
    //        {
    //            float h = Mathf.Max(0f, safe.height - extraTopPadding);
    //            safe = new Rect(safe.x, safe.y, safe.width, h);
    //        }

    //        if (safe == _lastApplied && Application.isPlaying) return;
    //        _lastApplied = safe;

    //        float sw = Screen.width;
    //        float sh = Screen.height;

    //        // 2) 用安全区矩形换算成锚点（0~1），把整套 UI 限定在 SafeArea 内
    //        Vector2 anchorMin = new Vector2(safe.xMin / sw, safe.yMin / sh);
    //        Vector2 anchorMax = new Vector2(safe.xMax / sw, safe.yMax / sh);

    //        _rt.anchorMin = anchorMin;
    //        _rt.anchorMax = anchorMax;

    //        // 3) 取消偏移，让它正好贴住安全区
    //        _rt.offsetMin = Vector2.zero;
    //        _rt.offsetMax = Vector2.zero;

    //        // 4) 推荐把 pivot 设成中点，避免某些动画/缩放奇怪偏移
    //        _rt.pivot = new Vector2(0.5f, 0.5f);
    //    }

    //    private Rect GetSafeAreaRectInUnity()
    //    {
    //        float sw = Screen.width;
    //        float sh = Screen.height;

    //#if WEIXINMINIGAME || WECHAT_MINIGAME
    //        // 微信小游戏：GetSystemInfoSync + pixelRatio，且微信的 safeArea 是以“左上角”为原点
    //        var info = WeChatWASM.WX.GetSystemInfoSync();
    //        float px = SafeF(info.pixelRatio);

    //        float winW = SafeF(info.windowWidth) * px;
    //        float winH = SafeF(info.windowHeight) * px;

    //        float saLeft   = SafeF(info.safeArea.left)   * px;
    //        float saTop    = SafeF(info.safeArea.top)    * px;
    //        float saRight  = SafeF(info.safeArea.right)  * px;
    //        float saBottom = SafeF(info.safeArea.bottom) * px;
    //        float saWidth  = SafeF(info.safeArea.width)  * px;
    //        float saHeight = SafeF(info.safeArea.height) * px;

    //        if (saWidth <= 0f && saRight > 0f)   saWidth  = saRight - saLeft;
    //        if (saHeight <= 0f && saBottom > 0f) saHeight = saBottom - saTop;

    //        // 兜底：微信没给就当全屏
    //        if (saWidth <= 0f || saHeight <= 0f)
    //            return new Rect(0, 0, sw, sh);

    //        // 微信坐标：左上为原点；Unity：左下为原点
    //        float x = saLeft;
    //        float y = winH - (saTop + saHeight);
    //        Rect r = new Rect(x, y, saWidth, saHeight);

    //        // 如果 Unity 的 Screen 与微信窗口像素不一致，做一次比例映射
    //        if (!Mathf.Approximately(sw, winW) || !Mathf.Approximately(sh, winH))
    //        {
    //            float sx = sw / winW;
    //            float sy = sh / winH;
    //            r = new Rect(r.x * sx, r.y * sy, r.width * sx, r.height * sy);
    //        }
    //        return r;
    //#else
    //        // 其他平台：直接用 Unity 的 Screen.safeArea
    //        Rect r = Screen.safeArea;

    //#if UNITY_EDITOR
    //        if (simulateInEditor)
    //        {
    //            float left = Mathf.Max(0f, simulateLeftCut);
    //            float right = Mathf.Max(0f, simulateRightCut);
    //            float top = Mathf.Max(0f, simulateTopCut);
    //            float bottom = Mathf.Max(0f, simulateBottomCut);

    //            r = new Rect(
    //                left,
    //                bottom,
    //                sw - left - right,
    //                sh - top - bottom
    //            );
    //        }
    //#endif
    //        return r;
    //#endif
    //    }

    //    private static float SafeF(object v)
    //    {
    //        if (v == null) return 0f;
    //        try { return System.Convert.ToSingle(v); }
    //        catch { return 0f; }
    //    }
}
