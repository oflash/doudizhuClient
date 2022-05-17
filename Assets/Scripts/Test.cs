using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
	public InputField inputField;
	GameFacade gameFacade => GameFacade.Instance;
	UIManager uiMng => gameFacade.UIMng;
	private void Start() {

		// StartCoroutine(TEst());
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



	private void Update() {
		// if (Input.GetKeyDown(KeyCode.J)) {
		// 	ScreenShotFile("tupian.jpg");
		// }
	}
	/// <summary>
	/// UnityEngine自带截屏Api，只能截全屏
	/// </summary>
	/// <param name="fileName">文件名</param>
	public void ScreenShotFile(string fileName) {
		UnityEngine.ScreenCapture.CaptureScreenshot(fileName);//截图并保存截图文件
		Debug.Log(string.Format("截取了一张图片: {0}", fileName));

#if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh();//刷新Unity的资产目录
#endif
	}


}
