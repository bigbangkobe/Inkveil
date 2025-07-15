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
    /// �첽��ʼ���������ϵͳ
    /// </summary>
    public async Task OnInitAsync()
    {
        if (m_IsInitialized) return;

        try
        {
            // �첽���������ı�
            var levelHandle = ResourceService.LoadAsync<TextAsset>(ConfigDefine.playerLevelsInfo);
            var shieldHandle = ResourceService.LoadAsync<TextAsset>(ConfigDefine.shieldGrowthInfo);
            var assetsHandle = ResourceService.LoadAsync<TextAsset>(ConfigDefine.playerAssets);

            TextAsset levelAsset = await levelHandle.Task;
            TextAsset shieldAsset = await shieldHandle.Task;
            TextAsset assetsAsset = await assetsHandle.Task;

            // ��������ش浵����
            string assetsJson = PlayerPrefs.GetString("PlayerAssetsInfo");
            if (string.IsNullOrEmpty(assetsJson))
            {
                if (assetsAsset == null)
                {
                    Debug.LogError($"����ʲ����ü���ʧ�ܣ�{ConfigDefine.playerAssets}");
                    return;
                }
                assetsJson = assetsAsset.text;
            }
            m_PlayerAssetsInfo = JsonMapper.ToObject<PlayerAssetsInfo>(assetsJson);

            if (levelAsset == null)
            {
                Debug.LogError($"�����ļ�����ʧ�ܣ�{ConfigDefine.playerLevelsInfo}");
                return;
            }
            var levelList = JsonMapper.ToObject<List<PlayerLevelsInfo>>(levelAsset.text)
                             ?? new List<PlayerLevelsInfo>();
            foreach (var info in levelList)
                m_PlayerLevelsDic[info.playerLevels] = info;

            if (shieldAsset == null)
            {
                Debug.LogError($"�����ļ�����ʧ�ܣ�{ConfigDefine.shieldGrowthInfo}");
                return;
            }
            var shieldList = JsonMapper.ToObject<List<ShieldGrowthInfo>>(shieldAsset.text)
                              ?? new List<ShieldGrowthInfo>();
            foreach (var info in shieldList)
                m_ShieldGrowthInfoDic[info.level] = info;

            m_IsInitialized = true;
            Debug.Log("�������ϵͳ��ʼ�����");
        }
        catch (Exception ex)
        {
            Debug.LogError($"������ó�ʼ���쳣��{ex}");
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
        Debug.Log($"����ʲ��ѱ���: {str}");
    }

    /// <summary>
    /// ����ϵͳ��Դ
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
