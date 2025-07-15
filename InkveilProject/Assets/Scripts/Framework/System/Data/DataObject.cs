namespace Framework
{
	/// <summary>
	/// 数据对象
	/// </summary>
	public abstract class DataObject
	{
		/// <summary>
		/// 获取数据
		/// </summary>
		/// <param name="key">数据键</param>
		/// <returns>返回数据</returns>
		public virtual object Get(string key)
		{
			return null;
		}
	}
}