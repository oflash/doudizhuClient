using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeaderPanel : BasePanel
{

	PlayerInfoPanel playerInfoPanel => (uiMng.GetPanel(UIPanelType.PlayerInfoPanel) as PlayerInfoPanel);

	protected override void Start() {
		base.Start();
		uiPanelType = UIPanelType.HeaderPanel;

	}

	Transform bigHead => transform.Find("Viewport").Find("BigHead");
	Transform changeHead => transform.Find("Viewport").Find("ChangeHead");
	public override void OnOpen(object obj = null) {
		base.OnOpen(obj);
		if (obj != null) {
			bool changed = System.Convert.ToBoolean(obj);
			if (changed) {   // 打开自己的
				changeHead.localScale = Vector3.one;
				bigHead.localScale = Vector3.zero;
			} else {    // 打开别人的
				changeHead.localScale = Vector3.zero;
				bigHead.localScale = Vector3.one;
				bigHead.GetChild(0).GetComponent<Image>().sprite = playerInfoPanel.Head.GetComponent<Image>().sprite;
				
			}
		}
	}


	public void OnSelectHeader(Transform item) {
		uiMng.PopStack(UIPanelType.HeaderPanel);
		int index = int.Parse(item.name.Substring(0, 2));

		GetComponent<HeaderRequest>().RequestHeader(index);

		playerInfoPanel.Head.GetComponent<Image>().sprite = uiMng.GetSprite(SpriteType.Header, false, index);
	}


	/// <summary>
	/// 查看别人的头像, 关闭按钮
	/// </summary>
	public void CloseHeader_Click() {
		uiMng.PopStack(UIPanelType.HeaderPanel);
	}


}
