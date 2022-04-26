using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRequest : BaseRequest
{

	PlayerInfoPanel playerInfoPanel;
	protected override void Start() {
		requestCode = RequestCode.Player;
		actionCode = ActionCode.Attack;
		playerInfoPanel = GetComponent<PlayerInfoPanel>();
		base.Start();
	}

	/// <summary>
	/// 发送攻击请求
	/// </summary>
	/// <param name="target">攻击目标</param>
	/// <param name="index">攻击Item</param>
	public void RequestAttack(string target, int index) {

	}
	IEnumerator Attack() {

		yield return new WaitUntil(() => Response);
		// 可选择性添加方法(接收响应后)
		gameFacade.WaitResponse(false);

	}

	public override void OnResponse(Content content) {
		base.OnResponse(content);

	}


}
