using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTest : MonoBehaviour
{
	// Start is called before the first frame update
	void Start() {

		// Debug.Log(StartCoroutine(Test1()));
		coroutine = StartCoroutine(Test1());


	}
	Coroutine coroutine;


	IEnumerator Test1() {
		Debug.Log("Test1 Start");
		yield return new WaitForSeconds(1);
		Debug.Log("Test1 Completed");
	}


	IEnumerator Test2() {

		Debug.Log("Test2 Start");
		yield return new WaitUntil(() => StartCoroutine(Test1()) != null);
		Debug.Log("Test2 Completed");
	}




	// Update is called once per frame
	void Update() {
		Debug.Log(coroutine);
	}
}
