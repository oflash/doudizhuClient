using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rule
{
	/// <summary>
	/// 返回牌的类型
	/// </summary>
	/// <param name="list"></param>
	/// <returns></returns>
	public static CardType GameRule(List<Card> list) {
		// CardType cardType = CardType.wrong;
		if (list == null || list.Count == 0) return CardType.wrong;
		string s = "";
		for (int i = 0; i < list.Count; i++) {
			s += (list[i].grade + " ");
		}
		Debug.Log(s);

		// Debug.Log("单牌");
		// 单牌, 张数为1
		if (list.Count == 1) {
			return CardType.danPai;
		}

		// Debug.Log("对子或王炸");
		// 对子或王炸, 两张牌的等级一样
		if (list.Count == 2) {
			if (list[0].grade == list[1].grade)
				return CardType.duiZi;
			if (list[0].grade + list[1].grade == 27)
				return CardType.wangZha;
		}

		// Debug.Log("三不带");
		// 如果是三不带
		if (list.Count == 3) {
			if (list[0].grade == list[1].grade && list[0].grade == list[2].grade)
				return CardType.sanBuDai;
		}

		// Debug.Log("三带一或炸弹");
		// 三带一或炸弹
		if (list.Count == 4) {
			int grade1 = list[0].grade;
			int grade2 = list[1].grade;
			int grade3 = list[2].grade;
			int grade4 = list[3].grade;
			if (grade1 == grade2 && grade1 == grade3 && grade1 == grade4) {
				return CardType.zhaDan;
			} else if (grade1 == grade2 && grade1 == grade3) {
				return CardType.sanDaiYi;
			} else if (grade2 == grade3 && grade2 == grade4) {
				return CardType.sanDaiYi;
			}
		}

		// Debug.Log("三带一对");
		// 三带对, 前三张或后三张
		if (list.Count == 5) {
			int grade1 = list[0].grade;
			int grade2 = list[1].grade;
			int grade3 = list[2].grade;
			int grade4 = list[3].grade;
			int grade5 = list[4].grade;
			if (grade1 == grade2 && grade3 == grade4 && grade3 == grade5) {     // 后三张
				return CardType.sanDaiYiDui;
			}
			if (grade1 == grade2 && grade1 == grade3 && grade4 == grade5) {     // 前三张
				return CardType.sanDaiYiDui;
			}
		}

		// Debug.Log("四带二");
		// 四带二单
		if (list.Count == 6) {
			int grade1 = list[0].grade;
			int grade2 = list[1].grade;
			int grade3 = list[2].grade;
			int grade4 = list[3].grade;
			int grade5 = list[4].grade;
			int grade6 = list[5].grade;
			if (grade1 == grade2 && grade1 == grade3 && grade1 == grade4) {
				return CardType.siDaiEr;
			}
			if (grade2 == grade3 && grade2 == grade4 && grade2 == grade5) {
				return CardType.siDaiEr;
			}
			if (grade3 == grade4 && grade3 == grade5 && grade3 == grade6) {
				return CardType.siDaiEr;
			}
		}


		// Debug.Log("四带两对");
		// 四代二对
		if (list.Count == 8) {
			bool siDaiLiangDui = true;
			// 两两相对
			for (int i = 0; i < 8; i += 2)
				siDaiLiangDui &= (list[i].grade == list[i + 1].grade);
			bool tag = false;
			tag |= (list[1].grade == list[2].grade && list[5].grade != list[6].grade);
			tag |= (list[3].grade == list[4].grade);
			tag |= (list[5].grade == list[6].grade && list[1].grade != list[2].grade);

			if (siDaiLiangDui & tag) return CardType.siDaiLiangDui;
		}

		// Debug.Log("顺子");
		// 顺子, 张数在5~12之间
		if (list.Count >= 5 && list.Count <= 12) {
			bool isShunZi = true;
			for (int i = 0; i < list.Count; i++) {
				if (list[i].grade >= 12) {
					isShunZi = false;
					break;
				}
				if (i >= 1) {
					if (list[i].grade - list[i - 1].grade != -1) {
						isShunZi = false;
						break;
					}
				}
			}
			if (isShunZi) return CardType.shunZi;
		}

		// Debug.Log("连对");
		// 连队
		if (list.Count >= 6 && list.Count % 2 == 0) {
			bool isLianDui = true;

			for (int i = 0; i < list.Count; i += 2) {
				if (list[i].grade >= 12 || list[i + 1].grade >= 12) {       // 2，小王，大王
					isLianDui = false;
					break;
				}
				if (list[i].grade != list[i + 1].grade) {                   // 不成对
					isLianDui = false;
					break;
				}
				if (i >= 2) {
					if (list[i].grade - list[i - 2].grade != -1) {          // 不相邻
						isLianDui = false;
						break;
					}
				}
			}
			if (isLianDui) return CardType.lianDui;
		}

		// Debug.Log("飞机不带");
		// 飞机不带
		if (list.Count >= 6 && list.Count % 3 == 0) {
			bool isFeiJiBuDai = true;

			for (int i = 0; i < list.Count; i += 3) {
				if (list[i].grade >= 12 || list[i + 1].grade >= 12 || list[i + 2].grade >= 12) {    // 2，小王，大王
					isFeiJiBuDai = false;
					break;
				}
				if (list[i].grade != list[i + 1].grade || list[i].grade != list[i + 2].grade) {     // 不成三
					isFeiJiBuDai = false;
					break;
				}
				if (i >= 3) {
					if (list[i].grade - list[i - 3].grade != -1) {                                  // 不相邻
						isFeiJiBuDai = false;
						break;
					}
				}
			}
			if (isFeiJiBuDai) return CardType.feiJiBuDai;
		}

		// Debug.Log("飞机带单");
		// 飞机带单
		if (list.Count >= 8 && list.Count % 4 == 0) {
			int m = list.Count / 4;     // 三条个数

			for (int i = 0; i <= m; i++) {
				bool feiji = true;
				for (int j = i; j < 3 * m + i; j += 3) {
					if (list[j].grade >= 12) {                                                          // 2，小王，大王
						feiji = false;
						break;
					}
					if (list[j].grade != list[j + 1].grade || list[j].grade != list[j + 2].grade) {     // 不成三
						feiji = false;
						break;
					}
					if (j >= 3) {
						if (list[j].grade - list[j - 3].grade != -1) {                                  // 不相邻
							feiji = false;
							break;
						}
					}
				}
				if (feiji) return CardType.feiJiDai;
			}
		}

		// Debug.Log("飞机带对");
		// 飞机带对
		if (list.Count >= 10 && list.Count % 5 == 0) {
			int m = list.Count / 5;     // 三条个数


			for (int i = 0; i <= m; i++) {
				bool feiji = true;
				for (int j = i * 2; j < 3 * m + 2 * i; j += 3) {
					if (list[j].grade >= 12) {                                                          // 2，小王，大王
						feiji = false;
						break;
					}
					if (list[j].grade != list[j + 1].grade || list[j].grade != list[j + 2].grade) {     // 不成三
						feiji = false;
						break;
					}
					if (j >= 3) {
						if (list[j].grade - list[j - 3].grade != -1) {                                  // 不相邻
							feiji = false;
							break;
						}
					}
				}
				for (int k = (3 * m + 2 * i) % list.Count, cnt = 0; cnt < m; k = (k + 2) % list.Count, cnt++) { // 带的不成对
					if (list[k].grade != list[k + 1].grade) {
						feiji = false;
						break;
					}
				}
				if (feiji) return CardType.feiJiDai;
			}
		}

		return CardType.wrong;
	}


	/// <summary>
	/// 比较牌的大小, 判断能不能出牌
	/// </summary>
	/// <param name="pre"></param>
	/// <param name="list"></param>
	/// <returns></returns>
	public static bool Compare(List<Card> prev, List<Card> list) {
		CardType prevType = GameRule(prev);     // 上一手牌
		CardType listType = GameRule(list);     // 当前牌

		if ((prev == null || prev.Count == 0) && listType != CardType.wrong) {
			return true;   // 先手直接出牌
		}
		if (listType == CardType.wrong) return false;       // 错误牌组
		if (listType == CardType.wangZha) return true;      // 王炸

		// 如果是相同牌型, 且牌数量相同
		if (prevType == listType && prev.Count == list.Count) {

			// 单牌, 对子, 三不带, 炸弹, 顺子, 连对, 飞机不带
			if (listType == CardType.danPai || listType == CardType.duiZi ||
				listType == CardType.sanBuDai || listType == CardType.zhaDan ||
				listType == CardType.shunZi || listType == CardType.lianDui ||
				listType == CardType.feiJiBuDai) {
				return list[0].grade > prev[0].grade;
			}

			// 三带一, 三带一对, 四带二单
			if (listType == CardType.sanDaiYi || listType == CardType.sanDaiYiDui) {
				return list[list.Count / 2].grade > prev[prev.Count / 2].grade;
			}

			// 飞机带, 4 or 5 倍数
			if (listType == CardType.feiJiDai) {
				int m = 0;
				if (list.Count % 4 == 0) {
					m = list.Count / 4;
				} else {
					m = list.Count / 5;
				}
				// 由于一种牌最多4张，只需比较m位置grade即可
				return list[m].grade > prev[m].grade;
			}

			// 四带二对
			if (listType == CardType.siDaiLiangDui) {
				int grade0 = 0, grade1 = 0;
				for (int i = 1; i <= 5; i += 2) {
					if (prev[i].grade == prev[i + 1].grade) {
						grade0 = prev[i].grade;
						break;
					}
				}
				// 找出4
				for (int i = 1; i <= 5; i += 2) {
					if (list[i].grade == list[i + 1].grade) {
						grade1 = list[i].grade;
						break;
					}
				}
				return grade1 > grade0;
			}

		} else if (listType == CardType.zhaDan && prevType != CardType.wangZha) {
			return true;
		}
		return false;
	}
}
