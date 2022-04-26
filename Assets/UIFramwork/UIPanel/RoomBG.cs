using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制人物动画等
/// </summary>
public class RoomBG : BasePanel
{
	public Image BG => GetComponent<Image>();
	InputChatPanel inputChatPanel;   // 输入或选择聊天信息的面板
	LogPanel logPanel;                  // 日志面板
	Transform gamePanel;        // 游戏面板
	Transform startGameButton;
	Transform readyTagPanel;
	public Player[] players = new Player[3];        // 三个玩家的面板



	#region 出牌顺序
	Dictionary<string, string> nextPlayer;           // 下一个出牌的人
	public string GetNextPlayer(string id) {
		string next = "";
		if (!nextPlayer.TryGetValue(id, out next)) next = id;
		return next;
	}
	public void AddNextPlayer(string key, string value) {
		if (nextPlayer == null) nextPlayer = new Dictionary<string, string>();
		string s;
		if (nextPlayer.TryGetValue(key, out s)) {   // 原来有, 替换它
			nextPlayer[key] = value;
			return;
		}

		nextPlayer.Add(key, value);
	}
	/// <summary>
	/// 获取出牌顺序
	/// </summary>
	public void GetOrder() {
		GetComponent<OrderRequest>().RequestOrder();
	}
	#endregion


	protected override void Start() {
		base.Start();
		uiPanelType = UIPanelType.RoomBG;
		inputChatPanel = transform.Find("InputChatPanel").GetComponent<InputChatPanel>();
		logPanel = transform.Find("LogPanel").GetComponent<LogPanel>();
		readyTagPanel = transform.Find("ReadyTagPanel");
		gamePanel = transform.Find("GamePanel");
		startGameButton = transform.Find("StartGameButton");

		for (int i = 0; i < players.Length; i++) {
			players[i] = transform.Find("Player" + i).GetComponent<Player>();
		}
		SetReadyPanel(false);
		SetStartGameButton(false);

		// OnInit();
	}

	/// <summary>
	/// 新一局游戏, 会初始化
	/// </summary>
	public override void OnInit() {
		GameFacade.Instance.PlayMusic(AudioType.MusicEx_Normal, false, true, true);     // 如果没有音频, 什么也不做
		foreach (Player player in players) {
		//	Debug.Log(player.joined);
			if (player.joined) player.SetPlayer(false);     // 如果加入了房间
			player.SetLandowner(false);
		}
	}



	#region UI操作

	/// <summary>
	/// 设置背景图片
	/// </summary>
	/// <param name="index"></param>
	public void SetBGSprite(int index) {
		BG.sprite = uiMng.GetSprite(SpriteType.BG, false, index);
	}

	/// <summary>
	/// 设置开始游戏Button显示状况
	/// </summary>
	public void SetStartGameButton(bool b) {
		startGameButton.localScale = b ? Vector3.one : Vector3.zero;
	}

	/// <summary>
	/// 设置AllreadyTag的显示状况
	/// </summary>
	/// <param name="b"></param>
	public void SetReadyPanel(bool b) {
		for (int i = 0; i < readyTagPanel.childCount; i++)
			readyTagPanel.GetChild(i).localScale = b ? Vector3.one : Vector3.zero;
		readyTagPanel.localScale = b ? Vector3.one : Vector3.zero;
	}

	/// <summary>
	/// 设置日志信息显示
	/// </summary>
	/// <param name="log"></param>
	public void SetLogPanel(string log) {
		logPanel.CreateNewLog(log);
	}

	#endregion

	#region 点击事件
	/// <summary>
	/// 点击Player获取PlayerInfo
	/// </summary>
	public void PlayerButton_Click(int index) {
		Debug.Log(index + " player");

		uiMng.PushStack(UIPanelType.PlayerInfoPanel, true, players[index]);       // obj为要显示的玩家
	}

	/// <summary>
	/// 点击聊天按钮, 打开或关闭聊天面板
	/// </summary>
	public void ChatButton_Click() {
		int v = ((int)inputChatPanel.transform.localScale.x + 1) % 2;   // v = 1 or 0
		inputChatPanel.transform.localScale = new Vector3(v, v, v);
		inputChatPanel.ResumeScale(v);
	}

	/// <summary>
	/// 点击开始按钮
	/// </summary>
	public void StartGame_Click() {
		GetComponent<ReadyGameRequest>().RequestReadyGame();    // 发送准备游戏请求
		SetStartGameButton(false);                              // 关闭Button按钮
	}

	/// <summary>
	/// 点击日志(记录)按钮
	/// </summary>
	public void LogButton_Click() {
		int v = ((int)logPanel.transform.localScale.x + 1) % 2;   // v = 1 or 0
		logPanel.transform.localScale = new Vector3(v, v, v);     // 打开或关闭日志面板
	}

	/// <summary>
	/// 点击空白处，关闭聊天Panel
	/// </summary>
	public void Blank_Click() {
		inputChatPanel.transform.localScale = Vector3.zero;
		inputChatPanel.ResumeScale(0);
	}


	#endregion
}
