using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[Serializable]
public class AudioInfo
{
	[JsonConverter(typeof(StringEnumConverter))]
	public AudioType audioType;
	public string path;

	public AudioInfo(AudioType audioType, string path) {
		this.audioType = audioType;
		this.path = path;
	}

}
