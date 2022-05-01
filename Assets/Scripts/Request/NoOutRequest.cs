using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoOutRequest : BaseRequest
{

	OutCardPanel outCardPanel;
	GamePanel gamePanel;
	RoomBG roomBG;
	protected override void Start() {
		requestCode = RequestCode.PlayGame;
		actionCode = ActionCode.Pass;
		outCardPanel = GetComponent<OutCardPanel>();
		gamePanel = transform.parent.Find("GamePanel").GetComponent<GamePanel>();
		roomBG = transform.parent.GetComponent<RoomBG>();
		base.Start();
	}



	/// <summary>
	/// 发送不要请求
	/// </summary>
	public void RequestNoOut() {

		StartCoroutine(NoOut());
	}

	IEnumerator NoOut() {

		Content send = new Content(ContentType.Default, actionCode, requestCode);
		send.id = gameFacade.Id;

		SendRequest(send);
		gameFacade.WaitResponse(true);
		yield return new WaitUntil(() => Response);
		gameFacade.WaitResponse(false);
		response = false;

		outCardPanel.ClosePanel();
	}


	public override void OnResponse(Content content) {
		base.OnResponse(content);

		if (content.returnCode == ReturnCode.Success) {
			int local_index = gameFacade.GetPlayer(content.id).local_index;   // 位置
			string next = roomBG.GetNextPlayer(content.id);             // 下一人出牌

			outCardPanel.cnt_pass = Mathf.Max(0, outCardPanel.cnt_pass);// 从0开始计数
			outCardPanel.cnt_pass++;

			outCardPanel.SendNoOut(local_index);      // 发送不出文字
			AudioType audio = Audio.GetNoOutAudio(gameFacade.GetPlayer(content.id).Sex);
			gameFacade.PlayMusic(audio);        // 播放不出音乐
			gameFacade.GetPlayer(content.id).PlayerIdle();  // 要不起后, 不笑

			/* 到我出牌了 */
			if (outCardPanel.cnt_pass == 2) {   // 连续两次不要, 只展示出牌, 清空prev
				outCardPanel.prev = null;
				if (next == gameFacade.Id) {    // 出牌者显示自己的出牌Button
					outCardPanel.One_OutCard();
				} else {                        // 非出牌者显示出牌者的倒计时
					int local_index2 = gameFacade.GetPlayer(next).local_index;
					gamePanel.pokerS[local_index2].ShowText_Content("Timing");
					gamePanel.pokerS[local_index2].GetComponentInChildren<PlayerTiming>().BeginTiming(20, null);
				}
				outCardPanel.cnt_pass = 0;      // 清零计数
			} else {
				if (next == gameFacade.Id) {    // 出牌者显示自己的出牌Button
					outCardPanel.Two_OutCard();
				} else {                        // 非出牌者显示出牌者的倒计时
					int local_index2 = gameFacade.GetPlayer(next).local_index;
					gamePanel.pokerS[local_index2].ShowText_Content("Timing");
					gamePanel.pokerS[local_index2].GetComponentInChildren<PlayerTiming>().BeginTiming(20, null);
				}
			}

			gameFacade.RecordLog(gameFacade.GetPlayer(content.id).Name + " 不出", true);
		}
	}


}
