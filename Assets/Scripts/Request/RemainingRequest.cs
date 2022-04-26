using Common;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemainingRequest : BaseRequest
{
	GamePanel gamePanel;
	RoomBG roomBG;

	protected override void Start() {
		requestCode = RequestCode.PlayGame;
		actionCode = ActionCode.Remaining;
		gamePanel = GetComponent<GamePanel>();
		roomBG = transform.parent.GetComponent<RoomBG>();

		base.Start();
	}


	/// <summary>
	/// 发送自己剩余的牌
	/// </summary>
	/// <param name="ids"></param>
	public void RequestRemaining(int[] ids) {
		StartCoroutine(Remaining(ids));
	}

	IEnumerator Remaining(int[] ids) {
		string content = JsonConvert.SerializeObject(ids);

		Content send = new Content(ContentType.CardList, actionCode, requestCode, content);
		send.id = gameFacade.Id;

		SendRequest(send);
		gameFacade.WaitResponse(true);
		yield return new WaitUntil(() => Response);
		gameFacade.WaitResponse(false);
		response = false;

	}


	public override void OnResponse(Content content) {
		base.OnResponse(content);

		if (content.returnCode == ReturnCode.Success) {
			Player player = gameFacade.GetPlayer(content.id);
			int[] ids = JsonConvert.DeserializeObject<int[]>(content.content);

			gamePanel.pokerS[player.local_index].ShowCard_Content(ids);
			gameFacade.RecordLog(player.Name + " 剩余牌", true);
		}

	}

}
