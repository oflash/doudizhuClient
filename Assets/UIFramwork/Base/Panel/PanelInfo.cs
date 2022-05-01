using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[Serializable]
public class PanelInfo
{
	[JsonConverter(typeof(StringEnumConverter))]
	public UIPanelType uiPanelType;
	public string path;
}
