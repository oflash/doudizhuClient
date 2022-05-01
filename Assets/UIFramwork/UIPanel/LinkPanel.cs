using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinkPanel : BasePanel
{
	int _create = -1;   // 创建房间还是加入房间
	InputField inputIP, inputPort, inputName;

	Player player => (uiMng.GetPanel(UIPanelType.RoomBG) as RoomBG).players[0]; // 本地玩家
	Toggle netToggle;

	#region 需要传递出去的信息
	public int RoomSence => _create;
	bool Sex => transform.Find("MPlayer").GetComponent<Toggle>().isOn;

	string Name;

	#endregion


	string ip;
	int port;
	int room_num;   // 如果使用公网，则会有房间号


	protected override void Start() {
		base.Start();
		uiPanelType = UIPanelType.LinkPanel;
		inputIP = transform.Find("InputIP").GetComponent<InputField>();
		inputPort = transform.Find("InputPort").GetComponent<InputField>();
		inputName = transform.Find("InputName").GetComponent<InputField>();
		netToggle = transform.Find("NetToggle").GetComponent<Toggle>();
		inputIP.text = "";
		inputPort.text = "";
		inputName.text = "";

#if UNITY_EDITOR
		netToggle.gameObject.SetActive(true);
		inputIP.text = "127.0.0.1";
		inputPort.text = "9876";
		inputName.text = "player" + Random.Range(100, 1000);
#elif UNITY_STANDALONE_WIN
		netToggle.gameObject.SetActive(true);
#elif UNITY_ANDROID
		netToggle.gameObject.SetActive(false);		// 安卓平台不使用局域网
#endif

	}

	public override void OnOpen(object obj = null) {
		base.OnOpen(obj);
		if (obj != null)
			_create = (int)obj;    // 进入之前确定是加入房间还是创建房间

		// Debug.Log(_create);
	}

	public override void OnClose(object obj = null) {
		base.OnClose(obj);
	}



	#region UI点击事件

	/// <summary>
	/// Toggle选择性别
	/// </summary>
	/// <param name="toggle"></param>
	public void OnToggle(Toggle toggle) {

		if (toggle.isOn == false) {
			toggle.GetComponent<Image>().color = Color.black;
		} else {
			toggle.GetComponent<Image>().color = Color.white;
		}
		// Debug.Log(Sex);
	}


	public void OnEnterClick() {
		bool success = true;
		string prompt = "连接成功";
		ip = inputIP.text;
		Name = inputName.text;
		string portStr = inputPort.text;
		try {
			if (string.IsNullOrEmpty(Name)) {
				int a = port / 0;           // 使报除0异常
			}

			if (portStr.Length == 4) {
				port = int.Parse(portStr);
				room_num = 0;
				PlayerInfo info = new PlayerInfo("None", Name, Sex);
				info.other = "这是我的基本信息, 现在我要连接服务器, 请告诉我的id";

				// 使用公网
				if (!netToggle.isOn) {
					room_num = port;
					ip = "127.0.0.1";
#if UNITY_ANDROID || UNITY_STANDALONE_WIN
					// ip = "47.110.129.92";
					ip = "43.138.162.204";
#endif
				//	ip = "127.0.0.1";
					port = 9876;
				}

				if (_create != -1) {      // 如果是创建房间, _create 就是房间场景编号

					if (netToggle.isOn) {   // 如果还是局域网内
						GetComponent<LinkRequest>().OperServer(ip, port);       // 打开服务器并连接
						GetComponent<JoinRoomRequest>().RequestJionRoom(true, room_num);      // 请求加入房间
					} else {    // 使用公网
						GetComponent<LinkRequest>().LinkServer(ip, port);       // 连接服务器
						GetComponent<CreateRoomRequest>().RequestCreateRoom(room_num, _create);  // 创建房间, 成功后自动加入

					}
				} else {
					GetComponent<LinkRequest>().LinkServer(ip, port);       // 连接服务器

					GetComponent<JoinRoomRequest>().RequestJionRoom(false, room_num);      // 请求加入房间
				}


				// uiMng.ConnectServer(ip, port);      // 发送请求(加入房间)
			} else {
				success = false;
				prompt = "房间号必须是4位数字!";
			}

		} catch (System.DivideByZeroException) {  // 除0异常
			success = false;
			prompt = "姓名不能为空";
		} catch (System.FormatException) {
			success = false;
			prompt = "房间号必须是4位数字!";
			// Debug.Log(e);
		} catch (System.Net.Sockets.SocketException) {
			success = false;
			// Debug.Log(e);
			prompt = "连接失败";
		}

		if (!success) {
			// GameFacade.Instance.WaitResponse(false);                    // 停止等待
			uiMng.PushStack(UIPanelType.PromptPanel, false, prompt);    // 显示错误提示信息
		}

		// 没有报错
		if (success) {
			player.Name = Name; player.Sex = Sex;           // 设置player属性

			// uiMng.PopStack(UIPanelType.LinkPanel);
			// uiMng.PushStack(UIPanelType.LoadingPanel);      // 连接成功后, 等待3人都连接

			// Content content = new Content(ContentType.Chat);
			// content.content = "我连接成功了";
			// uiMng.SendRequest(content);
		}


	}

	public void OnCloseClick() {
		uiMng.PopStack(UIPanelType.LinkPanel);
	}


	/// <summary>
	/// 点击局域网的toggle
	/// </summary>
	/// <param name="value"></param>
	public void OnNetToggle(bool value) {
		Debug.Log(value);
		// 选择局域网
		if (value) {
			inputIP.gameObject.SetActive(true);
		} else {
			inputIP.gameObject.SetActive(false);
		}
	}

	#endregion
}
