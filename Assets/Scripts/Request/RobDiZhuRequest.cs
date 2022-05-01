using Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobDiZhuRequest : BaseRequest
{
	RobDiZhuPanel robDiZhuPanel;
	OutCardPanel outCardPanel;
	RoomBG roomBG;
	GamePanel gamePanel;
	protected override void Start() {
		requestCode = RequestCode.PlayGame;
		actionCode = ActionCode.RobDiZhu;
		robDiZhuPanel = GetComponent<RobDiZhuPanel>();
		roomBG = transform.parent.GetComponent<RoomBG>();
		gamePanel = roomBG.transform.Find("GamePanel").GetComponent<GamePanel>();
		outCardPanel = transform.parent.Find("OutCardPanel").GetComponent<OutCardPanel>();
		base.Start();
	}

	bool confirmed;   // 确认地主是谁

	/// <summary>
	/// 发送抢地主相关的请求
	/// </summary>
	/// <param name="rob"></param>
	public void RequestRobDiZhu(bool rob) {
		StartCoroutine(RobDiZhu(rob));
	}
	IEnumerator RobDiZhu(bool rob) {
		Content send = new Content(ContentType.Default, ActionCode.RobDiZhu,
			RequestCode.PlayGame, rob.ToString());
		send.id = gameFacade.Id;

		SendRequest(send);

		gameFacade.WaitResponse(true);
		yield return new WaitUntil(() => Response);     // 收到响应后
		gameFacade.WaitResponse(false);
		response = false;
		robDiZhuPanel.ClosePanel();                     // 关闭按钮


		yield return new WaitUntil(() => confirmed);  // 确认处出地主后
		confirmed = false;
		/* 展示加倍按钮 */
		outCardPanel.One_Double();          // 第一次展示加倍按钮

		gamePanel.pokerS[1].ShowText_Content("Timing");
		gamePanel.pokerS[1].GetComponentInChildren<PlayerTiming>().BeginTiming(10, null);   // 显示其他玩家倒计时

		gamePanel.pokerS[2].ShowText_Content("Timing");
		gamePanel.pokerS[2].GetComponentInChildren<PlayerTiming>().BeginTiming(10, null);   // 显示其他玩家倒计时
	}


	public override void OnResponse(Content content) {
		base.OnResponse(content);
		// gameFacade.ShowPromot("收到抢地主相关信息...");

		RobInfo robinfo = JsonConvert.DeserializeObject<RobInfo>(content.content);

		bool next_call = false;     // 下一人是否为叫
		{   /* 播放音频, 显示PokerS */
			string audioType = "";
			Player player = gameFacade.GetPlayer(content.id);
			audioType += player.Sex ? "Man_" : "Woman_";
			// call -> 有人叫了地主?
			if (robinfo.call && robinfo.rob) {              // 叫地主
				audioType += "Order";
				robDiZhuPanel.SendRobDiZhu(player.local_index, false);
				gameFacade.RecordLog(player.Name + " 叫地主", true);

			} else if (robinfo.call && !robinfo.rob) {      // 不叫
				audioType += "NoOrder";
				next_call = true;
				robDiZhuPanel.SendNoRob(player.local_index, false);
				gameFacade.RecordLog(player.Name + " 不叫", true);

			} else if (!robinfo.call && !robinfo.rob) {     // 不抢
				audioType += "NoRob";
				robDiZhuPanel.SendNoRob(player.local_index, true);
				gameFacade.RecordLog(player.Name + " 不抢", true);
			} else if (!robinfo.call && robinfo.rob) {      // 抢地主
				audioType += "Rob" + (UnityEngine.Random.Range(1, 4));
				robDiZhuPanel.SendRobDiZhu(player.local_index, true);
				gamePanel.MulDouble(2);                     // 有人抢地主, 加倍
				gameFacade.ShowPromot("倍数 x2!");
				gameFacade.RecordLog(player.Name + " 抢地主", true);
			}
			AudioType audio = Audio.GetAudio(audioType);
			gameFacade.PlayMusic(audio);
		}

		// Debug.Log(robinfo.call + ": " + robinfo.next);
		if (content.returnCode == ReturnCode.Fail) {            // 没有确定地主

			// Debug.Log(content.id + "点击了抢地主或不抢");
			// Debug.Log("next:" + next);
			if (robinfo.next == gameFacade.Id) {        // 下一个出牌人

				// gameFacade.ShowPromot("到你抢地主了");
				if (next_call) {
					robDiZhuPanel.ShowCallDiZhu();
				} else {
					robDiZhuPanel.ShowRobDiZhu();
				}
			} else {        // 显示出牌人倒计时
				int local_index = gameFacade.GetPlayer(robinfo.next).local_index;
				gamePanel.pokerS[local_index].ShowText_Content("Timing");
				gamePanel.pokerS[local_index].GetComponentInChildren<PlayerTiming>().BeginTiming(10, null);   // 显示其他玩家倒计时
			}
		} else if (content.returnCode == ReturnCode.Success) {  // 确定出地主是谁

			foreach (Player player in roomBG.players) {
				if (robinfo.next == player.Id) {                // 修改地主人物图片
					player.SetPlayer(true);
					player.SetLandowner(true);
				}
			}
			gamePanel.ReverseDiZhuCard();       // 翻转地主牌

			if (robinfo.next == gameFacade.Id) {    // 如果我是地主
				int[] ids = gamePanel.GetIds(gamePanel.dizhu_cards);
				gamePanel.AddMyCard(gamePanel.poker0H, ids, gamePanel.my_cards);    // 增加地主牌
				gamePanel.MulDouble(2);             // 改变地主倍数
			} else {
				int local_index = gameFacade.GetPlayer(robinfo.next).local_index;
				gamePanel.SetOtherPlayerCardNumber(local_index, 20);               // 地主的牌显示为20张
			}
			confirmed = true;
		}


	}



}
