using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[ExecuteAlways]
public class PanelUnderWeChatSettings : MonoBehaviour
{
    //    [Header("��ѡ���ڰ�ȫ�������������¼�һ�㶥�߾ࣨ���أ�")]
    //    [SerializeField] private float extraTopPadding = 0f;

    //    [Header("�༭��Ԥ������΢�Ż�����")]
    //    [SerializeField] private bool simulateInEditor = true;
    //    [SerializeField] private float simulateTopCut = 44f;   // ����״̬��/����ģ��
    //    [SerializeField] private float simulateBottomCut = 0f; // �ײ�������/����ģ��
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

    //        // 1) �õ����� Unity ���½�Ϊԭ�㡱�İ�ȫ������
    //        Rect safe = GetSafeAreaRectInUnity();

    //        // ����������һ�㶥�߾ֻࣨ�԰�ȫ����������
    //        if (extraTopPadding > 0f)
    //        {
    //            float h = Mathf.Max(0f, safe.height - extraTopPadding);
    //            safe = new Rect(safe.x, safe.y, safe.width, h);
    //        }

    //        if (safe == _lastApplied && Application.isPlaying) return;
    //        _lastApplied = safe;

    //        float sw = Screen.width;
    //        float sh = Screen.height;

    //        // 2) �ð�ȫ�����λ����ê�㣨0~1���������� UI �޶��� SafeArea ��
    //        Vector2 anchorMin = new Vector2(safe.xMin / sw, safe.yMin / sh);
    //        Vector2 anchorMax = new Vector2(safe.xMax / sw, safe.yMax / sh);

    //        _rt.anchorMin = anchorMin;
    //        _rt.anchorMax = anchorMax;

    //        // 3) ȡ��ƫ�ƣ�����������ס��ȫ��
    //        _rt.offsetMin = Vector2.zero;
    //        _rt.offsetMax = Vector2.zero;

    //        // 4) �Ƽ��� pivot ����е㣬����ĳЩ����/�������ƫ��
    //        _rt.pivot = new Vector2(0.5f, 0.5f);
    //    }

    //    private Rect GetSafeAreaRectInUnity()
    //    {
    //        float sw = Screen.width;
    //        float sh = Screen.height;

    //#if WEIXINMINIGAME || WECHAT_MINIGAME
    //        // ΢��С��Ϸ��GetSystemInfoSync + pixelRatio����΢�ŵ� safeArea ���ԡ����Ͻǡ�Ϊԭ��
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

    //        // ���ף�΢��û���͵�ȫ��
    //        if (saWidth <= 0f || saHeight <= 0f)
    //            return new Rect(0, 0, sw, sh);

    //        // ΢�����꣺����Ϊԭ�㣻Unity������Ϊԭ��
    //        float x = saLeft;
    //        float y = winH - (saTop + saHeight);
    //        Rect r = new Rect(x, y, saWidth, saHeight);

    //        // ��� Unity �� Screen ��΢�Ŵ������ز�һ�£���һ�α���ӳ��
    //        if (!Mathf.Approximately(sw, winW) || !Mathf.Approximately(sh, winH))
    //        {
    //            float sx = sw / winW;
    //            float sy = sh / winH;
    //            r = new Rect(r.x * sx, r.y * sy, r.width * sx, r.height * sy);
    //        }
    //        return r;
    //#else
    //        // ����ƽ̨��ֱ���� Unity �� Screen.safeArea
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
