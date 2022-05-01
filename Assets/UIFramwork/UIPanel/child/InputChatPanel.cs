using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;
using Newtonsoft.Json;

public class InputChatPanel : BasePanel
{
	// 该面板为RoomBG子面板, 不需要纳入UI框架

	// public bool Open => transform.localScale.x == 1;

	RoomBG roomBG;
	Player player => roomBG.players[0];

	InputField _inputField;
	InputField inputText => _inputField == null ? transform.Find("InputText").GetComponent<InputField>() : _inputField;

	Dictionary<string, Transform> select;

	protected override void Start() {
		select = new Dictionary<string, Transform>();
		select.Add("AudioChatToggle", transform.Find("Viewport").Find("ContentAudio"));
		select.Add("EmoticonChatToggle", transform.Find("Viewport").Find("ContentEmoticon"));
		roomBG = transform.parent.GetComponent<RoomBG>();
		OnInit();
	}

	public override void OnInit() {
		roomBG.Blank_Click();
	}


	#region ToggleOrButton事件
	/// <summary>
	/// Toggle选择不同的Chat板块
	/// </summary>
	/// <param name="toggle"></param>
	public void OnToggleSelectChat(Toggle toggle) {
		if (toggle.isOn) {
			// Debug.Log(toggle.name + ":" + toggle.isOn);
			select[toggle.name].localScale = Vector3.one;
			select[toggle.name].localPosition = Vector3.zero;
			transform.GetComponent<ScrollRect>().content = select[toggle.name].GetComponent<RectTransform>();
			toggle.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1);
			// select[toggle.name].gameObject.SetActive(true);
		} else {
			// Debug.Log(toggle.name + ":" + toggle.isOn);
			select[toggle.name].localScale = Vector3.zero;
			select[toggle.name].localPosition = Vector3.zero;
			toggle.GetComponent<Image>().color = Color.white;
			// select[toggle.name].gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// 输入文本聊天信息, 按回车键执行
	/// </summary>
	public void OnInputEndEdit() {
		string chat = inputText.text;
		inputText.text = "";
		if (!string.IsNullOrEmpty(chat))
			GetComponent<ChatRequest>().RequestSendChat(chat);
		roomBG.Blank_Click();
	}
	/// <summary>
	/// 选择了一个语音聊天
	/// </summary>
	/// <param name="item"></param>
	public void OnSelectAudioChat(Transform item) {
		string t = item.name;
		int index = int.Parse(t.Substring(t.Length - 2));

		// string other = player.Sex ? "Man_Chat_" : "Woman_Chat_";
		// other += index;     // 对应的枚举
		string chat = item.Find("Text").GetComponent<Text>().text;
		GetComponent<ChatRequest>().RequestSendChat(chat, 1, index.ToString());
		roomBG.Blank_Click();
	}


	/// <summary>
	/// 选择了一个表情包聊天
	/// </summary>
	/// <param name="item"></param>
	public void OnSelectEmoChat(Transform item) {
		string t = item.name;
		int index = int.Parse(t.Substring(t.Length - 2));
		string other = index.ToString();
		string chat = "表情" + index;
		GetComponent<ChatRequest>().RequestSendChat(chat, 2, other);
		roomBG.Blank_Click();
	}
	#endregion



	/// <summary>
	/// 使打开聊天面板时, 动画不跳跃
	/// </summary>
	/// <param name="scale"></param>
	public void ResumeScale(int scale) {
		StartCoroutine(_ResumeScale(scale));
	}
	IEnumerator _ResumeScale(int scale) {

		ScrollRect sr = GetComponent<ScrollRect>();
		if (scale == 0) {   // 关闭
			sr.movementType = ScrollRect.MovementType.Clamped;
		} else {
			// yield return new WaitForSeconds(0.5f);
			yield return null;
			sr.movementType = ScrollRect.MovementType.Elastic;
			foreach (var item in select) {      // 位置为展示顶部
				item.Value.localPosition = Vector3.zero;
			}
		}
		// Debug.Log("退出锁定");
	}

}
