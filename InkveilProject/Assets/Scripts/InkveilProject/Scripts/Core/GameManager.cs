using System;
using Framework;
using StarWarsEve;
using static GameConfig;

public class GameManager : MonoSingleton<GameManager>
{
    private bool isInltial;

    /// <summary>
    /// 游戏状态机
    /// </summary>
    private GameState.State m_GameStateEnum = GameState.State.None;
    public Action<GameState.State> onGameState;

    public GameState.State GameStateEnum
    {
        get { return m_GameStateEnum; }
        set
        {
            m_GameStateEnum = value;
            OnGameStateChanageHandler(value);
            onGameState?.Invoke(value);
        }
    }

    private void OnGameStateChanageHandler(GameState.State value)
    {
        switch (value)
        {
            case GameState.State.None:
                break;
            case GameState.State.Ready:
                break;
            case GameState.State.Start:
                break;
            case GameState.State.Play:
                break;
            case GameState.State.Pause:
                break;
            case GameState.State.Stop:
                break;
            case GameState.State.Over:
                break;
            default:
                break;
        }
    }

    internal void StartGame()
    {
        GameStateEnum = GameState.State.Play;
    }

    /// <summary>
    /// 初始化函数
    /// </summary>
    public void OnInit() 
    {
        if (isInltial) return;
        SoundSystem.instance.OnInit();
        EffectSystem.instance.OnInit();
        EnemyManager.instance.OnInit();
        WeaponManager.instance.OnInit();
        PlayerManager.instance.OnInit();
        LevelManager.instance.OnInit();
        PropertyManager.instance.OnInit();
        BagManager.instance.OnInit();
        ShopManager.instance.OnInit();
        LotteryDispositionManager.instance.OnInit();
        GodDispositionManager.instance.OnInit();
        GuideDispositionManager.instance.OnInit();
        GuideManager.instance.OnInit();
        isInltial = true;
    }

    protected override void Awake()
    {
        base.Awake();
        OnInit();
    }

    private void Update()
    {
        if (m_GameStateEnum == GameState.State.Play)
        {
            EnemyManager.instance.OnUpdate();
            LevelManager.instance.OnUpdate();
        }
    }

    private void LateUpdate()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    
    /// <summary>
    /// 内存释放函数
    /// </summary>
    public void OnClear() 
    {
        SoundSystem.instance.Clear();
        EffectSystem.instance.Clear();
        EnemyManager.instance.Clear();
        WeaponManager.instance.Clear();
        PlayerManager.instance.Clear();
        LevelManager.instance.Clear();
        PropertyManager.instance.Clear();
        BagManager.instance.Clear();
        ShopManager.instance.Clear();
        LotteryDispositionManager.instance.Clear();
    }
}