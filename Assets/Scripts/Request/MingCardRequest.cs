using Common;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MingCardRequest : BaseRequest
{
	OutCardPanel outCardPanel;
	GamePanel gamePanel;
	RoomBG roomBG;

	protected override void Start() {
		requestCode = RequestCode.PlayGame;
		actionCode = ActionCode.MingCard;
		outCardPanel = GetComponent<OutCardPanel>();
		gamePanel = transform.parent.Find("GamePanel").GetComponent<GamePanel>();
		roomBG = transform.parent.GetComponent<RoomBG>();

		base.Start();
	}

	/// <summary>
	/// 发送明牌请求
	/// </summary>
	public void RequestMingCard(List<Card> cards) {
		StartCoroutine(IMingCarg(cards));
	}


	IEnumerator IMingCarg(List<Card> cards) {

		List<int> ids = new List<int>();
		foreach (Card card in cards) ids.Add(card.Id);

		Content send = new Content(ContentType.CardList, actionCode, requestCode);
		send.id = gameFacade.Id;
		send.content = JsonConvert.SerializeObject(ids);    // 发送对象是数组
		SendRequest(send);

		gameFacade.WaitResponse(true);          // 显示等待面板, 玩家不可以点击其他按钮
		yield return new WaitUntil(() => Response);
		gameFacade.WaitResponse(false);
		response = false;

		outCardPanel.One_EnterMingCard();
	}


	public override void OnResponse(Content content) {
		base.OnResponse(content);

		if (content.returnCode == ReturnCode.Success) {
			int[] ids = JsonConvert.DeserializeObject<int[]>(content.content);
			int index = gameFacade.GetPlayer(content.id).local_index;   // 本地id

			gamePanel.IsMing = true;        // 设置本局为明牌
			if (index > 0)
				gamePanel.mingCardPanels[index - 1].ShowCard_Content(ids);


			gamePanel.MulDouble(2);      // 明牌, 所有人都加倍
			Player player = gameFacade.GetPlayer(content.id);   //

			AudioType audio = Audio.GetMingCardAudio(player.Sex);
			gameFacade.PlayMusic(audio);
			gameFacade.ShowPromot("地主明牌了！");
			gameFacade.RecordLog(player.Name + " 地主明牌", true);

		}

	}


}
