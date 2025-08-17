using UnityEngine;
using System.Collections.Generic;
using Framework;
using System;
using static UnityEngine.EventSystems.EventTrigger;
using System.Threading.Tasks;

public class PlayerController : MonoBehaviour
{
    // 动画状态枚举
    public enum PlayerState
    {
        None,
        Idle,
        Run,
        Attack,
        Hurt,
        Die,
        Victory
    }

    // 朝向枚举
    public enum FacingDirection
    {
        Former = 0,
        Right = 1,
        Left = -1
    }
    public static PlayerController instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType<PlayerController>() ?? new GameObject().AddComponent<PlayerController>();
            }

            return s_Instance;
        }
    }
    private static PlayerController s_Instance;


    [Header("动画设置")]
    public LegacyAnimationController animationController;

    [Header("玩家属性")]
    public float baseMoveSpeed = 5f;
    [Range(0.1f, 1f)] public float speedCoefficient = 0.5f;
    public float BaseDamage;
    public float AttackSpeedBase;

    public int level = 1;
    private float skillBar = 0;

    public float SkillBar
    {
        get => skillBar;
        set
        {
            skillBar = value;
            onSkillBar?.Invoke(value);
        }
    }

    private float inviteGodBar = 0;

    public float InviteGodBar
    {
        get => inviteGodBar;
        set
        {
            inviteGodBar = value;
            onInviteGodBar?.Invoke(value);
        }
    }

    [Header("朝向设置")]
    public FacingDirection initialFacing = FacingDirection.Right;
    [Tooltip("是否使用Scale翻转朝向（否则使用Rotation）")]
    public bool useScaleForFacing = false;
    private FacingDirection _currentFacing;

    [Header("护盾设置")]
    public float maxShieldHealth = 100f;
    public float currentShieldHealth = 100f;
    public float ShieldHealth => currentShieldHealth / maxShieldHealth;

    private WeaponInfo curWeaponInfo;
    public int maxHitPoints = 5;
    public int currentHitPoints = 5;

    public float HitPoints => (float)currentHitPoints / maxHitPoints;

    public GameObject shieldVisual;

    [Header("移动设置")]
    private float moveDirection;
    private bool isMoving = false;

    [Header("状态")]
    [SerializeField] public PlayerState _currentState;
    private float _stateEnterTime;
    private Dictionary<PlayerState, string> _stateToAnimationMap = new Dictionary<PlayerState, string>();

    // 护盾相关
    private float _shieldAmount = 0f;
    private float _shieldDamageMultiplier = 1f;
    private bool _isInvincible = false;
    private List<GameObject> _monstersOnShield = new List<GameObject>();
    private bool isAttack;
    private Transform _currentTarget;
    public Action onDamage;
    public Action onInitial;
    public Action<float> onSkillBar;
    public Action<float> onInviteGodBar;
    private bool isLeftButtonPressed;
    private bool isRightButtonPressed;
    public bool isGod;
    private Vector3 initialPoing = new Vector3(0, -2.5f, -15);

    [Header("攻击设置")]
    public bool autoFireWhenIdle = true;        // 站立自动连射
    public float baseAttacksPerSecond = 1f;     // 基准射速（1 表示 AttackSpeedBase=1 时每秒1发）
    private float _nextAttackTime = 0f;         // 下一次可攻击时间戳



    private void Start()
    {
        if (transform.parent == null) 
        {
            Destroy(gameObject);
            return;
        }
        transform.parent.position = initialPoing;
        InitializePlaierInfo();
        SetFacingDirection(initialFacing);
        InitializeAnimationStates();

        EnemyManager.instance.OnEnemyDestroyed += OnEnemyDestroyedHandler;
    }

    private void OnDestroy()
    {
        EnemyManager.instance.OnEnemyDestroyed -= OnEnemyDestroyedHandler;
    }

    private void OnEnemyDestroyedHandler()
    {
        if (!GuideDispositionManager.instance.isGuide) return;
            SkillBar += 0.02f;
        if (SkillBar >= 1)
        {
            //重置状态
            SkillBar = 0;
        }
    }

    /// <summary>
    /// 设置角色朝向
    /// </summary>
    /// <param name="direction">朝向方向</param>
    public void SetFacingDirection(FacingDirection direction)
    {
        if (_currentFacing == direction) return;

        _currentFacing = direction;

        if (useScaleForFacing)
        {
            // 使用Scale翻转
            Vector3 newScale = transform.localScale;
            newScale.x = Mathf.Abs(newScale.x) * (int)direction;
            transform.localScale = newScale;
        }
        else
        {
            float y = transform.localScale.y;
            switch (direction)
            {
                case FacingDirection.Former:
                    y = 0;
                    break;
                case FacingDirection.Right:
                    y = 90;
                    break;
                case FacingDirection.Left:
                    y = -90;
                    break;
                default:
                    break;
            }

            // 使用Rotation翻转
            transform.rotation = Quaternion.Euler(
                0f,
                y,
                0f
            );
        }
    }

    /// <summary>
    /// 平滑转向
    /// </summary>
    /// <param name="direction">目标方向</param>
    /// <param name="turnSpeed">转向速度</param>
    public void SmoothTurnTo(FacingDirection direction, float turnSpeed = 5f)
    {
        if (_currentFacing == direction) return;

        if (useScaleForFacing)
        {
            float targetScale = (int)direction * Mathf.Abs(transform.localScale.x);
            float newX = Mathf.Lerp(
                transform.localScale.x,
                targetScale,
                Time.deltaTime * turnSpeed
            );

            transform.localScale = new Vector3(
                newX,
                transform.localScale.y,
                transform.localScale.z
            );

            if (Mathf.Abs(newX - targetScale) < 0.1f)
            {
                SetFacingDirection(direction);
            }
        }
        else
        {
            float targetY = direction == FacingDirection.Right ? 0f : 180f;
            float currentY = transform.rotation.eulerAngles.y;

            // 处理角度环绕
            if (Mathf.Abs(currentY - targetY) > 180f)
            {
                if (currentY < targetY) currentY += 360f;
                else targetY += 360f;
            }

            float newY = Mathf.LerpAngle(
                currentY,
                targetY,
                Time.deltaTime * turnSpeed
            );

            transform.rotation = Quaternion.Euler(0f, newY, 0f);

            if (Mathf.Abs(newY - targetY) < 1f)
            {
                SetFacingDirection(direction);
            }
        }
    }

    /// <summary>
    /// 获取当前朝向
    /// </summary>
    public FacingDirection GetCurrentFacing()
    {
        return _currentFacing;
    }

    private void InitializeAnimationStates()
    {
        _stateToAnimationMap.Add(PlayerState.Idle, "idle");
        _stateToAnimationMap.Add(PlayerState.Run, "run");
        _stateToAnimationMap.Add(PlayerState.Attack, "attack");
        _stateToAnimationMap.Add(PlayerState.Hurt, "hurt");
        _stateToAnimationMap.Add(PlayerState.Die, "die");
        _stateToAnimationMap.Add(PlayerState.Victory, "victory");

        ChangeState(PlayerState.Idle);
    }

    public void InitializePlaierInfo()
    {
        playerLevelsInfo = PlayerManager.instance.GetCurPlayerLevel();

        if (playerLevelsInfo == null) return;
        ShieldGrowthInfo shieldGrowthInfo = PlayerManager.instance.GetCurShieldGrowthLevel();
        curWeaponInfo = PlayerManager.instance.WeaponInfo;
        maxHitPoints = playerLevelsInfo.hpBase;
        currentHitPoints = playerLevelsInfo.hpBase;
        AttackSpeedBase = (float)playerLevelsInfo.attackSpeedBase;
        BaseDamage = playerLevelsInfo.attackBase + curWeaponInfo.GetCurrentDamage();

        currentShieldHealth = shieldGrowthInfo.shieldHP;
        maxShieldHealth = shieldGrowthInfo.shieldHP;
        UpdateShieldVisual();
        SkillBar = 0;
        onInitial?.Invoke();
    }

    public void AddBaseDamage(float attack,bool isBai = false) 
    {
        BaseDamage += isBai ? BaseDamage * attack / 100 : attack;
    }

    public void AddAttackSpeed(float attackSpeed)
    {
        AttackSpeedBase += (float)playerLevelsInfo.attackSpeedBase * attackSpeed / 100;
    }

    public void AddShieldHP(float shieldHP)
    {
        currentShieldHealth = Mathf.Clamp(currentShieldHealth + shieldHP,0, maxShieldHealth);
    }

    public void Update()
    {

        if (GameManager.instance.GameStateEnum != GameConfig.GameState.State.Play) { _currentState = PlayerState.Idle; return; }
        if (GuideDispositionManager.instance.isGuide) 
        SkillBar += Time.deltaTime / 180;
        if (SkillBar >= 1)
        {
            SkillBar = 0;
        }
        //AttackSpeedBase = Math.Clamp((int)(Time.time / 60) * 0.08f + 1, 1, 5);
        //int inedx = Math.Clamp((int)(Time.time / 180) + 1, 1, 30);
        //if (level != inedx)
        //{
        //    level = inedx;  
        //    PlayerManager.instance.SetCurPlayerLevel(level);
        //    InitializePlaierInfo();
        //}


        HandleInput();
        UpdateMovement();
        UpdateState();
        UpdateMonstersOnShield();
    }

    public void OnLeftButtonDown()
    {
        isLeftButtonPressed = true;
    }

    public void OnLeftButtonUp()
    {
        isLeftButtonPressed = false;
    }

    public void OnRightButtonDown()
    {
        isRightButtonPressed = true;
    }

    public void OnRightButtonUp()
    {
        isRightButtonPressed = false;
    }
    private void HandleInput()
    {
        float move = 0f;
        bool hasMoveIntent = false;
        bool handledByTouch = false;

        // ========= 触摸输入（移动端/微信） =========
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began || t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            {
                float screenMiddle = Screen.width * 0.5f;
                move = (t.position.x < screenMiddle) ? -1f : 1f;
                hasMoveIntent = true;
                handledByTouch = true;
            }
        }

        // ========= 鼠标输入（PC/WebGL） =========
        if (!handledByTouch)
        {
            if (Input.GetMouseButton(0))
            {
                float screenMiddle = Screen.width * 0.5f;
                move = (Input.mousePosition.x < screenMiddle) ? -1f : 1f;
                hasMoveIntent = true;
            }
        }

        // ========= UI按钮输入 =========
        if (!hasMoveIntent)
        {
            if (isRightButtonPressed) { move = 1f; hasMoveIntent = true; }
            else if (isLeftButtonPressed) { move = -1f; hasMoveIntent = true; }
        }

        // ========= 键盘轴（仅编辑器/非WebGL） =========
#if UNITY_EDITOR || !(UNITY_WEBGL || UNITY_WASM)
        if (!hasMoveIntent)
        {
            float axis = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(axis) >= 0.5f)
            {
                move = Mathf.Sign(axis);
                hasMoveIntent = true;
            }
        }
#endif

        // ========= God/引导限制 =========
        if (isGod && !GuideDispositionManager.instance.isGuide)
        {
            hasMoveIntent = false;
            move = 0f;
        }

        // ========= 状态更新 =========
        if (hasMoveIntent)
        {
            moveDirection = move;
            if (!isMoving || _currentState != PlayerState.Run)
            {
                isMoving = true;
                ChangeState(PlayerState.Run);
            }

            if (move > 0) SetFacingDirection(FacingDirection.Right);
            else if (move < 0) SetFacingDirection(FacingDirection.Left);
        }
        else
        {
            if (isMoving)
            {
                moveDirection = 0f;
                isMoving = false;
                ChangeState(PlayerState.Idle);
                SetFacingDirection(FacingDirection.Former);
            }
        }

        // ========= 自动攻击逻辑（只在不移动时触发） =========
        if (_currentState != PlayerState.Run && autoFireWhenIdle)
        {
            TryAutoAttackForward();
        }
    }


    private void FindNearestEnemyAndAttack()
    {
        EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
        if (enemies.Length == 0) return;

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

        if (nearestEnemy != null && Vector3.Distance(transform.position, nearestEnemy.position) < 20)
        {
            _currentTarget = nearestEnemy;
            Attack();
        }
    }

    private void Attack()
    {
        if (_currentTarget == null) return;

        // Face the enemy (only rotate Y axis)
        Vector3 direction = _currentTarget.position - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

        ChangeState(PlayerState.Attack, AttackSpeedBase);
    }

    private void TryAutoAttackForward()
    {
        if (Time.time < _nextAttackTime) return;

        AttackForward(); // 这行里会用 AttackSpeedBase 播放动画

        float aps = Mathf.Max(baseAttacksPerSecond * AttackSpeedBase, 0.01f);
        _nextAttackTime = Time.time + 1f / aps;
    }


    /// <summary>
    /// 不锁定目标，按当前朝向向前攻击（发射子弹）
    /// </summary>
    public void AttackForward()
    {
        // 用 AttackSpeedBase 播放攻击动画（越大越快）
        ChangeState(PlayerState.Attack, AttackSpeedBase);
    }

    /// <summary>
    /// 设置攻速
    /// </summary>
    /// <param name="newBase">新的攻速</param>
    public void SetAttackSpeed(float newBase)
    {
        AttackSpeedBase = Mathf.Max(0.1f, newBase);  // 防止 0 或负数
                                                     // 立刻按新攻速刷新下一发时间
        float aps = Mathf.Max(baseAttacksPerSecond * AttackSpeedBase, 0.01f);
        _nextAttackTime = Mathf.Min(_nextAttackTime, Time.time + 1f / aps);
    }


    #region 输入和移动控制
    private float characterHalfWidth = 10f; // 人物半宽(世界单位)
    private PlayerLevelsInfo playerLevelsInfo;

    private void UpdateMovement()
    {
        if (!isMoving) return;

        var cam = Camera.main;
        if (!cam) return;

        float actualMoveSpeed = baseMoveSpeed * speedCoefficient;

        // 你原本是按世界X移动；若想按屏幕左右移动，换成 cam.transform.right（可选）
        Vector3 moveAxis = Vector3.right; // 或者：Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
        Vector3 targetWorld = transform.position + moveAxis * (moveDirection * actualMoveSpeed * Time.deltaTime);

        // —— 计算“半宽”的视口等效量（用物体自身的右方向，更贴近模型朝向）——
        Vector3 vpNow = cam.WorldToViewportPoint(transform.position);
        float halfWidthVP = Mathf.Abs(
            cam.WorldToViewportPoint(transform.position + transform.right * characterHalfWidth).x - vpNow.x
        );

        // 目标位置转到视口坐标，按左右边界(0~1)留边后夹紧
        Vector3 vpTarget = cam.WorldToViewportPoint(targetWorld);
        float depth = vpTarget.z; // 保持与目标相同的深度
        vpTarget.x = Mathf.Clamp(vpTarget.x, 0f + halfWidthVP, 1f - halfWidthVP);

        // 转回世界坐标并应用（保持屏幕Y与深度不变，只修正X越界）
        Vector3 worldClamped = cam.ViewportToWorldPoint(new Vector3(vpTarget.x, vpTarget.y, depth));
        transform.position = worldClamped;
    }

    #endregion

    #region 状态机管理
    public void ChangeState(PlayerState newState, float speed = 1)
    {
        if (_currentState == newState) return;

        OnStateExit(_currentState);
        _currentState = newState;
        _stateEnterTime = Time.time;

        if (_stateToAnimationMap.TryGetValue(newState, out string animName))
        {
            animationController.PlayAnimationImmediate(animName, speed);
        }

        OnStateEnter(newState);
    }

    private void OnStateEnter(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Attack:
                break;
            case PlayerState.Hurt:
                break;
            case PlayerState.Die:
                GameOver();
                break;
        }
    }

    private void OnStateExit(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Attack:
                break;
        }
    }

    /// <summary>
    /// 更新状态
    /// </summary>
    private async Task UpdateState()
    {
        switch (_currentState)
        {
            case PlayerState.Attack:
                if (animationController.GetCurrentAnimationProgress() >= 0.5f && !isAttack)
                {
                    isAttack = true;
                    EffectObject effect = await EffectSystem.instance.GetEffect(curWeaponInfo.trailEffect);
                    effect.gameObject.SetActive(true);
                    Bullet3D bullet3D = effect.gameObject.GetComponent<Bullet3D>();
                    bullet3D.transform.position = transform.position + transform.forward * 1 + transform.up * 0.5f;
                    bullet3D.Initialize(transform.forward, 10, BaseDamage);
                    effect.Play();
                    SoundSystem.instance.Play(curWeaponInfo.attackSound, 1);
                }

                if (animationController.GetCurrentAnimationProgress() >= 0.95f)
                {
                    ChangeState(PlayerState.Idle);
                    isAttack = false;
                }
                break;

            case PlayerState.Hurt:
                if (Time.time - _stateEnterTime > 0.5f)
                {
                    ChangeState(PlayerState.Idle);
                }
                break;
            default:
                isAttack = false;
                break;
        }
    }
    #endregion

    #region 护盾和伤害系统
    public void TakeDamage(float damage)
    {
        if (_isInvincible) return;

        if (currentShieldHealth > 0)
        {
            currentShieldHealth -= damage;
            onDamage?.Invoke();
            if (currentShieldHealth <= 0)
            {
                currentShieldHealth = 0;
                OnShieldBroken();
            }
            UpdateShieldVisual();
            return;
        }

        currentHitPoints--;
        //ChangeState(PlayerState.Hurt);

        if (currentHitPoints <= 0)
        {
            currentHitPoints = 0;
            GameOver();
            //ChangeState(PlayerState.Die);
        }
    }

    private void OnShieldBroken()
    {
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
    }

    private void UpdateShieldVisual()
    {
        //if (shieldVisual != null)
        //{
        //    shieldVisual.SetActive(currentShieldHealth > 1000);
        //}
    }

    private void UpdateMonstersOnShield()
    {
        foreach (var monster in _monstersOnShield)
        {
            if (monster != null)
            {
                // 怪物围绕护盾逻辑
            }
        }
    }

    public void AddMonsterToShield(GameObject monster)
    {
        if (!_monstersOnShield.Contains(monster))
        {
            _monstersOnShield.Add(monster);
        }
    }

    public void RemoveMonsterFromShield(GameObject monster)
    {
        if (_monstersOnShield.Contains(monster))
        {
            _monstersOnShield.Remove(monster);
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over");
        GameManager.instance.GameStateEnum = GameConfig.GameState.State.Over;
        // 这里可以添加游戏结束的逻辑
    }
    #endregion

    #region 升级系统
    public void UpgradeShield(float healthIncrease)
    {
        maxShieldHealth += healthIncrease;
        currentShieldHealth = maxShieldHealth;
        UpdateShieldVisual();
    }

    public void UpgradeHitPoints(int additionalLives)
    {
        maxHitPoints += additionalLives;
        currentHitPoints = maxHitPoints;
    }

    public void UpgradeMoveSpeed(float speedIncrease)
    {
        baseMoveSpeed += speedIncrease;
    }

    public void AdjustSpeedCoefficient(float newCoefficient)
    {
        speedCoefficient = Mathf.Clamp(newCoefficient, 0.1f, 1f);
    }
    #endregion

    #region 原有方法
    internal void SetInvincible(bool isInvincible)
    {
        _isInvincible = isInvincible;
    }

    internal void AddShield(float skillDamageMulti, float amount)
    {
        _shieldDamageMultiplier = skillDamageMulti;
        _shieldAmount += amount;
    }

    internal void AddShield(float skillDamageMulti)
    {
        _shieldDamageMultiplier = skillDamageMulti;
        _shieldAmount = Mathf.Infinity;
    }
    #endregion

    #region 辅助方法
    public float GetStateDuration()
    {
        return Time.time - _stateEnterTime;
    }

    public bool IsInState(PlayerState state)
    {
        return _currentState == state;
    }

    public bool IsShieldActive()
    {
        return currentShieldHealth > 0;
    }

    public float GetCurrentSpeed()
    {
        return baseMoveSpeed * speedCoefficient;
    }
    #endregion
}