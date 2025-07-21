using UnityEngine;
using System.Collections;
using System;
using Framework;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class WeaponBase : MonoBehaviour
{
    #region 组件引用
    private Animator m_Animator;
    private AudioSource m_Audio;
    private PlayerController m_Player;
    private SpriteRenderer m_SpriteRenderer;
    #endregion

    #region 状态系统
    public enum WeaponState { Idle, Attacking, Reloading, Special }
    private WeaponState m_CurrentState = WeaponState.Idle;
    private float m_AttackCooldown;
    #endregion

    #region 核心数据
    private WeaponInfo m_WeaponInfo;
    private float m_CurrentDamage;
    private float m_CurrentAttackSpeed;
    private int m_ComboCounter;
    private float m_ComboResetTimer;
    #endregion

    #region 技能系统
    private Coroutine m_SkillCoroutine;
    private bool m_IsSpecialActive;
    private float m_SkillDuration;
    #endregion

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Audio = GetComponent<AudioSource>();
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_Player = GetComponentInParent<PlayerController>();
    }

    internal void OnInit(WeaponInfo weaponInfo)
    {
        m_WeaponInfo = weaponInfo;
        RefreshWeaponStats();
        SetupVisuals();
        InitSkillSystem();

        Debug.Log($"装备武器:{weaponInfo.weaponName} Lv.{weaponInfo.currentLevel}");
    }

    void Update()
    {
        UpdateCooldown();
        UpdateComboSystem();
    }

    #region 核心逻辑
    public void TriggerAttack()
    {
        if (m_CurrentState != WeaponState.Idle) return;
        if (m_AttackCooldown > 0) return;

        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        m_CurrentState = WeaponState.Attacking;

        // 基础攻击
        PlayAttackAnimation();
        PlaySound(m_WeaponInfo.attackSound);

        // 连击计数
        m_ComboCounter = Mathf.Clamp(m_ComboCounter + 1, 1, 3);
        m_ComboResetTimer = 0;

        // 触发技能
        if (ShouldTriggerSkill())
        {
            TriggerWeaponSkill();
        }

        // 重置状态
        yield return new WaitForSeconds(0.2f);
        m_CurrentState = WeaponState.Idle;
        m_AttackCooldown = 1f / m_CurrentAttackSpeed;
    }

    private bool ShouldTriggerSkill()
    {
        return m_ComboCounter >= GetRequiredCombo() ||
               UnityEngine.Random.value < m_WeaponInfo.critRate;
    }
    #endregion

    #region 技能系统
    private void InitSkillSystem()
    {
        switch ((WeaponInfo.SkillTriggerType)m_WeaponInfo.triggerType)
        {
            case WeaponInfo.SkillTriggerType.OnAttack:
                break;
            case WeaponInfo.SkillTriggerType.OnKill:
                break;
            case WeaponInfo.SkillTriggerType.Continuous:
                break;
        }
    }

    private void TriggerWeaponSkill()
    {
        switch ((WeaponInfo.WeaponType)m_WeaponInfo.weaponType)
        {
            case WeaponInfo.WeaponType.Sword:
                break;
            case WeaponInfo.WeaponType.Spear:
                break;
            case WeaponInfo.WeaponType.Hammer:
                break;
        }

        PlaySound(m_WeaponInfo.skillSound);
        ShowSkillEffect();
    }
    #endregion

    #region 数值系统
    private void RefreshWeaponStats()
    {
        m_CurrentDamage = m_WeaponInfo.GetCurrentDamage();
    }

    private void ApplyDivineBonus()
    {
        m_CurrentDamage *= 1.5f;
        m_CurrentAttackSpeed *= 1.2f;
    }
    #endregion

    #region 辅助方法
    private void UpdateCooldown()
    {
        if (m_AttackCooldown > 0)
        {
            m_AttackCooldown -= Time.deltaTime;
        }
    }

    private void UpdateComboSystem()
    {
        if (m_ComboCounter > 0)
        {
            m_ComboResetTimer += Time.deltaTime;
            if (m_ComboResetTimer > 2f)
            {
                m_ComboCounter = 0;
                m_ComboResetTimer = 0;
            }
        }
    }

    private int GetRequiredCombo()
    {
        return (WeaponInfo.WeaponType)m_WeaponInfo.weaponType switch
        {
            WeaponInfo.WeaponType.Sword => 2,
            WeaponInfo.WeaponType.Blade => 3,
            _ => 4
        };
    }

    private async void SetupVisuals()
    {
        var weaponSprite = await ResourceService.LoadAsync<Sprite>(m_WeaponInfo.iconPath);
        if (weaponSprite != null)
        {
            m_SpriteRenderer.sprite = weaponSprite;
        }
    }
    #endregion

    #region 可视化反馈
    private void PlayAttackAnimation()
    {
        m_Animator.Play($"Attack_{m_ComboCounter}");
    }

    private void ShowSkillEffect()
    {
       
    }

    private void PlaySound(string soundKey)
    {
        SoundSystem.instance.Play(soundKey);
    }
    #endregion

    void OnDestroy()
    {
      
    }
}