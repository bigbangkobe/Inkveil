using UnityEngine;
using System.Collections;

#if UNITY_WECHAT
using WeChatWASM;
#endif

[RequireComponent(typeof(Canvas))]
public class WeChatSafeAreaFitter : MonoBehaviour
{
    private Canvas canvas;
    private RectTransform canvasRect;

    [Tooltip("�Ƿ����䰲ȫ����")]
    public bool adaptSafeArea = true;

    [Tooltip("�Ƿ�����ο��ֱ���")]
    public bool adjustReferenceResolution = true;

    [Tooltip("��ȫ����߾ࣨ���ڲ��ԣ�")]
    public Vector4 safeAreaMargins = Vector4.zero;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        canvasRect = GetComponent<RectTransform>();

        // ���ó�ʼê��
        canvasRect.anchorMin = Vector2.zero;
        canvasRect.anchorMax = Vector2.one;

        // ���䰲ȫ����
        if (adaptSafeArea)
        {
            StartCoroutine(AdaptToSafeArea());
        }
    }

    private IEnumerator AdaptToSafeArea()
    {
        // �ȴ�һ֡ȷ��΢��API�ѳ�ʼ��
        yield return null;

#if UNITY_WECHAT
        // ��ȡ΢�Ŵ�����Ϣ
        var windowInfo = WX.GetWindowInfo();
        
        // ʹ����ʵ��ȫ��������ֵ
        Rect safeArea;
        if (safeAreaMargins != Vector4.zero)
        {
            // ʹ�ò���ֵ
            safeArea = new Rect(
                safeAreaMargins.x,
                safeAreaMargins.y,
                windowInfo.windowWidth - safeAreaMargins.x - safeAreaMargins.z,
                windowInfo.windowHeight - safeAreaMargins.y - safeAreaMargins.w
            );
        }
        else
        {
            // ʹ����ʵ��ȫ����
            safeArea = new Rect(
                windowInfo.safeArea.left,
                windowInfo.safeArea.top,
                windowInfo.safeArea.width,
                windowInfo.safeArea.height
            );
        }
        
        // ���㰲ȫ�������
        float left = safeArea.x / windowInfo.windowWidth;
        float bottom = safeArea.y / windowInfo.windowHeight;
        float right = (windowInfo.windowWidth - (safeArea.x + safeArea.width)) / windowInfo.windowWidth;
        float top = (windowInfo.windowHeight - (safeArea.y + safeArea.height)) / windowInfo.windowHeight;
        
        // Ӧ�ð�ȫ����
        canvasRect.anchorMin = new Vector2(left, bottom);
        canvasRect.anchorMax = new Vector2(1 - right, 1 - top);
        
        // �����ο��ֱ���
        if (adjustReferenceResolution)
        {
            CanvasScaler scaler = GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                // ���ݰ�ȫ����߶ȵ����ο��ֱ���
                float heightRatio = safeArea.height / windowInfo.windowHeight;
                scaler.referenceResolution = new Vector2(
                    scaler.referenceResolution.x,
                    scaler.referenceResolution.y / heightRatio
                );
            }
        }
#endif
    }

    // �༭�����Է���
    [ContextMenu("Test Safe Area")]
    public void TestSafeArea()
    {
        if (Application.isPlaying) return;

        // ģ�ⰲȫ����
        canvasRect = GetComponent<RectTransform>();
        canvasRect.anchorMin = new Vector2(
            safeAreaMargins.x / Screen.width,
            safeAreaMargins.y / Screen.height
        );
        canvasRect.anchorMax = new Vector2(
            1 - safeAreaMargins.z / Screen.width,
            1 - safeAreaMargins.w / Screen.height
        );
    }
}