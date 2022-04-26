using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
	#region Card相关属性及其设置
	private int id; // 牌的数字ID, 1到54
	public CardBigType bigType; // 牌的大类型, 花色
	public CardSmallType smallType; // 牌的小类型, 数字
	public int grade; // 牌的等级, 等级越大, 牌越大

	/// <summary>
	///  设置Card Id后, 获取Card的其他属性(1 <= Id <= 54)
	/// </summary>
	/// <value></value>
	public int Id {
		get {
			return id;
		}
		set {
			id = value;
			this.bigType = GetBigType(id);
			this.smallType = GetSmallType(id);
			this.grade = GetGrade(id);
		}
	}

	//表示花色
	public enum CardBigType
	{
		Spade,          // 黑桃
		Heart,          // 红桃
		Club,           // 梅花
		Square,         // 方块
		Black_Joker,    // 小王
		Red_Joker,      // 大王
	}

	//表示牌大小
	public enum CardSmallType
	{
		A, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, J, Q, K, Black_Joker, Red_Joker,
	}


	/// <summary>
	/// 根据id获取牌的花色
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public static CardBigType GetBigType(int id) {
		CardBigType bigType = CardBigType.Spade;
		if (id == 53) {
			bigType = CardBigType.Black_Joker;
		} else if (id == 54) {
			bigType = CardBigType.Red_Joker;
		} else if (id >= 1 && id <= 52) {
			bigType = (CardBigType)((id - 1) / 13);
		}
		return bigType;
	}


	/// <summary>
	/// 根据id获取牌的大小
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public static CardSmallType GetSmallType(int id) {
		CardSmallType smallType = CardSmallType.A;
		if (id == 53) {
			smallType = CardSmallType.Black_Joker;
		} else if (id == 54) {
			smallType = CardSmallType.Red_Joker;
		} else if (id >= 1 && id <= 52) {
			smallType = (CardSmallType)((id - 1) % 13);
		}
		return smallType;
	}

	/// <summary>
	/// 根据id获取牌的等级
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public static int GetGrade(int id) {
		int grade = 0; //A 的等级为11, 2的等级为12
		CardSmallType smallType = GetSmallType(id);
		if (smallType == CardSmallType.Black_Joker) {
			grade = 13;
		} else if (smallType == CardSmallType.Red_Joker) {
			grade = 14;
		} else {
			grade = ((int)smallType - 2 + 13) % 13; //'3' 的grade是0
		}
		return grade;
	}

	#endregion


	bool allowSelect => transform.parent.parent.parent.parent.name == "Poker0Panel";    // 允许被选中

	private static List<Card> cards = new List<Card>();     // 被选中的卡牌
	private bool selected = false;
	public bool Selected => selected;
	private static Vector3 begin_v;     // 开始点击位置
	private Vector3 end_v0;             // 结束点击位置0
	private Vector3 end_v1;             // 结束点击位置1
	private static bool down;           // 点击了向下

	private Vector3 selected_v;         // 选中后的坐标
	private Vector3 noselect_v;         // 不选时的坐标

	private static void AddCard(Card card) {
		if (!cards.Contains(card)) cards.Add(card);
	}
	private static void RemoveCard(Card card) {
		if (cards.Contains(card)) cards.Remove(card);
	}

	/// <summary>
	/// 恢复未选中状态
	/// </summary>
	public void ResumeState() {
		selected = false;
		transform.GetChild(0).localPosition = noselect_v;
	}

	private void Start() {
		float offset = (int)(this.GetComponent<RectTransform>().rect.height * GameObject.Find("Canvas").transform.localScale.x / 3.5f);
		noselect_v = transform.GetChild(0).localPosition;
		selected_v = noselect_v + Vector3.up * offset;
	}
	private void Update() {
		// bool isUI = RectTransformUtility.RectangleContainsScreenPoint(GameObject.Find("Desk").GetComponent<RectTransform>(), Input.mousePosition);

		// if (isUI && Input.GetKeyUp(KeyCode.Mouse0)) {    // 没有选中, 单击空白处
		// 	ResumeState();
		// }

		if (down && Input.GetKeyUp(KeyCode.Mouse0)) {
			down = false;
			// Debug.Log("结束选择");
			foreach (Card card in cards) {
				card.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
				card.selected = !card.selected;
				card.transform.GetChild(0).localPosition = card.selected ? card.selected_v : card.noselect_v;
			}
			// if (isUI) {
			// 	Debug.Log("正确");
			// } else {
			// 	Debug.Log("错误");
			// }
			cards.Clear();
		}

		end_v0 = end_v1;
		end_v1 = Input.mousePosition;
	}

	public void OnPointerDown(PointerEventData eventData) {
		// throw new System.NotImplementedException();
		// Debug.Log("开始选择");
		if (allowSelect) {
			down = true;
			begin_v = Input.mousePosition;
			OnPointerEnter(eventData);          // 按下也算作进入
		}
	}

	public void OnPointerEnter(PointerEventData eventData) {
		if (down) {
			// Debug.Log(this.Id);
			AddCard(this);
			transform.GetChild(0).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
		}
	}

	public void OnPointerExit(PointerEventData eventData) {
		if (down) {
			// transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
			Vector3 v1 = (end_v1 - end_v0).normalized;          // 移动的方向向量
			Vector3 v2 = (end_v1 - begin_v).normalized;         // 增加的方向向量
			if (Vector3.Angle(v1, v2) < 90) {   // 同向

			} else {    // 反向
				transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
				RemoveCard(this);
			}
		}

	}
}


public class myComparer : IComparer<Card>
{
	public int Compare(Card x, Card y) {
		return y.grade.CompareTo(x.grade);      // y < x
	}
}