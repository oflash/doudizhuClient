using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.UI;
using System;
using Newtonsoft.Json;

public class GameFacade : MonoBehaviour
{
	#region 单例
	private static GameFacade _instance;
	private void Awake() => _instance = this;
	public static GameFacade Instance => _instance;
	#endregion

	public string Id => playerMng.players.Count > 0 ? playerMng.players[0].Id : "None";         // 账号id
	public bool Mute { get => audioMng.mute; set => audioMng.mute = value; }                    // 静音开启?
	public bool Loaded => uiMng != null && audioMng != null && uiMng.Loaded && audioMng.Loaded; // 资源加载完成?

	Queue<bool> waitResponse;


	#region AllManager
	private UIManager uiMng;
	public UIManager UIMng => uiMng;
	// private AniManager aniMng;
	private AudioManager audioMng;
	private CameraManager cameraMng;
	private PlayerManager playerMng;
	public PlayerManager PlayerMng => playerMng;
	private ResponseManager responseMng;
	public ResponseManager ResponseMng => responseMng;
	private ClientManager clientMng;
	// private PoolManager poolMng;

	private IEnumerator ManagerInit() {
		uiMng = new UIManager(this);
		// aniMng = new AniManager(this);
		audioMng = new AudioManager(this);
		cameraMng = new CameraManager(this);
		playerMng = new PlayerManager(this);
		responseMng = new ResponseManager(this);
		clientMng = new ClientManager(this);
		// poolMng = new PoolManager(this);

		uiMng.OnInit();
		yield return new WaitForSeconds(1f);
		ShowPromot("正在加载UI资源...");
		yield return new WaitForSeconds(0.5f);
		yield return new WaitUntil(() => uiMng.Loaded);     // 等待加载完成
		ShowPromot("UI加载完成!");
		yield return new WaitForSeconds(0.5f);

		ShowPromot("正在加载音频资源...");
		yield return new WaitForSeconds(0.5f);
		audioMng.OnInit();
		yield return new WaitUntil(() => audioMng.Loaded);  // 等待加载完成
		ShowPromot("音频加载完成!");
		yield return new WaitForSeconds(0.5f);

		cameraMng.OnInit();
		yield return null;
		playerMng.OnInit();
		yield return null;
		responseMng.OnInit();
		yield return null;
		clientMng.OnInit();
		yield return null;
		// poolMng.OnInit();
	}
	private void ManagerDestroy() {
		uiMng.OnDestroy();
		// aniMng.OnDestroy();
		audioMng.OnDestroy();
		cameraMng.OnDestroy();
		playerMng.OnDestroy();
		responseMng.OnDestroy();
		clientMng.OnDestroy();
		// poolMng.OnDestroy();
	}
	#endregion


	#region 生命周期函数
	private void Start() {
		StartCoroutine(ManagerInit());
		waitResponse = new Queue<bool>();
		StartCoroutine(IWaitResponse());
		// string s = "{\"requestCode\": 0,\"actionCode\":1 ,\"contentType\":0,\"returnCode\":0,\"id\":null,\"content\":\"client29\",\"sendTo\":3}";
		// Debug.Log(s);
		// Content c = JsonConvert.DeserializeObject<Content>(s);
		// Debug.Log(c);

	}

	private void Update() {

	}

	private void OnDestroy() {
		ManagerDestroy();
	}

	#endregion


	/// <summary>
	/// 添加请求
	/// </summary>
	/// <param name="actionCode"></param>
	/// <param name="baseRequest"></param>
	public void AddAction(ActionCode actionCode, BaseRequest baseRequest) {
		responseMng.AddRequest(actionCode, baseRequest);
	}

	/// <summary>
	/// 移除请求
	/// </summary>
	/// <param name="actionCode"></param>
	public void RemoveAction(ActionCode actionCode) {
		responseMng.RemoveRequest(actionCode);
	}

	/// <summary>
	/// 处理响应
	/// </summary>
	/// <param name="content"></param>
	public void HandleResponse(Content content) {
		responseMng.HandleResponse(content);
	}

	/// <summary>
	/// 连接服务器
	/// </summary>
	/// <param name="ip"></param>
	/// <param name="port"></param>
	public void ConnectServer(string ip, int port) {
		clientMng.ConnectServer(ip, port);
	}

	/// <summary>
	/// 断开服务器
	/// </summary>
	public void DisconnectServer() {
		clientMng.DisconnectServer();
	}

	/// <summary>
	/// 发送请求
	/// </summary>
	/// <param name="content"></param>
	public void SendRequest(Content content) {
		clientMng.SendMessage(content);
	}


	/// <summary>
	/// 调试, 展示信息
	/// </summary>
	/// <param name="s"></param>
	public void ShowPromot(string s) {
		uiMng.PushStack(UIPanelType.PromptPanel, false, s);
	}

	/// <summary>
	/// 记录日志
	/// </summary>
	/// <param name="s"></param>
	public void RecordLog(string log, bool show) {
		if (show) {     // 是否展示

			// 客户端UI界面显示
			RoomBG r = uiMng.dicPanels[UIPanelType.RoomBG] as RoomBG;
			r.SetLogPanel(log);
		}

		// 存储到本地文本文件
	}

	/// <summary>
	/// 等待请求响应, 期间加载LoadingPanel
	/// </summary>
	public void WaitResponse(bool wait) {
		// StartCoroutine(_WaitResponse(wait));
		waitResponse.Enqueue(wait);
	}

	IEnumerator IWaitResponse() {
		while (true) {
			yield return new WaitUntil(() => waitResponse.Count > 0);
			if (waitResponse.Dequeue())
				uiMng.PushStack(UIPanelType.LoadingPanel, false);
			else
				uiMng.PopStack(UIPanelType.LoadingPanel, false);
			yield return null;
		}
	}

	/// <summary>
	/// 播放音频
	/// </summary>
	/// <param name="actionCode">音频索引</param>
	/// <param name="loop">是否循环播放</param>
	/// <param name="main">是否使用主声源</param>
	public void PlayMusic(AudioType audioType, bool kill = true, bool loop = false, bool main = false) {

		audioMng.PlayMusic(audioType, kill, loop, main);
	}

	/// <summary>
	/// 通过Id获取Player
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public Player GetPlayer(string id) {
		return playerMng.GetPlayer(id);
	}

}
