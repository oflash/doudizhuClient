using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Header : MonoBehaviour
{
	public int index { get; set; }
	UIManager uiMng => GameFacade.Instance.UIMng;

	void Start() {
		// uiMng = GameFacade.Instance.UIMng;
		index = int.Parse(this.name.Substring(0, 2));
		// Debug.Log(index);
		StartCoroutine(AllGetHead());
	}

	


	IEnumerator AllGetHead() {
		yield return new WaitUntil(() => uiMng.Loaded);     // 等待UIMng初始化完成
		// Debug.Log(uiMng.GetSprites(SpriteType.Header, false).Count);

		transform.GetChild(0).GetComponent<Image>().sprite = uiMng.GetSprite(SpriteType.Header, false, index);

	}

}
