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

        float statusBar   = (float)sys.statusBarHeight;
        float capsuleBottom = (float)cap.bottom;

        float needTop = Mathf.Max(statusBar, capsuleBottom) + extraPadding;
        ApplyTopPadding(Mathf.CeilToInt(needTop));

#else
        // ��΢�Ż��������豸��ȫ����������
        var safe = Screen.safeArea;
        float topInset = (Screen.height - (safe.y + safe.height));
        ApplyTopPadding(Mathf.CeilToInt(topInset) + extraPadding);
#endif
    }

    void ApplyTopPadding(int topPx)
    {
        // ��ʽA���� LayoutGroup �� padding����� Top ���ǲ��֣�
        var layout = topBar.GetComponent<HorizontalOrVerticalLayoutGroup>();
        if (layout != null)
        {
            layout.padding.top = topPx;
            LayoutRebuilder.ForceRebuildLayoutImmediate(topBar);
            return;
        }

        // ��ʽB��ֱ�Ӹ� RectTransform ƫ�ƣ�Top-Stretch ê��ǰ�ᣩ
        var offMax = topBar.offsetMax; // (right, top)
        offMax.y = -topPx;             // ������
        topBar.offsetMax = offMax;
    }
}
