using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkRequest : BaseRequest
{
	bool connected;
	public bool Connected => connected;     // 是否连接成功(这里的成功表示, 收到第一条初始化消息)

	Player player => (gameFacade.UIMng.GetPanel(UIPanelType.RoomBG) as RoomBG).players[0]; // 本地玩家
	LinkPanel linkPanel;
	protected override void Start() {
		requestCode = RequestCode.None;
		actionCode = ActionCode.Initialization;
		linkPanel = GetComponent<LinkPanel>();
		base.Start();
	}

	/// <summary>
	/// 创建房间前打开服务器
	/// </summary>
	/// <param name="ip"></param>
	/// <param name="port"></param>
	public void OperServer(string ip, int port) {
		StartCoroutine(_OperServer(ip, port));
	}
	IEnumerator _OperServer(string ip, int port) {
		gameFacade.WaitResponse(true);
		try {
			ServerProcess.Instance.OpenServer(ip, port);
		} catch {
			gameFacade.ShowPromot("创建失败!");
			gameFacade.WaitResponse(false);
			yield break;
		}
		// StartCoroutine(_LinkServer("", 1));

		gameFacade.ShowPromot("正在开启服务器, 请稍后...");
		yield return new WaitForSeconds(1);
		yield return new WaitUntil(() => ServerProcess.Instance.Running);

		gameFacade.ShowPromot("服务器已开启!");
		yield return new WaitForSeconds(1);
		// gameFacade.WaitResponse(false);
		LinkServer(ip, port);
	}



	/// <summary>
	/// 加入房间前, 连接服务器
	/// </summary>
	public void LinkServer(string ip, int port) {
		if (connected) return;		// 已经连接不需要重复连接
		StartCoroutine(_LinkServer(ip, port));
	}
	IEnumerator _LinkServer(string ip, int port) {
		connected = false;
		gameFacade.ShowPromot("正在连接服务器...");
		gameFacade.WaitResponse(true);
		// gameFacade.DisconnectServer();			// 连接之前如果有连接，先断开
		yield return new WaitForSeconds(1f);
		try {
			gameFacade.ConnectServer(ip, port);
		} catch {
			gameFacade.ShowPromot("没有网络或输入错误!");
			gameFacade.WaitResponse(false);
			yield break;
		}

		yield return new WaitUntil(() => Response);
		// gameFacade.WaitResponse(false);
		// Debug.Log("aaaaaaaaaaaaaaaaaa");
	}


	/// <summary>
	/// 连接服务器后, 接收到服务器第一条消息, 初始化一些属性
	/// </summary>
	/// <param name="content"></param>
	public override void OnResponse(Content content) {
		base.OnResponse(content);
		// Debug.Log(content.returnCode);
		// Content init_attr = new Content(ReturnCode.Success, ActionCode.Initialization, ContentType.None, SendTo.Single, Id);
		if (content.returnCode == ReturnCode.Success) {

			player.SetId(content.content);              // 设置本机player属性

			connected = true;       // 连接并初始化完成
			Debug.Log("连接成功, 账号id:" + content.content);
			gameFacade.ShowPromot("连接成功, 账号id:" + content.content);

		} else if (content.returnCode == ReturnCode.Fail) {

			gameFacade.ShowPromot("连接失败");
			gameFacade.WaitResponse(false);
		}
	}

}
