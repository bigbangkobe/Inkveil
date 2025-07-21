using Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace StarWarsEve
{
    public class MagicManager : MonoSingleton<MagicManager>
    {
        private readonly List<MagicBase> m_MagicList = new List<MagicBase>();

        /// <summary>
        /// Magic对象池
        /// </summary>
        private readonly Dictionary<string, ObjectPool> m_MagicPoolDic = new Dictionary<string, ObjectPool>();

        /// <summary>
        /// Magic预制体
        /// </summary>
        public Dictionary<string, GameObject> m_MagicPrefabDic = new Dictionary<string, GameObject>();

        public List<MagicBase> MagicList => m_MagicList;

        /// <summary>
        /// 初始化函数
        /// </summary>
        public void OnInit()
        {

        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }

        /// <summary>
        /// 内存释放函数
        /// </summary>
        public void OnClear()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// 从对象池中获取Magic对象
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public async Task<MagicBase> GetMagicByName(string name)
        {
            MagicInfo magicInfo = MagicDispositionManager.instance.GetMagicByName(name);
            ObjectPool MagicPool = GetMagicPool(name);
            MagicBase Magic = await MagicPool.GetAsync(name) as MagicBase;
            Magic.OnInit(magicInfo);
            MagicList.Add(Magic);
            return Magic;
        }

        internal void OnUpdate()
        {
           
        }

        /// <summary>
        /// 从对象池中移除Magic对象
        /// </summary>
        /// <param name="name">对象池ID名称</param>
        /// <param MagicBase="Magic">移除的对象</param>
        public void RemoveMagic(string name, MagicBase Magic)
        {
            ObjectPool MagicPool = GetMagicPool(name);
            MagicList.Remove(Magic);
            MagicPool.Remove(Magic);
        }

        /// <summary>
        /// 获得Magic对象池
        /// </summary>
        /// <param name="name">Magic对象池的名称</param>
        /// <returns></returns>
        private ObjectPool GetMagicPool(string name)
        {
            if (!m_MagicPoolDic.TryGetValue(name, out ObjectPool MagicPool))
            {
                MagicPool = new ObjectPool(OnMagicConstruct, OnMagicDestroy, OnMagicEnabled, OnMagicDisabled);
                m_MagicPoolDic.Add(name, MagicPool);
            }

            return MagicPool;
        }

        /// <summary>
        /// 取消显示Magic
        /// </summary>
        /// <param name="arg1">Magic对象</param>
        /// <param name="arg2">额外参数</param>
        private void OnMagicDisabled(object arg1, object arg2)
        {
            MagicBase Magic = arg1 as MagicBase;
            Magic.gameObject.SetActive(false);
            Magic.transform.SetParent(transform);
        }

        /// <summary>
        /// 显示Magic
        /// </summary>
        /// <param name="arg1">Magic对象</param>
        /// <param name="arg2">额外参数</param>
        private void OnMagicEnabled(object arg1, object arg2)
        {
            MagicBase Magic = arg1 as MagicBase;
            Magic.gameObject.SetActive(true);
        }

        /// <summary>
        /// 摧毁Magic
        /// </summary>
        /// <param name="arg1">Magic对象</param>
        /// <param name="arg2">额外参数</param>
        private void OnMagicDestroy(object arg1, object arg2)
        {
            MagicBase Magic = arg1 as MagicBase;
            Destroy(Magic);
        }

        /// <summary>
        /// Magic生成构造函数
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private async Task<object> OnMagicConstruct(object arg)
        {
            string name = arg.ToString();
            GameObject gameObject;
            MagicInfo Magic = MagicDispositionManager.instance.GetMagicByName(name);

            if (!m_MagicPrefabDic.ContainsKey(name))
            {
                string path = Magic.prefabPath;
                if (!string.IsNullOrEmpty(path))
                {
                    gameObject = await ResourceService.LoadAsync<GameObject>(path);
                    if (gameObject) m_MagicPrefabDic.Add(name, gameObject);
                    else return null;
                }
            }
            gameObject = m_MagicPrefabDic[name];
            if (gameObject == null)
            {
                Debug.LogError("从资源中读取Magic资源失败");
                return null;
            }
            GameObject go = Instantiate(gameObject, transform);
            go.SetActive(false);
            MagicBase magic = go.GetComponent<MagicBase>();
            return Magic;
        }
    }
}