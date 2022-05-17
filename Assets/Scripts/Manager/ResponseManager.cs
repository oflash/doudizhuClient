using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseManager : BaseManager
{
	/// <summary>
	/// 所有的接收到的响应
	/// </summary>
	/// <typeparam name="ActionCode"></typeparam>
	/// <typeparam name="BaseRequest"></typeparam>
	/// <returns></returns>
	public Dictionary<ActionCode, BaseRequest> dicReq = new Dictionary<ActionCode, BaseRequest>();

	public ResponseManager(GameFacade gameFacade) : base(gameFacade) {

	}


	/// <summary>
	/// 增加
	/// </summary>
	/// <param name="actionCode"></param>
	/// <param name="baseRequest"></param>
	public void AddRequest(ActionCode actionCode, BaseRequest baseRequest) {
		dicReq.Add(actionCode, baseRequest);
	}

	/// <summary>
	/// 删除
	/// </summary>
	/// <param name="actionCode"></param>
	/// <param name="baseRequest"></param>
	public void RemoveRequest(ActionCode actionCode) {
		dicReq.Remove(actionCode);
	}

	/// <summary>
	/// 处理响应
	/// </summary>
	/// <param name="content"></param>
	public void HandleResponse(Content content) {
		ActionCode actionCode = content.actionCode;
		// Debug.Log(actionCode);
		BaseRequest baseRequest;        // 具体是哪一个请求
		if (!dicReq.TryGetValue(actionCode, out baseRequest)) {
			Debug.Log("没有请求:" + actionCode);
			return;
		}

		baseRequest.AddResponse(content);

		// 简单显示
		// gameFacade.ShowPromot(content.content);
	}

}
