using Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GPSManager : MonoSingleton<GPSManager>
{
	/// <summary>
	/// 纬度
	/// </summary>
	private static float latitude;

	/// <summary>
	/// 经度
	/// </summary>
	private static float longitude;

	/// <summary>
	/// 获取纬度
	/// </summary>
	/// <returns></returns>
	public static float OnGetLatitude()
	{
		return latitude;
	}

	/// <summary>
	/// 获取经度
	/// </summary>
	/// <returns></returns>
	public static float OnGetLongitude()
	{
		return longitude;
	}

	/// <summary>
	/// 开始启动定位并获取数据（初始化调用）
	/// </summary>
	public static void OnUpdateGPS()
	{
        Core.instance.StartCoroutine(StartGPS());
	}

	/// <summary>
	/// 停止刷新定位（节省手机电量）
	/// </summary>
	public static void OnStopGPS()
	{
		Input.location.Stop();
	}

	private static IEnumerator StartGPS()
	{
		// Input.location 用于访问设备的位置属性（手持设备）, 静态的LocationService位置  
		// LocationService.isEnabledByUser 用户设置里的定位服务是否启用  
		if (!Input.location.isEnabledByUser)
		{
			Debug.LogError("用户未开启位置权限");
			yield break;
		}

		// LocationService.Start() 启动位置服务的更新,最后一个位置坐标会被使用  
		//Input.location.Start(10.0f, 10.0f);
		Input.location.Start();
		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
		{
			// 暂停协同程序的执行(1秒)  
			yield return new WaitForSeconds(1);
			maxWait--;
		}

		if (maxWait < 1)
		{
			Debug.LogError("定位超时");
			yield break;
		}

		if (Input.location.status == LocationServiceStatus.Failed)
		{
			Debug.LogError("定位失败");
			yield break;
		}
		else if (Input.location.status != LocationServiceStatus.Stopped)
		{
			latitude = Input.location.lastData.latitude;
			longitude = Input.location.lastData.longitude;
			Input.location.Stop();
			yield return new WaitForSeconds(20);
		}
	}
}
