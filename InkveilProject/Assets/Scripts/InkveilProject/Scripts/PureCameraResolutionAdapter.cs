using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ResolutionFitter : MonoBehaviour
{
    [Header("�ο��ֱ�������")]
    [Tooltip("Ĭ�Ϸֱ��ʿ��")] public float referenceWidth = 1440f;
    [Tooltip("Ĭ�Ϸֱ��ʸ߶�")] public float referenceHeight = 3200f;

    [Header("�������")]
    [Tooltip("�ü�ģʽ")] public FitMode fitMode = FitMode.Fill;
    [Tooltip("����ԭʼ���ݱ���")] public bool preserveContentAspect = true;

    [Header("�߼�����")]
    [Tooltip("���Ż���ϵ��")][Range(1.0f, 1.2f)] public float scaleBuffer = 1.05f;
    [Tooltip("������ֵ")][Range(1f, 20f)] public float updateThreshold = 5f;

    private Camera _camera;
    private float _targetAspect;
    private Vector2 _lastScreenSize;
    private float _lastAspect;
    private RenderTexture _renderTexture;

    public enum FitMode
    {
        Fill,       // ���������Ļ�����ܲü���
        Fit         // ��Ӧ��Ļ����������϶��
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
        // ֻ�ڷֱ��ʱ仯ʱ���£������Ż���
        if (ShouldUpdateResolution())
        {
            UpdateCamera();
            _lastScreenSize = new Vector2(Screen.width, Screen.height);
            _lastAspect = (float)Screen.width / Screen.height;
        }
    }

    private bool ShouldUpdateResolution()
    {
        // ���ֱ��ʱ仯
        if (Mathf.Abs(_lastScreenSize.x - Screen.width) > updateThreshold ||
            Mathf.Abs(_lastScreenSize.y - Screen.height) > updateThreshold)
        {
            return true;
        }

        // ����߱ȱ仯
        float currentAspect = (float)Screen.width / Screen.height;
        return Mathf.Abs(currentAspect - _lastAspect) > 0.01f;
    }

    private void InitializeCamera()
    {
        // ������Ⱦ�������ڸ߼����䣩
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
        // �������ű���
        float scaleRatio;

        if (preserveContentAspect)
        {
            // �������ݱ������ü����ಿ��
            scaleRatio = currentAspect > _targetAspect ?
                (float)Screen.height / referenceHeight :
                (float)Screen.width / referenceWidth;
        }
        else
        {
            // �����ֱ�������ȫ���
            scaleRatio = Mathf.Max(
                (float)Screen.width / referenceWidth,
                (float)Screen.height / referenceHeight
            );
        }

        // Ӧ�����ţ�������ϵ����
        float bufferScale = scaleRatio * scaleBuffer;
        transform.localScale = new Vector3(bufferScale, bufferScale, 1f);

        // ��������ӿ�
        _camera.rect = new Rect(0, 0, 1, 1);

        // ������������ĳߴ�
        if (_camera.orthographic)
        {
            _camera.orthographicSize = referenceHeight / 2f / scaleRatio;
        }
    }

    private void ApplyFitMode(float currentAspect)
    {
        // �������ű���
        float scaleRatio;

        if (preserveContentAspect)
        {
            // �������ݱ�������Ӧ��Ļ
            scaleRatio = currentAspect < _targetAspect ?
                (float)Screen.height / referenceHeight :
                (float)Screen.width / referenceWidth;
        }
        else
        {
            // �����ֱ�������䵫���ü�
            scaleRatio = Mathf.Min(
                (float)Screen.width / referenceWidth,
                (float)Screen.height / referenceHeight
            );
        }

        // Ӧ�����ţ�������ϵ����
        float bufferScale = scaleRatio * scaleBuffer;
        transform.localScale = new Vector3(bufferScale, bufferScale, 1f);

        // ��������ӿ�
        _camera.rect = new Rect(0, 0, 1, 1);

        // ������������ĳߴ�
        if (_camera.orthographic)
        {
            _camera.orthographicSize = referenceHeight / 2f / scaleRatio;
        }
    }

    // �߼���Ⱦģʽ�л�
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

    // ������Դ
    void OnDestroy()
    {
        if (_renderTexture != null)
        {
            _renderTexture.Release();
            Destroy(_renderTexture);
        }
    }

#if UNITY_EDITOR
    [Header("�༭������")]
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
            // ģ��ֱ��ʱ仯
            Screen.SetResolution((int)testResolution.x, (int)testResolution.y, false);
            UpdateCamera();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // �ڳ�����ͼ����ʾ����ӿڱ߽�
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