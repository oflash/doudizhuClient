using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : BasePanel
{
	// public LoadingPanelType loadingType;


	protected override void Start() {
		base.Start();
		uiPanelType = UIPanelType.LoadingPanel;
	}


	public override void OnOpen(object obj = null) {
		base.OnOpen(obj);
		// q.Enqueue(base.OnOpen);
	}



	public override void OnClose(object obj = null) {
		base.OnClose(obj);
		// q.Enqueue(base.OnClose);
	}


	/// <summary>
	/// 监测到完成自动关闭
	/// </summary>
	/// <returns></returns>
	private IEnumerator AutoClose() {

		while (true) {

			yield return null;
		}
	}

}
