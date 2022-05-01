using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : BasePanel
{
	public Transform tableContent;
	public Transform playerInfo;
	public Image titleImage;
	public Sprite[] sprites;        // 地主胜, 农民胜, 地主负, 农民负

	RoomBG roomBG => uiMng.GetPanel(UIPanelType.RoomBG) as RoomBG;


	protected override void Start() {
		base.Start();
		uiPanelType = UIPanelType.GameOverPanel;

	}


	public override void OnOpen(object obj = null) {
		base.OnOpen(obj);

	}

	public void SetResultInfo(Player player, ResultInfo info) {
		int local_index = player.local_index;
		Transform element = tableContent.Find("element" + local_index);
		element.Find("Text0").GetComponent<Text>().text = player.Name;              // 昵称
		element.Find("Text1").GetComponent<Text>().text = info.Bottom.ToString();   // 底分
		element.Find("Text2").GetComponent<Text>().text = info.Double.ToString();   // 倍数
		element.Find("Text3").GetComponent<Text>().text = info.Douzi.ToString();    // 豆子
	}

	public void SetHeaderInfo() {
		Sprite sprite = (uiMng.GetPanel(UIPanelType.PlayerInfoPanel) as PlayerInfoPanel).GetHeadSprite();
		playerInfo.Find("Header").GetComponent<Image>().sprite = sprite;
	}

	/// <summary>
	/// 设置结算Panel的Title图片
	/// </summary>
	/// <param name="landowner"></param>
	/// <param name="win"></param>
	public void SetTitleSprite(bool landowner, bool win) {
		int index = landowner ? 0 : 1;
		if (!win) index += 2;
		titleImage.sprite = sprites[index];
	}




	/// <summary>
	/// 点击继续游戏, 开始新一局游戏
	/// </summary>
	public void OnClickContinue() {
		StartCoroutine(RestartGame());
	}

	IEnumerator RestartGame() {
		uiMng.PopStack(UIPanelType.GameOverPanel);
		GameFacade.Instance.WaitResponse(true);
		GameFacade.Instance.ShowPromot("正在重新开始...");
		yield return null;
		OnInit();
		yield return new WaitForSeconds(1);
		GameFacade.Instance.ShowPromot("正在准备...");

		roomBG.GetComponent<ReadyGameRequest>().RequestReadyGame();

	}
	public override void OnInit() {
		_OnInit(roomBG.transform);
	}

	/// <summary>
	/// 递归遍历执行所有OnInit方法
	/// </summary>
	/// <param name="trans"></param>
	void _OnInit(Transform trans) {
		if (trans == null) return;

		if (trans.GetComponent<BasePanel>())
			trans.GetComponent<BasePanel>().OnInit();

		for (int i = 0; i < trans.childCount; i++) {
			Transform child = trans.GetChild(i);
			if (child.childCount > 0)
				_OnInit(child);
		}
	}

}
