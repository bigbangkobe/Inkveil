using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework;
using LitJson;
using UnityEngine;

public class PlayerDispositionManager : Singleton<PlayerDispositionManager>
{
    private Dictionary<int, PlayerLevelsInfo> m_PlayerLevelsDic = new Dictionary<int, PlayerLevelsInfo>();
    private Dictionary<int, ShieldGrowthInfo> m_ShieldGrowthInfoDic = new Dictionary<int, ShieldGrowthInfo>();

    private bool m_IsInitialized = false;
    private PlayerAssetsInfo m_PlayerAssetsInfo;
    public PlayerAssetsInfo PlayerAssetsInfo { get => m_PlayerAssetsInfo; set => m_PlayerAssetsInfo = value; }
    public Action onPlayerAssetsChanged;

    /// <summary>
    /// 异步初始化玩家配置系统
    /// </summary>
    public async Task OnInitAsync()
    {
        if (m_IsInitialized) return;

        try
        {
            // 异步加载配置文本
            var levelHandle = ResourceService.LoadAsync<TextAsset>(ConfigDefine.playerLevelsInfo);
            var shieldHandle = ResourceService.LoadAsync<TextAsset>(ConfigDefine.shieldGrowthInfo);
            var assetsHandle = ResourceService.LoadAsync<TextAsset>(ConfigDefine.playerAssets);

            TextAsset levelAsset = await levelHandle.Task;
            TextAsset shieldAsset = await shieldHandle.Task;
            TextAsset assetsAsset = await assetsHandle.Task;

            // 解析或加载存档数据
            string assetsJson = PlayerPrefs.GetString("PlayerAssetsInfo");
            if (string.IsNullOrEmpty(assetsJson))
            {
                if (assetsAsset == null)
                {
                    Debug.LogError($"玩家资产配置加载失败：{ConfigDefine.playerAssets}");
                    return;
                }
                assetsJson = assetsAsset.text;
            }
            m_PlayerAssetsInfo = JsonMapper.ToObject<PlayerAssetsInfo>(assetsJson);

            if (levelAsset == null)
            {
                Debug.LogError($"配置文件加载失败：{ConfigDefine.playerLevelsInfo}");
                return;
            }
            var levelList = JsonMapper.ToObject<List<PlayerLevelsInfo>>(levelAsset.text)
                             ?? new List<PlayerLevelsInfo>();
            foreach (var info in levelList)
                m_PlayerLevelsDic[info.playerLevels] = info;

            if (shieldAsset == null)
            {
                Debug.LogError($"配置文件加载失败：{ConfigDefine.shieldGrowthInfo}");
                return;
            }
            var shieldList = JsonMapper.ToObject<List<ShieldGrowthInfo>>(shieldAsset.text)
                              ?? new List<ShieldGrowthInfo>();
            foreach (var info in shieldList)
                m_ShieldGrowthInfoDic[info.level] = info;

            m_IsInitialized = true;
            Debug.Log("玩家配置系统初始化完成");
        }
        catch (Exception ex)
        {
            Debug.LogError($"玩家配置初始化异常：{ex}");
        }
    }

    public PlayerLevelsInfo GetPlayerLevelInfo(int id)
    {
        if (!m_IsInitialized) return null;
        return m_PlayerLevelsDic.TryGetValue(id, out var info) ? info : null;
    }

    public ShieldGrowthInfo GetShieldGrowthInfo(int id)
    {
        if (!m_IsInitialized) return null;
        return m_ShieldGrowthInfoDic.TryGetValue(id, out var info) ? info : null;
    }

    public void SavePlayerAsset()
    {
        if (!m_IsInitialized) return;
        string str = JsonMapper.ToJson(m_PlayerAssetsInfo);
        PlayerPrefs.SetString("PlayerAssetsInfo", str);
        PlayerPrefs.Save();
        onPlayerAssetsChanged?.Invoke();
        Debug.Log($"玩家资产已保存: {str}");
    }

    /// <summary>
    /// 清理系统资源
    /// </summary>
    public void OnClear()
    {
        m_PlayerLevelsDic.Clear();
        m_ShieldGrowthInfoDic.Clear();
        m_IsInitialized = false;
    }

    internal void AddXianH(int v)
    {
        m_PlayerAssetsInfo.XianHInit += v;
        onPlayerAssetsChanged?.Invoke();
    }

    internal void AddStaminaInit(int v)
    {
        m_PlayerAssetsInfo.staminaInit += v;
        onPlayerAssetsChanged?.Invoke();
    }

    internal bool HasEnoughCurrency(int v)
    {
        return m_PlayerAssetsInfo.XianHInit >= v;
    }

    internal void DeductCurrency(int v)
    {
        m_PlayerAssetsInfo.XianHInit -= v;
        onPlayerAssetsChanged?.Invoke();
        SavePlayerAsset();
    }
}
