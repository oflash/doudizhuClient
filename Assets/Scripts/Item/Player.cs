using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

	#region Player属性
	public bool Sex { get => sex; set => sex = value; }         // 性别	-->	连接之前	(本地)
	private bool sex;

	public string Name { get => _name; set => _name = value; } // 姓名 -->	连接之前	(本地)
	private string _name;

	public string Id => id;         // 账号	-->	连接之后收到第一条服务器消息后		(网络)
	private string id;

	public int Index => _index;     // 本局游戏的index(0,1,2)	-->	加入房间之后, 首先被设置index	(网络)
	private int _index;

	public bool Landowner => landowner;  // 地主	-->	每局游戏开始之后	(网络)
	private bool landowner;

	public int header { get; set; }     // 头像索引

	#endregion

	#region Unity编辑器上可见的变量
	public int local_index;   // 以我为视角的第几个位置(0, 1, 2), 不用代码设置, 在unity编辑器设置
	public bool joined; // 该位置是否已经有加入了

	#endregion


	Animator ani => parent.GetChild(0).GetComponent<Animator>();
	GameFacade gameFacade => GameFacade.Instance;
	UIManager UIMng => GameFacade.Instance.UIMng;
	public PrefabType playerType;
	PlayerManager playerMng => GameFacade.Instance.PlayerMng;
	Transform parent, readyTagPanel;
	GameObject textBox, emoBox;
	private void Start() {
		parent = transform.Find("Parent");
		readyTagPanel = transform.parent.Find("ReadyTagPanel");
		emoBox = transform.Find("EmoChatBox").gameObject;
		textBox = transform.Find("TextChatBox").gameObject;
		emoBox.SetActive(false);
		textBox.SetActive(false);
	}

	public override string ToString() {
		// return base.ToString();
		return "账号:" + id + "\t" +
				"姓名:" + _name + "\t" +
				"性别:" + sex + "\t" +
				"位置:" + local_index + "\t" +
				"地主:" + landowner;
	}



	public void SetPlayerInfo(PlayerInfo info) {
		this.id = info.id;
		this.Name = info.name;
		this.header = info.header;
		this.sex = info.sex;
		this._index = info.index;
	}


	public void SetHeader(int header) => this.header = header;
	public void SetId(string id) => this.id = id;
	public void SetIndex(int index) => this._index = index;
	public void SetLandowner(bool b) => this.landowner = b;

	/// <summary>
	/// 生成该位置的人物头像, 加入房间, 重新开始游戏
	/// </summary>
	public void SetPlayer(bool landowner) {
		GetComponent<Button>().enabled = true;      // 可以点击查看属性
		GameObject go;
		if (parent.childCount > 0) {
			for (int i = 0; i < parent.childCount; i++)
				Destroy(parent.GetChild(i).gameObject);
		}

		if (!landowner) {

			if (sex) {   // 男角色
				go = Instantiate(UIMng.GetPrefab(PrefabType.MPlayer0), parent);
				playerType = PrefabType.MPlayer0;
			} else {   // 女角色
				go = Instantiate(UIMng.GetPrefab(PrefabType.WPlayer0), parent);
				playerType = PrefabType.WPlayer0;
			}
		} else {
			if (sex) {
				go = Instantiate(UIMng.GetPrefab(PrefabType.MPlayer1), parent);
				playerType = PrefabType.MPlayer1;
			} else {
				go = Instantiate(UIMng.GetPrefab(PrefabType.WPlayer1), parent);
				playerType = PrefabType.WPlayer1;
			}
		}
	}



	/// <summary>
	/// 准备游戏
	/// </summary>
	public void ReadyGame() {
		// Debug.Log("显示Ready");
		readyTagPanel.localScale = Vector3.one;
		readyTagPanel.Find("ReadyTag" + local_index).localScale = Vector3.one;
	}


	/// <summary>
	/// 聊天, 展示UI面板上的聊天信息
	/// </summary>
	/// <param name="s"></param>
	/// <param name="audio"></param>
	public void Chat(ChatInfo chatInfo) {
		// string chat = chatInfo.chat;
		if (chatInfo.chatType == 2) {   // 表情
			emoBox.SetActive(false);
			emoBox.SetActive(true);
			emoBox.transform.GetChild(0).GetComponent<Animator>().enabled = true;
			int index = int.Parse(chatInfo.other);
			StartCoroutine(AudoHide(true, 3, ++emoCnt));
			emoBox.transform.GetChild(0).GetComponent<Animator>().SetInteger("emo", index);

			gameFacade.RecordLog(Name + ":表情" + index, true);
		} else {                        // 文字
			textBox.SetActive(true);
			textBox.transform.GetChild(0).GetComponent<Text>().text = chatInfo.chat;
			StartCoroutine(AudoHide(false, 2, ++textCnt));
			if (chatInfo.chatType == 1) {
				// Debug.Log(chatInfo.other);
				// Debug.Log((AudioType)Enum.Parse(typeof(AudioType), chatInfo.other));
				AudioType audio = Audio.GetChatAudio(int.Parse(chatInfo.other), Sex);
				gameFacade.PlayMusic(audio);
			}

			gameFacade.RecordLog(Name + ":" + chatInfo.chat, true);
		}
	}
	int emoCnt = 0, textCnt = 0;
	IEnumerator AudoHide(bool emo, float time, int cnt) {

		yield return new WaitForSeconds(time);
		if (emo && emoCnt == cnt) emoBox.SetActive(false);
		if (!emo && textCnt == cnt) textBox.SetActive(false);
	}


	/// <summary>
	/// 人物笑, 大过别人牌后, 游戏胜利后
	/// </summary>
	public void PlayerLaunch() {
		ani.SetBool("IsLaugh", true);
		ani.SetBool("IsWeep", false);
	}

	/// <summary>
	/// 人物哭, 游戏失败后
	/// </summary>
	public void PlayerWeep() {
		ani.SetBool("IsWeep", true);
		ani.SetBool("IsLaugh", false);
	}

	/// <summary>
	/// 人物Idle, 没有大过别人牌后
	/// </summary>
	public void PlayerIdle() {
		ani.SetBool("IsLaugh", false);
		ani.SetBool("IsWeep", false);
	}


	/// <summary>
	/// 攻击
	/// </summary>
	/// <param name="target"></param>
	public void Attack(Player target) {

	}


	/// <summary>
	/// 要不起
	/// </summary>
	public void Pass() {

	}

	/// <summary>
	/// 出牌
	/// </summary>
	public void OutCard() {

	}
}
