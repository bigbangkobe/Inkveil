using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Framework
{
    /// <summary>
    /// 特效系统
    /// </summary>
    public sealed class EffectSystem : MonoSingleton<EffectSystem>,IResLoadListener
	{
		private string LOG_ASSET = "Can't find out effect asset:{0}.";
		private string LOG_GET = "Can't find out effect:{0}.";

		/// <summary>
		/// 资源包名
		/// </summary>
		private string m_BundleName;
		/// <summary>
		/// 特效对象池表 [键:特效名 值:特效对象池]
		/// </summary>
		private Dictionary<string, ObjectPool> m_EffectPoolMap = new Dictionary<string, ObjectPool>();

        /// <summary>
        /// 特效资源字典
        /// </summary>
        private Dictionary<string, GameObject> effectPrefabDic = new Dictionary<string, GameObject>();

        /// <summary>
        /// 初始化加载特效
        /// </summary>
        public void OnInit()
		{
            ResMgr.Instance.Load("VFX_Nature_Flash", this, typeof(GameObject));
            ResMgr.Instance.Load("VFX_Nature_Projectile_Only", this, typeof(GameObject));
            ResMgr.Instance.Load("GroundExplSkill", this, typeof(GameObject));
            ResMgr.Instance.Load("GroundHit", this, typeof(GameObject));
            ResMgr.Instance.Load("Lightning front attack", this, typeof(GameObject));
            ResMgr.Instance.Load("Slash wave green", this, typeof(GameObject));
            ResMgr.Instance.Load("Slash wave green 1", this, typeof(GameObject));
            ResMgr.Instance.Load("Sword Slash 4", this, typeof(GameObject));
            ResMgr.Instance.Load("Sword Slash 5", this, typeof(GameObject));
            ResMgr.Instance.Load("Lightning strike 2", this, typeof(GameObject));
            ResMgr.Instance.Load("Temporary explosion", this, typeof(GameObject));
            ResMgr.Instance.Load("Sword Slash 15", this, typeof(GameObject)); 
            //ResMgr.Instance.Load("ULT Sword", this, typeof(GameObject));
        }

        /// <summary>
        /// 获得特效对象
        /// </summary>
        /// <param name="name">特效名称</param>
        /// <returns></returns>
        public async Task<EffectObject> GetEffect(string name)
        {
			//Debug.Log("获得特效对象:" + name);
            ObjectPool effectPool = GetEffectPool(name);
            EffectObject effectObject = await effectPool.GetAsync(name) as EffectObject;
            return effectObject;
        }

        /// <summary>
        /// 移除特效返回到对象池中
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public void RemoveEffect(EffectObject effectObject)
        {
            ObjectPool effectPool = GetEffectPool(effectObject.name);
            effectPool.Remove(effectObject);
        }

		/// <summary>
		/// 播放特效
		/// </summary>
		/// <param name="name">特效名</param>
		/// <returns>返回特效对象</returns>
		public async Task<EffectObject> Play(string name)
		{
			ObjectPool effectPool = GetEffectPool(name);
			EffectObject effectObject = await effectPool.GetAsync(name) as EffectObject;
			effectObject.Play();

			return effectObject;
		}

		/// <summary>
		/// 停止特效
		/// </summary>
		/// <param name="effectObject">特效对象</param>
		public void Stop(EffectObject effectObject)
		{
			ObjectPool effectPool = GetEffectPool(effectObject.name);
            effectPool.Remove(effectObject);
		}

		/// <summary>
		/// 清理特效
		/// </summary>
		public void Clear()
		{
			foreach (ObjectPool effectPool in instance.m_EffectPoolMap.Values)
			{
				effectPool.Clear();
			}
			instance.m_EffectPoolMap.Clear();
		}

		/// <summary>
		/// 获取资源
		/// </summary>
		/// <param name="name">资源名</param>
		/// <returns>返回资源对象</returns>
		private GameObject GetAsset(string name)
		{
            //GameObject asset = AssetSystem.Load(instance.m_BundleName, name, typeof(GameObject)) as GameObject;
            //if (asset == null)
            //{
            //	Debug.LogError(string.Format(LOG_ASSET, name));
            //}

            GameObject asset = ResMgr.Instance.GetAsset(name).asset as GameObject;
            if (asset == null)
            {
                Debug.LogError(string.Format(LOG_ASSET, name));
            }

            return asset;
		}

		/// <summary>
		/// 获取特效对象池
		/// </summary>
		/// <param name="name">特效名</param>
		/// <returns>返回特效对象池</returns>
		private ObjectPool GetEffectPool(string name)
		{
			ObjectPool effectPool = null;
			if (!instance.m_EffectPoolMap.TryGetValue(name, out effectPool))
			{
				effectPool = new ObjectPool(OnEffectConstruct, OnEffectDestroy, OnEffectEnabled, OnEffectDisabled);
                instance.m_EffectPoolMap.Add(name, effectPool);

            }

			return effectPool;
		}

        /// <summary>
        /// 特效构造回调
        /// </summary>
        /// <returns>返回特效对象</returns>
        private Task<object> OnEffectConstruct(object obj)
        {
            string name = obj as string;

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("特效名为空！");
                return Task.FromResult<object>(null);
            }

            if (!effectPrefabDic.TryGetValue(name, out GameObject gameObject) || gameObject == null)
            {
                Debug.LogError($"从资源中读取特效资源失败：{name}");
                return Task.FromResult<object>(null);
            }

            GameObject go = GameObject.Instantiate(gameObject);
            go.transform.SetParent(transform, false);
            go.SetActive(false);
            go.name = name;

            EffectObject effectObject = new EffectObject(go);
            return Task.FromResult<object>(effectObject);
        }



        /// <summary>
        /// 特效销毁回调
        /// </summary>
        /// <param name="obj">对象</param>
        private void OnEffectDestroy(object obj, object o = null)
		{
			EffectObject effectObject = obj as EffectObject;
			effectObject.Stop();
			Destroy(effectObject.gameObject);
		}

		/// <summary>
		/// 特效开启回调
		/// </summary>
		/// <param name="obj">对象</param>
		private void OnEffectEnabled(object obj, object o = null)
		{
			//EffectObject effectObject = obj as EffectObject;
			//effectObject.gameObject.SetActive(true);
            //effectObject.Play();
        }

		/// <summary>
		/// 特效关闭回调
		/// </summary>
		/// <param name="obj">对象</param>
		private void OnEffectDisabled(object obj, object o = null)
		{
			EffectObject effectObject = obj as EffectObject;
			effectObject.Stop();
			effectObject.transform.SetParent(instance.transform, false);
		}

        public void Finish(object asset, string name)
        {
            Debug.LogFormat("加载{0}特效资源成功", name);
            effectPrefabDic.Add(name, asset as GameObject);
        }

        public void Failure()
        {
            Debug.LogFormat("加载{0}特效资源失败", name);
        }
    }
}