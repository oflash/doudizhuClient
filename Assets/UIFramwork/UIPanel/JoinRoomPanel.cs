using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinRoomPanel : BasePanel
{

	protected override void Start() {
		base.Start();
		uiPanelType = UIPanelType.JoinRoomPanel;
	}


	#region Button点击事件

	public void OnCreateRoomClick() {
		uiMng.PushStack(UIPanelType.SelectScenePanel);      // 选择场景

	}

	public void OnJoinRoomClick() {
		uiMng.PushStack(UIPanelType.LinkPanel, true, -1);    // 设置加入别人创建的房间
	}

	public void OnExitGameClick() {
		Debug.Log("退出游戏");
		Application.Quit();
	}

	#endregion

}
