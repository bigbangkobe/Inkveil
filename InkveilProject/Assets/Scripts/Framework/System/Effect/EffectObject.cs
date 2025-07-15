using DG.Tweening;
using TMPro;
using UnityEngine;
//using UnityEngine.UI;

namespace Framework
{
	/// <summary>
	/// 特效对象
	/// </summary>
	public sealed class EffectObject
	{
		/// <summary>
		/// 特效名
		/// </summary>
		public string name
		{
			get { return gameObject.name; }
			set { gameObject.name = value; }
		}
		/// <summary>
		/// 游戏物体
		/// </summary>
		public GameObject gameObject { get; private set; }
		/// <summary>
		/// 世界变换
		/// </summary>
		public Transform transform { get { return gameObject.transform; } }

        /// <summary>
		/// 粒子组件
		/// </summary>
		public ParticleSystem particleSystem { get; private set; }

        /// <summary>
		/// 文本组件
		/// </summary>
		public TextMeshPro textMeshPro { get; private set; }

        /// <summary>
        /// 文本颜色
        /// </summary>
        public Color textColor { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        public EffectObject(GameObject gameObject)
		{
			this.gameObject = gameObject;
            this.particleSystem = gameObject.GetComponent<ParticleSystem>();
            this.textMeshPro = gameObject.GetComponent<TextMeshPro>();
            if(this.textMeshPro != null)
            {
                textColor = this.textMeshPro.color;
            }
        }

		/// <summary>
		/// 播放特效
		/// </summary>
		public void Play()
		{
			gameObject.SetActive(true);
            this.particleSystem.Play();
        }

		/// <summary>
		/// 停止特效
		/// </summary>
		public void Stop()
		{
            if(this.particleSystem != null)
            {
                this.particleSystem.Stop();
            }
            if(gameObject != null)
            {
                gameObject.SetActive(false);
            }
        }


        public void ShowText(string str, Transform targetTran)
        {
            EffectObject effectObject = this;
            if (textMeshPro == null || gameObject == null)
                return;
            textMeshPro.color = textColor;
            textMeshPro.text = str;
            transform.position = targetTran.position;
            transform.localScale = new Vector3(2, 2, 2);
            transform.rotation = Camera.main.transform.rotation;
            gameObject.SetActive(true);
            textMeshPro.DOFade(0f, 1f);
            transform.DOScale(Vector3.one, 1f);
            transform.DOMoveY(transform.position.y + 10, 1f)
                .OnComplete(() =>
                {
                    EffectSystem.instance.RemoveEffect(effectObject);
                });
        }
	}
}