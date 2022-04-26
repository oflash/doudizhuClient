using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeaderRequest : BaseRequest
{
	List<Player> players => gameFacade.PlayerMng.players;
	HeaderPanel headerPanel;
	protected override void Start() {
		requestCode = RequestCode.Player;
		actionCode = ActionCode.Header;
		headerPanel = GetComponent<HeaderPanel>();
		base.Start();
	}



	/// <summary>
	/// 发送修改头像请求
	/// </summary>
	/// <param name="index"></param>
	public void RequestHeader(int index) {
		Debug.Log("正在修改头像:" + index);
		StartCoroutine(Header(index));
	}

	IEnumerator Header(int index) {
		Content send = new Content(ContentType.Default, ActionCode.Header,
			RequestCode.Player, index.ToString());
		send.id = gameFacade.Id;

		SendRequest(send);      // 发送修改聊天请求
		gameFacade.WaitResponse(true);              // 发送请求后等待响应
		yield return new WaitUntil(() => Response);
		gameFacade.WaitResponse(false);
		response = false;
	}



	public override void OnResponse(Content content) {
		base.OnResponse(content);

		if (content.returnCode == ReturnCode.Success) {
			int index = int.Parse(content.content);

			Player player = gameFacade.GetPlayer(content.id);
			player.header = index;

			gameFacade.ShowPromot(player.Name + " " + "修改了头像");
			gameFacade.RecordLog(player.Name + " " + "修改了头像", true);
		}
	}

}
