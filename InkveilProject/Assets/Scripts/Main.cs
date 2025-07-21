using Framework;
using System;
using UnityEngine;

/// <summary>
/// 主入口
/// </summary>
public sealed class Main : MonoSingleton<Main>
{
	private const string FORMAT_LOG = "[{0}] {1}";
	private const string FORMAT_PATH = "{0}/Log_{1}.txt";
	private const string FORMAT_DATE = "yyyyMMdd";
	private const string FORMAT_TIME = "HH:mm:ss.ffff";

	/// <summary>
	/// 是否销毁
	/// </summary>
	public static bool isDestroy { get; private set; }

	protected override void Awake()
	{
		base.Awake();

		isDestroy = false;

		Application.targetFrameRate = 60;
		Application.runInBackground = true;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Input.multiTouchEnabled = false;
#if DEBUG
        DebugSystem.Add(typeof(DebugStats).Name, typeof(DebugStats));
        (DebugSystem.Add(typeof(DebugConsole).Name, typeof(DebugConsole)) as DebugConsole).onInputSubmit += (chunk) =>
        {
            //LuaManager.DoString(chunk);
        };
        DebugSystem.Add(typeof(DebugTools).Name, typeof(DebugTools));

		string path = string.Format(FORMAT_PATH, Config.PATH_LOG, DateTime.Now.ToString(FORMAT_DATE));
        LogSystem.Init(path, (message) =>
        {
            return string.Format(FORMAT_LOG, DateTime.Now.ToString(FORMAT_TIME), message);
        });
        LogSystem.receivedLog = true;
        LogSystem.Log("==========Game Start==========");
#endif

#if REYUN
        Debug.Log("热云SDK初始化");
        // 热云SDK初始化
        ReYunManager.instance.Init();
#endif
#if TALKINGDATA
        Debug.Log("TalkingData初始化");
        // TalkingData初始化
        TalkingDataManager.instance.Init();
#endif
#if FACEBOOK
        Debug.Log("Facebook初始化");
        // Facebook初始化
        FacebookManager.instance.Init();
#endif
#if TOPON
        Debug.Log("Topon初始化");
        // Topon初始化
        ToponManager.instance.Init();
#endif

        //NativeAPI.Init();
        //DownloadSystem.Init(Config.PATH_CACHE);
        //UISystem.Init(RenderMode.ScreenSpaceCamera,
        //	Camera.main,
        //	CanvasScaler.ScaleMode.ScaleWithScreenSize,
        //	new Vector2(1136, 640));
        //LogicManager.Init();
        //Input.multiTouchEnabled = false;
        //ADSManager.instance.InitAdSDK();

       

		//在所有UI初始化完成之后调用
		//if (ADSManager.instance.WaitingRewardCallback != null)
		//	ADSManager.instance.WaitingRewardCallback.Invoke();
    }

    public void OnInit() 
    {
        gameObject.AddComponent<ResMgr>();
    }

    private void Update()
	{

	}

	private void FixedUpdate()
	{
		
	}

	private void OnDestroy()
	{
		isDestroy = true;

		LogSystem.Log("==========Game Exit==========\n");
        
	}

#region 切换后台接口
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Debug.Log("OnApplicationPause");
        }
    }

    //游戏失去焦点也就是进入后台时 focus为false 切换回前台时 focus为true
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            //Debug.Log("切换到后台");
            //ReordOfflineTime();
        }
        else
        {
            //Debug.Log("切换回游戏");
            //ReordOfflineTime();
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
    }
#endregion
}