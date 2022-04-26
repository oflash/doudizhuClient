using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderRequest : BaseRequest
{
	RoomBG roomBG;
	protected override void Start() {
		requestCode = RequestCode.Room;
		actionCode = ActionCode.Order;
		roomBG = GetComponent<RoomBG>();
		base.Start();
	}


	/// <summary>
	/// 发送获取出牌顺序请求
	/// </summary>
	public void RequestOrder() {
		StartCoroutine(IOrder());
	}

	IEnumerator IOrder() {
		Content send = new Content(ContentType.OrderInfo, actionCode, requestCode, "获取出牌顺序");
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

			Dictionary<string, string> dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(content.content);
			string s = "";
			foreach (var item in dic) {
				roomBG.AddNextPlayer(item.Key, item.Value);
				s += item.Key + "->" + item.Value + "\n";
			}
			Debug.Log("已获取玩家出牌顺序...");              // 获取3次?
			Debug.Log(s);
			// gameFacade.ShowPromot("已获取玩家出牌顺序");

		} else if (content.returnCode == ReturnCode.Fail) {
			gameFacade.ShowPromot("获取失败, 服务器异常!");
		}

	}

}
