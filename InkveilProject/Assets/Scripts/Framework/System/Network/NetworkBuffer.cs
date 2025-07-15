using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
	/// <summary>
	/// 网络缓存
	/// </summary>
	public sealed class NetworkBuffer
	{
		private const string FORMAT_NAME = "{{{0}}}";
		private const string FORMAT_DATA = "{0}, {1}";
		private static readonly Type TYPE_SIZE = typeof(short);

		/// <summary>
		/// 当前长度
		/// </summary>
		public int length { get { return m_BufferList.Count; } }
		/// <summary>
		/// 获取字节数组
		/// </summary>
		public byte[] bytes { get { return m_BufferList.ToArray(); } }

		/// <summary>
		/// 缓存链表
		/// </summary>
		private List<byte> m_BufferList = new List<byte>();
		/// <summary>
		/// 当前索引值
		/// </summary>
		private int m_Index;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="buffer">缓存</param>
		public void Init(byte[] buffer = null)
		{
			Clear();

			if (buffer != null)
			{
				m_BufferList.AddRange(buffer);
			}
		}

		/// <summary>
		/// 清理
		/// </summary>
		public void Clear()
		{
			m_BufferList.Clear();
			m_Index = 0;
		}

		/// <summary>
		/// 压入数据
		/// </summary>
		/// <param name="value">数据</param>
		/// <param name="type">数据类型</param>
		public void Push(object value, Type type = null)
		{
			type = type ?? value.GetType();

			if (type.IsArray)
			{
				Type elementType = type.GetElementType();
				Array array = (Array)value;
				PushLength(array.Length);
				for (int i = 0; i < array.Length; ++i)
				{
					Push(array.GetValue(i), elementType);
				}
			}
			else
			{
				if (type == typeof(bool))
				{
					Add(BitConverter.GetBytes(Convert.ToBoolean(value)));
				}
				else if (type == typeof(char))
				{
					Add(BitConverter.GetBytes(Convert.ToChar(value)));
				}
				else if (type == typeof(sbyte))
				{
					Add(new byte[] { Convert.ToByte(Convert.ToSByte(value)) });
				}
				else if (type == typeof(byte))
				{
					Add(new byte[] { Convert.ToByte(value) });
				}
				else if (type == typeof(short))
				{
					Add(BitConverter.GetBytes(Convert.ToInt16(value)));
				}
				else if (type == typeof(ushort))
				{
					Add(BitConverter.GetBytes(Convert.ToUInt16(value)));
				}
				else if (type == typeof(int))
				{
					Add(BitConverter.GetBytes(Convert.ToInt32(value)));
				}
				else if (type == typeof(uint))
				{
					Add(BitConverter.GetBytes(Convert.ToUInt32(value)));
				}
				else if (type == typeof(long))
				{
					Add(BitConverter.GetBytes(Convert.ToInt64(value)));
				}
				else if (type == typeof(ulong))
				{
					Add(BitConverter.GetBytes(Convert.ToUInt64(value)));
				}
				else if (type == typeof(float))
				{
					Add(BitConverter.GetBytes(Convert.ToSingle(value)));
				}
				else if (type == typeof(double))
				{
					Add(BitConverter.GetBytes(Convert.ToDouble(value)));
				}
				else if (type == typeof(string))
				{
					byte[] bytes = Encoding.UTF8.GetBytes((string)value);
					PushLength(bytes.Length);
					for (int i = 0; i < bytes.Length; ++i)
					{
						Add(new byte[] { bytes[i] });
					}
				}
			}
		}

		/// <summary>
		/// 弹出数据
		/// </summary>
		/// <param name="type">数据类型</param>
		/// <returns>返回数据</returns>
		public object Pop(Type type)
		{
			if (type.IsArray)
			{
				Type elementType = type.GetElementType();
				int length = PopLength();
				Array array = Array.CreateInstance(elementType, length);
				for (int i = 0; i < array.Length; ++i)
				{
					object value = Pop(elementType);
					array.SetValue(value, i);
				}

				return array;
			}
			else
			{
				if (type == typeof(bool))
				{
					return BitConverter.ToBoolean(Get(sizeof(bool)), 0);
				}
				else if (type == typeof(char))
				{
					return BitConverter.ToChar(Get(sizeof(char)), 0);
				}
				else if (type == typeof(sbyte))
				{
					return Convert.ToSByte(Get(sizeof(sbyte))[0]);
				}
				else if (type == typeof(byte))
				{
					return Get(sizeof(byte))[0];
				}
				else if (type == typeof(short))
				{
					return BitConverter.ToInt16(Get(sizeof(short)), 0);
				}
				else if (type == typeof(ushort))
				{
					return BitConverter.ToUInt16(Get(sizeof(ushort)), 0);
				}
				else if (type == typeof(int))
				{
					return BitConverter.ToInt32(Get(sizeof(int)), 0);
				}
				else if (type == typeof(uint))
				{
					return BitConverter.ToUInt32(Get(sizeof(uint)), 0);
				}
				else if (type == typeof(long))
				{
					return BitConverter.ToInt64(Get(sizeof(long)), 0);
				}
				else if (type == typeof(ulong))
				{
					return BitConverter.ToUInt64(Get(sizeof(ulong)), 0);
				}
				else if (type == typeof(float))
				{
					return BitConverter.ToSingle(Get(sizeof(float)), 0);
				}
				else if (type == typeof(double))
				{
					return BitConverter.ToDouble(Get(sizeof(double)), 0);
				}
				else if (type == typeof(string))
				{
					int length = PopLength();
					byte[] bytes = new byte[length];
					for (int i = 0; i < length; ++i)
					{
						bytes[i] = Get(1)[0];
					}

					return Encoding.UTF8.GetString(bytes);
				}
			}

			return null;
		}

		/// <summary>
		/// 压入长度
		/// </summary>
		/// <param name="length">长度</param>
		public void PushLength(int length)
		{
			Push(Convert.ChangeType(length, TYPE_SIZE));
		}

		/// <summary>
		/// 弹出长度
		/// </summary>
		/// <returns>返回长度</returns>
		public int PopLength()
		{
			return (int)Convert.ChangeType(Pop(TYPE_SIZE), typeof(int));
		}

		/// <summary>
		/// 添加数据
		/// </summary>
		/// <param name="value">数据</param>
		private void Add(byte[] value)
		{
			Array.Reverse(value);
			m_BufferList.AddRange(value);
		}

		/// <summary>
		/// 获取数据
		/// </summary>
		/// <param name="length">长度</param>
		/// <returns>返回数据</returns>
		private byte[] Get(int length)
		{
			byte[] bytes = new byte[length];
			m_BufferList.CopyTo(m_Index, bytes, 0, length);
			Array.Reverse(bytes);

			m_Index += length;

			return bytes;
		}

		/// <summary>
		/// 转换字符串
		/// </summary>
		/// <returns>返回字符串</returns>
		public override string ToString()
		{
            return "";// Extension.ToString(bytes);
		}
	}
}