using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MingCardPanel : BasePanel
{
	Transform card_Content;

	ScrollRect sr;
	GamePanel gamePanel;

	List<int> cardIds = new List<int>();        // 明牌地主的卡牌组id

	protected override void Start() {
		base.Start();
		card_Content = transform.Find("Viewport").Find("Content");
		gamePanel = transform.parent.GetComponent<GamePanel>();
		sr = GetComponent<ScrollRect>();

		OnInit();
	}

	/// <summary>
	/// 初始化，删除所有卡牌，新游戏开始
	/// </summary>
	public override void OnInit() {
		for (int i = 0; i < card_Content.childCount; i++)
			Destroy(card_Content.GetChild(i).gameObject);
		cardIds.Clear();
	}

	/// <summary>
	/// 明牌地主牌变化后, 修改明牌位置牌
	/// </summary>
	/// <param name="ids"></param>
	public List<Card> ShowCard_Content(int[] ids) {

		// 先清理原来的牌
		for (int i = 0; i < card_Content.childCount; i++)
			Destroy(card_Content.GetChild(i).gameObject);

		// 删除地主出的牌, 计算地主剩余的牌
		for (int i = 0; i < ids.Length; i++) {
			if (cardIds.Contains(ids[i]))
				cardIds.Remove(ids[i]);
			else
				cardIds.Add(ids[i]);
		}

		List<Card> cards = gamePanel.GenerateCards(card_Content, cardIds.ToArray());
		return cards;
	}

}
