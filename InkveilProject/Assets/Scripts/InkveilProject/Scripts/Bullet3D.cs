using UnityEngine;

/// <summary>
/// 3D�ӵ��࣬�����������
/// </summary>
public class Bullet3D : MonoBehaviour
{
    [Header("�ӵ�����")]
    [SerializeField] private float speed = 20f;          // �ӵ��ƶ��ٶ�
    [SerializeField] private float damage = 10f;         // �ӵ��˺�ֵ
    [SerializeField] private float lifeTime = 3f;       // �ӵ�����ʱ�䣨�룩
    [SerializeField] private LayerMask targetLayer;     // ���Ի��е�Ŀ��㼶
    [SerializeField] private LayerMask obstacleLayer;    // ���赲�ӵ����ϰ���㼶

    [Header("Ч��")]
    [SerializeField] private GameObject hitEffect;       // ����Ŀ��ʱ����Ч
    [SerializeField] private AudioClip hitSound;         // ����Ŀ��ʱ����Ч
    [SerializeField] private float hitEffectDuration = 1f; // ������Ч����ʱ��

    private Vector3 direction;                           // �ӵ��ƶ�����
    private Rigidbody rb;                               // �������

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // ����Ϊ�˶�ѧ���壬������������Ӱ��
        rb.isKinematic = true;
    }

    private void Start()
    {
        // �����ӵ��Զ�����
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// ��ʼ���ӵ�
    /// </summary>
    /// <param name="dir">���䷽��</param>
    public void Initialize(Vector3 dir, float? customSpeed = null, float? customDamage = null)
    {
        direction = dir.normalized;

        // ��ѡ���Զ����ٶȺ��˺�
        if (customSpeed.HasValue) speed = customSpeed.Value;
        if (customDamage.HasValue) damage = customDamage.Value;

        // ���ݷ�����ת�ӵ�����ѡ��
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }

    private void Update()
    {

        if (GameManager.instance.GameStateEnum == GameConfig.GameState.State.Play)
        {
            // ʹ��֡���޹ص��ƶ���ʽ
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ����Ƿ������Ŀ����ϰ���
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            // ����Ŀ��
            HandleTargetHit(other.gameObject);
        }
        else if (((1 << other.gameObject.layer) & obstacleLayer) != 0)
        {
            // �����ϰ���
            HandleObstacleHit();
        }
    }

    /// <summary>
    /// �������Ŀ��
    /// </summary>
    private void HandleTargetHit(GameObject target)
    {
        // ��Ŀ������˺�
        EnemyBase damageable = target.GetComponent<EnemyBase>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        // ���Ż���Ч��
        PlayHitEffects();

        // �����ӵ�
        Destroy(gameObject);
    }

    /// <summary>
    /// ��������ϰ���
    /// </summary>
    private void HandleObstacleHit()
    {
        // ���Ż���Ч��
        PlayHitEffects();

        // �����ӵ�
        Destroy(gameObject);
    }

    /// <summary>
    /// ���Ż���Ч��
    /// </summary>
    private void PlayHitEffects()
    {
        // ����������Ч������У��������Զ�����
        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, hitEffectDuration);
        }

        // ���Ż�����Ч������У�
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }
    }
}