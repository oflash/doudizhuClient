using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : BaseManager
{
	public PlayerManager(GameFacade gameFacade) : base(gameFacade) { }

	public List<Player> players;        	// 加入房间才添加到该链表
	public bool Full => players.Count == 3;	// 房间是否满了

	Dictionary<string, int> dicPlayers;     // id -> index(本地index, 不是服务器index), 加入房间后设置
	Dictionary<PrefabType, GameObject> playerPrefabs => gameFacade.UIMng.dicPrefabs;


	public override void OnInit() {
		dicPlayers = new Dictionary<string, int>();
		players = new List<Player>();

	}


	/// <summary>
	/// 加入房间
	/// </summary>
	/// <param name="player"></param>
	public void AddPlayer(Player player) {
		dicPlayers.Add(player.Id, player.Index);        // 对应本地index
		players.Add(player);
	}


	/// <summary>
	/// 通过Id获取Player
	/// </summary>
	/// <param name="id"></param>
	public Player GetPlayer(string id) {
		foreach (Player player in players) if (player.Id == id) return player;
		return null;
	}


	public void RemovePlayer(Player player) {
		dicPlayers.Remove(player.Id);
		players.Remove(player);
	}
}
