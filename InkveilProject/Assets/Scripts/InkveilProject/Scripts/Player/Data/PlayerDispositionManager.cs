using System;
using System.Collections.Generic;
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

    public Action onPlayerAssetsChangde;      

    /// <summary>
    /// ��ʼ����������ϵͳ
    /// </summary>
    public void OnInit()
    {
        if (m_IsInitialized) return;
        InitiaConfig();
    }

    private async void InitiaConfig()
    {
        string playerLevelAsset = (await ResourceService.LoadAsync<TextAsset>(ConfigDefine.playerLevelsInfo)).text;
        string shieldGrowthAsset = (await ResourceService.LoadAsync<TextAsset>(ConfigDefine.shieldGrowthInfo)).text;

        string playerAssetsStr =  PlayerPrefs.GetString("PlayerAssetsInfo");
        if (string.IsNullOrEmpty(playerAssetsStr))
        {
            playerAssetsStr = (await ResourceService.LoadAsync<TextAsset>(ConfigDefine.playerAssets)).text;
            
            PlayerAssetsInfo = JsonHelper.ToObject<List<PlayerAssetsInfo>>(playerAssetsStr)[0];
        }
        else 
        {
            PlayerAssetsInfo = JsonHelper.ToObject<PlayerAssetsInfo>(playerAssetsStr);
        }

        if (playerLevelAsset == null)
        {
            Debug.LogError($"�����ļ�����ʧ�ܣ�{ConfigDefine.playerLevelsInfo}");
            return;
        }

        List<PlayerLevelsInfo> playerLevelList = JsonHelper.ToObject<List<PlayerLevelsInfo>>(playerLevelAsset);
        if (playerLevelList == null || playerLevelList.Count == 0)
        {
            Debug.LogError("�������ݽ���ʧ��" + playerLevelAsset);
            return;
        }

        foreach (var info in playerLevelList)
        {
            // ��������
            m_PlayerLevelsDic[info.playerLevels] = info;
        }

        if (shieldGrowthAsset == null)
        {
            Debug.LogError($"�����ļ�����ʧ�ܣ�{ConfigDefine.shieldGrowthInfo}");
            return;
        }

        List<ShieldGrowthInfo> shieldGrowthList = JsonHelper.ToObject<List<ShieldGrowthInfo>>(shieldGrowthAsset);
        if (shieldGrowthList == null || shieldGrowthList.Count == 0)
        {
            Debug.LogError("�������ݽ���ʧ��");
            return;
        }

        foreach (var info in shieldGrowthList)
        {
            // ��������
            m_ShieldGrowthInfoDic[info.level] = info;
        }

        m_IsInitialized = true;
    }

    public PlayerLevelsInfo GetPlayerLevelInfo(int id)
    {
        if (!m_IsInitialized) return null;
        return m_PlayerLevelsDic.TryGetValue(id, out var PlayerLevelsInfo) ? PlayerLevelsInfo : null;
    }

    public ShieldGrowthInfo GetShieldGrowthInfo(int id)
    {
        if (!m_IsInitialized) return null;
        return m_ShieldGrowthInfoDic.TryGetValue(id, out var ShieldGrowthInfo) ? ShieldGrowthInfo : null;
    }

    public void SavePlayerAsset() 
    {
        string str = JsonHelper.ToJson(PlayerAssetsInfo);
        PlayerPrefs.SetString("PlayerAssetsInfo", str);
        Debug.Log($"����ʲ�:{str}");
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
        PlayerAssetsInfo.XianHInit += v;
        onPlayerAssetsChangde?.Invoke();
    }

    internal void AddStaminaInit(int v)
    {
        PlayerAssetsInfo.staminaInit += v;
        onPlayerAssetsChangde?.Invoke();
    }

    internal void DeductStaminaInit(int v)
    {
        PlayerAssetsInfo.staminaInit -= v;
        onPlayerAssetsChangde?.Invoke();
    }

    internal bool HasEnoughCurrency(int v)
    {
        if (PlayerAssetsInfo.XianHInit < v) HintPopPanelManager.instance.ShowHintPop("�����㣡");
        return PlayerAssetsInfo.XianHInit >= v;
    }

    internal bool HasEnoughStaminaInit(int v)
    {
        if (PlayerAssetsInfo.staminaInit < v) HintPopPanelManager.instance.ShowHintPop("��������5��");
        return PlayerAssetsInfo.staminaInit >= v;
    }

    internal void DeductCurrency(int v)
    {
        BagManager.instance.UseItem(1, v);
        PlayerAssetsInfo.XianHInit -= v;
        onPlayerAssetsChangde?.Invoke();
        SavePlayerAsset();
    }
}
