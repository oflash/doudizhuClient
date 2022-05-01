using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SpriteInfo
{
	[JsonConverter(typeof(StringEnumConverter))]
	public SpriteType spriteType;
	public List<string> paths = new List<string>();		// 可以是一个或多个
}
