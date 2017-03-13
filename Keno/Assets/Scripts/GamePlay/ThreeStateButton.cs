using UnityEngine;
using System.Collections;

public class ThreeStateButton : MonoBehaviour {

	public GameObject m_normal;
	public GameObject m_active;
	public GameObject m_disable;
	public bool isBlink = false;
	public bool isEnable = true;
	int count = 0;

	// Use this for initialization
	void Start () {
		
	}

	void FixedUpdate () {
		if (isBlink) {
			blink ();
		}
	}

	public void disableButtons () {
		disableTwoStatesOfTheButton ();
		Debug.Log ("disableButtons");
		m_normal.SetActive (false);
		m_active.SetActive (false);
		m_disable.SetActive (true);
		isEnable = false;
	}

	public void enableButtons () {
		enableTwoStatesOfTheButton ();
		Debug.Log ("enableButtons");
		m_normal.SetActive (true);
		m_active.SetActive (false);
		m_disable.SetActive (false);
		isEnable = true;
	}

	public void disableTwoStatesOfTheButton () {
		this.gameObject.GetComponent<tk2dUIUpDownButton> ().isActive = false;
	}

	public void enableTwoStatesOfTheButton () {
		this.gameObject.GetComponent<tk2dUIUpDownButton> ().isActive = true;
	}

	public void startBlink () {
		isBlink = true;
	}

	public void stopBlink () {
		isBlink = false;
		m_normal.SetActive (true);
		m_active.SetActive (false);
	}

	void blink () {
		count++;

		if (count > 30) {
			count = 0;
			if (m_normal.activeSelf) {
				m_normal.SetActive (false);
				m_active.SetActive (true);
			} else {
				m_normal.SetActive (true);
				m_active.SetActive (false);
			}
		}
	}
}
