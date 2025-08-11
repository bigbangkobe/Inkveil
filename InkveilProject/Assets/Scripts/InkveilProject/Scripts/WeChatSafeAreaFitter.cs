using UnityEngine;
using UnityEngine.UI;

public class WeChatSafeAreaFitter : MonoBehaviour
{
//    public RectTransform safeRoot;     // ȫ�� Stretch ������
//    public CanvasScaler canvasScaler;  // CanvasScaler (Scale With Screen Size)

//    // ����
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
//        // �ֱ��ʻ� Unity ��ȫ���б仯������
//        if (Screen.width != _lw || Screen.height != _lh || Screen.safeArea != _lastSafe)
//            Apply();
//    }

//    public void Apply()
//    {
//        if (safeRoot == null || canvasScaler == null) return;

//        _lw = Screen.width; _lh = Screen.height; _lastSafe = Screen.safeArea;

//        // ���� 1) Unity �Դ� safeArea��Բ��/����/Home ������λ������ px������
//        float insetTopPx = Screen.height - (Screen.safeArea.y + Screen.safeArea.height);
//        float insetBottomPx = Screen.safeArea.y;
//        float insetLeftPx = Screen.safeArea.x;
//        float insetRightPx = Screen.width - (Screen.safeArea.x + Screen.safeArea.width);

//        // ���� 2) ����΢�� SystemInfo.safeArea���߼� px����Ҫ �� pixelRatio �� ���� px������
//#if UNITY_WEBGL && !UNITY_EDITOR
//        try
//        {
//            // �������İ��������ռ��滻��ʾ����WeChatWASM��
//            var info = WeChatWASM.WX.GetSystemInfoSync();
//            if (info != null && info.safeArea != null)
//            {
//                float dpr = (float)info.pixelRatio;
//                // ΢������ԭ�������ϣ���ֵ���߼� px
//                float topPx    = (float)info.safeArea.top    * dpr;
//                float bottomPx = ((float)info.windowHeight - (float)info.safeArea.bottom) * dpr;
//                float leftPx   = (float)info.safeArea.left   * dpr;
//                float rightPx  = ((float)info.windowWidth - (float)info.safeArea.right)  * dpr;

//                // ȡ����ֵ�����⡰˭Сȡ˭�����±���
//                insetTopPx    = Mathf.Max(insetTopPx,    topPx);
//                insetBottomPx = Mathf.Max(insetBottomPx, bottomPx);
//                insetLeftPx   = Mathf.Max(insetLeftPx,   leftPx);
//                insetRightPx  = Mathf.Max(insetRightPx,  rightPx);
//            }

//            // ���� 3) ���ӡ����Ұ�ť�����Խ��� bottom ��Ϊ������Ե���߼� px �� dpr������
//            var cap = WeChatWASM.WX.GetMenuButtonBoundingClientRect();
//            if (cap != null)
//            {
//                float dpr = (float)info.pixelRatio;
//                float capsuleBottomPx = (float)cap.bottom * dpr;
//                insetTopPx = Mathf.Max(insetTopPx, capsuleBottomPx);
//            }
//        }
//        catch { /* ��΢�Ż������ϻ����⣬���� */ }
//#endif

//        // ���� 4) ���� px �� UI ���أ�ƥ�� CanvasScaler ���򣩡���
//        float topUI = DevicePxToUI(insetTopPx);
//        float bottomUI = DevicePxToUI(insetBottomPx);
//        float leftUI = DevicePxToUI(insetLeftPx);
//        float rightUI = DevicePxToUI(insetRightPx);

//        // ���� 5) Ӧ�õ� safeRoot���ı��ڱ߾ࣩ����
//        // safeRoot Ҫ�� AnchorMin(0,0) AnchorMax(1,1) sizeDelta(0,0)
//        safeRoot.offsetMin = new Vector2(leftUI, bottomUI);
//        safeRoot.offsetMax = new Vector2(-rightUI, -topUI);
//    }

//    float DevicePxToUI(float devicePx)
//    {
//        // �� CanvasScaler ������һ��
//        var refRes = canvasScaler.referenceResolution;
//        float match = canvasScaler.matchWidthOrHeight;
//        float scaleX = Screen.width / refRes.x;
//        float scaleY = Screen.height / refRes.y;
//        float scale = Mathf.Lerp(scaleX, scaleY, match); // Match=0:����1:����
//        return devicePx / scale;
//    }
}
