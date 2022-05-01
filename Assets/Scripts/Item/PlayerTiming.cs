using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTiming : MonoBehaviour
{
	public Sprite[] num_sprites;

	/// <summary>
	/// 开始计时
	/// </summary>
	/// <param name="time"></param>
	/// <param name="action">到达时间后自动执行方法</param>
	public void BeginTiming(int time, Action action) {

		StartCoroutine(_BeginTiming(time, ++cnt_num, action));
	}
	int cnt_num;    // 记录是第几个计时器, 一旦变化, 后面的方法就不执行
	IEnumerator _BeginTiming(int time, int cnt, Action action) {

		int loop = time;
		while (loop >= 0) {
			if (cnt != cnt_num) break;
			SetNum(loop--);
			yield return new WaitForSeconds(1);
		}
		if (cnt == cnt_num && action != null)    // 执行自动方法
			action();
	}
	private void SetNum(int num) {

		int num0 = num / 10;
		int num1 = num % 10;

		if (num0 > 0) {
			transform.Find("num0").gameObject.SetActive(true);
			transform.Find("num0").GetComponent<Image>().sprite = num_sprites[num0];
		} else {
			transform.Find("num0").gameObject.SetActive(false);
		}

		transform.Find("num1").GetComponent<Image>().sprite = num_sprites[num1];
	}

	/// <summary>
	/// 手动结束计时
	/// </summary>
	public void EndTiming() {
		cnt_num++;
	}

}
