using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 发送卡牌等信息的展示区域
/// </summary>
public class PokerSPanel : BasePanel
{
	Transform card_Content, text_Content;
	ScrollRect sr;
	GamePanel gamePanel;


	protected override void Start() {
		base.Start();
		card_Content = transform.Find("Viewport").Find("CardContent");
		text_Content = transform.Find("Viewport").Find("TextContent");
		gamePanel = transform.parent.parent.GetComponent<GamePanel>();
		sr = GetComponent<ScrollRect>();

		OnInit();
	}

	/// <summary>
	/// 删除发送区域的所有东西, 新游戏或轮到出牌了
	/// </summary>
	public override void OnInit() {
		for (int i = 0; i < card_Content.childCount; i++)
			Destroy(card_Content.GetChild(i).gameObject);
		for (int i = 0; i < text_Content.childCount; i++)
			text_Content.GetChild(i).gameObject.SetActive(false);
	}


	/// <summary>
	/// 展示文字信息
	/// </summary>
	/// <param name="text"></param>
	public void ShowText_Content(string text) {

		card_Content.gameObject.SetActive(false);
		text_Content.gameObject.SetActive(true);
		sr.content = text_Content.GetComponent<RectTransform>();
		for (int i = 0; i < text_Content.childCount; i++) {
			if (text_Content.GetChild(i).name == text)
				text_Content.GetChild(i).gameObject.SetActive(true);
			else
				text_Content.GetChild(i).gameObject.SetActive(false);
		}
	}

	public List<Card> ShowCard_Content(int[] ids) {
		card_Content.gameObject.SetActive(true);
		text_Content.gameObject.SetActive(false);
		sr.content = card_Content.GetComponent<RectTransform>();

		// 先删除该位置上原有的Card
		for (int i = 0; i < card_Content.childCount; i++)
			Destroy(card_Content.GetChild(i).gameObject);
			
		List<Card> cards = gamePanel.GenerateCards(card_Content, ids);
		return cards;
	}

}
