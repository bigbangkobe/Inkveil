namespace Framework
{
	/// <summary>
	/// 单例
	/// </summary>
	public abstract class Singleton<T> where T : Singleton<T>, new()
	{
		/// <summary>
		/// 实例句柄
		/// </summary>
		public static readonly T instance = new T();
	}
}