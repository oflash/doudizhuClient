using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoPanel : BasePanel
{

	public Transform Info => info == null ? info = transform.Find("Info") : info;
	Transform info;     // 玩家信息

	public Transform Goods => goods == null ? transform.Find("Goods") : goods;
	Transform goods;    // 物体

	public Transform Head => head == null ? transform.Find("head") : head;
	Transform head;         // 头像



	protected override void Start() {
		base.Start();
		uiPanelType = UIPanelType.PlayerInfoPanel;
		// Debug.Log(transform.Find("Info"));
	}

	public override void OnOpen(object obj = null) {
		base.OnOpen(obj);
		if (obj != null) {
			Player p = obj as Player;
			SetPlayerInfo(p);
			if (p.Id == GameFacade.Instance.Id) {
				Head.GetChild(0).GetComponent<Text>().text = "点击修改图片";
			} else {
				Head.GetChild(0).GetComponent<Text>().text = "点击查看图片";
			}
		}
	}

	void SetPlayerInfo(Player player) {
		Info.Find("id").Find("value").GetComponent<Text>().text = player.Id;
		Info.Find("name").Find("value").GetComponent<Text>().text = player.Name;
		Info.Find("sex").Find("value").GetComponent<Text>().text = player.Sex ? "男" : "女";
		Head.GetComponent<Image>().sprite = uiMng.GetSprite(SpriteType.Header, false, player.header);
	}

	public Sprite GetHeadSprite() {
		Sprite sprite = Head.GetComponent<Image>().sprite;

		return sprite;
	}



	public void OpenHeaderPanel_Click() {
		string thid = Info.Find("id").Find("value").GetComponent<Text>().text;

		bool changed = thid == GameFacade.Instance.Id ? true : false;   // 点击的是自己的头像
		uiMng.PushStack(UIPanelType.HeaderPanel, true, changed);
	}

	public void CloseButton_Click() {
		uiMng.PopStack(UIPanelType.PlayerInfoPanel);
	}
}
