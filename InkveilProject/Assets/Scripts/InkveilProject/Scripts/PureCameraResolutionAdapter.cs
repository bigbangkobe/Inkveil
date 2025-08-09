using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ResolutionFitter : MonoBehaviour
{
    [Header("参考分辨率设置")]
    [Tooltip("默认分辨率宽度")] public float referenceWidth = 1440f;
    [Tooltip("默认分辨率高度")] public float referenceHeight = 3200f;

    [Header("适配策略")]
    [Tooltip("裁剪模式")] public FitMode fitMode = FitMode.Fill;
    [Tooltip("保持原始内容比例")] public bool preserveContentAspect = true;

    [Header("高级设置")]
    [Tooltip("缩放缓冲系数")][Range(1.0f, 1.2f)] public float scaleBuffer = 1.05f;
    [Tooltip("更新阈值")][Range(1f, 20f)] public float updateThreshold = 5f;

    private Camera _camera;
    private float _targetAspect;
    private Vector2 _lastScreenSize;
    private float _lastAspect;
    private RenderTexture _renderTexture;

    public enum FitMode
    {
        Fill,       // 填充整个屏幕（可能裁剪）
        Fit         // 适应屏幕（可能留空隙）
    }

    void Start()
    {
        _camera = GetComponent<Camera>();
        _targetAspect = referenceWidth / referenceHeight;

        InitializeCamera();
        UpdateCamera();

        _lastScreenSize = new Vector2(Screen.width, Screen.height);
        _lastAspect = (float)Screen.width / Screen.height;
    }

    void Update()
    {
        // 只在分辨率变化时更新（性能优化）
        if (ShouldUpdateResolution())
        {
            UpdateCamera();
            _lastScreenSize = new Vector2(Screen.width, Screen.height);
            _lastAspect = (float)Screen.width / Screen.height;
        }
    }

    private bool ShouldUpdateResolution()
    {
        // 检测分辨率变化
        if (Mathf.Abs(_lastScreenSize.x - Screen.width) > updateThreshold ||
            Mathf.Abs(_lastScreenSize.y - Screen.height) > updateThreshold)
        {
            return true;
        }

        // 检测宽高比变化
        float currentAspect = (float)Screen.width / Screen.height;
        return Mathf.Abs(currentAspect - _lastAspect) > 0.01f;
    }

    private void InitializeCamera()
    {
        // 创建渲染纹理（用于高级适配）
        if (SystemInfo.supportsRenderTextures)
        {
            _renderTexture = new RenderTexture(
                (int)referenceWidth,
                (int)referenceHeight,
                24,
                RenderTextureFormat.Default
            );

            _camera.targetTexture = _renderTexture;
        }
    }

    private void UpdateCamera()
    {
        float currentAspect = (float)Screen.width / Screen.height;

        switch (fitMode)
        {
            case FitMode.Fill:
                ApplyFillMode(currentAspect);
                break;
            case FitMode.Fit:
                ApplyFitMode(currentAspect);
                break;
        }
    }

    private void ApplyFillMode(float currentAspect)
    {
        // 计算缩放比例
        float scaleRatio;

        if (preserveContentAspect)
        {
            // 保持内容比例，裁剪多余部分
            scaleRatio = currentAspect > _targetAspect ?
                (float)Screen.height / referenceHeight :
                (float)Screen.width / referenceWidth;
        }
        else
        {
            // 不保持比例，完全填充
            scaleRatio = Mathf.Max(
                (float)Screen.width / referenceWidth,
                (float)Screen.height / referenceHeight
            );
        }

        // 应用缩放（带缓冲系数）
        float bufferScale = scaleRatio * scaleBuffer;
        transform.localScale = new Vector3(bufferScale, bufferScale, 1f);

        // 调整相机视口
        _camera.rect = new Rect(0, 0, 1, 1);

        // 调整正交相机的尺寸
        if (_camera.orthographic)
        {
            _camera.orthographicSize = referenceHeight / 2f / scaleRatio;
        }
    }

    private void ApplyFitMode(float currentAspect)
    {
        // 计算缩放比例
        float scaleRatio;

        if (preserveContentAspect)
        {
            // 保持内容比例，适应屏幕
            scaleRatio = currentAspect < _targetAspect ?
                (float)Screen.height / referenceHeight :
                (float)Screen.width / referenceWidth;
        }
        else
        {
            // 不保持比例，填充但不裁剪
            scaleRatio = Mathf.Min(
                (float)Screen.width / referenceWidth,
                (float)Screen.height / referenceHeight
            );
        }

        // 应用缩放（带缓冲系数）
        float bufferScale = scaleRatio * scaleBuffer;
        transform.localScale = new Vector3(bufferScale, bufferScale, 1f);

        // 调整相机视口
        _camera.rect = new Rect(0, 0, 1, 1);

        // 调整正交相机的尺寸
        if (_camera.orthographic)
        {
            _camera.orthographicSize = referenceHeight / 2f / scaleRatio;
        }
    }

    // 高级渲染模式切换
    public void ToggleRenderTexture(bool useRenderTexture)
    {
        if (SystemInfo.supportsRenderTextures)
        {
            if (useRenderTexture && _renderTexture != null)
            {
                _camera.targetTexture = _renderTexture;
            }
            else
            {
                _camera.targetTexture = null;
            }
        }
    }

    // 清理资源
    void OnDestroy()
    {
        if (_renderTexture != null)
        {
            _renderTexture.Release();
            Destroy(_renderTexture);
        }
    }

#if UNITY_EDITOR
    [Header("编辑器调试")]
    public Vector2 testResolution = new Vector2(1080, 1920);
    public bool applyTestResolution;
    public bool forceUpdate;

    private void OnValidate()
    {
        if (forceUpdate && Application.isPlaying)
        {
            UpdateCamera();
        }

        if (applyTestResolution && Application.isPlaying)
        {
            // 模拟分辨率变化
            Screen.SetResolution((int)testResolution.x, (int)testResolution.y, false);
            UpdateCamera();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // 在场景视图中显示相机视口边界
        Gizmos.color = Color.green;
        Vector3 center = transform.position;
        Vector3 size = new Vector3(
            referenceWidth * transform.localScale.x,
            referenceHeight * transform.localScale.y,
            0.1f
        );

        Gizmos.DrawWireCube(center, size);
    }
#endif
}