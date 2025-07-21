using UnityEngine;
using System;
using System.Collections.Generic;
using Framework;

/// <summary>
/// 网络调试工具
/// </summary>
public sealed class DebugNetwork : DebugObject
{
	private const string TITLE = "Network";
	private const string FORMAT_NAME = "{0}\t({1})";
	private const string FORMAT_FIELD = "({0}) {1}";
	private const string FORMAT_ARRAY = "[{0}]";
	private const string GUI_BOX = "Box";
	private const string GUI_RELOAD = "Reload";
	private const string GUI_RETURN = "Return";
	private const string GUI_RECEIVE = "Receive";
	private const string GUI_SEND = "Send";

	public override string title { get { return TITLE; } }

	private NetworkMessage[] m_AllMessages;
	private NetworkMessage[] m_SearchMessages;
	private NetworkMessage m_EditMessage;
	private Vector2 m_ListPosition;
	private Vector2 m_MessagePosition;
	private string m_SearchText;

	public override void OnGUI()
	{
		base.OnGUI();

		if (m_AllMessages == null)
		{
			List<NetworkMessage> messageList = new List<NetworkMessage>();
			NetworkServer[] networkServers = NetworkSystem.GetAll();
			if (networkServers != null)
			{
				for (int i = 0; i < networkServers.Length; ++i)
				{
					NetworkServer networkServer = networkServers[i];
					messageList.AddRange(networkServer.GetAllMessage());
				}
				m_AllMessages = messageList.ToArray();
			}

			Reload();
		}

		if (m_EditMessage == null)
		{
			OnDrawMessages();
		}
		else
		{
			OnDrawEditMessage();
		}
	}

	private void OnDrawMessages()
	{
		GUILayout.BeginHorizontal(GUI_BOX);

		string text = GUILayout.TextField(m_SearchText);
		if (text != m_SearchText)
		{
			m_SearchText = text;
			List<NetworkMessage> messageList = new List<NetworkMessage>();
			for (int i = 0; i < m_AllMessages.Length; ++i)
			{
				NetworkMessage networkMessage = m_AllMessages[i];
				string name = string.Format(FORMAT_NAME, networkMessage.id, networkMessage.name);
				if (name.ToLower().Contains(m_SearchText.ToLower()))
				{
					messageList.Add(networkMessage);
				}
			}

			m_SearchMessages = messageList.ToArray();
		}

		if (GUILayout.Button(GUI_RELOAD, GUILayout.Width(Screen.width * 0.2f)))
		{
			Reload();
		}

		GUILayout.EndHorizontal();

		m_ListPosition = GUILayout.BeginScrollView(m_ListPosition, GUI_BOX);
		GUI.skin.button.alignment = TextAnchor.MiddleLeft;

		if (m_SearchMessages != null)
		{
			for (int i = 0; i < m_SearchMessages.Length; ++i)
			{
				NetworkMessage networkMessage = m_SearchMessages[i];
				string name = string.Format(FORMAT_NAME, networkMessage.id, networkMessage.name);
				if (GUILayout.Button(name))
				{
					m_EditMessage = networkMessage;
					m_MessagePosition = Vector2.zero;

					break;
				}
			}
		}

		GUI.skin.button.alignment = TextAnchor.MiddleCenter;
		GUILayout.EndScrollView();
	}

	private void OnDrawEditMessage()
	{
		GUILayout.BeginHorizontal(GUI_BOX);
		if (GUILayout.Button(GUI_RETURN))
		{
			m_EditMessage = null;
		}
		else if (GUILayout.Button(GUI_RECEIVE))
		{
			m_EditMessage.server.Receive(m_EditMessage);
		}
		else if (GUILayout.Button(GUI_SEND))
		{
			m_EditMessage.server.Send(m_EditMessage);
		}
		GUILayout.EndHorizontal();

		if (m_EditMessage == null)
		{
			return;
		}

		GUILayout.BeginHorizontal(GUI_BOX);
		GUILayout.Label(m_EditMessage.ToString());
		GUILayout.EndHorizontal();

		m_MessagePosition = GUILayout.BeginScrollView(m_MessagePosition);
		GUILayoutOption option = GUILayout.Width(Screen.width * 0.25f);
		GUILayout.EndScrollView();
	}

	private void Reload()
	{
		m_SearchText = string.Empty;
		m_ListPosition = Vector2.zero;
		m_SearchMessages = m_AllMessages;
		m_EditMessage = null;
	}
}