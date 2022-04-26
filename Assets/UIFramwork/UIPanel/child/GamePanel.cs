using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
	public List<Card> my_cards;
	public List<Card> dizhu_cards;

	public Transform poker0H;       // 主玩家
	public Transform poker1H, poker2H;
	public Transform poker3;                            // 地主牌
	public PokerSPanel[] pokerS;

	public MingCardPanel[] mingCardPanels;

	public Text doubleTxt;                              // 本局倍数
	public bool IsMing { get; set; }    // 是否明牌


	protected override void Start() {
		base.Start();
		uiPanelType = UIPanelType.GamePanel;
		OnInit();
	}

	public override void OnInit() {
		poker1H.localScale = Vector3.zero;
		poker2H.localScale = Vector3.zero;
		doubleTxt.text = "5";
		dizhu_cards.Clear();
		my_cards.Clear();
		IsMing = false;

		poker1H.localScale = Vector3.zero;
		poker2H.localScale = Vector3.zero;
		for (int i = 0; i < poker3.childCount; i++) {
			Destroy(poker3.GetChild(i).gameObject);
		}
		for (int i = 0; i < poker0H.childCount; i++) {
			Destroy(poker0H.GetChild(i).gameObject);
		}

	}

	public override void OnOpen(object obj = null) {
		// base.OnOpen(obj);
	}

	#region 数据
	/// <summary>
	/// 发牌请求
	/// </summary>
	public void RequestDivide() {
		GetComponent<DivideRequest>().RequestDivide();
	}

	/// <summary>
	/// 发送胜利请求
	/// </summary>
	public void RequestWin() {
		GetComponent<WinRequest>().RequestWin();
	}

	/// <summary>
	/// 本局游戏结束
	/// /// </summary>
	public void RequestResult(bool win) {
		GetComponent<ResultRequest>().RequestResult(win);   // 将自己的计算结果发送给服务器
	}

	/// <summary>
	/// 将我剩余的牌发送给大家
	/// </summary>
	public void RequestRemaining() {
		List<int> ids = new List<int>();
		foreach (Card card in my_cards) {
			ids.Add(card.Id);
		}
		GetComponent<RemainingRequest>().RequestRemaining(ids.ToArray());
	}


	/// <summary>
	/// 获取卡牌组的id组
	/// </summary>
	/// <param name="cards"></param>
	/// <returns></returns>
	public int[] GetIds(List<Card> cards) {
		List<int> ids = new List<int>();
		foreach (Card card in cards)
			ids.Add(card.Id);
		return ids.ToArray();
	}


	#endregion

	#region 游戏中_UI

	/// <summary>
	/// 设置加倍_加法_UI
	/// </summary>
	/// <param name="add"></param>
	public void AddDouble(int add) {
		int m;
		try {
			m = int.Parse(doubleTxt.text);
			m += add;
		} catch (System.Exception) {
			m = 0;
		}
		doubleTxt.text = m.ToString();
	}

	/// <summary>
	/// 设置加倍_乘法_UI
	/// </summary>
	/// <param name="mul"></param>
	public void MulDouble(int mul) {
		int m;
		try {
			m = int.Parse(doubleTxt.text);
			m *= mul;
		} catch (System.Exception) {
			m = 0;
		}
		doubleTxt.text = m.ToString();
	}


	/// <summary>
	/// 点击桌面空白处, 卡牌隐藏
	/// </summary>
	public void OnClickResume() {
		for (int i = 0; i < poker0H.childCount; i++) {
			poker0H.GetChild(i).GetComponent<Card>().ResumeState();
		}
	}

	/// <summary>
	///  在UIPanel上生成Card, 非延迟
	/// </summary>
	/// <param name="parent"></param>
	/// <param name="cardIds"></param>
	/// <param name="back">是否只显示背景</param>
	/// <returns></returns>
	public List<Card> GenerateCards(Transform parent, int[] cardIds, bool back = false) {
		List<Card> cards = new List<Card>();
		Card cardPrefab = uiMng.GetPrefab(PrefabType.Card).GetComponent<Card>();

		foreach (int item in cardIds) {
			Card c = Instantiate<Card>(cardPrefab, parent);
			c.Id = item;
			cards.Add(c);
			c.transform.GetChild(0).GetComponent<Image>().sprite =
				uiMng.GetSprite(SpriteType.Poker, false, back ? 0 : item);
		}
		cards.Sort(new myComparer());
		foreach (Card card in cards) {
			card.transform.SetParent(null);
			card.transform.SetParent(parent);
			card.transform.localScale = Vector3.one;
		}
		return cards;
	}




	#endregion





	#region 初始收到发送过来的卡牌_UI
	/// <summary>
	/// 初始获取卡牌, 包括地主牌(显示背面)
	/// </summary>
	public void GetInitCard(int[] my_ids, int[] dizhu_ids) {
		Debug.Log(my_ids.Length);
		/* 我的牌 */
		// int[] my_ids = new int[17];
		StartCoroutine(_GetInitCard(my_ids));           // 获取初始牌

		/* 地主牌, 显示背面 */
		// int[] dizhu_ids = new int[3];
		dizhu_cards = GenerateCards(poker3, dizhu_ids, true);
	}
	IEnumerator _GetInitCard(int[] my_ids) {
		GameFacade.Instance.WaitResponse(true);     // 等待发牌
		if (my_cards == null) my_cards = new List<Card>();
		Card card = uiMng.GetPrefab(PrefabType.Card).GetComponent<Card>();
		for (int i = 0; i < my_ids.Length; i++) {
			Card c = Instantiate<Card>(card);
			c.Id = my_ids[i];
			c.transform.GetChild(0).GetComponent<Image>().sprite =
				uiMng.GetSprite(SpriteType.Poker, false, my_ids[i]);
			my_cards.Add(c);
			my_cards.Sort(new myComparer());
			foreach (Card item in my_cards) {
				// item.transform.parent = null;
				item.transform.SetParent(null);
				// item.transform.parent = poker0H;
				item.transform.SetParent(poker0H);
				item.transform.localScale = Vector3.one;
			}
			int loop = 8;
			while (loop-- > 0) yield return null;
		}
		GameFacade.Instance.WaitResponse(false);
		foreach (Card c in my_cards) {
			c.ResumeState();
		}
	}
	#endregion

	#region 卡牌的增加与删除_UI
	/// <summary>
	/// 翻转地主卡牌
	/// </summary>
	public void ReverseDiZhuCard() {
		ReverseCard(dizhu_cards, false);
	}


	/// <summary>
	/// 设置其他玩家剩余卡牌数量
	/// </summary>
	/// <param name="local_index"></param>
	/// <param name="num"></param>
	/// <param name="sub">如果是用现在的减去传过来的</param>
	public void SetOtherPlayerCardNumber(int local_index, int num, bool sub = false) {
		Transform target = local_index == 1 ? poker1H : poker2H;
		target.localScale = Vector3.one;
		if (sub) {
			int ori = int.Parse(target.GetChild(0).GetComponent<Text>().text);
			num = ori - num;
		}
		target.GetChild(0).GetComponent<Text>().text = num.ToString();
	}
	public void SetOtherPlayerCardNumber(Transform target, int num, bool sub = false) {
		target.localScale = Vector3.one;
		if (sub) {
			int ori = int.Parse(target.GetChild(0).GetComponent<Text>().text);
			num = ori - num;
		}
		target.GetChild(0).GetComponent<Text>().text = num.ToString();
	}

	/// <summary>
	/// 获取其他玩家卡牌数量
	/// </summary>
	/// <param name="local_index"></param>
	/// <returns></returns>
	public int GetOtherPlayerCardNumber(int local_index) {
		Transform target = local_index == 1 ? poker1H : poker2H;
		return int.Parse(target.GetChild(0).GetComponent<Text>().text);
	}



	/// <summary>
	/// 往区域增加卡牌
	/// </summary>
	/// <param name="target"></param>
	/// <param name="add_ids"></param>
	/// <param name="cards"></param>
	public void AddMyCard(Transform target, int[] add_ids, List<Card> cards = null) {
		List<Card> new_cards = GenerateCards(target, add_ids, false);
		if (cards != null) {            // 如果是添加到
			foreach (Card card in new_cards) cards.Add(card);
			cards.Sort(new myComparer());
			foreach (Card card in cards) {
				card.transform.SetParent(null);
				card.transform.SetParent(target);
				card.transform.localScale = Vector3.one;
			}
			// foreach (Card card in cards) card.ResumeState();
		}
	}

	/// <summary>
	/// 删除目标区域的卡牌
	/// </summary>
	/// <param name="target"></param>
	public void RemoveMyCard(Transform target) {
		for (int i = 0; i < target.childCount; i++) {
			Destroy(target.GetChild(i).gameObject);
		}
	}

	#endregion

	#region 私有方法_UI
	/// <summary>
	/// 翻转卡牌
	/// </summary>
	/// <param name="my_cards"></param>
	/// <param name="back">显示背面</param>
	private void ReverseCard(List<Card> my_cards, bool back = false) {
		foreach (Card card in my_cards) {
			card.transform.GetChild(0).GetComponent<Image>().sprite =
				uiMng.GetSprite(SpriteType.Poker, false, back ? 0 : card.Id);
		}
	}
	#endregion



}
