using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Newtonsoft.Json;

public class JoinRoomRequest : BaseRequest
{
	bool joined;
	public bool Joined => joined;

	Player[] players {
		get {
			return (gameFacade.UIMng.GetPanel(UIPanelType.RoomBG) as RoomBG).players;
		}
	}
	RoomBG roombg => gameFacade.UIMng.GetPanel(UIPanelType.RoomBG) as RoomBG;

	LinkPanel linkPanel;
	PlayerManager playerMng => gameFacade.PlayerMng;

	protected override void Start() {
		requestCode = RequestCode.Room;
		actionCode = ActionCode.JoinRoom;
		linkPanel = GetComponent<LinkPanel>();

		base.Start();
	}


	/// <summary>
	/// 请求加入房间, 第一次先连接服务器端
	/// </summary>
	public void RequestJionRoom(bool create, int room_num) {
		/* 设置请求的具体内容 */
		StartCoroutine(IJionRoom(create, room_num));
	}

	IEnumerator IJionRoom(bool create, int room_num) {

		// Debug.Log(GetComponent<LinkRequest>().Connected);
		gameFacade.WaitResponse(true);
		yield return new WaitUntil(() => GetComponent<LinkRequest>().Connected);    // 等待连接成功(设定, 接收到第一条消息才算连接成功)
		if (create)
			gameFacade.ShowPromot("正在创建房间...");
		else
			gameFacade.ShowPromot("正在加入房间...");
		Debug.Log("请求加入房间");
		yield return new WaitForSeconds(1);


		/* 初始化要传递的数据 */
		PlayerInfo info = new PlayerInfo(players[0].Id, players[0].Name, players[0].Sex);      // 开始加入房间,
		info.other = GetComponent<LinkPanel>().RoomSence;       // 设置房间场景
		info.room_num = room_num;


		info.header = 0;        // 初始为默认头像

		Content send = new Content(ContentType.PlayerInfo, ActionCode.JoinRoom, RequestCode.Room);
		send.id = players[0].Id;    // 让大家知道是谁加入了房间
		send.content = JsonConvert.SerializeObject(info);

		SendRequest(send);                       // 连接后可以通信了, 发送申请加入房间申请


		yield return new WaitUntil(() => Response); // 收到响应后
		response = false;

		// yield return new WaitUntil(() => joinedCnt == _joinedCnt);   // 最新的加入请求

		yield return null;
		if (!Joined) {
			// gameFacade.ShowPromot("直接剩余取消请求");
			gameFacade.WaitResponse(false);
			yield break;                                            // 不是最新的加入请求，则退出
		}

		yield return new WaitForSeconds(0.5f);
		gameFacade.WaitResponse(false);

		// 加入成功
		if (create)
			gameFacade.ShowPromot("创建成功!等待其他玩家加入...");
		else
			gameFacade.ShowPromot("加入成功");
		Debug.Log(gameFacade.UIMng.dicPanels[UIPanelType.TopLayerPanel].GetComponent<TopLayerPanel>());

		gameFacade.UIMng.PopStack(UIPanelType.LinkPanel);
		gameFacade.UIMng.PopStack(UIPanelType.JoinRoomPanel);

		gameFacade.UIMng.PushStack(UIPanelType.RoomBG);

		gameFacade.PlayMusic(AudioType.MusicEx_Normal, false, true, true);      // 切换背景音乐

		Debug.Log("次数" + playerMng.players.Count);
		yield return new WaitUntil(() => playerMng.Full);       // 如果房间人数满了

		gameFacade.UIMng.dicPanels[UIPanelType.TopLayerPanel].GetComponent<TopLayerPanel>().SetDouNum(0);       // 修改自己界面的豆子值
		Debug.Log("房间人数已满");
		roombg.SetStartGameButton(true);
		roombg.GetOrder();                                      // 获取出牌顺序
	}


	/// <summary>
	/// 接收到有人加入房间的信息(3个人, 因此该响应会有3次)
	/// </summary>
	/// <param name="content"></param>
	public override void OnResponse(Content content) {
		base.OnResponse(content);
		Debug.Log("收到加入房间响应:" + content.returnCode);
		// Debug.Log(content.ToString());

		if (content.returnCode == ReturnCode.Success) {
			// gameFacade.ShowPromot("已进入房间, 载入房间场景...");
			PlayerInfo[] infos = JsonConvert.DeserializeObject<PlayerInfo[]>(content.content);

			if (infos.Length > 0) {
				string joinName = infos[infos.Length - 1].name;
				gameFacade.ShowPromot(joinName + " 加入了房间...");
				gameFacade.RecordLog(joinName + " 加入了房间...", true);
			}

			Debug.Log("我的账号id:" + players[0].Id);
			foreach (PlayerInfo info in infos)
				if (info.id == players[0].Id) {
					if (!players[0].joined) {
						players[0].joined = true;
						players[0].SetIndex(info.index);            // 获取网络index

						players[0].SetPlayer(false);
						playerMng.AddPlayer(players[0]);          // 加入房间

						// roombg.SetBGSprite(System.Convert.ToInt32(info.other));
					}
					Debug.Log("游戏场景为:" + System.Convert.ToInt32(info.other));
					roombg.SetBGSprite(System.Convert.ToInt32(info.other));
					// gameFacade.ShowPromot("游戏场景为:" + System.Convert.ToInt32(info.other));
				}

			int diff = (players[0].Index - players[0].local_index + players.Length) % players.Length;     // 计算网络Index与本地index的差值

			foreach (PlayerInfo info in infos) {
				if (info.id != players[0].Id) {         // 载入其他人的信息
					int i = (info.index - diff + players.Length) % players.Length;
					Debug.Log("玩家:" + i + players[i].Sex);

					if (!players[i].joined) {       // 该客户端第一次知道它加入
						players[i].joined = true;
						players[i].Name = info.name; players[i].Sex = info.sex;
						players[i].SetId(info.id);  // 设置Id
						players[i].SetIndex(info.index);            // 设置其他玩家的网络Index
						players[i].SetHeader(info.header);          // 设置其他玩家头像

						players[i].SetPlayer(false);
						playerMng.AddPlayer(players[i]);          // 加入房间
					}
				}
			}

			joined = true;
		} else if (content.returnCode == ReturnCode.Fail) {

			gameFacade.ShowPromot("加入房间失败, 房间不存在或房间已满!");
			gameFacade.WaitResponse(false);
		}
	}
}
