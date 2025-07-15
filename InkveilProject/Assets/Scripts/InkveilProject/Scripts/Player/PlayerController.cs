using UnityEngine;
using System.Collections.Generic;
using Framework;
using System;
using static UnityEngine.EventSystems.EventTrigger;

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
        set { 
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
    private Vector3 initialPoing = new Vector3(0,0,-15);

    private void Start()
    {
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
        SkillBar += 0.02f;
        if(SkillBar >= 1)
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
        PlayerLevelsInfo playerLevelsInfo = PlayerManager.instance.GetCurPlayerLevel();

        if (playerLevelsInfo == null) return;
        ShieldGrowthInfo shieldGrowthInfo = PlayerManager.instance.GetCurShieldGrowthLevel();
        curWeaponInfo = PlayerManager.instance.m_WeaponInfo;
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

    public void Update()
    {

        if (GameManager.instance.GameStateEnum != GameConfig.GameState.State.Play) { _currentState = PlayerState.Idle; return; }
        SkillBar += Time.deltaTime / 180;
        if(SkillBar >= 1)
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

// Keyboard input
        float keyboardInput = 0;
        keyboardInput += Input.GetAxisRaw("Horizontal");
        if (isRightButtonPressed) keyboardInput += 1;
        if (isLeftButtonPressed) keyboardInput -= 1;
        // 触屏输入
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                float touchX = touch.position.x;
                float screenMiddle = Screen.width * 0.5f;
                if (touchX < screenMiddle)
                {
                    keyboardInput -= 1; // 左边屏幕 → 左移
                }
                else
                {
                    keyboardInput += 1; // 右边屏幕 → 右移
                }
         
            }
        }

        // 鼠标点击输入（PC）
        if (Input.GetMouseButton(0)) // 鼠标左键按下
        {
            float mouseX = Input.mousePosition.x;
            float screenMiddle = Screen.width * 0.5f;

            
            if (mouseX < screenMiddle)
            {
                keyboardInput -= 1; // 左边点击 → 左移
            }
            else
            {
                keyboardInput += 1; // 右边点击 → 右移
            }
        }
        keyboardInput = Mathf.Clamp(keyboardInput,-1,1);

        if (isGod && !GuideDispositionManager.instance.isGuide)
        {
            keyboardInput = 0;
            isMoving = false;
        }

        if (keyboardInput != 0)
        {
            moveDirection = keyboardInput;
            isMoving = true;
            ChangeState(PlayerState.Run);

            if (keyboardInput > 0)
            {
                SetFacingDirection(FacingDirection.Right);
            }
            else if (keyboardInput < 0)
            {
                SetFacingDirection(FacingDirection.Left);
            }
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
            //if (Input.GetMouseButtonDown(0))
            //{
            //    float clickX = Input.mousePosition.x;
            //    float screenMiddle = Screen.width * 0.5f;

            //    if (clickX < screenMiddle)
            //    {
            //        moveDirection = -1f;
            //        SetFacingDirection(FacingDirection.Left);
            //    }
            //    else
            //    {
            //        moveDirection = 1f;
            //        SetFacingDirection(FacingDirection.Right);
            //    }

            //    isMoving = true;
            //    ChangeState(PlayerState.Run);
            //}

            //if (Input.GetMouseButtonUp(0))
            //{
            //    moveDirection = 0f;
            //    isMoving = false;
            //    ChangeState(PlayerState.Idle);
            //    SetFacingDirection(FacingDirection.Former);
            //}
        }

        if (_currentState != PlayerState.Run)
        {
            FindNearestEnemyAndAttack();
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

    #region 输入和移动控制
    private void UpdateMovement()
    {
        if (isMoving)
        {
            float actualMoveSpeed = baseMoveSpeed * speedCoefficient;
            float screenHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
            float playerHalfWidth = 0.5f;

            float newX = transform.position.x + moveDirection * actualMoveSpeed * Time.deltaTime;
            newX = Mathf.Clamp(newX, -screenHalfWidth + playerHalfWidth, screenHalfWidth - playerHalfWidth);

            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
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
    private void UpdateState()
    {
        switch (_currentState)
        {
            case PlayerState.Attack:
                if (animationController.GetCurrentAnimationProgress() >= 0.5f && !isAttack)
                {
                    isAttack = true;
                    EffectObject effect = EffectSystem.instance.GetEffect(curWeaponInfo.trailEffect);
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