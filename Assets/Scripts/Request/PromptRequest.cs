using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptRequest : BaseRequest
{
	PromptPanel promptPanel;

	protected override void Start() {
		requestCode = RequestCode.Default;
		actionCode = ActionCode.Prompt;
		promptPanel = GetComponent<PromptPanel>();

		base.Start();
	}


	public override void OnResponse(Content content) {
		base.OnResponse(content);

		// 收到系统提示，面板显示
		if (content.returnCode == ReturnCode.Success) {

			gameFacade.ShowPromot(content.content);
		}

	}

}
