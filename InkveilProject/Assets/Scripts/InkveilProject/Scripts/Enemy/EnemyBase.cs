using DG.Tweening; // 添加DOTween命名空间
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// 敌人基类：包含基础属性、攻击、受击、死亡、技能等核心逻辑
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class EnemyBase : MonoBehaviour
{
    // 基础属性
    private EnemyInfo m_Config;              // 敌人配置数据
    private float m_CurrentHP;               // 当前血量
    private float m_MaxHP;                   // 最大血量
    private float m_MoveSpeed;               // 移动速度
    private int m_AttackDamage;              // 攻击伤害
    private float m_AttackRange = 1f;        // 攻击范围
    private float m_LastAttackTime;          // 上次攻击时间
    public EnemyUI m_EnemyUI;                // 血条和伤害显示UI

    public Action OnDestroyed;               // 被销毁事件
    public Action OnSpawned;                 // 出生事件

    private enum EnemyState { Nono, Idle, Run, Attacking, Dead }
    private EnemyState m_CurrentState = EnemyState.Nono;

    // 组件引用
    private Rigidbody m_Rigidbody;
    private LegacyAnimationController m_AnimationController;
    private SkinnedMeshRenderer m_SpriteRenderer;

    // 技能与召唤
    private Coroutine m_SkillCoroutine;
    private List<GameObject> m_SummonedUnits = new List<GameObject>();

    // 短暂免疫检测
    private float m_LastHitTime;
    private bool m_IsImmune => Time.time - m_LastHitTime < 0.3f;

    private Transform m_PlayerTarget;

    // 特效引用
    [SerializeField] private ParticleSystem m_HitEffect;
    [SerializeField] private ParticleSystem m_DeathEffect;
    [SerializeField] private ParticleSystem m_AttackEffect;
    [SerializeField] private GameObject m_PortalEffectPrefab; // 传送门特效
    private GameObject m_PortalInstance; // 传送门实例

    // 音效与击退
    private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_HitSound;
    [SerializeField] private AudioClip m_AttackSound; // 攻击音效
    [SerializeField] private AudioClip m_DeathSound;  // 死亡音效
    [SerializeField] private float m_KnockbackDistance = 0.5f;
    [SerializeField] private float m_KnockbackDuration = 0.1f;

    // 受击变红
    [SerializeField] private Color m_HitColor = Color.red;
    [SerializeField] private float m_HitFlashDuration = 0.2f;
    private Coroutine m_HitFlashCoroutine;

    [Header("Debug")]
    [SerializeField] private bool m_ShowDebugInfo = false;

    #region 生命周期

    internal void OnInit(EnemyInfo enemy)
    {
        if (enemy == null)
        {
            Debug.LogError("Enemy config is null!");
            EnemyManager.instance.RemoveEnemy(m_Config.enemyName, this);
            return;
        }

        m_Config = enemy;

      

        m_PlayerTarget = PlayerController.instance.transform;

        // 获取组件
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AnimationController = GetComponent<LegacyAnimationController>();
        m_SpriteRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        m_AudioSource = GetComponent<AudioSource>();
        if (m_AudioSource == null)
        {
            Debug.LogWarning("EnemyBase 缺少 AudioSource，自动添加。");
            m_AudioSource = gameObject.AddComponent<AudioSource>();
        }

        // 初始化属性
        m_MaxHP = m_Config.hpBase;
        m_CurrentHP = m_Config.hpBase;
        m_MoveSpeed = m_Config.moveSpeed;
        m_AttackDamage = m_Config.attackDamage;

        // 初始化材质（URP兼容）
        if (m_SpriteRenderer != null && m_SpriteRenderer.material.HasProperty("_BaseColor"))
        {
            m_SpriteRenderer.material.SetColor("_BaseColor", Color.white);
        }

        m_EnemyUI.OnReset();

        InitAbilities();
        OnSpawned?.Invoke();
        StartCoroutine(SpawnRoutine());

    }

    public void OnUpdate()
    {
        if (GameManager.instance.GameStateEnum != GameConfig.GameState.State.Play) return;
        if (m_CurrentState == EnemyState.Dead) return;

        Transform blood = m_EnemyUI.transform.parent;
        blood.rotation = Quaternion.identity;

        if (m_CurrentState == EnemyState.Run && m_PlayerTarget != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, m_PlayerTarget.position);
            if (distanceToPlayer <= m_AttackRange && Time.time - m_LastAttackTime >= 1.5f)
            {
                StartCoroutine(AttackRoutine());
            }
        }

        if (m_CurrentState == EnemyState.Run)
        {
            UpdateMovement();
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        ClearSummonedUnits();
    }

    #endregion

    #region 核心行为

    private IEnumerator SpawnRoutine()
    {
        m_CurrentState = EnemyState.Idle;
        yield return new WaitForFixedUpdate();
        if (m_Config.enemyType > 2)
        {
            Vector3 vector3 = transform.position;
            vector3.x = 0;
            transform.position = vector3;
        }
        // 创建传送门特效
        if (m_PortalEffectPrefab != null)
        {
            m_PortalInstance = Instantiate(m_PortalEffectPrefab, transform.position, Quaternion.identity);
            m_PortalInstance.transform.position = transform.position + Vector3.up * 1;
            m_PortalInstance.transform.localScale = Vector3.one * 1f;

            // 传送门动画
            DOTween.Sequence()
                .Append(m_PortalInstance.transform.DOScale(Vector3.one * 1f, 0.5f).SetEase(Ease.OutBack))
                .AppendInterval(0.5f);
        }

        // 隐藏敌人本体
        SetEnemyVisible(false);
        yield return new WaitForSeconds(1f);

        // 显示敌人本体
        SetEnemyVisible(true);
        m_AnimationController.PlayAnimation("spawn");

        // 位置调整（从传送门中心出现）
        //transform.position = m_PortalInstance.transform.position;
        //transform.DOJump(transform.position + Vector3.forward, 0.5f, 1, 0.5f);

        yield return new WaitForSeconds(0.5f);

        // 销毁传送门
        if (m_PortalInstance != null)
        {
            DOTween.Sequence()
                .Append(m_PortalInstance.transform.DOScale(Vector3.one * 0.1f, 0.5f).SetEase(Ease.InBack)
                .OnComplete(() => Destroy(m_PortalInstance)));
        }

        m_CurrentState = EnemyState.Run;
        m_AnimationController.PlayAnimation("run");
    }

    private void SetEnemyVisible(bool visible)
    {
        if (m_SpriteRenderer != null)
        {
            m_SpriteRenderer.enabled = visible;
        }
    }

    private void UpdateMovement()
    {
        if (!m_PlayerTarget && m_CurrentState != EnemyState.Run) return;

        Vector3 direction = m_PlayerTarget.position - transform.position;
        direction.y = 0;
        transform.LookAt(m_PlayerTarget.position);
        transform.Translate(Vector3.forward * m_MoveSpeed * Time.deltaTime);
    }

    private IEnumerator AttackRoutine()
    {
        m_CurrentState = EnemyState.Attacking;
        m_Rigidbody.velocity = Vector3.zero;
        m_LastAttackTime = Time.time;

        m_AnimationController.PlayAnimationImmediate("attack");
        yield return new WaitForSeconds(0.75f);

        PerformAttack();
        yield return new WaitForSeconds(0.75f);

        if (m_CurrentHP > 0)
        {
            m_CurrentState = EnemyState.Run;
            m_AnimationController.PlayAnimationImmediate("run");
        }
    }

    private void PerformAttack()
    {
        if (m_PlayerTarget == null) return;

        if (Vector3.Distance(transform.position, m_PlayerTarget.position) <= m_AttackRange * 1.2f)
        {
            PlayerController.instance.TakeDamage(m_AttackDamage);

            // 攻击特效
            if (m_AttackEffect != null)
            {
                m_AttackEffect.transform.position = m_PlayerTarget.position + Vector3.up * 0.5f;
                m_AttackEffect.Play();
            }

            // 攻击音效
            if (m_AttackSound != null)
            {
                AudioSource.PlayClipAtPoint(m_AttackSound, transform.position, 0.5f);
            }
        }
    }

    #endregion

    #region 战斗系统

    public async Task TakeDamage(float damage, bool isDivineAttack = false)
    {
        if (m_IsImmune || m_CurrentState == EnemyState.Dead) return;

        float actualDamage = damage;//CalculateActualDamage(damage, isDivineAttack);

        if (damage> m_CurrentHP)
        {
            m_CurrentHP -= m_CurrentHP;
        }
        m_CurrentHP -= actualDamage;

        // 播放音效
        if (m_HitSound != null)
        {
            m_AudioSource.PlayOneShot(m_HitSound);
        }

        // 受伤特效
        if (m_HitEffect != null)
        {
           
            m_HitEffect.transform.position = transform.position + Vector3.up * 1f;
            m_HitEffect.Play();
        }

        // 播放击退
        if (m_PlayerTarget != null && m_Config.enemyType == 0)
        {
            Vector3 knockbackDir = (transform.position - m_PlayerTarget.position).normalized;
            knockbackDir.y = 0;
            StartCoroutine(KnockbackRoutine(knockbackDir, m_KnockbackDistance, m_KnockbackDuration));
        }

        // 播放变红
        if (m_SpriteRenderer != null)
        {
            if (m_HitFlashCoroutine != null) StopCoroutine(m_HitFlashCoroutine);
            m_HitFlashCoroutine = StartCoroutine(HitFlashRoutine());
        }

        // 更新UI
        EnemyUI enemyUI = await EnemyUIManager.instance.GetEnemyUIByName("EnemyUI");
        enemyUI.transform.position = m_EnemyUI.transform.position;
        enemyUI.OnDemage(actualDamage, m_CurrentHP, m_MaxHP, false, CriticalLevel.Normal);

        m_LastHitTime = Time.time;

        if (m_CurrentHP <= 0)
        {
            StartCoroutine(DieRoutine());
        }
    }

    private IEnumerator KnockbackRoutine(Vector3 direction, float distance, float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + direction * distance;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
    }

    private IEnumerator HitFlashRoutine()
    {
        Material mat = m_SpriteRenderer.material;

        // 保存原始颜色
        Color originalColor = mat.HasProperty("_BaseColor") ?
            mat.GetColor("_BaseColor") : Color.white;

        // 使用DOTween实现颜色变化
        Sequence flashSequence = DOTween.Sequence();
        flashSequence.Append(mat.DOColor(m_HitColor, "_BaseColor", m_HitFlashDuration / 2))
                    .Append(mat.DOColor(originalColor, "_BaseColor", m_HitFlashDuration / 2));

        yield return flashSequence.WaitForCompletion();

        // 确保恢复原始颜色
        mat.SetColor("_BaseColor", originalColor);
    }

    private float CalculateActualDamage(float damage, bool isDivineAttack)
    {
        double immunityChance = isDivineAttack ? m_Config.immunityProbability * 0.6f : m_Config.immunityProbability;
        if (Random.value < immunityChance)
        {
            ShowImmuneEffect();
            return 0;
        }
        return damage;
    }

    private IEnumerator DieRoutine()
    {
        m_CurrentState = EnemyState.Dead;
        m_Rigidbody.velocity = Vector3.zero;

        // 死亡特效
        if (m_DeathEffect != null)
        {
            ParticleSystem deathEffect = Instantiate(m_DeathEffect,
                transform.position + Vector3.up,
                Quaternion.identity);
            deathEffect.Play();
            Destroy(deathEffect.gameObject, deathEffect.main.duration);
        }

        // 死亡音效
        if (m_DeathSound != null)
        {
            AudioSource.PlayClipAtPoint(m_DeathSound, transform.position, 0.8f);
        }

        // 隐藏敌人
        SetEnemyVisible(false);

        yield return new WaitForSeconds(1f);

        int dropAmount = CalculateDropAmount();
        EnemyManager.instance.RemoveEnemy(m_Config.enemyName, this);
        OnDestroyed?.Invoke();
    }

    #endregion

    #region 技能系统

    private void InitAbilities()
    {
        if (((EnemyAbility)m_Config.abilities & EnemyAbility.Summon) != 0)
        {
            m_SkillCoroutine = StartCoroutine(SummonRoutine());
        }

        if (((EnemyAbility)m_Config.abilities & EnemyAbility.AuraEnhance) != 0)
        {
            StartCoroutine(AuraEnhanceRoutine());
        }
    }

    private IEnumerator SummonRoutine()
    {
        yield return new WaitForSeconds(Random.Range(8f, 12f));
    }

    private IEnumerator AuraEnhanceRoutine()
    {
        float checkInterval = 1f;
        Collider2D[] buffer = new Collider2D[20];

        while (m_CurrentState != EnemyState.Dead)
        {
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, 5f, buffer);
            for (int i = 0; i < count; i++)
            {
                if (buffer[i].TryGetComponent<EnemyBase>(out var enemy))
                {
                    enemy.ApplyAuraBoost();
                }
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void ApplyAuraBoost()
    {
        if (m_Config.enemyType != 0) return;
        m_MoveSpeed *= 1.1f;
        m_AttackDamage = (int)(m_AttackDamage * 1.1f);
    }

    private void ClearSummonedUnits()
    {
        foreach (var unit in m_SummonedUnits)
        {
            if (unit != null) Destroy(unit);
        }
        m_SummonedUnits.Clear();
    }

    #endregion

    #region 其他系统

    private int CalculateDropAmount()
    {
        return (EnemyType)m_Config.enemyType switch
        {
            EnemyType.Minion => UnityEngine.Random.Range(1, 3),
            EnemyType.Elite => UnityEngine.Random.Range(5, 8),
            EnemyType.General => Random.Range(15, 20),
            _ => 0
        };
    }

    private void ShowImmuneEffect()
    {
        m_AnimationController.PlayAnimation("immune");
    }

    #endregion
}