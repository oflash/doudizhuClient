using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager
{

	protected GameFacade gameFacade;
	public BaseManager(GameFacade gameFacade) {
		this.gameFacade = gameFacade;
	}

	protected bool loaded = false;
	public bool Loaded => loaded;

	/// <summary>
	/// 初始化
	/// </summary>
	public virtual void OnInit() {

	}

	/// <summary>
	/// 销毁
	/// </summary>
	public virtual void OnDestroy() {

	}




}
