using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutCardPanel : BasePanel
{

	Transform noOut;        // 不出
	Transform outCard;      // 出牌
	Transform mingCard;     // 明牌
	Transform double_0;     // 不加倍
	Transform double_1;     // 加倍
	Transform double_2;     // 超级加倍
	PlayerTiming timing;    // 倒计时

	GamePanel gamePanel;
	protected override void Start() {
		base.Start();
		timing = transform.Find("Timing").GetComponent<PlayerTiming>();
		noOut = transform.Find("NoOut");
		outCard = transform.Find("OutCard");
		mingCard = transform.Find("MingCard");
		double_0 = transform.Find("Double_0");
		double_1 = transform.Find("Double_1");
		double_2 = transform.Find("Double_2");
		gamePanel = transform.parent.Find("GamePanel").GetComponent<GamePanel>();

		OnInit();
	}


	public int cnt_pass { get; set; }       // 统计连续不要次数
	public List<Card> prev;

	public override void OnInit() {
		ClosePanel();
		cnt_pass = 0;
		prev = null;
	}


	#region 显示Button面板内容

	/// <summary>
	///  抢完地主后显示是否加倍
	/// </summary>
	public void One_Double() {
		ChangedMingCard(false);     // 关闭明牌
		ChangedNoOut(false);        // 关闭不出
		ChangedDouble_0(true);      // 显示不加倍
		ChangedOutCard(false);      // 关闭出牌
		ChangedDouble_1(true);      // 显示加倍
		ChangedDouble_2(true);      // 显示超级加倍
		ChangedTiming(true, 10, () => { OnClickDouble(1); });    // 倒计时10秒, 自动不加倍
	}

	/// <summary>
	/// 第一次地主出牌, 显示明牌和出牌
	/// </summary>
	public void One_DiZhuOut() {
		ChangedMingCard(true);      // 显示明牌
		ChangedNoOut(false);        // 关闭不出
		ChangedOutCard(true);       // 显示出牌
		ChangedDouble_0(false);     // 关闭加倍
		ChangedDouble_1(false);     // 关闭加倍
		ChangedDouble_2(false);     // 关闭超级加倍
		ChangedTiming(true, 30, DefaultOutCard);    // 倒计时30秒, 自动出最小的牌
	}

	/// <summary>
	/// 点击明牌后,隐藏明牌按钮
	/// </summary>
	public void One_EnterMingCard(){
		ChangedMingCard(false);     // 关闭明牌
	}


	/// <summary>
	/// 一轮的起点出牌, 只显示出牌
	/// </summary>
	public void One_OutCard() {
		ChangedMingCard(false);     // 关闭明牌
		ChangedNoOut(false);        // 关闭不出
		ChangedOutCard(true);       // 显示出牌
		ChangedDouble_0(false);     // 关闭加倍
		ChangedDouble_1(false);     // 关闭加倍
		ChangedDouble_2(false);     // 关闭超级加倍
		gamePanel.pokerS[0].OnInit();   // 关掉上一次出牌的内容
		ChangedTiming(true, 20, DefaultOutCard);    // 倒计时20秒, 自动出最小的牌
	}

	/// <summary>
	/// 一轮的非起点出牌, 显示出牌和不出
	/// </summary>
	public void Two_OutCard() {
		ChangedMingCard(false);     // 关闭明牌
		ChangedNoOut(true);         // 显示不出
		ChangedOutCard(true);       // 显示出牌
		ChangedDouble_0(false);     // 关闭加倍
		ChangedDouble_1(false);     // 关闭加倍
		ChangedDouble_2(false);     // 关闭超级加倍
		gamePanel.pokerS[0].OnInit();
		ChangedTiming(true, 20, OnClickNoOut);    // 倒计时20秒, 自动不出
	}

	/// <summary>
	/// 隐藏Panel
	/// </summary>
	public void ClosePanel() {
		transform.localScale = Vector3.zero;
		timing.EndTiming();
	}

	#endregion


	/// <summary>
	/// 发送加倍Text信息
	/// </summary>
	/// <param name="num"></param>
	public void SendDouble(int local_index, int num) {

		string s = "";
		if (num == 1) {
			s = "NoJiaBei";

		} else if (num == 2) {
			s = "JiaBei";
		} else if (num == 4) {
			s = "SuperJiaBei";
		}
		gamePanel.pokerS[local_index].ShowText_Content(s);
	}

	/// <summary>
	/// 发送不出信息
	/// </summary>
	/// <param name="index"></param>
	public void SendNoOut(int local_index) {
		gamePanel.pokerS[local_index].ShowText_Content("NoOut");
	}

	/// <summary>
	/// 发送倒计时信息
	/// </summary>
	/// <param name="index"></param>
	public void SendTiming(int local_index) {
		gamePanel.pokerS[local_index].ShowText_Content("Timing");
	}




	#region 点击事件

	/// <summary>
	/// 点击不出
	/// </summary>
	public void OnClickNoOut() {
		GetComponent<NoOutRequest>().RequestNoOut();
	}

	/// <summary>
	/// 点击出牌
	/// </summary>
	public void OnClickOutCard() {
		List<Card> outs = new List<Card>();
		for (int i = 0; i < gamePanel.poker0H.childCount; i++) {
			Card c = gamePanel.poker0H.GetChild(i).GetComponent<Card>();
			if (c.Selected)
				outs.Add(c);
		}
		outs.Sort(new myComparer());
		GetComponent<OutCardRequest>().RequestOutCard(outs);

		Debug.Log(Rule.GameRule(outs));
	}

	/// <summary>
	/// 默认出牌
	/// </summary>
	void DefaultOutCard() {
		List<Card> outs = new List<Card>();
		int len = gamePanel.poker0H.childCount;
		if (len > 0) {
			Card c = gamePanel.poker0H.GetChild(len - 1).GetComponent<Card>();
			outs.Add(c);
		}
		outs.Sort(new myComparer());
		GetComponent<OutCardRequest>().RequestOutCard(outs);
	}



	/// <summary>
	/// 点击加倍或超级加倍
	/// </summary>
	/// <param name="num">表示增加的倍数</param>
	public void OnClickDouble(int num) {
		int a = int.Parse(gamePanel.doubleTxt.text);      // 原始倍数

		// 小3位表示倍数, 其余的表示原始倍数
		a <<= 3;
		a |= num;
		GetComponent<DoubleRequest>().RequestDouble(a);   // 发送请求
	}


	/// <summary>
	/// 点击明牌
	/// </summary>
	public void OnClickMingCard() {
		// GameFacade.Instance.ShowPromot("地主明牌了！");
		// GetComponent<MingCardRequest>().
		List<Card> outs = new List<Card>();

		for (int i = 0; i < gamePanel.poker0H.childCount; i++) {
			Card c = gamePanel.poker0H.GetChild(i).GetComponent<Card>();
			outs.Add(c);
		}

		outs.Sort(new myComparer());
		GetComponent<MingCardRequest>().RequestMingCard(outs);
	}

	#endregion






	#region 显示还是隐藏Button

	private void ChangedTiming(bool show, int time, Action action) {
		transform.localScale = Vector3.one;
		timing.gameObject.SetActive(show);
		if (show) timing.BeginTiming(time, action);
	}
	private void ChangedNoOut(bool show) {
		transform.localScale = Vector3.one;
		noOut.gameObject.SetActive(show);
	}

	private void ChangedOutCard(bool show) {
		transform.localScale = Vector3.one;
		outCard.gameObject.SetActive(show);
	}
	private void ChangedMingCard(bool show) {
		transform.localScale = Vector3.one;
		mingCard.gameObject.SetActive(show);
	}
	private void ChangedDouble_0(bool show) {
		transform.localScale = Vector3.one;
		double_0.gameObject.SetActive(show);
	}
	private void ChangedDouble_1(bool show) {
		transform.localScale = Vector3.one;
		double_1.gameObject.SetActive(show);
	}
	private void ChangedDouble_2(bool show) {
		transform.localScale = Vector3.one;
		double_2.gameObject.SetActive(show);
	}
	#endregion




}
