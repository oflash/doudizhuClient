using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinRequest : BaseRequest
{
	GamePanel gamePanel;
	RoomBG roomBG;
	GameOverPanel gameOverPanel => gameFacade.UIMng.GetPanel(UIPanelType.GameOverPanel) as GameOverPanel;

	protected override void Start() {
		requestCode = RequestCode.PlayGame;
		actionCode = ActionCode.Win;            // 由胜利者发送胜利请求
		gamePanel = GetComponent<GamePanel>();
		roomBG = transform.parent.GetComponent<RoomBG>();


		base.Start();
	}


	/// <summary>
	/// 发送胜利请求
	/// </summary>
	public void RequestWin() {
		StartCoroutine(Win());
	}

	IEnumerator Win() {

		Content send = new Content(ContentType.Default, actionCode, requestCode);
		send.id = gameFacade.Id;

		SendRequest(send);

		gameFacade.WaitResponse(true);
		yield return new WaitUntil(() => Response);
		response = false;
		// yield return new WaitForSeconds(1);
		gameFacade.WaitResponse(false);

	}


	public override void OnResponse(Content content) {
		base.OnResponse(content);

		if (content.returnCode == ReturnCode.Success) {

			Player remote = gameFacade.GetPlayer(content.id);       // 我胜利了
			Player local = gameFacade.GetPlayer(gameFacade.Id);     // 我收到你胜利的消息了

			if (local.Id != remote.Id) {    // 我不是出完所有牌的胜利者, 将我剩余的牌发送给大家
				gamePanel.RequestRemaining();
			}
			foreach (Player player in roomBG.players) {
				if (remote.Landowner == player.Landowner) {         // 与胜利者在同一阶级, 笑
					player.PlayerLaunch();
				} else {
					player.PlayerWeep();
				}
			}

			if (remote.Landowner == local.Landowner) {
				gameFacade.PlayMusic(AudioType.MusicEx_Win, true, false, true);
				gamePanel.RequestResult(true);                          // 将各自的结算结果发送给服务器
				gameOverPanel.SetTitleSprite(local.Landowner, true);    // 结算界面设置TitleImage
			} else {
				gameFacade.PlayMusic(AudioType.MusicEx_Lose, true, false, true);
				gamePanel.RequestResult(false);
				gameOverPanel.SetTitleSprite(local.Landowner, false);
			}

			if (remote.Landowner) {
				gameFacade.ShowPromot("地主胜利...");
				gameFacade.RecordLog("地主胜利...", true);
			} else {
				gameFacade.ShowPromot("农民胜利...");
				gameFacade.RecordLog("农民胜利...", true);
			}
		}
	}
}
