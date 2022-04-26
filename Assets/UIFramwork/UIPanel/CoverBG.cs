using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoverBG : BasePanel
{
	Slider _slider;
	Slider slider {
		get {
			if (_slider == null) _slider = transform.Find("Center").GetComponent<Slider>();
			return _slider;
		}
	}

	protected override void Start() {
		base.Start();
		uiPanelType = UIPanelType.CoverBG;
	}

	public override void OnOpen(object obj = null) {
		base.OnOpen(obj);
		StartCoroutine(ILoading(3));
	}



	/// <summary>
	/// 加载进度条动画
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	private IEnumerator ILoading(float time) {
		if (time <= 0 || slider == null) yield break;
		yield return new WaitForSeconds(1);     // 1秒封面
		slider.gameObject.SetActive(true);
		float t = 0;
		while (true) {
			t += Time.deltaTime;
			slider.value = t / time;
			yield return null;
			if (t >= time) break;
		}
		Debug.Log(t);
		slider.gameObject.SetActive(false);
		uiMng.PushStack(UIPanelType.JoinRoomPanel);
	}

}
