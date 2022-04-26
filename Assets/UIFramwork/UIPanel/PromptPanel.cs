using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PromptPanel : BasePanel
{
	Queue<string> _prompts;
	Queue<string> prompts {
		get {
			if (_prompts == null) _prompts = new Queue<string>();
			return _prompts;
		}
	}

	Text _promptText;
	Text promptText {
		get {
			if (_promptText == null) _promptText = transform.Find("Text").GetComponent<Text>();
			return _promptText;
		}
	}

	protected override void Start() {
		base.Start();
		uiPanelType = UIPanelType.PromptPanel;
	}


	float t = 0.5f;
	private void Update() {
		t += Time.deltaTime;
		if (prompts.Count > 0) {
			ShowPanel(prompts.Dequeue());
			t = 0;
		}
	}


	/// <summary>
	/// 接收string为显示的信息, 注意该Panel不会入栈
	/// </summary>
	/// <param name="obj"></param>
	public override void OnOpen(object obj = null) {
		string s = obj as string;
		if (!string.IsNullOrEmpty(s)) {
			prompts.Enqueue(s);     // 加入消息队列, Update中展示Prompt
		}
	}

	public override void OnClose(object obj = null) {
		SetAnimator("IsShow", false);
		SetAnimator("IsHide");
		// UIMng.PopStack(uiPanelType);
	}

	private void ShowPanel(string s) {
		transform.SetAsLastSibling();
		Debug.Log("Prompt: " + s);
		if (t < 0.5f) {
			promptText.text += "\n" + s;
		} else {
			promptText.text = s;
		}
		// ani.Play(0);
		SetAnimator("IsShow", true);
		StartCoroutine(AudoHidePanel(1.8f, ++cntS));    // 1.8秒后自动关闭
	}
	int cntS = 0;
	IEnumerator AudoHidePanel(float time, int cnt) {
		yield return new WaitForSeconds(time);
		// Debug.Log(tmp_cntS + ":" + tmp_cntH);
		if (cntS == cnt) OnClose();
		// UIMng.PopStack(UIPanelType.PromptPanel, false);
	}


	private void SetAnimator(string name, bool value) {
		ani.SetBool(name, value);
	}
	private void SetAnimator(string name) {
		ani.SetTrigger(name);
	}
}
