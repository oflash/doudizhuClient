using Common;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivideRequest : BaseRequest
{

	GamePanel gamePanel;
	protected override void Start() {
		requestCode = RequestCode.Card;
		actionCode = ActionCode.Divide;
		gamePanel = GetComponent<GamePanel>();
		base.Start();
	}


	/// <summary>
	/// 发送发牌请求
	/// </summary>
	public void RequestDivide() {
		StartCoroutine(Divide());

	}
	IEnumerator Divide() {
		Content send = new Content(ContentType.Default, actionCode, requestCode);
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
			Debug.Log("收到牌.....\n" + content.content);

			string s = "";
			int[] arr = JsonConvert.DeserializeObject<int[]>(content.content);
			List<int> list = new List<int>();
			List<int> dizhu = new List<int>();
			for (int i = 0; i < arr.Length; i++) {
				s += arr[i] + ",";
				if (i < 17) {
					list.Add(arr[i]);
				} else {            // 地主牌
					dizhu.Add(arr[i]);
				}
			}

			gamePanel.GetInitCard(list.ToArray(), dizhu.ToArray());      // 获取卡牌及地主牌

			// gameFacade.ShowPromot(s);
			gameFacade.RecordLog("收到了卡牌", true);
		}
	}


}
