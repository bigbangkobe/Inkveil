using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraResolutionAdapter : MonoBehaviour
{
    [Header("参考分辨率设置")]
    [Tooltip("默认分辨率宽度")] public float referenceWidth = 1440f;
    [Tooltip("默认分辨率高度")] public float referenceHeight = 3200f;

    [Header("相机默认值 (1440x3200)")]
    [Tooltip("基础视野值")][Range(1f, 179f)] public float baseFOV = 20.8f;
    [Tooltip("基础焦距值")] private float baseFocalLength = 65.38286f;

    [Header("适配设置")]
    [Tooltip("视野调整模式")] public FOVAdjustMode fovAdjustMode = FOVAdjustMode.Balanced;
    [Tooltip("最大视野")][Range(1f, 179f)] public float maxFOV = 25f;
    [Tooltip("最小视野")][Range(1f, 179f)] public float minFOV = 18f;
    [Tooltip("宽高比容差值")][Range(0.01f, 0.5f)] public float aspectTolerance = 0.05f;
    [Tooltip("使用物理相机参数")] public bool usePhysicalCamera = true;

    public enum FOVAdjustMode
    {
        Vertical,   // 调整垂直FOV
        Horizontal, // 调整水平FOV
        Balanced    // 平衡调整（推荐）
    }

    private Camera _camera;
    private float _targetAspect;
    private Vector2 _lastScreenSize;
    private ScreenOrientation _lastOrientation;

    void Start()
    {
        _camera = GetComponent<Camera>();
        _targetAspect = referenceWidth / referenceHeight;

        // 应用默认值（1440x3200分辨率设置）
        ApplyCameraSettingsForResolution(referenceWidth, referenceHeight);

        // 保存当前屏幕状态
        _lastScreenSize = new Vector2(Screen.width, Screen.height);
        _lastOrientation = Screen.orientation;

        Debug.Log($"初始化相机: 焦距={baseFocalLength:F2}mm, FOV={baseFOV:F1}°");
    }

    void Update()
    {
        // 屏幕尺寸或方向变化时重新适配
        if (ScreenSizeChanged() || OrientationChanged())
        {
            UpdateCameraSettings();
            _lastScreenSize = new Vector2(Screen.width, Screen.height);
            _lastOrientation = Screen.orientation;
        }
    }

    private bool ScreenSizeChanged()
    {
        return Mathf.Abs(_lastScreenSize.x - Screen.width) > 0.1f ||
               Mathf.Abs(_lastScreenSize.y - Screen.height) > 0.1f;
    }

    private bool OrientationChanged()
    {
        return _lastOrientation != Screen.orientation;
    }

    private void UpdateCameraSettings()
    {
        ApplyCameraSettingsForResolution(Screen.width, Screen.height);
    }

    private void ApplyCameraSettingsForResolution(float width, float height)
    {
        // 计算当前宽高比
        float currentAspect = width / height;

        // 计算宽高比差异
        float aspectDifference = currentAspect / _targetAspect;

        // 在容差范围内视为相同宽高比，使用默认设置
        if (Mathf.Abs(1 - aspectDifference) < aspectTolerance)
        {
            ApplyDefaultSettings();
            return;
        }

        // 根据宽高比差异调整相机设置
        AdjustCameraForAspect(aspectDifference, currentAspect);
    }

    private void AdjustCameraForAspect(float aspectDifference, float currentAspect)
    {
        float targetFOV = baseFOV;

        switch (fovAdjustMode)
        {
            case FOVAdjustMode.Vertical:
                targetFOV = AdjustVerticalFOV(aspectDifference);
                break;
            case FOVAdjustMode.Horizontal:
                targetFOV = AdjustHorizontalFOV(aspectDifference);
                break;
            case FOVAdjustMode.Balanced:
                targetFOV = AdjustBalancedFOV(aspectDifference, currentAspect);
                break;
        }

        ApplyFOV(Mathf.Clamp(targetFOV, minFOV, maxFOV));

        // 调试信息
        Debug.Log($"分辨率: {Screen.width}x{Screen.height}, " +
                 $"宽高比: {currentAspect:F3}, " +
                 $"FOV: {targetFOV:F1}°, " +
                 $"模式: {fovAdjustMode}");
    }

    private float AdjustVerticalFOV(float aspectDifference)
    {
        // 垂直FOV调整：保持水平视野一致
        return baseFOV * aspectDifference;
    }

    private float AdjustHorizontalFOV(float aspectDifference)
    {
        // 水平FOV调整：保持垂直视野一致
        return baseFOV / aspectDifference;
    }

    private float AdjustBalancedFOV(float aspectDifference, float currentAspect)
    {
        // 平衡方法：保持对角线FOV一致（最自然的透视）
        float diagonalFOV = CalculateDiagonalFOV(baseFOV, _targetAspect);

        return 2.0f * Mathf.Atan(
            Mathf.Tan(diagonalFOV * Mathf.Deg2Rad / 2.0f) *
            Mathf.Sqrt(1.0f / (1.0f + currentAspect * currentAspect))
        ) * Mathf.Rad2Deg;
    }

    private float CalculateDiagonalFOV(float fov, float aspect)
    {
        // 计算对角线视野
        float verticalFOV = fov * Mathf.Deg2Rad;
        float horizontalFOV = 2.0f * Mathf.Atan(Mathf.Tan(verticalFOV / 2.0f) * aspect);
        return 2.0f * Mathf.Atan(Mathf.Sqrt(
            Mathf.Tan(verticalFOV / 2.0f) * Mathf.Tan(verticalFOV / 2.0f) +
            Mathf.Tan(horizontalFOV / 2.0f) * Mathf.Tan(horizontalFOV / 2.0f)
        )) * Mathf.Rad2Deg;
    }

    private void ApplyFOV(float fov)
    {
        if (usePhysicalCamera)
        {
            // 使用物理相机参数（焦距）
            // FOV = 2 * arctan(sensorSize / (2 * focalLength))
            // 因此：focalLength = sensorSize / (2 * tan(FOV/2))
            float sensorSize = _camera.sensorSize.y; // 使用垂直传感器尺寸
            float targetFocalLength = sensorSize / (2.0f * Mathf.Tan(fov * Mathf.Deg2Rad / 2.0f));

            _camera.usePhysicalProperties = true;
            _camera.focalLength = targetFocalLength;
        }
        else
        {
            // 使用标准FOV
            _camera.usePhysicalProperties = false;
            _camera.fieldOfView = fov;
        }
    }

    private void ApplyDefaultSettings()
    {
        if (usePhysicalCamera)
        {
            _camera.usePhysicalProperties = true;
            _camera.focalLength = baseFocalLength;
        }
        else
        {
            _camera.usePhysicalProperties = false;
            _camera.fieldOfView = baseFOV;
        }
    }

    // 编辑器调试方法
#if UNITY_EDITOR
    [Header("编辑器调试")]
    public Vector2 testResolution = new Vector2(1440, 3200);
    public bool applyTestResolution;
    public bool logDebugInfo;

    private void OnValidate()
    {
        // 确保基础值在合理范围内
        baseFOV = Mathf.Clamp(baseFOV, 1f, 179f);
        baseFocalLength = Mathf.Clamp(baseFocalLength, 10f, 300f);
        minFOV = Mathf.Clamp(minFOV, 1f, baseFOV);
        maxFOV = Mathf.Clamp(maxFOV, baseFOV, 179f);

        if (applyTestResolution && Application.isPlaying)
        {
            // 模拟分辨率变化
            Screen.SetResolution((int)testResolution.x, (int)testResolution.y, false);
            ApplyCameraSettingsForResolution(testResolution.x, testResolution.y);
        }
    }

    void OnGUI()
    {
        if (logDebugInfo && Application.isPlaying)
        {
            string mode = usePhysicalCamera ?
                $"Focal Length: {_camera.focalLength:F2}mm" :
                $"FOV: {_camera.fieldOfView:F1}°";

            string aspectInfo = $"目标宽高比: {_targetAspect:F3}, 当前: {(float)Screen.width / Screen.height:F3}";

            GUI.Label(new Rect(10, 10, 400, 30), $"分辨率: {Screen.width}x{Screen.height}");
            GUI.Label(new Rect(10, 40, 400, 30), aspectInfo);
            GUI.Label(new Rect(10, 70, 400, 30), $"模式: {fovAdjustMode}");
            GUI.Label(new Rect(10, 100, 400, 30), mode);

            // 显示适配策略说明
            string strategy = "";
            switch (fovAdjustMode)
            {
                case FOVAdjustMode.Vertical:
                    strategy = "保持水平视野一致，调整垂直视野";
                    break;
                case FOVAdjustMode.Horizontal:
                    strategy = "保持垂直视野一致，调整水平视野";
                    break;
                case FOVAdjustMode.Balanced:
                    strategy = "保持对角线视野一致（推荐）";
                    break;
            }
            GUI.Label(new Rect(10, 130, 500, 30), $"适配策略: {strategy}");
            GUI.Label(new Rect(10, 160, 500, 30), $"参考值: 1440x3200 @ {baseFOV:F1}°/{baseFocalLength:F2}mm");
        }
    }

    // 在场景视图中显示相机视锥
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || !logDebugInfo) return;

        Camera cam = GetComponent<Camera>();
        if (cam == null) return;

        Gizmos.color = Color.cyan;
        DrawCameraFrustum(cam);
    }

    private void DrawCameraFrustum(Camera cam)
    {
        Vector3[] frustumCorners = new Vector3[4];
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

        Vector3[] worldCorners = new Vector3[4];
        Transform camTransform = cam.transform;
        for (int i = 0; i < 4; i++)
        {
            worldCorners[i] = camTransform.TransformPoint(frustumCorners[i]);
        }

        // 绘制视锥边界
        Gizmos.DrawLine(worldCorners[0], worldCorners[1]);
        Gizmos.DrawLine(worldCorners[1], worldCorners[3]);
        Gizmos.DrawLine(worldCorners[3], worldCorners[2]);
        Gizmos.DrawLine(worldCorners[2], worldCorners[0]);

        // 绘制从相机到边界的线
        Vector3 camPos = camTransform.position;
        Gizmos.DrawLine(camPos, worldCorners[0]);
        Gizmos.DrawLine(camPos, worldCorners[1]);
        Gizmos.DrawLine(camPos, worldCorners[2]);
        Gizmos.DrawLine(camPos, worldCorners[3]);

        // 绘制对角线
        Gizmos.DrawLine(worldCorners[0], worldCorners[3]);
        Gizmos.DrawLine(worldCorners[1], worldCorners[2]);
    }
#endif
}