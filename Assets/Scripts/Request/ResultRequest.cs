using Common;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultRequest : BaseRequest
{
	GamePanel gamePanel;

	RoomBG roomBG;

	Dictionary<string, ResultInfo> dic = new Dictionary<string, ResultInfo>();

	protected override void Start() {
		requestCode = RequestCode.PlayGame;
		actionCode = ActionCode.Result;
		gamePanel = GetComponent<GamePanel>();
		roomBG = transform.parent.GetComponent<RoomBG>();

		base.Start();
	}



	/// <summary>
	/// 计算好自己的豆子变化, 发送给服务器
	/// </summary>
	public void RequestResult(bool win) {
		int bottom = 10;                                    // 底分
		int double_ = int.Parse(gamePanel.doubleTxt.text);  // 倍数
		int douzi = bottom * double_;                       // 豆子
		if (!win) douzi = -douzi;

		gameFacade.UIMng.dicPanels[UIPanelType.TopLayerPanel].GetComponent<TopLayerPanel>().SetDouNum(douzi);       // 修改自己界面的豆子值

		ResultInfo info = new ResultInfo(double_, douzi, bottom);
		string content = JsonConvert.SerializeObject(info);
		StartCoroutine(Result(content));
	}

	IEnumerator Result(string content) {
		Content send = new Content(ContentType.ResultInfo, actionCode, requestCode, content);
		send.id = gameFacade.Id;

		SendRequest(send);
		gameFacade.WaitResponse(true);
		yield return new WaitUntil(() => Response);
		response = false;

		yield return new WaitUntil(() => dic.Count == 3);      // 收到三人结果的消息后
		gameFacade.WaitResponse(false);
		GameOverPanel gameOverPanel = gameFacade.UIMng.GetPanel(UIPanelType.GameOverPanel) as GameOverPanel;
		foreach (var item in dic) {
			Player player = gameFacade.GetPlayer(item.Key);
			gameOverPanel.SetResultInfo(player, item.Value);
			yield return null;
		}
		dic.Clear();
		gameOverPanel.SetHeaderInfo();

		gameFacade.UIMng.PushStack(UIPanelType.GameOverPanel);  // 打开结果Panel

	}

	public override void OnResponse(Content content) {
		base.OnResponse(content);

		if (content.returnCode == ReturnCode.Success) {
			ResultInfo info = JsonConvert.DeserializeObject<ResultInfo>(content.content);
			// Player player = gameFacade.GetPlayer(content.id);
			dic.Add(content.id, info);
		}
	}

}
