using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Newtonsoft.Json;

public class UIManager : BaseManager
{
	public Dictionary<UIPanelType, BasePanel> dicPanels;    // PanelPool
	public Dictionary<SpriteType, List<Sprite>> dicSprites;    // Sprite
	public Dictionary<PrefabType, GameObject> dicPrefabs;

	public UIManager(GameFacade gameFacade) : base(gameFacade) { }

	public override void OnInit() {
		// ReadJson();
		gameFacade.StartCoroutine(ReadAllJson());
		gameFacade.StartCoroutine(LoadAllPanel());

		PushStack(UIPanelType.CoverBG);     // 从头开始

		// PushStack(UIPanelType.PromptPanel, false, "欢迎来到欢乐斗地主联机版");
		gameFacade.ShowPromot("欢迎来到欢乐斗地主联机版");


	}



	/// <summary>
	/// 加载所有Panel, 避免一些代码调用时, 调用空物体
	/// </summary>
	/// <returns></returns>
	IEnumerator LoadAllPanel() {

		yield return new WaitUntil(() => panel_json);
		foreach (var item in System.Enum.GetNames(typeof(UIPanelType))) {
			UIPanelType panel = (UIPanelType)System.Enum.Parse(typeof(UIPanelType), item);
			// Debug.Log(panel);
			if (panel == UIPanelType.Default || panel == UIPanelType.GamePanel ||
				panel == UIPanelType.CoverBG || panel == UIPanelType.PromptPanel) continue;
			PushStack(panel, false);
			PopStack(panel, false);
			yield return null;
		}
		loaded = true;
	}
	bool panel_json;     // 加载json完成

	#region  读取AllJson
	Dictionary<UIPanelType, string> dicPanelPath;
	Dictionary<SpriteType, List<string>> spritePath;
	Dictionary<PrefabType, string> prefabPath;
	private IEnumerator ReadAllJson() {
		if (dicPanelPath == null) dicPanelPath = new Dictionary<UIPanelType, string>();
		TextAsset text = Resources.Load<TextAsset>("PanelPath");

		Debug.Log(text.text);
		// PanelInfos infos = JsonUtility.FromJson<PanelInfos>(text.text);

		PanelInfo[] infos = JsonConvert.DeserializeObject<PanelInfo[]>(text.text);

		// Debug.Log(infos);
		foreach (PanelInfo info in infos) {
			dicPanelPath.Add(info.uiPanelType, info.path);
		}
		panel_json = true;
		// Debug.Log(panel_json);
		yield return null;

		ReadSpriteJson("BGSpritePath");
		yield return null;
		ReadSpriteJson("PokerSpritePath");
		yield return null;
		ReadSpriteJson("HeaderSpritePath");
		yield return null;
		ReadPrefabJson();

		// gameFacade.AddText("dicPanelPath:" + dicPanelPath.Count);
		// gameFacade.text.text += "dicPanelPath:" + dicPanelPath.Count;
		// gameFacade.AddText("spritePath:" + spritePath.Count);
		// gameFacade.AddText("prefabPath:" + prefabPath.Count);

	}

	private void ReadPrefabJson() {
		if (prefabPath == null) prefabPath = new Dictionary<PrefabType, string>();
		TextAsset text = Resources.Load<TextAsset>("PrefabPath");
		PrefabInfo[] infos = JsonConvert.DeserializeObject<PrefabInfo[]>(text.text);
		// PrefabInfos infos = JsonUtility.FromJson<PrefabInfos>(text.text);
		// Debug.Log(infos.prefabInfos.Count);
		string s = "";
		foreach (PrefabInfo info in infos) {
			prefabPath.Add(info.prefabType, info.path);
			s += info.prefabType + "\t" + info.path + "\n";
		}
		Debug.Log(s);
	}

	private void ReadSpriteJson(string json) {
		if (spritePath == null) spritePath = new Dictionary<SpriteType, List<string>>();
		TextAsset text = Resources.Load<TextAsset>(json);
		// Debug.Log(text.text);
		SpriteInfo[] infos = JsonConvert.DeserializeObject<SpriteInfo[]>(text.text);
		// Debug.Log(json + ":" + infos[0].paths.Count);
		// SpriteInfos infos = JsonUtility.FromJson<SpriteInfos>(text.text);
		// Debug.Log(infos[0].paths.Count);
		foreach (var info in infos) {
			List<string> paths = new List<string>();
			foreach (var path in info.paths) {
				paths.Add(path);
			}
			spritePath.Add(info.spriteType, paths);
		}
	}
	#endregion


	#region 读取资源
	/// <summary>
	/// 获取本地Sprite
	/// </summary>
	/// <param name="spriteType"></param>
	/// <param name="index"></param>
	public Sprite GetSprite(SpriteType spriteType, bool atlas, int index = 0) {
		if (dicSprites == null) dicSprites = new Dictionary<SpriteType, List<Sprite>>();
		List<Sprite> sprites;
		dicSprites.TryGetValue(spriteType, out sprites);
		if (sprites == null) {  // 没有
			sprites = new List<Sprite>();
			List<string> paths;
			spritePath.TryGetValue(spriteType, out paths);
			foreach (string path in paths) {
				if (atlas) {
					Sprite[] sprite1 = Resources.LoadAll<Sprite>(path);
					foreach (Sprite sprite in sprite1) {
						sprites.Add(sprite);
					}
				} else {
					Sprite sprite = Resources.Load<Sprite>(path);
					sprites.Add(sprite);
				}
			}
		}
		return sprites[index];
	}

	public List<Sprite> GetSprites(SpriteType spriteType, bool atlas) {
		if (dicSprites == null) dicSprites = new Dictionary<SpriteType, List<Sprite>>();
		List<Sprite> sprites;
		dicSprites.TryGetValue(spriteType, out sprites);
		if (sprites == null) {  // 没有
			sprites = new List<Sprite>();
			List<string> paths;
			spritePath.TryGetValue(spriteType, out paths);
			foreach (string path in paths) {
				if (atlas) {
					Sprite[] sprite1 = Resources.LoadAll<Sprite>(path);
					// Debug.Log(sprite1.Length);
					foreach (Sprite sprite in sprite1) {
						sprites.Add(sprite);
					}
				} else {
					Sprite sprite = Resources.Load<Sprite>(path);
					sprites.Add(sprite);
				}
			}
		}
		return sprites;
	}


	/// <summary>
	/// 获取Prefab
	/// </summary>
	/// <param name="prefabType"></param>
	/// <returns></returns>
	public GameObject GetPrefab(PrefabType prefabType) {
		if (dicPrefabs == null) dicPrefabs = new Dictionary<PrefabType, GameObject>();
		GameObject go;
		dicPrefabs.TryGetValue(prefabType, out go);
		if (go == null) {
			string path;
			// Debug.Log(prefabType);
			prefabPath.TryGetValue(prefabType, out path);
			if (string.IsNullOrEmpty(path)) {
				Debug.Log("没有对应的值：" + prefabType);
			}
			// Debug.Log(path);
			// go = GameObject.Instantiate(Resources.Load(path)) as GameObject;
			go = Resources.Load<GameObject>(path);
			dicPrefabs.Add(prefabType, go);
		}
		return go;
	}


	/// <summary>
	/// 获取Panel
	/// </summary>
	/// <param name="uiPanelType"></param>
	/// <returns></returns>
	public BasePanel GetPanel(UIPanelType uiPanelType, string parent = "Canvas") {
		if (dicPanels == null) dicPanels = new Dictionary<UIPanelType, BasePanel>();
		BasePanel basePanel = null;

		bool b = dicPanels.TryGetValue(uiPanelType, out basePanel);
		// Debug.Log(basePanel);

		if (basePanel == null) {
			string path;
			dicPanelPath.TryGetValue(uiPanelType, out path);
			GameObject go = null;
			// Debug.Log("资源加载结果为:" + Resources.Load(path));
			try {
				go = Object.Instantiate(Resources.Load(path)) as GameObject;
				go.transform.SetParent(GameObject.Find(parent).transform, false);
				dicPanels.Add(uiPanelType, go.GetComponent<BasePanel>());
				basePanel = go.GetComponent<BasePanel>();
			} catch (System.Exception e) {
				Debug.Log(e.Message);
			}

		}
		// basePanel.transform.localScale = Vector3.zero;
		return basePanel;
	}

	#endregion


	#region Stack, 参数bool表示是否入栈(部分提示性Panel不入栈)
	Stack<BasePanel> panelStack;

	public void PushStack(UIPanelType uiPanelType, bool push = true, object obj = null) {
		if (panelStack == null) panelStack = new Stack<BasePanel>();


		BasePanel panel = GetPanel(uiPanelType);
		panel.OnOpen(obj);      // 显示面板

		if (!push) return;      // 如果不入栈

		lock (dicPanels) {
			// Debug.Log(uiPanelType);
			if (panelStack.Count > 0) panelStack.Peek().OnPause();
			panelStack.Push(panel);
		}
	}

	public void PopStack(UIPanelType uiPanelType, bool pop = true, object obj = null) {
		if (panelStack == null) panelStack = new Stack<BasePanel>();
		if (panelStack.Count == 0) return;

		if (!pop) { // 如果不出栈(因为它同时没有入栈)
			BasePanel panel = GetPanel(uiPanelType);
			panel.OnClose(obj);
			return;
		}

		lock (dicPanels) {

			// 出栈, 但是不在栈顶(玩家点击Button出栈)
			Debug.Log(panelStack.Peek().uiPanelType + ": " + uiPanelType);
			// if (panelStack.Peek().uiPanelType != uiPanelType) return;

			panelStack.Peek().OnClose(obj);
			panelStack.Pop();
			// Debug.Log(panelStack.Peek());
			if (panelStack.Count > 0) panelStack.Peek().OnResume();
		}
	}
	#endregion


	// /// <summary>
	// /// 连接服务器
	// /// </summary>
	// /// <param name="ip"></param>
	// /// <param name="port"></param>
	// public void ConnectServer(string ip, int port) {
	// 	gameFacade.ConnectServer(ip, port);
	// }

	// public void SendRequest(Content content) {
	// 	gameFacade.SendRequest(content);
	// }
}
