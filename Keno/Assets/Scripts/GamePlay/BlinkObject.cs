using UnityEngine;
using System.Collections;

public class BlinkObject : MonoBehaviour {

	public bool isBlink = false;
	int count = 30;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (isBlink) {
			blink ();
		}
	}

	public void startBlink () {
		isBlink = true;
	}

	public void stopBlink () {
		isBlink = false;
		this.transform.localPosition = new Vector3 (this.transform.localPosition.x, this.transform.localPosition.y, -0.2f);
	}

	void blink () {
		count++;
		if (count > 30) {
			count = 0;
			if (this.transform.localPosition.z == 0) {
				this.transform.localPosition = new Vector3 (this.transform.localPosition.x, this.transform.localPosition.y, -0.2f);
			} else {
				this.transform.localPosition = new Vector3 (this.transform.localPosition.x, this.transform.localPosition.y, 0);
			}
		}
	}
}
