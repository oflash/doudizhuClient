using Common;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutCardRequest : BaseRequest
{
	OutCardPanel outCardPanel;
	GamePanel gamePanel;
	RoomBG roomBG;
	protected override void Start() {
		requestCode = RequestCode.PlayGame;
		actionCode = ActionCode.OutCard;
		outCardPanel = GetComponent<OutCardPanel>();
		gamePanel = transform.parent.Find("GamePanel").GetComponent<GamePanel>();
		roomBG = transform.parent.GetComponent<RoomBG>();

		base.Start();
	}

	/// <summary>
	/// 发送出牌请求
	/// </summary>
	public void RequestOutCard(List<Card> outs) {
		if (Rule.Compare(outCardPanel.prev, outs)) {     // 可以出
			StartCoroutine(OutCard(outs));
		} else {
			gamePanel.OnClickResume();                  // 取消所有选择
			gameFacade.ShowPromot("你的牌不能大过他...");
		}

	}
	IEnumerator OutCard(List<Card> outs) {
		List<int> ids = new List<int>();
		foreach (Card card in outs) ids.Add(card.Id);

		Content send = new Content(ContentType.CardList, actionCode, requestCode);
		send.id = gameFacade.Id;
		send.content = JsonConvert.SerializeObject(ids);

		SendRequest(send);
		gameFacade.WaitResponse(true);
		yield return new WaitUntil(() => Response);
		gameFacade.WaitResponse(false);
		response = false;
		outCardPanel.ClosePanel();

		/* 自己出牌, 删除手里的卡牌 */
		foreach (Card card in outs)
			gamePanel.my_cards.Remove(card);
		for (int i = 0; i < outs.Count; i++) {
			Destroy(outs[i].gameObject);
		}


		Debug.Log("剩余:" + gamePanel.my_cards);
		if (gamePanel.my_cards.Count == 0) {    // 我赢了
			gamePanel.RequestWin();     // 发送请求告诉大家, 我赢了
		} else if (gamePanel.my_cards.Count <= 2) {
			gameFacade.PlayMusic(AudioType.MusicEx_Normal2, false, true, true);     // 不会重复播放主声源

			string s = gameFacade.GetPlayer(gameFacade.Id).Sex ? "Man_baojing" : "Woman_baojing";
			s += gamePanel.my_cards.Count;
			AudioType _audio = Audio.GetAudio(s);
			gameFacade.PlayMusic(_audio, false);    // 播放报警音频
		}
	}


	public override void OnResponse(Content content) {
		base.OnResponse(content);

		if (content.returnCode == ReturnCode.Success) {
			int[] ids = JsonConvert.DeserializeObject<int[]>(content.content);
			int index = gameFacade.GetPlayer(content.id).local_index;   // 出牌位置
			outCardPanel.prev = gamePanel.pokerS[index].ShowCard_Content(ids);       // 出牌, 更新上一个出牌的卡牌
			string next = roomBG.GetNextPlayer(content.id);             // 下一个出牌的

			outCardPanel.cnt_pass--;      // 置负数cnt_pass, 不直接置零好处: 可以统计连续出牌次数
			/*
			 *	如果不叫一次, 刚好置零
			 *	否则, 为负数, 但计数最小从0开始
			 *	cnt_pass == 2 ? 不存在, 为2时, 会直接重新出牌
			 */

			int random = Random.Range(1, 100);
			Debug.Log("outCardPanel.cnt_pass:" + outCardPanel.cnt_pass);
			bool dani = (-outCardPanel.cnt_pass >= 3 && random % 3 == 0) ? true : false;     // 连续两次以上出牌, 随机大你语音
			AudioType audio = Audio.GetCardAudio(outCardPanel.prev, gameFacade.GetPlayer(content.id).Sex, dani);
			gameFacade.PlayMusic(audio, false);     // 播放音乐

			gameFacade.GetPlayer(content.id).PlayerLaunch();        // 出牌后笑


			if (content.id != gameFacade.Id) {      // 收到别人出的牌
				gamePanel.SetOtherPlayerCardNumber(index, ids.Length, true);    // 修改别人剩余的牌
				if (index > 0 && gamePanel.IsMing) {        // 来自别人的明牌出牌
					Player player = gameFacade.GetPlayer(content.id);
					if (player.Landowner)   // 出牌的是地主, 显示明牌值
						gamePanel.mingCardPanels[index - 1].ShowCard_Content(ids);
				}
			}

			int num = gamePanel.GetOtherPlayerCardNumber(index);                        // 该玩家出完牌后剩余牌
			if (num <= 2 && num > 0 && (index + 1) / 2 == 1) {                          // 别人出牌后, 剩余最后两张牌以下, 不在这里处理自己的牌
				gameFacade.PlayMusic(AudioType.MusicEx_Normal2, false, true, true);     // 不会重复播放主声源

				string s = gameFacade.GetPlayer(content.id).Sex ? "Man_baojing" : "Woman_baojing";
				s += num;
				AudioType _audio = Audio.GetAudio(s);
				gameFacade.PlayMusic(_audio, false);    // 播放报警音频
			}

			if (next == gameFacade.Id) {    // 轮到我出牌了
				outCardPanel.Two_OutCard();
			} else {                        // 不是我出牌, 显示出牌者的倒计时
				int local_index = gameFacade.GetPlayer(next).local_index;
				gamePanel.pokerS[local_index].ShowText_Content("Timing");
				gamePanel.pokerS[local_index].GetComponentInChildren<PlayerTiming>().BeginTiming(20, null);
			}

			gameFacade.RecordLog(gameFacade.GetPlayer(content.id).Name + " 出牌", true);
		}
	}

}
