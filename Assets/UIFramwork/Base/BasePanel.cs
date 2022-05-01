using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
	public UIPanelType uiPanelType { get; set; }
	protected CanvasGroup canvasGroup;
	protected Animator ani;
	UIManager _uiMng;
	protected UIManager uiMng {
		get {
			if (_uiMng == null) _uiMng = GameFacade.Instance.UIMng;
			return _uiMng;
		}
	}

	public virtual void OnInit() { }	// 新一局游戏, 都会执行一次的Init
	protected virtual void Start() {
		if (GetComponent<CanvasGroup>()) canvasGroup = GetComponent<CanvasGroup>();
		if (GetComponent<Animator>()) ani = GetComponent<Animator>();
		// Debug.Log(ani);
	}

	/// <summary>
	/// 打开Panel
	/// </summary>
	public virtual void OnOpen(object obj = null) {
		if (canvasGroup) {
			canvasGroup.alpha = 1;              // 显示
			canvasGroup.interactable = true;    // 可交互
		}
		transform.SetAsLastSibling();
		GetComponent<RectTransform>().localScale = Vector3.one;
	}

	/// <summary>
	/// 关闭Panel
	/// </summary>
	public virtual void OnClose(object obj = null) {
		if (canvasGroup) {
			canvasGroup.alpha = 0;
			canvasGroup.interactable = false;
		}
		GetComponent<RectTransform>().localScale = Vector3.zero;
	}


	/// <summary>
	/// 暂停执行(不可交互)
	/// </summary>
	public virtual void OnPause() {
		// Debug.Log("OnPause:" + canvasGroup);
		if (canvasGroup) canvasGroup.interactable = false;

	}

	/// <summary>
	/// 继续执行(可以交互)
	/// </summary>
	public virtual void OnResume() {
		if (canvasGroup) canvasGroup.interactable = true;
	}


	public override string ToString() {
		return uiPanelType.ToString();
	}
}
