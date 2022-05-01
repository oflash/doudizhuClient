using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UIPanelType
{
	Default = 0,
	CoverBG,        // 底层1(封面)
	RoomBG,         // 底层2, 包括所有玩家



	GamePanel,      // 游戏Panel, 所有的扑克牌所在层
	GameOverPanel, 	// 游戏结束结算面板


	SelectScenePanel,    // 选择房间背景
	JoinRoomPanel,      // 创建或加入房间3个Button
	LinkPanel,			// 连接前输入ip:port等信息

	PlayerInfoPanel,    // 玩家信息
	PromptPanel,        // 系统提示
	LoadingPanel,       // 加载

	HeaderPanel,        // 修改头像Panel

	TopLayerPanel,		// 顶层Panel, 一直在最顶层, 一些按钮
}