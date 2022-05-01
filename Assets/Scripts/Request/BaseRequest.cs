using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseRequest : MonoBehaviour
{
	protected RequestCode requestCode = RequestCode.None;
	protected ActionCode actionCode = ActionCode.None;

	protected Queue<Content> responseQueue = new Queue<Content>();

	protected GameFacade gameFacade;


	// 用于判断发送请求后是否收到响应
	protected bool Response => response;
	protected bool response;




	#region 生命周期函数
	protected virtual void Start() {
		gameFacade = GameFacade.Instance;
		// Debug.Log("加入ActionCode:" + actionCode);
		gameFacade.AddAction(actionCode, this);
	}

	protected virtual void Update() {
		if (responseQueue.Count > 0) {
			OnResponse(responseQueue.Dequeue());
		}
	}

	protected virtual void OnDestroy() {

	}

	#endregion


	/// <summary>
	/// 各子类需要具体请求
	/// </summary>
	/// <param name="content"></param>
	protected void SendRequest(Content content) {
		gameFacade.SendRequest(content);            // 通过Mng桥梁发送请求
	}


	/// <summary>
	/// 将各种Mng的响应发送给GameFacade
	/// </summary>
	/// <param name="content"></param>
	public virtual void OnResponse(Content content) {
		response = true;

	}

	public void AddResponse(Content content) {
		responseQueue.Enqueue(content);
	}
}
