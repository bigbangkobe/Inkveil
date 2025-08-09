using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraResolutionAdapter : MonoBehaviour
{
    [Header("�ο��ֱ�������")]
    [Tooltip("Ĭ�Ϸֱ��ʿ��")] public float referenceWidth = 1440f;
    [Tooltip("Ĭ�Ϸֱ��ʸ߶�")] public float referenceHeight = 3200f;

    [Header("���Ĭ��ֵ (1440x3200)")]
    [Tooltip("������Ұֵ")][Range(1f, 179f)] public float baseFOV = 20.8f;
    [Tooltip("��������ֵ")] private float baseFocalLength = 65.38286f;

    [Header("��������")]
    [Tooltip("��Ұ����ģʽ")] public FOVAdjustMode fovAdjustMode = FOVAdjustMode.Balanced;
    [Tooltip("�����Ұ")][Range(1f, 179f)] public float maxFOV = 25f;
    [Tooltip("��С��Ұ")][Range(1f, 179f)] public float minFOV = 18f;
    [Tooltip("��߱��ݲ�ֵ")][Range(0.01f, 0.5f)] public float aspectTolerance = 0.05f;
    [Tooltip("ʹ�������������")] public bool usePhysicalCamera = true;

    public enum FOVAdjustMode
    {
        Vertical,   // ������ֱFOV
        Horizontal, // ����ˮƽFOV
        Balanced    // ƽ��������Ƽ���
    }

    private Camera _camera;
    private float _targetAspect;
    private Vector2 _lastScreenSize;
    private ScreenOrientation _lastOrientation;

    void Start()
    {
        _camera = GetComponent<Camera>();
        _targetAspect = referenceWidth / referenceHeight;

        // Ӧ��Ĭ��ֵ��1440x3200�ֱ������ã�
        ApplyCameraSettingsForResolution(referenceWidth, referenceHeight);

        // ���浱ǰ��Ļ״̬
        _lastScreenSize = new Vector2(Screen.width, Screen.height);
        _lastOrientation = Screen.orientation;

        Debug.Log($"��ʼ�����: ����={baseFocalLength:F2}mm, FOV={baseFOV:F1}��");
    }

    void Update()
    {
        // ��Ļ�ߴ����仯ʱ��������
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
        // ���㵱ǰ��߱�
        float currentAspect = width / height;

        // �����߱Ȳ���
        float aspectDifference = currentAspect / _targetAspect;

        // ���ݲΧ����Ϊ��ͬ��߱ȣ�ʹ��Ĭ������
        if (Mathf.Abs(1 - aspectDifference) < aspectTolerance)
        {
            ApplyDefaultSettings();
            return;
        }

        // ���ݿ�߱Ȳ�������������
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

        // ������Ϣ
        Debug.Log($"�ֱ���: {Screen.width}x{Screen.height}, " +
                 $"��߱�: {currentAspect:F3}, " +
                 $"FOV: {targetFOV:F1}��, " +
                 $"ģʽ: {fovAdjustMode}");
    }

    private float AdjustVerticalFOV(float aspectDifference)
    {
        // ��ֱFOV����������ˮƽ��Ұһ��
        return baseFOV * aspectDifference;
    }

    private float AdjustHorizontalFOV(float aspectDifference)
    {
        // ˮƽFOV���������ִ�ֱ��Ұһ��
        return baseFOV / aspectDifference;
    }

    private float AdjustBalancedFOV(float aspectDifference, float currentAspect)
    {
        // ƽ�ⷽ�������ֶԽ���FOVһ�£�����Ȼ��͸�ӣ�
        float diagonalFOV = CalculateDiagonalFOV(baseFOV, _targetAspect);

        return 2.0f * Mathf.Atan(
            Mathf.Tan(diagonalFOV * Mathf.Deg2Rad / 2.0f) *
            Mathf.Sqrt(1.0f / (1.0f + currentAspect * currentAspect))
        ) * Mathf.Rad2Deg;
    }

    private float CalculateDiagonalFOV(float fov, float aspect)
    {
        // ����Խ�����Ұ
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
            // ʹ������������������ࣩ
            // FOV = 2 * arctan(sensorSize / (2 * focalLength))
            // ��ˣ�focalLength = sensorSize / (2 * tan(FOV/2))
            float sensorSize = _camera.sensorSize.y; // ʹ�ô�ֱ�������ߴ�
            float targetFocalLength = sensorSize / (2.0f * Mathf.Tan(fov * Mathf.Deg2Rad / 2.0f));

            _camera.usePhysicalProperties = true;
            _camera.focalLength = targetFocalLength;
        }
        else
        {
            // ʹ�ñ�׼FOV
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

    // �༭�����Է���
#if UNITY_EDITOR
    [Header("�༭������")]
    public Vector2 testResolution = new Vector2(1440, 3200);
    public bool applyTestResolution;
    public bool logDebugInfo;

    private void OnValidate()
    {
        // ȷ������ֵ�ں���Χ��
        baseFOV = Mathf.Clamp(baseFOV, 1f, 179f);
        baseFocalLength = Mathf.Clamp(baseFocalLength, 10f, 300f);
        minFOV = Mathf.Clamp(minFOV, 1f, baseFOV);
        maxFOV = Mathf.Clamp(maxFOV, baseFOV, 179f);

        if (applyTestResolution && Application.isPlaying)
        {
            // ģ��ֱ��ʱ仯
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
                $"FOV: {_camera.fieldOfView:F1}��";

            string aspectInfo = $"Ŀ���߱�: {_targetAspect:F3}, ��ǰ: {(float)Screen.width / Screen.height:F3}";

            GUI.Label(new Rect(10, 10, 400, 30), $"�ֱ���: {Screen.width}x{Screen.height}");
            GUI.Label(new Rect(10, 40, 400, 30), aspectInfo);
            GUI.Label(new Rect(10, 70, 400, 30), $"ģʽ: {fovAdjustMode}");
            GUI.Label(new Rect(10, 100, 400, 30), mode);

            // ��ʾ�������˵��
            string strategy = "";
            switch (fovAdjustMode)
            {
                case FOVAdjustMode.Vertical:
                    strategy = "����ˮƽ��Ұһ�£�������ֱ��Ұ";
                    break;
                case FOVAdjustMode.Horizontal:
                    strategy = "���ִ�ֱ��Ұһ�£�����ˮƽ��Ұ";
                    break;
                case FOVAdjustMode.Balanced:
                    strategy = "���ֶԽ�����Ұһ�£��Ƽ���";
                    break;
            }
            GUI.Label(new Rect(10, 130, 500, 30), $"�������: {strategy}");
            GUI.Label(new Rect(10, 160, 500, 30), $"�ο�ֵ: 1440x3200 @ {baseFOV:F1}��/{baseFocalLength:F2}mm");
        }
    }

    // �ڳ�����ͼ����ʾ�����׶
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

        // ������׶�߽�
        Gizmos.DrawLine(worldCorners[0], worldCorners[1]);
        Gizmos.DrawLine(worldCorners[1], worldCorners[3]);
        Gizmos.DrawLine(worldCorners[3], worldCorners[2]);
        Gizmos.DrawLine(worldCorners[2], worldCorners[0]);

        // ���ƴ�������߽����
        Vector3 camPos = camTransform.position;
        Gizmos.DrawLine(camPos, worldCorners[0]);
        Gizmos.DrawLine(camPos, worldCorners[1]);
        Gizmos.DrawLine(camPos, worldCorners[2]);
        Gizmos.DrawLine(camPos, worldCorners[3]);

        // ���ƶԽ���
        Gizmos.DrawLine(worldCorners[0], worldCorners[3]);
        Gizmos.DrawLine(worldCorners[1], worldCorners[2]);
    }
#endif
}