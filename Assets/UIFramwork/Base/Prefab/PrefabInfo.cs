using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[Serializable]
public class PrefabInfo
{
	[JsonConverter(typeof(StringEnumConverter))]
	public PrefabType prefabType;
	public string path;
}
