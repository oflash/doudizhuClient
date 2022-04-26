using System;
using System.Collections;
using System.Collections.Generic;

public class Audio
{
	/// <summary>
	/// 获取音频
	/// </summary>
	/// <param name="s"></param>
	/// <returns></returns>
	public static AudioType GetAudio(string s) {
		return (AudioType)Enum.Parse(typeof(AudioType), s);
	}


	/// <summary>
	/// 根据收到的卡牌获取对应的音频
	/// </summary>
	/// <param name="cards"></param>
	/// <param name="sex"></param>
	/// <param name="dani">播放音频"大你"</param>
	/// <returns></returns>
	public static AudioType GetCardAudio(List<Card> cards, bool sex, bool dani = false) {
		string s = sex ? "Man_" : "Woman_";
		if (dani) {
			Random r = new Random();
			s += "dani" + (r.Next(3) + 1);
			return (AudioType)Enum.Parse(typeof(AudioType), s);
		}

		CardType cardType = Rule.GameRule(cards);
		switch (cardType) {
			case CardType.danPai:                                   // 单牌
				if (cards[0].grade >= 13)   // 大小王
					s += (cards[0].grade + 1);
				else
					s += ((cards[0].grade + 2) % 13 + 1);
				break;
			case CardType.duiZi:                                    // 对子
				s += "dui";
				s += ((cards[0].grade + 2) % 13 + 1);
				break;
			case CardType.sanBuDai:                                 // 三不带
				s += "tuple";
				s += ((cards[0].grade + 2) % 13 + 1);
				break;
			case CardType.sanDaiYi:                                 // 三带一
				s += "sandaiyi";
				break;
			case CardType.sanDaiYiDui:                              // 三带一对
				s += "sandaiyidui";
				break;
			case CardType.shunZi:                                   // 顺子
				s += "shunzi";
				break;
			case CardType.lianDui:                                  // 连对
				s += "liandui";
				break;
			case CardType.feiJiDai:                                 // 飞机带
				s += "feiji";
				break;
			case CardType.feiJiBuDai:                               // 飞机不带
				s += "feiji";
				break;
			case CardType.siDaiEr:                                  // 四带二
				s += "sidaier";
				break;
			case CardType.siDaiLiangDui:                            // 四代两对
				s += "sidailiangdui";
				break;
			case CardType.zhaDan:                                   // 炸弹
				s += "zhadan";
				break;
			case CardType.wangZha:                                  // 王炸
				s += "wangzha";
				break;

			default:
				s = "None";
				break;
		}

		return (AudioType)Enum.Parse(typeof(AudioType), s);
	}


	/// <summary>
	/// 获取聊天语音
	/// </summary>
	/// <param name="index"></param>
	/// <param name="sex"></param>
	/// <returns></returns>
	public static AudioType GetChatAudio(int index, bool sex) {
		string s = sex ? "Man_" : "Woman_";
		s += "Chat_" + index;
		return (AudioType)Enum.Parse(typeof(AudioType), s);
	}

	/// <summary>
	/// 获取加倍聊天信息
	/// </summary>
	/// <param name="num"></param>
	/// <param name="sex"></param>
	/// <returns></returns>
	public static AudioType GetDoubleAudio(int num, bool sex) {
		string s = sex ? "Man_" : "Woman_";
		if (num == 1) {
			s += "bujiabei";
		} else if (num == 2) {
			s += "jiabei";
		} else if (num == 4) {
			s += "chaojijiabei";
		}
		return (AudioType)Enum.Parse(typeof(AudioType), s);
	}


	/// <summary>
	/// 获取抢地主音频(不改了)
	/// </summary>
	/// <param name="call"></param>
	/// <param name="rob"></param>
	/// <param name="sex"></param>
	/// <returns></returns>
	public static AudioType GetRobAudio(bool call, bool rob, bool sex) {
		string s = sex ? "Man_" : "Woman_";
		if (call && rob) {

		} else if (call && !rob) {

		} else if (!call && !rob) {

		} else if (!call && rob) {

		}

		return (AudioType)Enum.Parse(typeof(AudioType), s);
	}

	/// <summary>
	/// 返回不出音频
	/// </summary>
	/// <param name="sex"></param>
	/// <returns></returns>
	public static AudioType GetNoOutAudio(bool sex) {
		string s = sex ? "Man_" : "Woman_";
		s += "buyao";
		Random r = new Random();
		s += (r.Next(4) + 1);
		return (AudioType)Enum.Parse(typeof(AudioType), s);
	}

	/// <summary>
	/// 返回明牌音频
	/// </summary>
	/// <param name="sex"></param>
	/// <returns></returns>
	public static AudioType GetMingCardAudio(bool sex) {
		string s = sex ? "Man_" : "Woman_";
		s += "Share";
		return (AudioType)Enum.Parse(typeof(AudioType), s);
	}

}
