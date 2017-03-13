using UnityEngine;
using System.Collections;

public class NumberButtonsProperties : MonoBehaviour {

	GameObject selectImage;

	// Use this for initialization
	void Start () {
//		selectImage = GameObject.Find ("dub_number_select_1");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void getNumberedButton () {
//		GameObject[] numbrBtns = new GameObject[80];
//		for (int i = 0; i < 80; i++) {
//			numbrBtns [i] = GameObject.Find ("" + i + 1);
//			numbrBtns [i].gameObject.GetComponent<tk2dUIUpDownButton> ().downStateGO = selectImage;
//		}
		selectImage = GameObject.Find ("dub_number_select_1");
		this.gameObject.GetComponent<tk2dUIUpDownButton> ().downStateGO = GameObject.Find ("dub_number_select_1");
	}

}
