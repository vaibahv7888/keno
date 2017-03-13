//#define LOG_TRACE_INFO
//#define LOG_EXTRA_INFO

using UnityEngine;
using System.Collections;
using SimpleJSON;

//------------------------------------------------------------------------------
// class definition
//------------------------------------------------------------------------------
public class MenuController : MonoBehaviour
{
	private static MenuController menuController;

	public GameObject m_mainMenu;
	public GameObject m_pinMenu;
	public tk2dUITextInput m_pinInputField;
	public tk2dTextMesh m_warningMessage;
	public TestJson m_testJson;
	public tk2dTextMesh m_creditText;
	public GameObject m_networkPopUp;

	string m_serverInitResponse;
	bool isPopUp = false;

	//--------------------------------------------------------------------------
	// public static methods
	//--------------------------------------------------------------------------

	public void onCashInClicked () {
		
	}

	public void onCashOutClicked () {
		
	}

	public void onFireBallKenoClicked () {
		Debug.Log ("onFireBallKenoClicked");
		if (isPopUp) {
			return;
		}
		MainController.SwitchScene("Game Scene 2");
	}

	public void onTextChanged () {
		Debug.Log ("onTextChanged");
		if (isPopUp) {
			return;
		}
		if (m_pinInputField.Text.Length == 4) {
			if (m_pinInputField.Text == "1234") {
				m_pinMenu.SetActive (false);
				m_mainMenu.SetActive (true);
				m_pinInputField.Text = "";
				m_warningMessage.text = "";
			} else {
				m_warningMessage.text = "Wrong Pin";
			}
		} else {
			m_warningMessage.text = "";
		}
	}

	public void onRetryBtn () {
		initServer ();
	}

	//--------------------------------------------------------------------------
	// protected mono methods
	//--------------------------------------------------------------------------
	protected void Awake()
	{
		menuController = this;
	}
	
	protected void OnDestroy()
	{
		if(menuController != null)
		{
			menuController = null;
		}
	}
	
	protected void OnDisable()
	{
	}
	
	protected void OnEnable()
	{
	}
	
	protected void Start()
	{
//		if (PlayerPrefs.GetInt ("isFromGameScene", 0) == 0) {
//			m_pinMenu.SetActive (true);
//			m_mainMenu.SetActive (false);
//		} else {
//			PlayerPrefs.SetInt("isFromGameScene", 0);
//			PlayerPrefs.Save ();
//			m_pinMenu.SetActive (false);
//			m_mainMenu.SetActive (true);
//		}

		initServer ();
	}

	void initServer () {
//		string a = TestJson.sharedInstance ().serverInit ();
		string temp = m_testJson.serverInit ();
		if (!temp.Equals ("-1")) {
			if (m_networkPopUp.activeSelf) {
				isPopUp = false;
				m_networkPopUp.SetActive (false);
			}
			m_serverInitResponse = temp;
		} else {
			isPopUp = true;
			m_networkPopUp.SetActive (true);
			m_pinMenu.SetActive (false);
			m_mainMenu.SetActive (false);
			return;
		}

		if (PlayerPrefs.GetInt ("isFromGameScene", 0) == 0) {
			m_pinMenu.SetActive (true);
			m_mainMenu.SetActive (false);
		} else {
			PlayerPrefs.SetInt("isFromGameScene", 0);
			PlayerPrefs.Save ();
			m_pinMenu.SetActive (false);
			m_mainMenu.SetActive (true);
		}

		JSONNode N = JSON.Parse (m_serverInitResponse);

		m_creditText.text = "CREDIT $" + N ["user"] ["bank"].AsFloat;
	}

	protected void Update()
	{
//		if(Input.GetMouseButtonDown(0) == true)
//		{
//			MainController.SwitchScene("Game Scene");
//		}
	}

	//--------------------------------------------------------------------------
	// private methods
	//--------------------------------------------------------------------------
}
