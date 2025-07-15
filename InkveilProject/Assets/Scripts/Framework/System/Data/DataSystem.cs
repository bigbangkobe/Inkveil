using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
	/// <summary>
	/// 数据系统
	/// </summary>
	public sealed class DataSystem : Singleton<DataSystem>
	{
		private const string LOG_ASSET = "Can't find out data asset:{0}.";
		private const string LOG_GET = "Can't find out {0} data:{1}={2}({3}).";

		/// <summary>
		/// 资源包名
		/// </summary>
		private string m_BundleName;
		/// <summary>
		/// 数据解析回调
		/// </summary>
		private Func<string, DataObject[]> m_DataCallback;
		/// <summary>
		/// 数据库表 [键:数据库类型 值:数据库]
		/// </summary>
		private Dictionary<string, Database> m_DatabaseMap = new Dictionary<string, Database>();

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="bundleName">资源包名</param>
		/// <param name="dataCallback">数据解析回调</param>
		public static void Init(string bundleName, Func<string, DataObject[]> dataCallback)
		{
			instance.m_BundleName = bundleName;
			instance.m_DataCallback = dataCallback;
		}

		/// <summary>
		/// 获取数据
		/// </summary>
		/// <param name="key">数据键</param>
		/// <param name="value">数据值</param>
		/// <returns>返回数据</returns>
		public static DataObject GetData(string name, string key, object value)
		{
			Database database = GetDatabase(name);
			DataObject data = database.GetData(key, value);
			if (data == null)
			{
				Debug.Log(string.Format(LOG_GET, name, key, value, value.GetType()));
			}

			return data;
		}

		/// <summary>
		/// 获取数据
		/// </summary>
		/// <param name="key">数据键</param>
		/// <param name="value">数据值</param>
		/// <returns>返回数据</returns>
		public static DataObject[] GetDatas(string name, string key, object value)
		{
			Database database = GetDatabase(name);
			DataObject[] datas = database.GetDatas(key, value);
			if (datas.Length <= 0)
			{
				Debug.Log(string.Format(LOG_GET, name, key, value, value.GetType()));
			}

			return datas;
		}

		/// <summary>
		/// 获取所有数据
		/// </summary>
		/// <param name="name">资源名</param>
		/// <returns>返回所有数据</returns>
		public static DataObject[] GetAllDatas(string name)
		{
			Database database = GetDatabase(name);

			return database.GetAllDatas();
		}

		/// <summary>
		/// 获取资源
		/// </summary>
		/// <param name="name">资源名</param>
		/// <returns>返回资源对象</returns>
		private static TextAsset GetAsset(string name)
		{
#if UNITY_EDITOR
			if (instance.m_BundleName == null)
			{
                //这里没有热更就不需要这个代码
				//LuaManager.Init(ResourceDefine.BUNDLE_SCRIPT);
				//Init(ResourceDefine.BUNDLE_CONFIG, LuaData.DataCallback);
			}
#endif

			TextAsset asset = AssetSystem.Load(instance.m_BundleName, name, typeof(TextAsset)) as TextAsset;
			if (asset == null)
			{
				Debug.LogError(string.Format(LOG_ASSET, name));
			}

			return asset;
		}

		/// <summary>
		/// 获取数据库
		/// </summary>
		/// <param name="name">资源名</param>
		/// <returns>返回数据库</returns>
		private static Database GetDatabase(string name)
		{
			Database database = null;
			if (instance.m_DatabaseMap.TryGetValue(name, out database))
			{
				return database;
			}

			TextAsset textAsset = GetAsset(name);
			database = new Database(textAsset.text);
			instance.m_DatabaseMap[name] = database;

			return database;
		}

		/// <summary>
		/// 数据库类
		/// </summary>
		private sealed class Database
		{
			/// <summary>
			/// 数据数组
			/// </summary>
			private DataObject[] m_Datas;
			/// <summary>
			/// 数据表 [键:字段名 值:[键:字段值 值:数据链表]]
			/// </summary>
			private Dictionary<string, Dictionary<object, List<DataObject>>> m_DataMap = new Dictionary<string, Dictionary<object, List<DataObject>>>();

			/// <summary>
			/// 构造函数
			/// </summary>
			/// <param name="content">数据内容</param>
			public Database(string content)
			{
				m_Datas = instance.m_DataCallback(content);
			}

			/// <summary>
			/// 获取数据
			/// </summary>
			/// <param name="key">数据键</param>
			/// <param name="value">数据值</param>
			/// <returns>返回数据</returns>
			public DataObject GetData(string key, object value)
			{
				DataObject[] datas = GetDatas(key, value);

				return datas == null || datas.Length <= 0 ? null : datas[0];
			}

			/// <summary>
			/// 获取数据
			/// </summary>
			/// <param name="key">数据键</param>
			/// <param name="value">数据值</param>
			/// <returns>返回数据</returns>
			public DataObject[] GetDatas(string key, object value)
			{
				Dictionary<object, List<DataObject>> dataMap = null;
				if (!m_DataMap.TryGetValue(key, out dataMap))
				{
					dataMap = new Dictionary<object, List<DataObject>>();
					m_DataMap[key] = dataMap;
					for (int i = 0; i < m_Datas.Length; ++i)
					{
						DataObject data = m_Datas[i];
						object dataValue = data.Get(key);

						List<DataObject> list = null;
						if (!dataMap.TryGetValue(dataValue, out list))
						{
							list = new List<DataObject>();
							dataMap[dataValue] = list;
						}
						list.Add(data);
					}
				}

				List<DataObject> dataList = null;
				dataMap.TryGetValue(value, out dataList);

				return dataList == null ? null : dataList.ToArray();
			}

			/// <summary>
			/// 获取所有数据
			/// </summary>
			/// <returns>返回所有数据</returns>
			public DataObject[] GetAllDatas()
			{
				return m_Datas;
			}
		}
	}
}