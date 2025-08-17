using UnityEngine;
using System.Collections.Generic;
using Framework;
using System;
using static UnityEngine.EventSystems.EventTrigger;
using System.Threading.Tasks;

public class PlayerController : MonoBehaviour
{
    // ����״̬ö��
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

    // ����ö��
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


    [Header("��������")]
    public LegacyAnimationController animationController;

    [Header("�������")]
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

    [Header("��������")]
    public FacingDirection initialFacing = FacingDirection.Right;
    [Tooltip("�Ƿ�ʹ��Scale��ת���򣨷���ʹ��Rotation��")]
    public bool useScaleForFacing = false;
    private FacingDirection _currentFacing;

    [Header("��������")]
    public float maxShieldHealth = 100f;
    public float currentShieldHealth = 100f;
    public float ShieldHealth => currentShieldHealth / maxShieldHealth;

    private WeaponInfo curWeaponInfo;
    public int maxHitPoints = 5;
    public int currentHitPoints = 5;

    public float HitPoints => (float)currentHitPoints / maxHitPoints;

    public GameObject shieldVisual;

    [Header("�ƶ�����")]
    private float moveDirection;
    private bool isMoving = false;

    [Header("״̬")]
    [SerializeField] public PlayerState _currentState;
    private float _stateEnterTime;
    private Dictionary<PlayerState, string> _stateToAnimationMap = new Dictionary<PlayerState, string>();

    // �������
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

    [Header("��������")]
    public bool autoFireWhenIdle = true;        // վ���Զ�����
    public float baseAttacksPerSecond = 1f;     // ��׼���٣�1 ��ʾ AttackSpeedBase=1 ʱÿ��1����
    private float _nextAttackTime = 0f;         // ��һ�οɹ���ʱ���



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
            //����״̬
            SkillBar = 0;
        }
    }

    /// <summary>
    /// ���ý�ɫ����
    /// </summary>
    /// <param name="direction">������</param>
    public void SetFacingDirection(FacingDirection direction)
    {
        if (_currentFacing == direction) return;

        _currentFacing = direction;

        if (useScaleForFacing)
        {
            // ʹ��Scale��ת
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

            // ʹ��Rotation��ת
            transform.rotation = Quaternion.Euler(
                0f,
                y,
                0f
            );
        }
    }

    /// <summary>
    /// ƽ��ת��
    /// </summary>
    /// <param name="direction">Ŀ�귽��</param>
    /// <param name="turnSpeed">ת���ٶ�</param>
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

            // ����ǶȻ���
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
    /// ��ȡ��ǰ����
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

        // ========= �������루�ƶ���/΢�ţ� =========
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

        // ========= ������루PC/WebGL�� =========
        if (!handledByTouch)
        {
            if (Input.GetMouseButton(0))
            {
                float screenMiddle = Screen.width * 0.5f;
                move = (Input.mousePosition.x < screenMiddle) ? -1f : 1f;
                hasMoveIntent = true;
            }
        }

        // ========= UI��ť���� =========
        if (!hasMoveIntent)
        {
            if (isRightButtonPressed) { move = 1f; hasMoveIntent = true; }
            else if (isLeftButtonPressed) { move = -1f; hasMoveIntent = true; }
        }

        // ========= �����ᣨ���༭��/��WebGL�� =========
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

        // ========= God/�������� =========
        if (isGod && !GuideDispositionManager.instance.isGuide)
        {
            hasMoveIntent = false;
            move = 0f;
        }

        // ========= ״̬���� =========
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

        // ========= �Զ������߼���ֻ�ڲ��ƶ�ʱ������ =========
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

        AttackForward(); // ��������� AttackSpeedBase ���Ŷ���

        float aps = Mathf.Max(baseAttacksPerSecond * AttackSpeedBase, 0.01f);
        _nextAttackTime = Time.time + 1f / aps;
    }


    /// <summary>
    /// ������Ŀ�꣬����ǰ������ǰ�����������ӵ���
    /// </summary>
    public void AttackForward()
    {
        // �� AttackSpeedBase ���Ź���������Խ��Խ�죩
        ChangeState(PlayerState.Attack, AttackSpeedBase);
    }

    /// <summary>
    /// ���ù���
    /// </summary>
    /// <param name="newBase">�µĹ���</param>
    public void SetAttackSpeed(float newBase)
    {
        AttackSpeedBase = Mathf.Max(0.1f, newBase);  // ��ֹ 0 ����
                                                     // ���̰��¹���ˢ����һ��ʱ��
        float aps = Mathf.Max(baseAttacksPerSecond * AttackSpeedBase, 0.01f);
        _nextAttackTime = Mathf.Min(_nextAttackTime, Time.time + 1f / aps);
    }


    #region ������ƶ�����
    private float characterHalfWidth = 10f; // ������(���絥λ)
    private PlayerLevelsInfo playerLevelsInfo;

    private void UpdateMovement()
    {
        if (!isMoving) return;

        var cam = Camera.main;
        if (!cam) return;

        float actualMoveSpeed = baseMoveSpeed * speedCoefficient;

        // ��ԭ���ǰ�����X�ƶ������밴��Ļ�����ƶ������� cam.transform.right����ѡ��
        Vector3 moveAxis = Vector3.right; // ���ߣ�Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
        Vector3 targetWorld = transform.position + moveAxis * (moveDirection * actualMoveSpeed * Time.deltaTime);

        // ���� ���㡰������ӿڵ�Ч����������������ҷ��򣬸�����ģ�ͳ��򣩡���
        Vector3 vpNow = cam.WorldToViewportPoint(transform.position);
        float halfWidthVP = Mathf.Abs(
            cam.WorldToViewportPoint(transform.position + transform.right * characterHalfWidth).x - vpNow.x
        );

        // Ŀ��λ��ת���ӿ����꣬�����ұ߽�(0~1)���ߺ�н�
        Vector3 vpTarget = cam.WorldToViewportPoint(targetWorld);
        float depth = vpTarget.z; // ������Ŀ����ͬ�����
        vpTarget.x = Mathf.Clamp(vpTarget.x, 0f + halfWidthVP, 1f - halfWidthVP);

        // ת���������겢Ӧ�ã�������ĻY����Ȳ��䣬ֻ����XԽ�磩
        Vector3 worldClamped = cam.ViewportToWorldPoint(new Vector3(vpTarget.x, vpTarget.y, depth));
        transform.position = worldClamped;
    }

    #endregion

    #region ״̬������
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
    /// ����״̬
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

    #region ���ܺ��˺�ϵͳ
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
                // ����Χ�ƻ����߼�
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
        // ������������Ϸ�������߼�
    }
    #endregion

    #region ����ϵͳ
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

    #region ԭ�з���
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

    #region ��������
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