using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraResolutionAdapter : MonoBehaviour
{
    [Header("参考分辨率设置")]
    [Tooltip("默认分辨率宽度")] public float referenceWidth = 1080f;
    [Tooltip("默认分辨率高度")] public float referenceHeight = 1920f;

    [Header("相机默认值 (1440x3200)")]
    [Tooltip("基础视野值（垂直FOV）")]
    [Range(1f, 179f)] public float baseFOV = 20f;
    [Tooltip("基础焦距值（物理相机下参考分辨率的焦距）")]
    [SerializeField] private float baseFocalLength = 60f;

    [Header("适配设置")]
    [Tooltip("视野调整模式")] public FOVAdjustMode fovAdjustMode = FOVAdjustMode.Balanced;
    [Tooltip("最大垂直FOV")][Range(1f, 179f)] public float maxFOV = 25f;
    [Tooltip("最小垂直FOV")][Range(1f, 179f)] public float minFOV = 18f;
    [Tooltip("宽高比容差值")][Range(0.01f, 0.5f)] public float aspectTolerance = 0.05f;
    [Tooltip("使用物理相机参数")] public bool usePhysicalCamera = true;

    public enum FOVAdjustMode
    {
        Vertical,   // 保持水平FOV一致，调整垂直FOV
        Horizontal, // 保持垂直FOV一致（垂直FOV恒为baseFOV）
        Balanced    // 保持对角线FOV一致（推荐）
    }

    private Camera _camera;
    private float _targetAspect;                 // 参考分辨率宽高比
    private Vector2 _lastScreenSize;
    private ScreenOrientation _lastOrientation;

    void Start()
    {
        _camera = GetComponent<Camera>();
        _targetAspect = referenceWidth / referenceHeight;

        // 先按参考分辨率套一次默认构图
        ApplyCameraSettingsForResolution(referenceWidth, referenceHeight);

        // 再按当前设备分辨率自适配
        _lastScreenSize = new Vector2(Screen.width, Screen.height);
        _lastOrientation = Screen.orientation;
        UpdateCameraSettings();

        Debug.Log($"初始化相机: 焦距={baseFocalLength:F2}mm, 基准垂直FOV={baseFOV:F1}°, 参考宽高比={_targetAspect:F3}");
    }

    void Update()
    {
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
        float currentAspect = width / height;          // 当前宽高比
        float rel = currentAspect / _targetAspect;     // 相对差异

        // 在容差范围内当作同一宽高比，直接使用默认设置
        if (Mathf.Abs(1f - rel) < aspectTolerance)
        {
            ApplyDefaultSettings();
            return;
        }

        AdjustCameraForAspect(currentAspect);
    }

    // —— 只传当前宽高比，内部根据模式计算目标FOV，并夹紧后再应用与打印 ——
    private void AdjustCameraForAspect(float currentAspect)
    {
        float targetFOV;

        switch (fovAdjustMode)
        {
            case FOVAdjustMode.Vertical:
                targetFOV = AdjustVerticalFOV(currentAspect); // 保持水平FOV
                break;

            case FOVAdjustMode.Horizontal:
                targetFOV = AdjustHorizontalFOV();            // 保持垂直FOV
                break;

            case FOVAdjustMode.Balanced:
            default:
                targetFOV = AdjustBalancedFOV(currentAspect); // 保持对角线FOV
                break;
        }

        float clamped = Mathf.Clamp(targetFOV, minFOV, maxFOV);
        ApplyFOV(clamped);

        Debug.Log($"分辨率: {Screen.width}x{Screen.height}, 宽高比: {currentAspect:F3}, " +
                  $"FOV(垂直): {clamped:F2}°, 模式: {fovAdjustMode}");
    }

    // 1) Vertical：保持“水平FOV”与参考一致，反推新的垂直FOV
    private float AdjustVerticalFOV(float currentAspect)
    {
        float vfRef = baseFOV * Mathf.Deg2Rad;
        float hfRef = 2f * Mathf.Atan(Mathf.Tan(vfRef / 2f) * _targetAspect);   // 参考水平FOV
        float vfNew = 2f * Mathf.Atan(Mathf.Tan(hfRef / 2f) / currentAspect);   // 新垂直FOV
        return vfNew * Mathf.Rad2Deg;
    }

    // 2) Horizontal：保持“垂直FOV”与参考一致
    private float AdjustHorizontalFOV()
    {
        return baseFOV;
    }

    // 3) Balanced：保持“对角线FOV”与参考一致，反推新的垂直FOV
    private float AdjustBalancedFOV(float currentAspect)
    {
        float vfRef = baseFOV * Mathf.Deg2Rad;
        float hfRef = 2f * Mathf.Atan(Mathf.Tan(vfRef / 2f) * _targetAspect);

        // tan(diag/2)^2 = tan(v/2)^2 + tan(h/2)^2
        float tV = Mathf.Tan(vfRef / 2f);
        float tH = Mathf.Tan(hfRef / 2f);
        float diagRef = 2f * Mathf.Atan(Mathf.Sqrt(tV * tV + tH * tH));

        // tan(vNew/2) = tan(diagRef/2) / sqrt(1 + aspect^2)
        float vNew = 2f * Mathf.Atan(Mathf.Tan(diagRef / 2f) / Mathf.Sqrt(1f + currentAspect * currentAspect));
        return vNew * Mathf.Rad2Deg;
    }

    private void ApplyFOV(float fov)
    {
        if (usePhysicalCamera)
        {
            // 物理相机：f = sensorSizeY / (2 * tan(FOV/2))
            float sensorSizeY = _camera.sensorSize.y;
            float targetFocalLength = sensorSizeY / (2.0f * Mathf.Tan(fov * Mathf.Deg2Rad / 2.0f));
            _camera.usePhysicalProperties = true;
            _camera.focalLength = targetFocalLength;
        }
        else
        {
            _camera.usePhysicalProperties = false;
            _camera.fieldOfView = fov; // 垂直FOV
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

#if UNITY_EDITOR
    [Header("编辑器调试")]
    public Vector2 testResolution = new Vector2(1080, 1920);
    public bool applyTestResolution;
    public bool logDebugInfo;

    private void OnValidate()
    {
        // 限制范围
        baseFOV = Mathf.Clamp(baseFOV, 1f, 179f);
        baseFocalLength = Mathf.Clamp(baseFocalLength, 10f, 300f);
        minFOV = Mathf.Clamp(minFOV, 1f, baseFOV);
        maxFOV = Mathf.Clamp(maxFOV, baseFOV, 179f);

        if (applyTestResolution && Application.isPlaying)
        {
            Screen.SetResolution((int)testResolution.x, (int)testResolution.y, false);
            ApplyCameraSettingsForResolution(testResolution.x, testResolution.y);
        }
    }

    void OnGUI()
    {
        if (logDebugInfo && Application.isPlaying)
        {
            string mode = usePhysicalCamera
                ? $"Focal Length: {_camera.focalLength:F2}mm"
                : $"FOV: {_camera.fieldOfView:F1}°";

            string aspectInfo = $"目标宽高比: {_targetAspect:F3}, 当前: {(float)Screen.width / Screen.height:F3}";

            GUI.Label(new Rect(10, 10, 400, 30), $"分辨率: {Screen.width}x{Screen.height}");
            GUI.Label(new Rect(10, 40, 500, 30), aspectInfo);
            GUI.Label(new Rect(10, 70, 400, 30), $"模式: {fovAdjustMode}");
            GUI.Label(new Rect(10, 100, 400, 30), mode);

            string strategy = "";
            switch (fovAdjustMode)
            {
                case FOVAdjustMode.Vertical: strategy = "保持水平视野一致，调整垂直视野"; break;
                case FOVAdjustMode.Horizontal: strategy = "保持垂直视野一致（垂直FOV恒为基准）"; break;
                case FOVAdjustMode.Balanced: strategy = "保持对角线视野一致（推荐）"; break;
            }
            GUI.Label(new Rect(10, 130, 700, 30), $"适配策略: {strategy}");
            GUI.Label(new Rect(10, 160, 700, 30), $"参考: 1440x3200 @ {baseFOV:F1}° / {baseFocalLength:F2}mm");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || !logDebugInfo) return;

        var cam = GetComponent<Camera>();
        if (cam == null) return;

        Gizmos.color = Color.cyan;
        DrawCameraFrustum(cam);
    }

    private void DrawCameraFrustum(Camera cam)
    {
        if (cam.orthographic) return;

        Vector3[] frustumCorners = new Vector3[4];
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

        Vector3[] worldCorners = new Vector3[4];
        Transform camTransform = cam.transform;
        for (int i = 0; i < 4; i++)
            worldCorners[i] = camTransform.TransformPoint(frustumCorners[i]);

        Gizmos.DrawLine(worldCorners[0], worldCorners[1]);
        Gizmos.DrawLine(worldCorners[1], worldCorners[3]);
        Gizmos.DrawLine(worldCorners[3], worldCorners[2]);
        Gizmos.DrawLine(worldCorners[2], worldCorners[0]);

        Vector3 camPos = camTransform.position;
        Gizmos.DrawLine(camPos, worldCorners[0]);
        Gizmos.DrawLine(camPos, worldCorners[1]);
        Gizmos.DrawLine(camPos, worldCorners[2]);
        Gizmos.DrawLine(camPos, worldCorners[3]);

        Gizmos.DrawLine(worldCorners[0], worldCorners[3]);
        Gizmos.DrawLine(worldCorners[1], worldCorners[2]);
    }
#endif
}
