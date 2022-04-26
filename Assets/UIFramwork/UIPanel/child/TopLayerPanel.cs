using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopLayerPanel : BasePanel
{
	protected override void Start() {
		base.Start();
		uiPanelType = UIPanelType.TopLayerPanel;
		musicToggle = transform.Find("MusicToggle");
		settingButton = transform.Find("SettingButton");
		settingPanel = transform.Find("SettingPanel");
		douNum = transform.Find("DouNum").Find("Text").GetComponent<Text>();
		Debug.Log(musicToggle);
		StartCoroutine(WaitOpenToggle());
	}
	Transform musicToggle, settingButton, settingPanel;
	Text douNum;
	IEnumerator WaitOpenToggle() {
		yield return new WaitUntil(() => GameFacade.Instance.Loaded);   // 等待加载完成

		// Debug.Log("加载完成LLLLLLLL");
		musicToggle.gameObject.SetActive(true);
		musicToggle.GetComponent<Toggle>().enabled = true;
		settingButton.gameObject.SetActive(true);
	}

	public override void OnInit() {
		settingPanel.localScale = Vector3.zero;
	}

	private void Update() {
		transform.SetAsLastSibling();
		transform.localScale = Vector3.one;
	}

	#region 点击事件
	public void OnToggleMusicEx(bool isOn) {

		// Debug.Log("开始就执行");

		GameFacade.Instance.Mute = isOn;   // isOn true静音
		float a = isOn ? 0.4f : 1;
		Color c = musicToggle.GetComponent<Image>().color;
		musicToggle.GetComponent<Image>().color = new Color(c.r, c.g, c.b, a);
	}

	public void OnOpenSetting() {
		// Debug.Log("OnClickSetting");
		// GameFacade.Instance.ShowPromot("暂无功能...");
		settingPanel.localScale = Vector3.one;
	}

	public void OnCloseSetting() {
		settingPanel.localScale = Vector3.zero;
	}

	public void OnExitGameClick() {
		Debug.Log("退出游戏");
		Application.Quit();
	}

	#endregion

	/// <summary>
	/// 修改豆子的值
	/// </summary>
	/// <param name="value"></param>
	public void SetDouNum(int diff) {
		douNum.transform.parent.gameObject.SetActive(true);
		int value = diff + System.Int32.Parse(douNum.text);
		value = Mathf.Max(0, value);
		douNum.text = value.ToString();
	}
}
