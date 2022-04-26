using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class ServerProcess
{
	#region 单例模式
	private static ServerProcess instance = new ServerProcess();
	private ServerProcess() { }
	public static ServerProcess Instance => instance;
	#endregion

	string cmd = Application.streamingAssetsPath + "/Plugins/游戏服务器端.exe 127.0.0.1 9876";
	Process process;
	public string fileName => Application.streamingAssetsPath + "/游戏服务器端.exe";  // 程序路径
	public string args => _ip + " " + _port;  // 命令行参数

	public bool Running => running;     // 是否开启服务器
	private bool running = false;

	string _ip; int _port;

	public void OpenServer(string ip = "127.0.0.1", int port = 9876) {
		this._ip = ip; this._port = port;
		try {
			process = new Process();
			ProcessStartInfo startInfo = new ProcessStartInfo(fileName, args);
			process.StartInfo = startInfo;
			process.StartInfo.UseShellExecute = true;

			process.EnableRaisingEvents = true;
			process.Exited += myProcess_Exited;

			process.Start();

			Thread.Sleep(500);


			running = true;         // 告诉外界, 已开启服务器

			// string output = process.StandardOutput.ReadToEnd();
			// UnityEngine.Debug.Log("Wait");
			// process.WaitForExit();
			// UnityEngine.Debug.Log("WaitEnd");

			UnityEngine.Debug.Log("成功");
		} catch (System.Exception e) {
			UnityEngine.Debug.Log("失败：" + e.Message);
		}
	}

	private void myProcess_Exited(object sender, System.EventArgs e) {
		running = false;
		GameFacade.Instance.ShowPromot("服务器开启失败!");
		try {
			GameFacade.Instance.WaitResponse(false);
		} catch (System.Exception ex) {
			UnityEngine.Debug.Log(ex.Message);
		}


		UnityEngine.Debug.Log("程序退出");
	}



	public void CloseServer() {
		process.Close();
	}



}
