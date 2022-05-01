using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Message
{
	/// <summary>
	/// 数组长度
	/// </summary>
	public int Length => data.Length;

	/// <summary>
	/// 字节数组, 包括4个字节的存储数据长度
	/// </summary>
	public byte[] Data => data;
	private byte[] data;



	/* Message对象一创建, this.Data就一定占有内存 */
	#region 构造函数
	/// <summary>
	/// 实例化一个没有内容的Message容器(用于接收消息, 可能一次接收多个消息)
	/// </summary>
	/// <param name="len"></param>
	public Message(int len) {
		this.data = new byte[len];
	}

	/// <summary>
	/// 实例化一个需要发送的消息(用于发送消息, 一次发送一个消息)
	/// </summary>
	/// <param name="content"></param>
	public Message(string msg) {
		this.data = GetMessageBytes(msg);
	}
	#endregion


	/// <summary>
	/// 解析自己Data的单条消息
	/// </summary>
	public string GetMessageString() {
		// if (size <= 4) return null;
		int len = BitConverter.ToInt32(data, 0);        // len不包括自身

		string msg = Encoding.UTF8.GetString(data, 4, len);
		return msg;
	}

	/// <summary>
	/// 解析Message byte[], (接收消息后)
	/// </summary>
	/// <param name="offset"></param>
	/// <param name="size"></param>
	/// <returns></returns>
	public string[] GetMessageStrings(int offset, int size) {
		if (size <= 4) return null;

		List<string> msgs = new List<string>();

		int len;
		Debug.Log("data.Length:" + data.Length);
		for (int i = offset; i < size && i < data.Length; i += 4 + len) {
			len = BitConverter.ToInt32(data, i);
			// Debug.Log("i:" + i + ", len:" + len);
			//Console.WriteLine(len);
			string msg = Encoding.UTF8.GetString(data, i + 4, len);
			msgs.Add(msg);
		}
		return msgs.ToArray();
	}


	/// <summary>
	/// 字符串数据, 转化成byte[], (发送消息前)
	/// </summary>
	/// <param name="actionCode"></param>
	/// <param name="msg"></param>
	/// <returns></returns>
	public byte[] GetMessageBytes(string msg) {
		byte[] data = Encoding.UTF8.GetBytes(msg);
		byte[] len = BitConverter.GetBytes(data.Length);    // 不包括自己的数据长度

		byte[] res = len.Concat(data).ToArray();
		return res;
	}
}
