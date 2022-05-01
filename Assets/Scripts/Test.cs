using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

	GameFacade gameFacade => GameFacade.Instance;
	UIManager uiMng => gameFacade.UIMng;
	private void Start() {

		StartCoroutine(TEst());
	}

	IEnumerator TEst() {
		yield return new WaitUntil(() => uiMng.Loaded);
		string path = "Header/";
		Sprite[] sprites = Resources.LoadAll<Sprite>(path);
		string s = "";
		foreach (Sprite sprite in sprites) {
			s += "\"" + path + sprite.name + "\",";
		}
		// Debug.Log(s);
	}




	int cnt = 0;
	public void testbutton_click() {
		gameFacade.ShowPromot("测试" + ++cnt);

		gameFacade.PlayMusic(AudioType.GuideAiXin, true, true);
	}


}
