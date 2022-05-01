using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleRequest : BaseRequest
{

	GamePanel gamePanel;
	OutCardPanel outCardPanel;
	protected override void Start() {
		requestCode = RequestCode.PlayGame;
		actionCode = ActionCode.Double;
		outCardPanel = GetComponent<OutCardPanel>();
		gamePanel = transform.parent.Find("GamePanel").GetComponent<GamePanel>();
		base.Start();
	}
	int cnt_mul;    // 加倍数量

	/// <summary>
	/// 发送加倍请求
	/// </summary>
	/// <param name="num">加倍倍数</param>
	public void RequestDouble(int num) {
		StartCoroutine(Double(num));

	}

	IEnumerator Double(int num) {
		Content send = new Content(ContentType.Default, actionCode, requestCode, num.ToString());
		send.id = gameFacade.Id;

		SendRequest(send);
		gameFacade.WaitResponse(true);
		yield return new WaitUntil(() => Response);
		gameFacade.WaitResponse(false);
		response = false;
		outCardPanel.ClosePanel();

		yield return new WaitUntil(() => cnt_mul == 0);     // 等待全部点击加倍
		gameFacade.WaitResponse(true);
		yield return new WaitForSeconds(1);
		gameFacade.WaitResponse(false);



		string landownerId = "";
		foreach (var item in gameFacade.PlayerMng.players)
			if (item.Landowner) landownerId = item.Id;

		if (gameFacade.GetPlayer(gameFacade.Id).Landowner) {    // 如果是地主
			outCardPanel.One_DiZhuOut();     // 显示出牌, 明牌
		} else {    // 否则显示地主倒计时
			int local_index = gameFacade.GetPlayer(landownerId).local_index;
			gamePanel.pokerS[local_index].ShowText_Content("Timing");
			gamePanel.pokerS[local_index].GetComponentInChildren<PlayerTiming>().BeginTiming(30, null);
		}
	}

	public override void OnResponse(Content content) {
		base.OnResponse(content);
		cnt_mul = (cnt_mul + 1) % 3;

		if (content.returnCode == ReturnCode.Success) {
			Player player = gameFacade.GetPlayer(content.id);
			int a = int.Parse(content.content);
			int num = a & 0b111;        // 倍数
			a >>= 3;                    // 原始倍数

			AudioType audio = Audio.GetDoubleAudio(num, player.Sex);
			gameFacade.PlayMusic(audio);
			outCardPanel.SendDouble(gameFacade.GetPlayer(content.id).local_index, num);


			if (num == 1) {
				gameFacade.RecordLog(player.Name + " 不加倍", true);
			} else if (num == 2) {
				gameFacade.RecordLog(player.Name + " 加倍", true);
			} else if (num == 4) {
				gameFacade.RecordLog(player.Name + " 超级加倍", true);
			}


			if (num != 2) gameFacade.ShowPromot("倍数 x" + num + "!");

			if (player.Landowner) { // 地主发过来的
				gamePanel.MulDouble(num);   // 直接乘以倍数
			} else {                // 非地主发过来的
				if (gameFacade.GetPlayer(gameFacade.Id).Landowner) {    // 发给地主
					gamePanel.AddDouble(a * num - a);                   // 增加倍数
				} else if (player.Id == gameFacade.Id) {                // 发给自己
					gamePanel.MulDouble(num);
				}
			}


		}
	}

}
