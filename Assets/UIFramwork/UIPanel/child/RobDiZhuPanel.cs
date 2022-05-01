using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobDiZhuPanel : BasePanel
{
	Transform noRob;
	Transform robDiZhu;
	GamePanel gamePanel;
	PlayerTiming timing;

	protected override void Start() {
		base.Start();
		noRob = transform.Find("NoRob");
		robDiZhu = transform.Find("RobDiZhu");
		timing = transform.Find("Timing").GetComponent<PlayerTiming>();
		gamePanel = transform.parent.Find("GamePanel").GetComponent<GamePanel>();


		OnInit();
	}

	public override void OnInit() {
		ClosePanel();
	}


	#region 修改文字
	private void OpenCallDiZhu() {
		transform.localScale = Vector3.one;
		robDiZhu.Find("Call").localScale = Vector3.one;
		robDiZhu.Find("Rob").localScale = Vector3.zero;
	}

	private void OpenRobDizhu() {
		transform.localScale = Vector3.one;
		robDiZhu.Find("Call").localScale = Vector3.zero;
		robDiZhu.Find("Rob").localScale = Vector3.one;
	}

	private void OpenNoRob() {
		transform.localScale = Vector3.one;
		noRob.Find("Rob").localScale = Vector3.one;
		noRob.Find("Call").localScale = Vector3.zero;
	}
	private void OpenNoCall() {
		transform.localScale = Vector3.one;
		noRob.Find("Rob").localScale = Vector3.zero;
		noRob.Find("Call").localScale = Vector3.one;
	}
	#endregion


	/// <summary>
	/// 隐藏Panel
	/// </summary>
	public void ClosePanel() {
		transform.localScale = Vector3.zero;
	}


	/// <summary>
	/// 展示抢地主
	/// </summary>
	public void ShowRobDiZhu() {
		OpenRobDizhu();
		OpenNoRob();
		timing.BeginTiming(10, () => { OnClickRobDiZhu(false); });      // 10秒后自动不抢
	}

	/// <summary>
	/// 展示叫地主Panel
	/// </summary>
	public void ShowCallDiZhu() {
		OpenCallDiZhu();
		OpenNoCall();
		timing.BeginTiming(10, () => { OnClickRobDiZhu(false); });      // 10秒后自动不叫
	}

	/// <summary>
	/// 发送抢(叫)地主信息
	/// </summary>
	/// <param name="index"></param>
	/// <param name="rob"></param>
	public void SendRobDiZhu(int local_index, bool rob) {
		string s = rob ? "RobDiZhu" : "CallDiZhu";
		gamePanel.pokerS[local_index].ShowText_Content(s);
	}

	/// <summary>
	/// 发送不抢(叫)信息
	/// </summary>
	/// <param name="index"></param>
	/// <param name="rob"></param>
	public void SendNoRob(int local_index, bool rob) {
		string s = rob ? "NoRob" : "NoCall";
		gamePanel.pokerS[local_index].ShowText_Content(s);
	}




	#region 点击事件

	public void OnClickRobDiZhu(bool rob) {
		timing.EndTiming();		// 关闭自动不抢
		GetComponent<RobDiZhuRequest>().RequestRobDiZhu(rob);
	}

	#endregion




}
