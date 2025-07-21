using Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace StarWarsEve
{
    public class WeaponManager : MonoSingleton<WeaponManager>
    {
        private readonly List<WeaponBase> m_WeaponList = new List<WeaponBase>();

        /// <summary>
        /// Weapon对象池
        /// </summary>
        private readonly Dictionary<string, ObjectPool> m_WeaponPoolDic = new Dictionary<string, ObjectPool>();

        /// <summary>
        /// Weapon预制体
        /// </summary>
        public Dictionary<string, GameObject> m_WeaponPrefabDic = new Dictionary<string, GameObject>();

        public List<WeaponBase> WeaponList => m_WeaponList;

        /// <summary>
        /// 初始化函数
        /// </summary>
        public void OnInit()
        {
            WeaponDispositionManager.instance.OnItin();
        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }

        /// <summary>
        /// 内存释放函数
        /// </summary>
        public void Clear()
        {
            // 遍历列表中的所有武器
            for (int i = m_WeaponList.Count - 1; i >= 0; i--)
            {
                WeaponBase weapon = m_WeaponList[i];
                if (weapon == null) continue;

                RemoveWeapon(weapon.name, weapon);
            }

            m_WeaponList.Clear();

            // 清理所有对象池
            foreach (ObjectPool weaponPool in m_WeaponPoolDic.Values)
            {
                weaponPool.Clear();
            }

            m_WeaponPoolDic.Clear();
        }

        /// <summary>
        /// 从对象池中获取Weapon对象
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public async Task<WeaponBase> GetWeaponByName(string name)
        {
            WeaponInfo weaponInfo = WeaponDispositionManager.instance.GetWeaponByName(name);
            ObjectPool WeaponPool = GetWeaponPool(name);
            WeaponBase Weapon = await WeaponPool.GetAsync(name) as WeaponBase;
            Weapon.OnInit(weaponInfo);
            WeaponList.Add(Weapon);
            return Weapon;
        }

        internal void OnUpdate()
        {
           
        }

        /// <summary>
        /// 从对象池中移除Weapon对象
        /// </summary>
        /// <param name="name">对象池ID名称</param>
        /// <param WeaponBase="Weapon">移除的对象</param>
        public void RemoveWeapon(string name, WeaponBase Weapon)
        {
            ObjectPool WeaponPool = GetWeaponPool(name);
            WeaponList.Remove(Weapon);
            WeaponPool.Remove(Weapon);
        }

        /// <summary>
        /// 获得Weapon对象池
        /// </summary>
        /// <param name="name">Weapon对象池的名称</param>
        /// <returns></returns>
        private ObjectPool GetWeaponPool(string name)
        {
            if (!m_WeaponPoolDic.TryGetValue(name, out ObjectPool WeaponPool))
            {
                WeaponPool = new ObjectPool(OnWeaponConstruct, OnWeaponDestroy, OnWeaponEnabled, OnWeaponDisabled);
                m_WeaponPoolDic.Add(name, WeaponPool);
            }

            return WeaponPool;
        }

        /// <summary>
        /// 取消显示Weapon
        /// </summary>
        /// <param name="arg1">Weapon对象</param>
        /// <param name="arg2">额外参数</param>
        private void OnWeaponDisabled(object arg1, object arg2)
        {
            WeaponBase Weapon = arg1 as WeaponBase;
            Weapon.gameObject.SetActive(false);
            Weapon.transform.SetParent(transform);
        }

        /// <summary>
        /// 显示Weapon
        /// </summary>
        /// <param name="arg1">Weapon对象</param>
        /// <param name="arg2">额外参数</param>
        private void OnWeaponEnabled(object arg1, object arg2)
        {
            WeaponBase Weapon = arg1 as WeaponBase;
            Weapon.gameObject.SetActive(true);
        }

        /// <summary>
        /// 摧毁Weapon
        /// </summary>
        /// <param name="arg1">Weapon对象</param>
        /// <param name="arg2">额外参数</param>
        private void OnWeaponDestroy(object arg1, object arg2)
        {
            WeaponBase Weapon = arg1 as WeaponBase;
            Destroy(Weapon);
        }

        /// <summary>
        /// Weapon生成构造函数
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private async Task<object> OnWeaponConstruct(object arg)
        {
            string name = arg.ToString();
            GameObject gameObject;
            WeaponInfo Weapon = WeaponDispositionManager.instance.GetWeaponByName(name);

            if (!m_WeaponPrefabDic.ContainsKey(name))
            {
                string path = Weapon.prefabPath;
                if (!string.IsNullOrEmpty(path))
                {
                    gameObject = await ResourceService.LoadAsync<GameObject>(path);
                    if (gameObject) m_WeaponPrefabDic.Add(name, gameObject);
                    else return null;
                }
            }
            gameObject = m_WeaponPrefabDic[name];
            if (gameObject == null)
            {
                Debug.LogError("从资源中读取Weapon资源失败");
                return null;
            }
            GameObject go = Instantiate(gameObject, transform);
            go.SetActive(false);
            WeaponBase weapon = go.GetComponent<WeaponBase>();
            return Weapon;
        }
    }
}