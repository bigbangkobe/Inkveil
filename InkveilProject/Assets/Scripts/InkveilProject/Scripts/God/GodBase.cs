using UnityEngine;
using System.Collections;
using System;
using Framework;
using System.Collections.Generic;

public class GodBase : MonoBehaviour
{
    // 神明状态枚举
    public enum GodState
    {
        None,       // 无状态
        Idle,       // 待机状态
        Summon,     // 召唤中
        Active,     // 激活状态
        Attack,     // 攻击状态
        Skill,      // 释放技能
        Dismiss,    // 消失中
        Cooldown    // 冷却中
    }

    // 神明配置数据
    protected GodInfo _godInfo;

    // 状态机相关
    protected GodState _currentState = GodState.None;          // 当前状态
    protected float _stateEnterTime;           // 状态进入时间
    private Dictionary<GodState, string> _stateToAnimationMap = new Dictionary<GodState, string>(); // 状态对应动画

    // 计时相关
    protected float _currentDuration = 0f;     // 当前剩余持续时间
    protected float _currentCooldown = 0f;     // 当前冷却时间
    protected int _currentEnergy = 0;          // 当前能量值

    // 组件引用
    protected Transform _transform;
    protected LegacyAnimationController _animationController;
    protected AudioSource _audioSource;
    protected Transform _currentTarget;        // 当前攻击目标
    private bool isAttack;
    private float skillTimer = 0;

    // 属性
    public int GodID => _godInfo.godID;
    public string GodName => _godInfo.godName;
    public bool IsActive => _currentState == GodState.Active;
    public bool IsReady => _currentCooldown <= 0f;
    public float EnergyPercentage => (float)_currentEnergy / _godInfo.skillEnergy;

    // 事件
    public event Action OnActivated;           // 激活事件
    public event Action OnDeactivated;         // 解除激活事件
    public event Action OnSkillUsed;           // 使用技能事件
    public event Action<float> OnCooldownChanged; // 冷却时间变化
    public event Action<int> OnEnergyChanged;  // 能量值变化
    public event Action<bool> OnIsEnergy;  // 能量值变化

    protected virtual void Awake()
    {
        _transform = transform;
        _animationController = GetComponent<LegacyAnimationController>();
        _audioSource = GetComponent<AudioSource>();

        InitializeAnimationStates(); // 初始化动画状态映射
    }

    // 初始化状态-动画映射
    private void InitializeAnimationStates()
    {
        _stateToAnimationMap.Add(GodState.Idle, "idle");
        _stateToAnimationMap.Add(GodState.Summon, "summon");
        _stateToAnimationMap.Add(GodState.Active, "active");
        _stateToAnimationMap.Add(GodState.Attack, "attack");
        _stateToAnimationMap.Add(GodState.Skill, "skill");
        _stateToAnimationMap.Add(GodState.Dismiss, "dismiss");

  
    }


    private void OnEnable()
    {
        EnemyManager.instance.OnEnemyDestroyed += OnEnemyDestroyedHandler;


        if (!GuideDispositionManager.instance.isGuide)
        {
            TimerSystem.Start((x) =>
            {
                AddEnergy(100);
            }, false, 12);
        }

        ChangeState(GodState.Idle); // 初始化为待机状态
    }

    private void OnDisable()
    {
        EnemyManager.instance.OnEnemyDestroyed -= OnEnemyDestroyedHandler;
    }

    // 敌人被摧毁时的处理
    private void OnEnemyDestroyedHandler()
    {
        if(GuideDispositionManager.instance.isGuide) AddEnergy(5); // 增加能量
    }

    // 初始化神明
    internal void OnInit(GodInfo godInfo)
    {
        _godInfo = godInfo;
        _currentDuration = _godInfo.maxDuration;
        _currentCooldown = 0f;
        _currentEnergy = 0;
    }

    protected virtual void Update()
    {
        if (GameManager.instance.GameStateEnum != GameConfig.GameState.State.Play) 
        {
            ChangeState(GodState.Idle);
            return;
        }
        UpdateState(); // 每帧更新状态

        FindNearestEnemyAndAttack(); // 激活状态下寻找敌人攻击
    }

    // 寻找最近的敌人并攻击
    private void FindNearestEnemyAndAttack()
    {
        EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
        if (enemies.Length == 0) return;

        // 找出最近的敌人
        Transform nearestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            if (!enemy.gameObject.activeInHierarchy) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }

        // 如果敌人在攻击范围内则攻击
        if (nearestEnemy != null && Vector3.Distance(transform.position, nearestEnemy.position) < 20)
        {
            _currentTarget = nearestEnemy;
            Attack();
        }
        else
        {
            ChangeState(GodState.Idle); // 切换到攻击状态
        }
    }

    // 攻击逻辑
    private void Attack()
    {
        if (_currentTarget == null && _currentState == GodState.Skill) return;
        ChangeState(GodState.Attack, (float)_godInfo.attackSpeed);
        // 朝向敌人（仅旋转Y轴）
        Vector3 direction = _currentTarget.position - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

    }

    // 切换状态
    public void ChangeState(GodState newState,float speed = 1)
    {
        if (_currentState == newState) return;

        //OnStateExit(_currentState); // 退出当前状态
        _currentState = newState;
        _stateEnterTime = Time.time;

        // 播放对应动画
        if (_stateToAnimationMap.TryGetValue(newState, out string animName))
        {
            _animationController.PlayAnimationImmediate(animName, speed);
        }

        OnStateEnter(newState); // 进入新状态
    }

    // 状态进入处理
    private void OnStateEnter(GodState state)
    {
        switch (state)
        {
            case GodState.Summon:
                // 播放召唤音效
                if (!string.IsNullOrEmpty(_godInfo.summonSound))
                {
                    PlaySound(_godInfo.summonSound);
                }
                break;

            case GodState.Active:
                _currentDuration = _godInfo.maxDuration;
                OnActivated?.Invoke(); // 触发激活事件
                break;

            case GodState.Attack:
                // 播放攻击音效
                if (!string.IsNullOrEmpty(_godInfo.basicAttackSound))
                {
                    PlaySound(_godInfo.basicAttackSound);
                }
                break;

            case GodState.Skill:
                _currentEnergy = 0;
                OnEnergyChanged?.Invoke(_currentEnergy);

                skillTimer += Time.deltaTime;

                if (skillTimer >= 3)
                {
                    ChangeState( GodState.Idle);
                    skillTimer = 0;
                }

                // 播放技能音效
                if (!string.IsNullOrEmpty(_godInfo.skillSound))
                {
                    PlaySound(_godInfo.skillSound);
                }

                ApplySkillEffects(); // 应用技能效果
                OnSkillUsed?.Invoke(); // 触发技能使用事件
                break;

            case GodState.Dismiss:
                // 播放消失音效
                //if (!string.IsNullOrEmpty(_godInfo.dismissSound))
                //{
                //    PlaySound(_godInfo.dismissSound);
                //}
                OnIsEnergy?.Invoke(false);
                Destroy(gameObject);
                break;

            case GodState.Cooldown:
                _currentCooldown = _godInfo.baseCooldown;
                OnDeactivated?.Invoke(); // 触发解除激活事件
                break;
        }
    }

    // 状态退出处理
    private void OnStateExit(GodState state)
    {
        switch (state)
        {
            case GodState.Summon:
                ChangeState(GodState.Active); // 召唤完成后进入激活状态
                break;

            case GodState.Dismiss:
                gameObject.SetActive(false);
                GodManager.instance.RemoveGod(_godInfo.godName, this); // 从管理器移除
                break;
        }
    }

    // 更新状态逻辑
    private async void UpdateState()
    {
        switch (_currentState)
        {
            case GodState.Active:
                _currentDuration -= Time.deltaTime;

                if (_currentDuration <= 0f)
                {
                    ChangeState(GodState.Dismiss); // 持续时间结束进入消失状态
                }
                break;

            case GodState.Attack:
                if (_animationController.GetCurrentAnimationProgress() >= 0.5f && !isAttack)
                {
                    isAttack = true;
                    EffectObject effect = await EffectSystem.instance.GetEffect(_godInfo.basicAttackEffect);
                    effect.gameObject.SetActive(true);
                    GodAttackCtrl bullet3D = effect.gameObject.GetComponent<GodAttackCtrl>();
                    bullet3D.transform.LookAt(_currentTarget.transform.position);
                    bullet3D.OnInitialFlag(_godInfo, transform, _currentTarget);
                    //bullet3D.transform.position = transform.position + transform.forward * 1 + transform.up * 0.5f;
                    //bullet3D.Initialize(transform.forward, 10, BaseDamage);
                    //effect.Play();
                    //SoundSystem.instance.Play(_godInfo.basicAttackSound, 1);
                }

                if (_animationController.GetCurrentAnimationProgress() >= 0.9f)
                {
                    ChangeState(GodState.Idle);
                    isAttack = false;
                }
                break;

            case GodState.Skill:
                if (_animationController.GetCurrentAnimationProgress() >= 0.95f)
                {
                    ChangeState(GodState.Active); // 技能动画播放完成后返回激活状态
                }
                break;

            case GodState.Cooldown:
                _currentCooldown -= Time.deltaTime;
                OnCooldownChanged?.Invoke(_currentCooldown / _godInfo.baseCooldown);

                if (_currentCooldown <= 0f)
                {
                    _currentCooldown = 0f;
                    ChangeState(GodState.Idle); // 冷却结束返回待机状态
                }
                break;
        }
    }

    // 激活神明
    public virtual void Activate()
    {
        if (_currentState != GodState.Idle || !IsReady) return;

        ChangeState(GodState.Summon); // 进入召唤状态
    }

    // 解除激活
    public virtual void Deactivate()
    {
        ChangeState(GodState.Dismiss); // 进入消失状态
    }

    // 普通攻击
    public virtual void BasicAttack()
    {
        if (_currentState != GodState.Active) return;

        Attack(); // 执行攻击
    }

    // 使用技能
    public virtual async void UseSkill()
    {
        if (_currentEnergy < _godInfo.skillEnergy) return;

        EffectObject effect = await EffectSystem.instance.GetEffect(_godInfo.skillEffect);
        effect.gameObject.SetActive(true);
        GodAttackCtrl bullet3D = effect.gameObject.GetComponent<GodAttackCtrl>();
        bullet3D.OnInitialFlag(_godInfo, transform, _currentTarget);

        ChangeState(GodState.Skill); // 进入技能状态
    }

    // 应用技能效果（子类实现）
    protected virtual void ApplySkillEffects()
    {
        // 由具体神明子类实现
    }

    // 增加能量
    public void AddEnergy(int amount)
    {
        _currentEnergy = Mathf.Min(_currentEnergy + amount, _godInfo.skillEnergy);
        OnEnergyChanged?.Invoke(_currentEnergy);
        OnIsEnergy?.Invoke(EnergyPercentage >= 1);
    }

    // 播放音效
    protected void PlaySound(string soundPath)
    {
        //AudioClip clip = Resources.Load<AudioClip>(soundPath);
        //if (clip != null && _audioSource != null)
        //{
        //    _audioSource.PlayOneShot(clip);
        //}
    }

    // 升级神明
    public void LevelUp()
    {
        _godInfo.level++;
        _godInfo.maxDuration = Mathf.RoundToInt(_godInfo.maxDuration * 1.1f);
        _godInfo.skillDamageMulti *= 1.05f;
    }

    // 命中目标回调
    public virtual void OnHitTarget()
    {
        if (GuideDispositionManager.instance.isGuide) AddEnergy(5); // 命中目标增加能量
    }
}