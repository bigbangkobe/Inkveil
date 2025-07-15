using UnityEngine;

/// <summary>
/// 客户端配置
/// </summary>
public static class Config
{
	/// <summary>
	/// 资源服务器地址
	/// </summary>
	public const string URL_RESOURCE = "http://ewalletpay.net:8080";

#if UNITY_ANDROID
	public const string PLATFORM = "Android";
#elif UNITY_IPHONE
	public const string PLATFORM = "iOS";
#else
    public const string PLATFORM = "PC";
#endif

#if UNITY_EDITOR
    public static readonly string PATH_ASSET_BUNDLE = System.IO.Directory.GetCurrentDirectory().Replace('\\', '/') + "/AssetBundle/" + PLATFORM;
    public static readonly string URL_ASSET_BUNDLE = "file://" + PATH_ASSET_BUNDLE;
    public static readonly string PATH_LOG = Application.dataPath + "/Log";
    public const string KEYSTORE_NAME = "user.keystore";
    public const string KEYSTORE_PROJECTNAME = "Inkveil";
    public const string KEYSTORE_PASS = "123456";
#else
#if DEBUG || LOCAL_AB
#if UNITY_ANDROID
	public static readonly string URL_ASSET_BUNDLE = Application.streamingAssetsPath + "/AssetBundle/" + PLATFORM;
#else
	public static readonly string URL_ASSET_BUNDLE = "file://" + Application.streamingAssetsPath + "/AssetBundle/" + PLATFORM;
#endif
#else
	public static readonly string URL_ASSET_BUNDLE = URL_RESOURCE + "/AssetBundle/" + PLATFORM;
#endif
	public static readonly string PATH_LOG = Application.persistentDataPath + "/Log";
#endif

    /// <summary>
    /// 缓存目录
    /// </summary>
    public static readonly string PATH_CACHE = Application.persistentDataPath + "/Cache";

    /// <summary>
    /// Lua脚本加密串
    /// </summary>
    public const string LUA_KEY = "BgIAAACkAABSU0ExAAQAAAEAAQCl+x5OJx94kcygsi/MYk4K+5y+NN5dAsiIKsRRYS8oKvm8ngdQKgFxokHQ9B7rsNtnUbZSTAIk59VbC5q8kZDCY/jRoptCX1P/HC5b3egpDHl9aN6E1+kf1WO9aTzMaQXTkfKzeuAoYPbbnDS/Uxjmzia2IRKbYfJMoYF8NsPKwg==";

    /// <summary>
    /// 微信APPID
    /// </summary>
    public const string WECHAT_APP_ID = "wxb1ab806104882755";//"wxbe33e964f07fb3ec";

    /// <summary>
    /// Game Voice APPID
    /// </summary>
    public const string GVOICE_APP_ID = "1029418011";
    /// <summary>
    /// Game Voice APP密钥
    /// </summary>
    public const string GVOICE_APP_SECRET = "4b7f6ec287eb64c34c14fcc5b58aea46";
}