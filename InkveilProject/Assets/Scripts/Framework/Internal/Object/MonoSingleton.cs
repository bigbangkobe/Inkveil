using UnityEngine;

namespace Framework
{
	/// <summary>
	/// MonoBehaviour单例
	/// </summary>
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
		/// <summary>
		/// 实例句柄
		/// </summary>
		public static T instance
		{
			get
			{
				if (s_Instance == null)
				{
					s_Instance = FindObjectOfType<T>() ?? new GameObject().AddComponent<T>();
				}

				return s_Instance;
			}
		}
		private static T s_Instance;

		protected virtual void Awake()
		{
			gameObject.name = GetType().Name;
			transform.SetParent(Core.instance.transform, false);
			//transform.hideFlags = HideFlags.NotEditable;
		}
	}
}