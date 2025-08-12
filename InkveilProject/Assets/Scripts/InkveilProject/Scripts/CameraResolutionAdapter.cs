using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraResolutionAdapter : MonoBehaviour
{
    [Header("�ο��ֱ�������")]
    [Tooltip("Ĭ�Ϸֱ��ʿ��")] public float referenceWidth = 1080f;
    [Tooltip("Ĭ�Ϸֱ��ʸ߶�")] public float referenceHeight = 1920f;

    [Header("���Ĭ��ֵ (1440x3200)")]
    [Tooltip("������Ұֵ����ֱFOV��")]
    [Range(1f, 179f)] public float baseFOV = 20f;
    [Tooltip("��������ֵ����������²ο��ֱ��ʵĽ��ࣩ")]
    [SerializeField] private float baseFocalLength = 60f;

    [Header("��������")]
    [Tooltip("��Ұ����ģʽ")] public FOVAdjustMode fovAdjustMode = FOVAdjustMode.Balanced;
    [Tooltip("���ֱFOV")][Range(1f, 179f)] public float maxFOV = 25f;
    [Tooltip("��С��ֱFOV")][Range(1f, 179f)] public float minFOV = 18f;
    [Tooltip("��߱��ݲ�ֵ")][Range(0.01f, 0.5f)] public float aspectTolerance = 0.05f;
    [Tooltip("ʹ�������������")] public bool usePhysicalCamera = true;

    public enum FOVAdjustMode
    {
        Vertical,   // ����ˮƽFOVһ�£�������ֱFOV
        Horizontal, // ���ִ�ֱFOVһ�£���ֱFOV��ΪbaseFOV��
        Balanced    // ���ֶԽ���FOVһ�£��Ƽ���
    }

    private Camera _camera;
    private float _targetAspect;                 // �ο��ֱ��ʿ�߱�
    private Vector2 _lastScreenSize;
    private ScreenOrientation _lastOrientation;

    void Start()
    {
        _camera = GetComponent<Camera>();
        _targetAspect = referenceWidth / referenceHeight;

        // �Ȱ��ο��ֱ�����һ��Ĭ�Ϲ�ͼ
        ApplyCameraSettingsForResolution(referenceWidth, referenceHeight);

        // �ٰ���ǰ�豸�ֱ���������
        _lastScreenSize = new Vector2(Screen.width, Screen.height);
        _lastOrientation = Screen.orientation;
        UpdateCameraSettings();

        Debug.Log($"��ʼ�����: ����={baseFocalLength:F2}mm, ��׼��ֱFOV={baseFOV:F1}��, �ο���߱�={_targetAspect:F3}");
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
        float currentAspect = width / height;          // ��ǰ��߱�
        float rel = currentAspect / _targetAspect;     // ��Բ���

        // ���ݲΧ�ڵ���ͬһ��߱ȣ�ֱ��ʹ��Ĭ������
        if (Mathf.Abs(1f - rel) < aspectTolerance)
        {
            ApplyDefaultSettings();
            return;
        }

        AdjustCameraForAspect(currentAspect);
    }

    // ���� ֻ����ǰ��߱ȣ��ڲ�����ģʽ����Ŀ��FOV�����н�����Ӧ�����ӡ ����
    private void AdjustCameraForAspect(float currentAspect)
    {
        float targetFOV;

        switch (fovAdjustMode)
        {
            case FOVAdjustMode.Vertical:
                targetFOV = AdjustVerticalFOV(currentAspect); // ����ˮƽFOV
                break;

            case FOVAdjustMode.Horizontal:
                targetFOV = AdjustHorizontalFOV();            // ���ִ�ֱFOV
                break;

            case FOVAdjustMode.Balanced:
            default:
                targetFOV = AdjustBalancedFOV(currentAspect); // ���ֶԽ���FOV
                break;
        }

        float clamped = Mathf.Clamp(targetFOV, minFOV, maxFOV);
        ApplyFOV(clamped);

        Debug.Log($"�ֱ���: {Screen.width}x{Screen.height}, ��߱�: {currentAspect:F3}, " +
                  $"FOV(��ֱ): {clamped:F2}��, ģʽ: {fovAdjustMode}");
    }

    // 1) Vertical�����֡�ˮƽFOV����ο�һ�£������µĴ�ֱFOV
    private float AdjustVerticalFOV(float currentAspect)
    {
        float vfRef = baseFOV * Mathf.Deg2Rad;
        float hfRef = 2f * Mathf.Atan(Mathf.Tan(vfRef / 2f) * _targetAspect);   // �ο�ˮƽFOV
        float vfNew = 2f * Mathf.Atan(Mathf.Tan(hfRef / 2f) / currentAspect);   // �´�ֱFOV
        return vfNew * Mathf.Rad2Deg;
    }

    // 2) Horizontal�����֡���ֱFOV����ο�һ��
    private float AdjustHorizontalFOV()
    {
        return baseFOV;
    }

    // 3) Balanced�����֡��Խ���FOV����ο�һ�£������µĴ�ֱFOV
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
            // ���������f = sensorSizeY / (2 * tan(FOV/2))
            float sensorSizeY = _camera.sensorSize.y;
            float targetFocalLength = sensorSizeY / (2.0f * Mathf.Tan(fov * Mathf.Deg2Rad / 2.0f));
            _camera.usePhysicalProperties = true;
            _camera.focalLength = targetFocalLength;
        }
        else
        {
            _camera.usePhysicalProperties = false;
            _camera.fieldOfView = fov; // ��ֱFOV
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
    [Header("�༭������")]
    public Vector2 testResolution = new Vector2(1080, 1920);
    public bool applyTestResolution;
    public bool logDebugInfo;

    private void OnValidate()
    {
        // ���Ʒ�Χ
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
                : $"FOV: {_camera.fieldOfView:F1}��";

            string aspectInfo = $"Ŀ���߱�: {_targetAspect:F3}, ��ǰ: {(float)Screen.width / Screen.height:F3}";

            GUI.Label(new Rect(10, 10, 400, 30), $"�ֱ���: {Screen.width}x{Screen.height}");
            GUI.Label(new Rect(10, 40, 500, 30), aspectInfo);
            GUI.Label(new Rect(10, 70, 400, 30), $"ģʽ: {fovAdjustMode}");
            GUI.Label(new Rect(10, 100, 400, 30), mode);

            string strategy = "";
            switch (fovAdjustMode)
            {
                case FOVAdjustMode.Vertical: strategy = "����ˮƽ��Ұһ�£�������ֱ��Ұ"; break;
                case FOVAdjustMode.Horizontal: strategy = "���ִ�ֱ��Ұһ�£���ֱFOV��Ϊ��׼��"; break;
                case FOVAdjustMode.Balanced: strategy = "���ֶԽ�����Ұһ�£��Ƽ���"; break;
            }
            GUI.Label(new Rect(10, 130, 700, 30), $"�������: {strategy}");
            GUI.Label(new Rect(10, 160, 700, 30), $"�ο�: 1440x3200 @ {baseFOV:F1}�� / {baseFocalLength:F2}mm");
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
