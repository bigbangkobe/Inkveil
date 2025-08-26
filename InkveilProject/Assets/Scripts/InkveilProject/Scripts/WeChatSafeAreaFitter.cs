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

    [Tooltip("是否适配安全区域")]
    public bool adaptSafeArea = true;

    [Tooltip("是否调整参考分辨率")]
    public bool adjustReferenceResolution = true;

    [Tooltip("安全区域边距（用于测试）")]
    public Vector4 safeAreaMargins = Vector4.zero;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        canvasRect = GetComponent<RectTransform>();

        // 设置初始锚点
        canvasRect.anchorMin = Vector2.zero;
        canvasRect.anchorMax = Vector2.one;

        // 适配安全区域
        if (adaptSafeArea)
        {
            StartCoroutine(AdaptToSafeArea());
        }
    }

    private IEnumerator AdaptToSafeArea()
    {
        // 等待一帧确保微信API已初始化
        yield return null;

#if UNITY_WECHAT
        // 获取微信窗口信息
        var windowInfo = WX.GetWindowInfo();
        
        // 使用真实安全区域或测试值
        Rect safeArea;
        if (safeAreaMargins != Vector4.zero)
        {
            // 使用测试值
            safeArea = new Rect(
                safeAreaMargins.x,
                safeAreaMargins.y,
                windowInfo.windowWidth - safeAreaMargins.x - safeAreaMargins.z,
                windowInfo.windowHeight - safeAreaMargins.y - safeAreaMargins.w
            );
        }
        else
        {
            // 使用真实安全区域
            safeArea = new Rect(
                windowInfo.safeArea.left,
                windowInfo.safeArea.top,
                windowInfo.safeArea.width,
                windowInfo.safeArea.height
            );
        }
        
        // 计算安全区域比例
        float left = safeArea.x / windowInfo.windowWidth;
        float bottom = safeArea.y / windowInfo.windowHeight;
        float right = (windowInfo.windowWidth - (safeArea.x + safeArea.width)) / windowInfo.windowWidth;
        float top = (windowInfo.windowHeight - (safeArea.y + safeArea.height)) / windowInfo.windowHeight;
        
        // 应用安全区域
        canvasRect.anchorMin = new Vector2(left, bottom);
        canvasRect.anchorMax = new Vector2(1 - right, 1 - top);
        
        // 调整参考分辨率
        if (adjustReferenceResolution)
        {
            CanvasScaler scaler = GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                // 根据安全区域高度调整参考分辨率
                float heightRatio = safeArea.height / windowInfo.windowHeight;
                scaler.referenceResolution = new Vector2(
                    scaler.referenceResolution.x,
                    scaler.referenceResolution.y / heightRatio
                );
            }
        }
#endif
    }

    // 编辑器测试方法
    [ContextMenu("Test Safe Area")]
    public void TestSafeArea()
    {
        if (Application.isPlaying) return;

        // 模拟安全区域
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