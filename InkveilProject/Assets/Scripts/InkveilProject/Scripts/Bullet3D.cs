using UnityEngine;

/// <summary>
/// 3D子弹类，用于射击敌人
/// </summary>
public class Bullet3D : MonoBehaviour
{
    [Header("子弹属性")]
    [SerializeField] private float speed = 20f;          // 子弹移动速度
    [SerializeField] private float damage = 10f;         // 子弹伤害值
    [SerializeField] private float lifeTime = 3f;       // 子弹存在时间（秒）
    [SerializeField] private LayerMask targetLayer;     // 可以击中的目标层级
    [SerializeField] private LayerMask obstacleLayer;    // 会阻挡子弹的障碍物层级

    [Header("效果")]
    [SerializeField] private GameObject hitEffect;       // 击中目标时的特效
    [SerializeField] private AudioClip hitSound;         // 击中目标时的音效
    [SerializeField] private float hitEffectDuration = 1f; // 击中特效持续时间

    private Vector3 direction;                           // 子弹移动方向
    private Rigidbody rb;                               // 刚体组件

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // 设置为运动学刚体，避免物理引擎影响
        rb.isKinematic = true;
    }

    private void Start()
    {
        // 设置子弹自动销毁
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// 初始化子弹
    /// </summary>
    /// <param name="dir">发射方向</param>
    public void Initialize(Vector3 dir, float? customSpeed = null, float? customDamage = null)
    {
        direction = dir.normalized;

        // 可选的自定义速度和伤害
        if (customSpeed.HasValue) speed = customSpeed.Value;
        if (customDamage.HasValue) damage = customDamage.Value;

        // 根据方向旋转子弹（可选）
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }

    private void Update()
    {

        if (GameManager.instance.GameStateEnum == GameConfig.GameState.State.Play)
        {
            // 使用帧率无关的移动方式
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查是否击中了目标或障碍物
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            // 击中目标
            HandleTargetHit(other.gameObject);
        }
        else if (((1 << other.gameObject.layer) & obstacleLayer) != 0)
        {
            // 击中障碍物
            HandleObstacleHit();
        }
    }

    /// <summary>
    /// 处理击中目标
    /// </summary>
    private void HandleTargetHit(GameObject target)
    {
        // 对目标造成伤害
        EnemyBase damageable = target.GetComponent<EnemyBase>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        // 播放击中效果
        PlayHitEffects();

        // 销毁子弹
        Destroy(gameObject);
    }

    /// <summary>
    /// 处理击中障碍物
    /// </summary>
    private void HandleObstacleHit()
    {
        // 播放击中效果
        PlayHitEffects();

        // 销毁子弹
        Destroy(gameObject);
    }

    /// <summary>
    /// 播放击中效果
    /// </summary>
    private void PlayHitEffects()
    {
        // 创建击中特效（如果有）并设置自动销毁
        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, hitEffectDuration);
        }

        // 播放击中音效（如果有）
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }
    }
}