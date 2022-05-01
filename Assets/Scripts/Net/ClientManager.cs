using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using Common;
using Newtonsoft.Json;

public class ClientManager : BaseManager
{
	private const string ipstr = "127.0.0.1";
	private const int port = 9876;


	Socket cliSocket;
	Message recv;
	public ClientManager(GameFacade gameFacade) : base(gameFacade) { }

	public override void OnInit() {
		cliSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		// recv = new Message(1024);       // 接收消息前, 创建一个接收消息的容器

		// Start();
	}


	/// <summary>
	/// 客户端连接服务器
	/// </summary>
	public void ConnectServer(string ip, int port) {
		Debug.Log(ip + ":" + port);
		try {
			recv = new Message(1024);       // 接收消息前, 创建一个接收消息的容器
			cliSocket.Connect(ip, port);
			cliSocket.BeginReceive(recv.Data, 0, recv.Length, SocketFlags.None, ReceiveCallback, null);

		} catch (System.Exception e) {
			gameFacade.ShowPromot("连接失败");
			gameFacade.WaitResponse(false);
			Debug.Log(e.Message);
		}
	}

	/// <summary>
	/// 客户端断开连接
	/// </summary>
	public void DisconnectServer() {
		if (cliSocket != null) {
			cliSocket.Close();
			cliSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}
	}


	public override void OnDestroy() {
		cliSocket.Close();
	}

	/// <summary>
	/// 接收消息的回调函数
	/// </summary>
	/// <param name="ar"></param>
	private void ReceiveCallback(IAsyncResult ar) {
		try {

			int size = cliSocket.EndReceive(ar);    // 字节数组有效内容大小

			// Debug.Log(size);
			string ss = "";
			for (int i = 0; i < size; i++) {
				ss += recv.Data[i];
			}
			// Debug.Log(ss);
			string[] msgs = recv.GetMessageStrings(0, size);
			foreach (string msg in msgs) {
				Debug.Log(msg);
				Content content = GetReceiveMessage(msg);       // 收到服务器发送过来的响应
				Debug.Log("1111111111111111111111111111");
				gameFacade.HandleResponse(content);
				Debug.Log("22222222222222222222222");
			}

		} catch (Exception e) {
			Debug.LogWarning(e);
		}

		recv = new Message(1024);
		cliSocket.BeginReceive(recv.Data, 0, recv.Length, SocketFlags.None, ReceiveCallback, recv);
	}


	/// <summary>
	/// 客户端接收到服务器消息(响应)
	/// </summary>
	private Content GetReceiveMessage(string msg) {
		// T t = JsonUtility.FromJson<T>(msg);
		return JsonConvert.DeserializeObject<Content>(msg);    // 收到服务器发送过来的响应
	}



	/// <summary>
	/// 客户端发送消息给服务器(请求)
	/// </summary>
	/// <param name="content"></param>
	public void SendMessage(Content content) {
		// string msg = JsonUtility.ToJson(content);
		try {
			string msg = JsonConvert.SerializeObject(content);
			Message send = new Message(msg);
			Debug.Log(send.GetMessageString());
			cliSocket.Send(send.Data);

		} catch (System.Exception) {

			gameFacade.ShowPromot("发送错误");
		}

	}
}