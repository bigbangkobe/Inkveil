using Framework;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    public int m_PlayerLevel = 1;
    public int m_ShieldGrowthLevel = 1;

    private WeaponInfo m_WeaponInfo;
    public WeaponInfo WeaponInfo
    {
        get
        {
            if(m_WeaponInfo == null)
            {
                return WeaponDispositionManager.instance.GetWeaponById(1);
            }
            else
            {
                return m_WeaponInfo;
            }
        }
    }
    

    public void OnInit() 
    {
        PlayerDispositionManager.instance.OnInit();
        m_WeaponInfo = WeaponDispositionManager.instance.GetWeaponById(1);
    }

    public void SetCurPlayerLevel(int level)
    {
        m_PlayerLevel = level;
    }

    public PlayerLevelsInfo GetCurPlayerLevel() 
    {
        return PlayerDispositionManager.instance.GetPlayerLevelInfo(m_PlayerLevel);
    }

    public ShieldGrowthInfo GetCurShieldGrowthLevel()
    {
        return PlayerDispositionManager.instance.GetShieldGrowthInfo(m_ShieldGrowthLevel);
    }

    internal void Clear()
    {
        
    }
}