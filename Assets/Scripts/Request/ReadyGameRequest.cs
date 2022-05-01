using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyGameRequest : BaseRequest
{
	RoomBG roomBG;
	RobDiZhuPanel robDiZhuPanel;
	GamePanel gamePanel;
	bool allReady = false;      // 全部准备好

	protected override void Start() {
		requestCode = RequestCode.Room;
		actionCode = ActionCode.ReadyGame;
		roomBG = GetComponent<RoomBG>();
		gamePanel = transform.Find("GamePanel").GetComponent<GamePanel>();
		robDiZhuPanel = transform.Find("RobDiZhuPanel").GetComponent<RobDiZhuPanel>();
		base.Start();
	}

	/// <summary>
	/// 发送准备请求
	/// </summary>
	public void RequestReadyGame() {
		Debug.Log("已准备!");
		StartCoroutine(IReadyGame());
	}

	IEnumerator IReadyGame() {

		Content send = new Content(ContentType.Default, ActionCode.ReadyGame, RequestCode.Room);
		send.id = gameFacade.Id;
		SendRequest(send);          // 发送准备游戏请求
		gameFacade.WaitResponse(true);
		yield return new WaitUntil(() => Response);
		gameFacade.WaitResponse(false);
		response = false;

		// gamePanel.RequestDivide();              		// 申请发牌
		yield return new WaitUntil(() => allReady);     // 全部准备好
		allReady = false;
		roomBG.SetReadyPanel(false);                    // 隐藏readyTag

		gameFacade.WaitResponse(true);
		yield return new WaitForSeconds(1);
		gameFacade.WaitResponse(false);

		gamePanel.RequestDivide();                      // 申请发牌

		yield return new WaitForSeconds(1);
		yield return new WaitUntil(() => gamePanel.my_cards.Count == 17);   // 所有牌都发完
		if (gameFacade.Id == one_id) {  // 我是第一个抢地主的人
			one_id = "";
			robDiZhuPanel.ShowCallDiZhu();
		} else {                        // 显示第一个叫的人的倒计时
			int local_index = gameFacade.GetPlayer(one_id).local_index;
			gamePanel.pokerS[local_index].ShowText_Content("Timing");
			gamePanel.pokerS[local_index].GetComponentInChildren<PlayerTiming>().BeginTiming(10, null);
		}


		gamePanel.SetOtherPlayerCardNumber(gamePanel.poker1H, 17);
		gamePanel.SetOtherPlayerCardNumber(gamePanel.poker2H, 17);
	}


	string one_id;
	public override void OnResponse(Content content) {
		base.OnResponse(content);
		// Debug.Log("收到已准备响应");
		if (content.returnCode == ReturnCode.Success) {
			string id = content.id;
			Player player = gameFacade.GetPlayer(id);
			player.ReadyGame();     // 单击准备, 展示readyTag

			Debug.Log(content.content);
			try {
				if (int.Parse(content.content) == 0) {      // 3个人都准备了
					Debug.Log("开始游戏");
					allReady = true;
					// gameFacade.ShowPromot("游戏开始, 开始抢地主...");
					gameFacade.ShowPromot("本局游戏由" + player.Name + "开始叫地主");
					one_id = id;

				}
			} catch (System.Exception) {
				gameFacade.ShowPromot("数据错误...");
			}

			gameFacade.RecordLog(player.Name + " 准备", true);
		} else if (content.returnCode == ReturnCode.Fail) {
			gameFacade.ShowPromot("失败，服务器异常!");
		}
	}
}
