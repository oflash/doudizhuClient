using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogPanel : BasePanel
{
	// 该面板为RoomBG子面板, 不需要纳入UI框架

	RoomBG roomBG;
	Transform contentUI;        // text父物体
	GameObject textPrefab;

	protected override void Start() {
		roomBG = transform.parent.GetComponent<RoomBG>();
		contentUI = transform.Find("Scroll View").GetChild(0).GetChild(0);
		textPrefab = contentUI.GetChild(0).gameObject;
		OnInit();
	}


	/// <summary>
	/// 初始化,:关闭面板, 清零
	/// </summary>
	public override void OnInit() {
		transform.localScale = Vector3.zero;

		for (int i = 1; i < contentUI.childCount; i++) {
			Destroy(contentUI.GetChild(i).gameObject);
		}
	}


	/// <summary>
	/// 产生新的日志
	/// </summary>
	/// <param name="s"></param>
	public void CreateNewLog(string s) {
		Text text = Instantiate(textPrefab, contentUI).GetComponent<Text>();

		text.alignment = TextAnchor.MiddleLeft;
		text.text = s;
	}


}
