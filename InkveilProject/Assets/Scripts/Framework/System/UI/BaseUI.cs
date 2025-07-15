using UnityEngine;
using DG.Tweening;
using System;

namespace Framework
{
    /// <summary>
    /// 基础UI类
    /// </summary>
    public class BaseUI : MonoBehaviour
    {
        protected long updateTime;

        /// <summary>
        /// 当前界面名称
        /// </summary>
        [HideInInspector]
        public string UIName;

        private Transform mTransform;
        public Transform CacheTransform
        {
            get
            {
                if (mTransform == null) mTransform = this.transform;
                return mTransform;
            }
        }

        private GameObject mGo;
        public GameObject CacheGameObject
        {
            get
            {
                if (mGo == null) mGo = this.gameObject;
                return mGo;
            }
        }

        private void OnEnable()
        {
            OnShowEnable();
        }

        private void OnDisable()
        {
            OnHideDisable();
        }

        protected virtual void OnShowEnable()
        {
  
        }

        protected virtual void OnHideDisable()
        {
  
        }

        /// <summary>
        /// 显示当前UI
        /// </summary>
        /// <param name="param">附加参数</param>
        public void Show(object param = null)
        {
            OnShow(param);
        }

        /// <summary>
        /// 隐藏当前界面
        /// </summary>
        public void Hide()
        {
            OnHide();
        }

        /// <summary>
        /// 绑定脚本并且激活游戏物体会调用的方法
        /// </summary>
        void Awake()
        {
            OnAwake();
        }

        /// <summary>
        /// 初始化UI主要用于寻找组件等
        /// </summary>
        public void UIInit()
        {
            OnInit();
        }

        /// <summary>
        /// 显示当前界面
        /// </summary>
        /// <param name="param">附加参数</param>
        protected virtual void OnShow(object param)
        {
            transform.localScale = Vector3.zero;
            CacheGameObject.SetActive(true);
            transform.DOScale(1f, 0.5f)
                .SetEase(Ease.OutBack);
        }

        /// <summary>
        /// 隐藏当前界面
        /// </summary>
        protected virtual void OnHide()
        {
            CacheGameObject.SetActive(false);
        }

        /// <summary>
        /// 初始化当前界面
        /// </summary>
        protected virtual void OnInit()
        {

        }

        protected virtual void OnAwake() { }

        /// <summary>
        /// 删除当前UI 
        /// </summary>
        protected virtual void OnDestroy() { }


        /// <summary>
        /// 更新函数
        /// </summary>
        public virtual void OnUpdate()
        {

        }
        /// <summary>
        /// 每日重置
        /// </summary>
        /// <param name="day">周几重置，-1表示每天，0-6表示周日到周六</param>
        protected virtual void OnDayReset(object obj)
        {
            Debug.Log("每日重置");
        }

        /// <summary>
        /// 每周重置
        /// </summary>
        protected virtual void OnWeekReset(object obj)
        {
            Debug.Log("每周重置");
        }

        /// <summary>
        /// 每月重置
        /// </summary>
        protected virtual void OnMonthReset(object obj)
        {
            Debug.Log("每月重置");
        }
    }

}
