using UnityEngine;
using System.Collections;

public class NumberedButton : MonoBehaviour {

	public enum ButtonState
	{
		NONE,
		NORMAL,
		SELECTED,
		WIN,
		LOSE
	}

	public ButtonState m_buttonState = ButtonState.NORMAL;

	public GameObject m_normal;
	public GameObject m_selected;
	public GameObject m_win;
	public GameObject m_loose;
	public GameObject m_gameConstants;

	tk2dSlicedSprite m_btnSprite;
	tk2dTextMesh m_btnText;

	Vector3 m_normalBtnPos = new Vector3 (-6.2f, 9.715f, -0.1f);
	Vector3 m_winBtnPos = new Vector3(-6.160043f, 9.651662f, -0.15f);
	Vector3 m_winBtnScale = new Vector3(1.35f, 1.45f, 1f);

//	tk2dUIUpDownButton 

	// Use this for initialization
	void Start () {
		m_btnSprite = transform.GetComponentInChildren<tk2dSlicedSprite> (false);
//		Debug.Log ("m_btnSprite.gameObject.name= "+m_btnSprite.gameObject.name);
		m_btnText = transform.GetComponentInChildren<tk2dTextMesh>(false);
		m_gameConstants = GameObject.Find ("GameConstants");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void normal () {
		m_btnSprite.SetSprite ("dub_number_normal");
		m_btnSprite.transform.localPosition = m_normalBtnPos;
		m_btnSprite.scale = Vector3.one;
		m_btnText.font = m_gameConstants.GetComponent<GameConstants>().m_dubNumbersNormal60;
		changeButtonState (ButtonState.NORMAL);
	}

	public void selected () {
		m_btnSprite.SetSprite ("dub_number_select");
		m_btnSprite.transform.localPosition = m_normalBtnPos;
		m_btnSprite.scale = Vector3.one;
		m_btnText.font = m_gameConstants.GetComponent<GameConstants>().m_dubNumbersSelect60;
		changeButtonState (ButtonState.SELECTED);
	}

	public void win () {
		m_btnSprite.SetSprite ("dub_number_win");
		m_btnSprite.transform.localPosition = m_winBtnPos;
		m_btnSprite.scale = m_winBtnScale;
		m_btnText.font = m_gameConstants.GetComponent<GameConstants>().m_dubNumbersWin60;
		changeButtonState (ButtonState.WIN);
	}

	public void lose () {
		m_btnSprite.SetSprite ("dub_number_lose");
		m_btnSprite.transform.localPosition = m_normalBtnPos;
		m_btnSprite.scale = Vector3.one;
		m_btnText.font = m_gameConstants.GetComponent<GameConstants>().m_dubNumbersDead60;
		changeButtonState (ButtonState.LOSE);
	}

	public ButtonState getButtonState () {
		return m_buttonState;
	}

	public void disableTk2dUIUPDownButton () {
//		this.gameObject.GetComponent<tk2dUIUpDownButton> ().enable = false;
	}

	public void enableTk2dUIUPDownButton () {
//		this.gameObject.GetComponent<tk2dUIUpDownButton> ().enable = true;
	}

	void changeButtonState (ButtonState _State) {
		m_buttonState = _State;
	}

	public void setSelectedSprite () {
		m_normal.SetActive (false);
		m_selected.SetActive (true);
		m_win.SetActive (false);
		m_loose.SetActive (false);
	}

	public void setNormalSprite () {
		m_normal.SetActive (true);
		m_selected.SetActive (false);
		m_win.SetActive (false);
		m_loose.SetActive (false);
	}

	public void setWinSprite () {
		m_normal.SetActive (false);
		m_selected.SetActive (false);
		m_win.SetActive (true);
		m_loose.SetActive (false);
	}

	public void setLooseSprite () {
		m_normal.SetActive (false);
		m_selected.SetActive (false);
		m_win.SetActive (false);
		m_loose.SetActive (true);
	}
}
