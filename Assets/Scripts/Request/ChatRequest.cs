using Common;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatRequest : BaseRequest
{
	InputChatPanel inputChatPanel;

	protected override void Start() {
		requestCode = RequestCode.Player;
		actionCode = ActionCode.Chat;
		inputChatPanel = GetComponent<InputChatPanel>();
		base.Start();
	}


	/// <summary>
	/// 发送聊天请求
	/// </summary>
	/// <param name="chat">聊天内容</param>
	/// <param name="type">0表示普通文字聊天, 1表示语音文字, 2表示表情包</param>
	/// <param name="other">其他参数, 1 -> AudioType, 2 -> 表情包</param>
	public void RequestSendChat(string chat, int type = 0, string other = "0") {
		Debug.Log("正在发送消息:" + chat);
		ChatInfo chatInfo = new ChatInfo(chat, type, other);
		StartCoroutine(SendChat(chatInfo));
	}

	IEnumerator SendChat(ChatInfo chatInfo) {

		Content send = new Content(ContentType.ChatInfo, ActionCode.Chat,
			RequestCode.Player, JsonConvert.SerializeObject(chatInfo));
		send.id = gameFacade.Id;
		SendRequest(send);      // 发送聊天请求
		gameFacade.WaitResponse(true);              // 发送请求后等待响应
		yield return new WaitUntil(() => Response);
		gameFacade.WaitResponse(false);
		response = false;
	}



	public override void OnResponse(Content content) {
		base.OnResponse(content);
		ChatInfo chatInfo = JsonConvert.DeserializeObject<ChatInfo>(content.content);

		if (content.returnCode == ReturnCode.Success) {

			string id = content.id;     // 是谁发送过来的消息
			Player player = gameFacade.GetPlayer(id);
			player.Chat(chatInfo);

			gameFacade.ShowPromot(player.Name + ":" + chatInfo.chat);
		} else if (content.returnCode == ReturnCode.Fail) {
			gameFacade.ShowPromot("发送失败!");
		}

	}
}
