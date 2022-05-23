using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectScenePanel : BasePanel
{
	#region 私有变量
	List<Sprite> _sprites;
	List<Sprite> sprites {
		get {
			if (_sprites == null)
				_sprites = uiMng.GetSprites(SpriteType.mBG, true);
			return _sprites;
		}
	}

	Transform panelMask;
	Transform curr, next;   // 移动的ItenSelect
	Button left, right;

	int index = 0;
	int len = -1;
	int Length {
		get {
			if (len < 0) len = sprites.Count / 2;
			return len;
		}
	}
	#endregion



	public int Index => index;      // 选择的场景编号
	public Transform tl, tm, tr;




	protected override void Start() {
		base.Start();
		uiPanelType = UIPanelType.SelectScenePanel;
		left = transform.Find("Left").GetComponent<Button>();
		right = transform.Find("Right").GetComponent<Button>();
		panelMask = transform.Find("PanelMask").GetComponent<Transform>();
		next = curr = panelMask.Find("Scene_Select").GetComponent<Transform>();
		panelMask.GetComponent<Button>().enabled = true;
	}

	#region 鼠标点击事件
	public void OnClickLeft() {
		index = (index - 1 + Length) % Length;
		{
			next = Instantiate(uiMng.GetPrefab(PrefabType.Scene_Select), panelMask).transform;
			next.position = tl.position;
			next.GetComponent<SceneSelectItem>().Id = index;
			next.GetComponent<SceneSelectItem>().SetSprite(sprites[index * 2], sprites[index * 2 + 1]);
		}


		// Debug.Log(Length);
		StartCoroutine(MoveTo(next, tm, false));
		StartCoroutine(MoveTo(curr, tr, true));
	}

	public void OnClickRight() {
		index = (index + 1) % Length;
		{
			next = Instantiate(uiMng.GetPrefab(PrefabType.Scene_Select), panelMask).transform;
			next.position = tr.position;
			next.GetComponent<SceneSelectItem>().Id = index;
			next.GetComponent<SceneSelectItem>().SetSprite(sprites[index * 2], sprites[index * 2 + 1]);
		}

		StartCoroutine(MoveTo(next, tm, false));
		StartCoroutine(MoveTo(curr, tl, true));
	}


	public void OnClickSelect() {
		// Debug.Log(next.GetComponent<SceneSelectItem>().Id + ":" + index);
		uiMng.PopStack(UIPanelType.SelectScenePanel);
		uiMng.PushStack(UIPanelType.LinkPanel, true, index);         // 设置为创建房间
	}

	public void OnClickClose() {
		uiMng.PopStack(UIPanelType.SelectScenePanel);
	}

	#endregion


	// 将物体trans移动到target位置之后, destroy是否销毁trans
	IEnumerator MoveTo(Transform trans, Transform target, bool destroy) {
		Vector3 dic = (target.position - trans.position).normalized;
		Vector3 pos = trans.position;   // 原来坐标
		float speed = 2000;

		left.enabled = false; right.enabled = false;        // 进入移动, 不能点击

		if (destroy) {  // 中 -> 边
			panelMask.GetComponent<Button>().enabled = false;
		}

		while (true) {
			yield return null;
			trans.Translate(dic * Time.deltaTime * speed);
			Vector3 v1 = pos - trans.position;
			Vector3 v2 = target.position - trans.position;
			if (Vector3.Angle(v1, v2) < 30) {       // 到达target后
				if (destroy)
					Destroy(trans.gameObject);  // 中 -> 边
				else {
					panelMask.GetComponent<Button>().enabled = true;
					trans.position = tm.position;
					curr = next;                // 边 -> 中
				}

				break;
			}
		}
		left.enabled = true; right.enabled = true;  // 离开移动可以点击
	}
}
