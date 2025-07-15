using UnityEngine;

[SerializeField]
public class LeeGameAppConfig : ScriptableObject
{
	/// <summary>
	/// stringXml路径
	/// </summary>
	public string stringXmlPath;

	/// <summary>
	/// 
	/// </summary>
	public string FaceBookAppID;

	/// <summary>
	/// 
	/// </summary>
	public string TalkingDataAppID = "";

	/// <summary>
	/// 渠道
	/// </summary>
	public string TalkingDataChannel;

	/// <summary>
	/// 是否过审核
	/// </summary>
	public bool isAuditing = false;

	/// <summary>
	/// 当前游戏场景
	/// </summary>
	public string atGameSceneName = string.Empty;

	/// <summary>
	/// 是否开启广告
	/// </summary>
	public bool OpenADS = false;

	/// <summary>
	/// TopAppID
	/// </summary>
	public string TopAppID;

	/// <summary>
	/// TopAppKey
	/// </summary>
	public string TopAppKey;

	/// <summary>
	/// 插屏ID
	/// </summary>
	public string InterstitialAdID = "";

	/// <summary>
	/// 激励ID
	/// </summary>
	public string RewardAdID = "";

	/// <summary>
	/// 横幅ID
	/// </summary>
	public string BannerAdID = "";

	/// <summary>
	/// 横幅类型
	/// </summary>
	public BannerType bannerType;

	#region Adsforce

	public string devKey = "";

	public string publicKey = "";

	public string trackUrl = "";

	public string channelId = "";

	public string appId = "";

	#endregion

	[System.Serializable]
	public enum BannerType
	{
		CLOSE,
		TOP,
		BOTTOM
	}
}
