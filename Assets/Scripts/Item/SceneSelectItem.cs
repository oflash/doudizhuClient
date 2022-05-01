using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneSelectItem : MonoBehaviour
{
	UIManager _uiMng;
	UIManager uiMng {
		get {
			if (_uiMng == null)
				_uiMng = GameFacade.Instance.UIMng;
			return _uiMng;
		}
	}

	Image _image;
	Image image {
		get {
			if (_image == null) _image = GetComponent<Image>();
			return _image;
		}
	}
	Image _text;
	Image text {
		get {
			if (_text == null) _text = transform.GetChild(0).GetComponent<Image>();
			return _text;
		}
	}

	// public int Id { get => id; set => id = value; }
	private int id;

	public int Id {
		set {
			// Debug.Log(value);
			id = value;
		}
		get {
			return id;
		}
	}

	public void SetSprite(Sprite s1, Sprite s2) {
		image.sprite = s1;
		text.sprite = s2;
	}
}
