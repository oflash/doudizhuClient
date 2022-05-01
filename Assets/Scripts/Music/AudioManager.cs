using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AudioManager : BaseManager
{
	public AudioManager(GameFacade gameFacade) : base(gameFacade) { }

	public Dictionary<AudioType, AudioClip> dicAudio;

	AudioSource mainSource;
	public bool mute { get => mainSource.mute; set => mainSource.mute = value; }

	public override void OnInit() {
		mainSource = gameFacade.gameObject.AddComponent<AudioSource>();

		// Debug.Log(Time.realtimeSinceStartup);
		// ReadAllJson();
		gameFacade.StartCoroutine(ReadAllJson());   // 加载本地audio路径

		// string path = "最新音效/MusicEx/";
		// AudioClip[] clips = Resources.LoadAll<AudioClip>(path);
		// string s = "";
		// List<AudioInfo> infoList = new List<AudioInfo>();
		// foreach (AudioClip clip in clips) {
		// 	s += "{\"audioType\": " + "\"" + clip.name + "\",\n" +
		// 	"\"path\": " + "\"" + path + clip.name + "\"},\n";
		// 	// s += clip.name + ",";
		// }

		// Debug.Log(s);

		gameFacade.StartCoroutine(FirstPlayMusic());    // 等待加载完成后, 播放背景音乐

	}

	string[] audioPaths = {
		"AudioGuidePath",
		"AudioAvatarPath",
		"AudioManPath",
		"AudioWomanPath",
		"AudioSpecialPath",
		"AudioMusicExPath",
	};

	private IEnumerator FirstPlayMusic() {
		yield return new WaitUntil(() => loaded);
		Debug.Log("加载音频完成");
		PlayMusic(AudioType.MusicEx_Welcome, false, true, true);    // 背景音乐, 使用主声源
	}

	private IEnumerator ReadAllJson() {
		for (int i = 0; i < audioPaths.Length; i++) {
			ReadJson(audioPaths[i]);
			yield return null;
		}
		loaded = true;          // 音乐加载完成
	}

	Dictionary<AudioType, string> audioPath;
	private void ReadJson(string path) {
		if (audioPath == null) audioPath = new Dictionary<AudioType, string>();
		try {
			TextAsset json = Resources.Load<TextAsset>(path);
			// Debug.Log(json);
			AudioInfo[] audioInfos = JsonConvert.DeserializeObject<AudioInfo[]>(json.text);

			foreach (AudioInfo audioInfo in audioInfos) {
				audioPath.Add(audioInfo.audioType, audioInfo.path);
			}

		} catch (System.Exception) {
			Debug.Log("加载资源失败");
			gameFacade.ShowPromot("加载资源失败请重启游戏!");
		}
	}

	private AudioClip GetAudio(AudioType audioType) {
		if (audioType == AudioType.None) return null;
		if (dicAudio == null) dicAudio = new Dictionary<AudioType, AudioClip>();
		AudioClip audioClip;
		dicAudio.TryGetValue(audioType, out audioClip);
		if (audioClip == null) {
			string path;
			try {
				audioPath.TryGetValue(audioType, out path);
				audioClip = Resources.Load<AudioClip>(path);
				dicAudio.Add(audioType, audioClip);
			} catch {
				Debug.Log("没有该音频或该音频还未加载...");
			}
		}
		return audioClip;
	}


	/// <summary>
	/// 播放音乐
	/// </summary>
	/// <param name="audioType">声音剪辑</param>
	/// <param name="kill">是否杀</param>
	/// <param name="loop">是否循环, 默认不循环</param>
	/// <param name="main">是否使用主声源, 默认不使用</param>
	public void PlayMusic(AudioType audioType, bool kill = true, bool loop = false, bool main = false) {
		AudioClip clip = GetAudio(audioType);
		if (clip == null) return;
		// Debug.Log(clip);
		// Debug.Log(clip.length);
		if (!main) {
			AudioSource audioSource = gameFacade.gameObject.AddComponent<AudioSource>();
			audioSource.clip = clip;
			audioSource.loop = loop;
			audioSource.mute = mute;        // 使用非主声源, 需要读取主声源mute
			float time = clip.length;
			audioSource.Play();
			if (kill) cnt_music++;
			gameFacade.StartCoroutine(PlayingMusic(audioSource, cnt_music));

			Object.Destroy(audioSource, time);
		} else {        // 使用主声源
			mainSource.loop = loop;
			if (mainSource.clip == null ||
				mainSource.clip != null && clip.name != mainSource.clip.name) {    // 正在播放这个音乐, 不用替换
				mainSource.clip = clip;
				mainSource.Play();
			}
		}
	}

	int cnt_music = 0;  // 使用的第几个非主声源

	/// <summary>
	/// 播放音乐中(非主声源)
	/// </summary>
	/// <param name="audioSource"></param>
	/// <returns></returns>
	IEnumerator PlayingMusic(AudioSource audioSource, int cnt) {

		while (true) {
			if (audioSource == null) break;
			if (cnt != cnt_music) audioSource.clip = null;      // 播放新声音时, 原来声音不使用
			audioSource.mute = mute;
			yield return null;
		}
		// Debug.Log("播放完成");
	}


}
