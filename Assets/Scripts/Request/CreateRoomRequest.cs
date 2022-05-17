using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRoomRequest : BaseRequest
{
	bool created;
	public bool Created => created;

	LinkPanel linkPanel;

	protected override void Start() {

		requestCode = RequestCode.Room;
		actionCode = ActionCode.CreateRoom;
		linkPanel = GetComponent<LinkPanel>();
		base.Start();
	}

	Player[] players {
		get {
			return (gameFacade.UIMng.GetPanel(UIPanelType.RoomBG) as RoomBG).players;
		}
	}
	RoomBG roombg => gameFacade.UIMng.GetPanel(UIPanelType.RoomBG) as RoomBG;


	/// <summary>
	/// 创建房间
	/// </summary>
	/// <param name="room_num">房间号</param>
	/// <param name="scene_id">房间背景编号</param>
	public void RequestCreateRoom(int room_num, int scene_id) {
		StartCoroutine(ICreateRoom(room_num, scene_id));
	}

	IEnumerator ICreateRoom(int room_num, int scene_id) {

		gameFacade.WaitResponse(true);
		yield return new WaitUntil(() => GetComponent<LinkRequest>().Connected);    // 等待连接到服务器

		gameFacade.ShowPromot("正在创建房间...");
		yield return new WaitForSeconds(1);

		int binary = room_num;    // 房间号
		binary <<= 8;             // 低8位存储房间场景编号
		binary |= scene_id;

		Content send = new Content(ContentType.Default, actionCode, requestCode, binary.ToString());
		send.id = players[0].Id;

		SendRequest(send);      // 发送创建房间请求
		yield return new WaitUntil(() => Response);
		response = false;
		yield return null;
		yield return new WaitForSeconds(0.5f);
		if (Created) {// 如果创建成功
			created = false;
			GetComponent<JoinRoomRequest>().RequestJionRoom(false, room_num);     // 加入房间
		} else {
			gameFacade.WaitResponse(false);
		}
	}

	public override void OnResponse(Content content) {
		base.OnResponse(content);

		if (content.returnCode == ReturnCode.Success) {
			gameFacade.ShowPromot(content.content);
			created = true;     // 创建成功
		} else if (content.returnCode == ReturnCode.Fail) {
			gameFacade.ShowPromot(content.content);

			// gameFacade.DisconnectServer();      // 创建失败断开连接
		}
	}
}
