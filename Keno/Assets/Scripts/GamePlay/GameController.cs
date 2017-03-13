//#define LOG_TRACE_INFO
//#define LOG_EXTRA_INFO

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using Random = UnityEngine.Random;

//------------------------------------------------------------------------------
// class definition
//------------------------------------------------------------------------------
public class GameController : MonoBehaviour
{
	private static GameController gameController;

	public enum GamePlayState
	{
		NONE,
		READYTOPLAY,
		PLAYING,
		RESULTS,
		AFTER_RESULTS,
		POPUP
	}

	public GamePlayState m_gamePlayState = GamePlayState.NONE;

	public ThreeStateButton[] m_threeStateButtons;

	public NumberedButton[] m_numberedButtons;

	public float[] m_betRange;

	public TestJson m_testJson;

	public tk2dTextMesh m_bankTxt;
	public tk2dTextMesh m_betTxt;
	public tk2dTextMesh m_winTxt;
	public tk2dTextMesh m_picksTxt;
	public tk2dTextMesh[] m_hitsTxt;
	public tk2dTextMesh[] m_paysTxt;
	public tk2dTextMesh m_mainGameWinAmount;
	public GameObject m_paysHolderHighlight;

	public string m_playRequestStr;

	public Transform m_ballStartPos;

	public GameObject m_bingoBallPrefabEven;
	public GameObject m_bingoBallPrefabOdd;

	List<GameObject> m_bingoBalls;

	private int m_currentNumberPressed;

	List<int> m_playerPickedNumbers;
	float m_bank;
	float m_maxBet;
	int m_currentBetIndex;
	int m_minPicks;
	int m_maxPicks;
	int m_currentPicks;

	string m_serverInitResponse;
	string m_serverResponsePlay;

	struct Paytable {
		public int m_picks;
		public Dictionary<string, float> m_multipliers;
		public string[] m_hits;
		public float[] m_pays;
	}

	List<Paytable> m_payTable;

	Dictionary<int, Dictionary<string, float>> PayTable;

	[Serializable]
	public class User
	{
		public int deviceCode { get; set; }
	}

	[Serializable]
	public class PlayRequest
	{
		public string action { get; set; }
		public int gameID { get; set; }
		public User user { get; set; }
		public List<int> playerPicks { get; set; }
		public double bet { get; set; }
	}

	[Serializable]
	public class PlayResponse
	{
		public int bank { get; set; }
		public List<int> pickedNumbers { get; set; }
		public List<int> matchedNumbers { get; set; }
		public double win { get; set; }
	}

	PlayResponse m_playResponse;

	public string[] m_messages;
	public string[] m_tempMessages;
	public string[] m_staticMessages;

	bool m_canUseQuickPick = false;
	List<int> m_matchedNumbers;
	List<int> m_serverPickedNumbers;

	bool isLastBall = false;
	bool toLastBallFinalPos = false;
	float speed = 30f;
	Vector3 m_numberPos = Vector3.zero;
	Vector3 m_finalPos = Vector3.zero;

	public Transform m_firstNumberPos;
	public Transform m_seconNumberePos;
	public Transform m_finalBallPos;

	public GameObject m_tubeBaseCollider;
	public GameObject m_ballStopper;

	public GameObject m_exitPopUp;
	bool isPopUp = false;
	public GameObject m_retryPopUp;
	//--------------------------------------------------------------------------
	// public static methods
	//--------------------------------------------------------------------------
	//--------------------------------------------------------------------------
	// protected mono methods
	//--------------------------------------------------------------------------
	protected void Awake()
	{
		gameController = this;
	}
	
	protected void OnDestroy()
	{
		if(gameController != null)
		{
			gameController = null;
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
		startGame ();
	}

	void startGame () {
		initializeGameVariables ();
		clearPayTableText ();
		if (initServer ()) {
			if (m_retryPopUp.activeSelf) {
				m_retryPopUp.SetActive (false);
				enaableAllButtons ();
			}
			changeGamePlayState (GamePlayState.READYTOPLAY);
		} else {
			bringRetryPopUp ();
		}
		setUITexts ();
		m_paysHolderHighlight.gameObject.SetActive (false);
//		m_mainGameWinAmount.text = m_messages [0];
//		staticMessageId = 2;
		setStaticMsg (2);
		m_winTxt.text = "00";

	}

	float dtMessageTime = 0;
	bool isTempMessage = false;
	bool isTranslateMsg = false;
	bool isTraslateOut = false;
	bool istraslateIn = false;
	int tempMessageId = 0;
	int staticMessageId = 0;
	Vector3 centerPos = new Vector3(-2.873037f, -0.63f, -0.1f);
	Vector3 leftPos = new Vector3(-8f, -0.63f, -0.1f);
	Vector3 rightPos = new Vector3(2.41f, -0.63f, -0.1f);

	protected void FixedUpdate()
	{
//		if(Input.GetMouseButtonDown(0) == true)
//		{
//			MainController.SwitchScene("Menu Scene");
//		}

		if (isLastBall) {
			m_lastBall.transform.position = Vector3.MoveTowards (m_lastBall.transform.position, m_numberPos, speed * Time.deltaTime);

			if(Vector3.Distance(m_lastBall.transform.position, m_numberPos) < 0.1f){
				//It is within ~0.1f range, do stuff
//				Debug.Log (">>>>>>>>>>>>vVVVVV");
				isLastBall = false;
				StartCoroutine (pauseLastBallAtLastNumber ());
			}
		} else if (toLastBallFinalPos) {
			m_lastBall.transform.position = Vector3.MoveTowards (m_lastBall.transform.position, m_finalBallPos.position, speed * Time.deltaTime);

			if (Vector3.Distance (m_lastBall.transform.position, m_finalBallPos.position) < 0.1f) {
//				Debug.Log (">>>>>>>>>>>>bBBBBB");
				isLastBall = false;
				toLastBallFinalPos = false;
				endLastBall ();
			}
		}

		if (isTranslateMsg) {
			translateMessages ();
		} else if (isTempMessage && dtMessageTime <= 3.0f) {
			dtMessageTime += Time.fixedDeltaTime;
//			m_mainGameWinAmount.text = m_tempMessages [tempMessageId];
//			Debug.Log ("tempMessageId= " + tempMessageId);
			Debug.Log ("AAAA 1");
		} else if (dtMessageTime > 3.0f) {
			dtMessageTime = 0;
//			isTempMessage = false;
//			m_mainGameWinAmount.text = m_staticMessages [staticMessageId];
			setStaticMsg (0);
			Debug.Log ("AAAA 2");
		} else if (!isTempMessage && dtMessageTime < 3.0f && dtMessageTime > 0) {
			dtMessageTime = -1f;
			setStaticMsg (0);
//			isTempMessage = false;
//			m_mainGameWinAmount.text = m_staticMessages [staticMessageId];
			Debug.Log ("AAAA 3");
		}
//		else if (!isTempMessage) {
//			dtMessageTime = 0;
//			isTempMessage = false;
//			m_mainGameWinAmount.text = m_staticMessages [staticMessageId];
////			Debug.Log ("staticMessageId= " + staticMessageId);
//			Debug.Log ("AAAA 4");
//		}

//		if (isTranslateMsg) {
//			m_mainGameWinAmount.transform.position = Vector3.MoveTowards (m_mainGameWinAmount.transform.position, centerPos, speed * Time.deltaTime);
//			if (Vector3.Distance (m_mainGameWinAmount.transform.position, centerPos) < 0.1f) {
//				m_mainGameWinAmount.transform.position = centerPos;
//				isTranslateMsg = false;
//			}
//		}
	}

	//--------------------------------------------------------------------------
	// private methods
	//--------------------------------------------------------------------------
	float textAlpha = 1f;
	void translateMessages () {
		//		m_mainGameWinAmount.transform.position = leftPos;	-2.873037+8
		if (isTraslateOut) {
			m_mainGameWinAmount.transform.position = Vector3.MoveTowards (m_mainGameWinAmount.transform.position, rightPos, speed * Time.deltaTime);
			textAlpha -= 5f * Time.deltaTime;
			m_mainGameWinAmount.GetComponent<tk2dTextMesh> ().color = new Color (255f, 255f, 255f, textAlpha);
			if (Vector3.Distance (m_mainGameWinAmount.transform.position, rightPos) < 0.1f) {
				m_mainGameWinAmount.transform.position = leftPos;
				if (isTempMessage) {
					textAlpha = 0;
					m_mainGameWinAmount.GetComponent<tk2dTextMesh> ().color = new Color (255f, 255f, 255f, textAlpha);
					if (tempMessageId == 1) {
						m_mainGameWinAmount.text = m_tempMessages [tempMessageId] + m_winTxt.text;	//"WON $"
					} else {
						m_mainGameWinAmount.text = m_tempMessages [tempMessageId];
					}
				} else {
					m_mainGameWinAmount.text = m_staticMessages [staticMessageId];
				}
//				isTranslateMsg = false;
				isTraslateOut = false;
				istraslateIn = true;
			}
		} else if (istraslateIn) {
			m_mainGameWinAmount.transform.position = Vector3.MoveTowards (m_mainGameWinAmount.transform.position, centerPos, speed * Time.deltaTime);
			textAlpha += 5f * Time.deltaTime;
			m_mainGameWinAmount.GetComponent<tk2dTextMesh> ().color = new Color (255f, 255f, 255f, textAlpha);
			if (Vector3.Distance (m_mainGameWinAmount.transform.position, centerPos) < 0.1f) {
				textAlpha = 1f;
				m_mainGameWinAmount.GetComponent<tk2dTextMesh> ().color = new Color (255f, 255f, 255f, textAlpha);
				m_mainGameWinAmount.transform.position = centerPos;
				istraslateIn = false;
				isTranslateMsg = false;
			}
		}
	}

	private void setTempMessage (int _messageId) {
		if (isTempMessage) {
			return;
		}
		dtMessageTime = 0;
		isTempMessage = true;
		tempMessageId = _messageId;
		isTranslateMsg = true;
		isTraslateOut = true;
		textAlpha = 1f;
		//		m_mainGameWinAmount.transform.position = leftPos;
	}

	private void setStaticMsg(int _messageId) {
		dtMessageTime = 0;
		isTempMessage = false;
		staticMessageId = _messageId;
		isTranslateMsg = true;
		isTraslateOut = true;
		textAlpha = 1f;
	}

	void startLastBall () {
		m_lastBall.GetComponent<SphereCollider> ().enabled = false;
		m_lastBall.GetComponent<Rigidbody> ().Sleep ();
		m_lastBall.GetComponent<Rigidbody> ().isKinematic = true;

		int _pn = m_serverPickedNumbers [19];		//x:0.769, y:-0.785

		if (_pn < 41) {
			m_numberPos = new Vector3 (m_firstNumberPos.transform.position.x + ((_pn % 10 - 1) * 0.769f),
				m_firstNumberPos.transform.position.y + ((((int)(_pn / 10)) % 10) * -0.785f),
				m_firstNumberPos.transform.position.z);
		} else {
//			m_numberPos = m_firstNumberPos.position;		//	m_numberedButtons [m_serverPickedNumbers [19]].gameObject.transform.position;
			m_numberPos = new Vector3 (m_seconNumberePos.transform.position.x + ((_pn % 10 - 1) * 0.769f),
				m_seconNumberePos.transform.position.y + ((((int)(_pn / 10)) % 10 - 4) * -0.785f),
				m_seconNumberePos.transform.position.z);
		}

		Debug.Log ("m_numberPos= " + m_numberPos);

		isLastBall = true;
	}

	void endLastBall () {
		m_lastBall.GetComponent<SphereCollider> ().enabled = true;
		m_lastBall.GetComponent<Rigidbody> ().WakeUp ();
		m_lastBall.GetComponent<Rigidbody> ().isKinematic = false;

		m_lastBall.GetComponent<Rigidbody> ().AddForce (800f, 100f, 0);

		StartCoroutine (startBingoBalls ());
	}

	private void initializeGameVariables () {
		m_playerPickedNumbers = new List<int> ();
		m_bingoBalls = new List<GameObject> ();
		m_payTable = new List<Paytable> ();
		m_matchedNumbers = new List<int> ();
		m_serverPickedNumbers = new List<int> ();
		m_currentBetIndex = 0;
		m_bank = 1000f;
		m_maxBet = m_betRange [m_betRange.Length - 1];
		m_minPicks = 2;
		m_maxPicks = 10;
		m_currentPicks = 0;
		m_canUseQuickPick = true;

		m_playResponse = new PlayResponse ();
	}

	private bool initServer () {
		string temp = m_testJson.serverInit ();
		if (!temp.Equals ("-1")) {
			m_serverInitResponse = temp;
		} else {
			return false;
		}
		JSONNode N = JSON.Parse (m_serverInitResponse);
		Debug.Log ("VAI= " + N ["user"].ToJSON (1));
		m_bank = N ["user"] ["bank"].AsFloat;
		m_minPicks = N ["gameConfig"] ["minPicks"].AsInt;
		m_maxPicks = N ["gameConfig"] ["maxPicks"].AsInt;
		int paytableLength = N ["gameConfig"] ["paytable"].Count;
		for (int i = 0; i < paytableLength; i++) {
			Paytable tempPaytable = new Paytable ();
			tempPaytable.m_picks = N ["gameConfig"] ["paytable"] [i] ["picks"].AsInt;
//			m_payTable.m_picks
			int multiplierCount = N ["gameConfig"] ["paytable"] [i] ["multipliers"].Count;
			tempPaytable.m_hits = new string[multiplierCount];
			tempPaytable.m_pays = new float[multiplierCount];

			for (int j = 0; j < multiplierCount; j++) {
				string picksCount = N ["gameConfig"] ["paytable"] [i] ["multipliers"] [j].ToString ();
				picksCount = picksCount.Substring (2);
				int indx = picksCount.IndexOf ("\"");
				picksCount = picksCount.Substring (0, indx);

				tempPaytable.m_hits[j] = picksCount;

				float picksValue = N ["gameConfig"] ["paytable"] [i] ["multipliers"] [j] [0].AsFloat;

				tempPaytable.m_pays [j] = picksValue;

//				Debug.Log ("tempPaytable.m_hits ["+j+"]= "+tempPaytable.m_hits [j]);
//				Debug.Log ("tempPaytable.m_pays ["+j+"]= "+tempPaytable.m_pays [j]);

				tempPaytable.m_multipliers = new Dictionary<string, float> ();
				tempPaytable.m_multipliers.Add (picksCount, picksValue);
			}
			m_payTable.Add (tempPaytable);
		}
		return true;
	}

	void setUITexts () {
		m_bankTxt.text = m_bank.ToString ();
		m_betTxt.text = m_betRange[m_currentBetIndex].ToString ();
//		Debug.Log ("m_currentPicks= " + m_currentPicks);
		if (m_currentPicks == 0) {
			m_picksTxt.text = "--";
		} else {
			m_picksTxt.text = m_currentPicks.ToString ();
		}

	}

	private void disableAllButtons () {
		for (int i = 0; i < m_threeStateButtons.Length; i++) {
			m_threeStateButtons [i].disableButtons ();
		}
	}
	private void enaableAllButtons () {
		for (int i = 0; i < m_threeStateButtons.Length; i++) {
			m_threeStateButtons [i].enableButtons ();
		}
		if (!m_canUseQuickPick) {
//			m_threeStateButtons [3].disableButtons ();
			m_canUseQuickPick = true;
		}
	}
	private void changeGamePlayState (GamePlayState _State) {
		m_gamePlayState = _State;
	}

	//--------------------------------------------------------------------------
	// public methods
	//--------------------------------------------------------------------------

	public void onPlayBtn (tk2dUIItem thingThatCalledMe) {
		if ((m_gamePlayState == GamePlayState.READYTOPLAY
			|| m_gamePlayState == GamePlayState.AFTER_RESULTS)
			&& m_playerPickedNumbers.Count >= 2
			&& thingThatCalledMe.gameObject.GetComponent<ThreeStateButton> ().isEnable) {
			//		string temp = m_testJson.serverInit ();
			if (m_gamePlayState == GamePlayState.AFTER_RESULTS) {
				setAllPlayerPickedNumbersToSelected ();
			}
			m_winTxt.text = "00";
//			m_mainGameWinAmount.text = m_messages [4];
//			setTempMessage (4);	// Need to keep it till the balls are comming so make it static message.
			setStaticMsg (3);
			changeGamePlayState (GamePlayState.PLAYING);
			m_threeStateButtons [7].gameObject.GetComponent<ThreeStateButton> ().stopBlink ();
			m_paysHolderHighlight.gameObject.GetComponent<BlinkObject> ().stopBlink ();
			m_paysHolderHighlight.gameObject.SetActive (false);
			disableAllButtons ();
			if (m_bingoBalls.Count > 0) {
				StartCoroutine (dropBallsAndPlay ());
			} else {
				createPlayRequest ();
			}
		}
	}

	public void onBetUpBtn (tk2dUIItem thingThatCalledMe) {
		if ((m_gamePlayState == GamePlayState.READYTOPLAY
		    || m_gamePlayState == GamePlayState.AFTER_RESULTS)
		    && thingThatCalledMe.gameObject.GetComponent<ThreeStateButton> ().isEnable) {
			
			m_currentBetIndex += 1;
//		Debug.Log ("m_currentBetIndex_1= " + m_currentBetIndex);
			if (m_currentBetIndex >= m_betRange.Length) {
				m_currentBetIndex = m_betRange.Length - 1;
			} else if (m_currentBetIndex == (m_betRange.Length - 1)) {
				m_threeStateButtons [4].GetComponent<ThreeStateButton> ().disableButtons ();
			} else if (m_currentBetIndex == 1) {
				m_threeStateButtons [5].GetComponent<ThreeStateButton> ().enableButtons ();
			}
			m_betTxt.text = m_betRange [m_currentBetIndex].ToString ();
			m_bankTxt.text = "" + (m_bank - m_betRange [m_currentBetIndex]);
		}
	}

	public void onBetDownBtn (tk2dUIItem thingThatCalledMe) {
		if ((m_gamePlayState == GamePlayState.READYTOPLAY
		    || m_gamePlayState == GamePlayState.AFTER_RESULTS)
		    && thingThatCalledMe.gameObject.GetComponent<ThreeStateButton> ().isEnable) {
		
			m_currentBetIndex -= 1;
//		Debug.Log ("m_currentBetIndex_2= " + m_currentBetIndex);
			if (m_currentBetIndex < 0) {
				m_currentBetIndex = 0;
			} else if (m_currentBetIndex == 0) {
				m_threeStateButtons [5].GetComponent<ThreeStateButton> ().disableButtons ();
			} else if (m_currentBetIndex == (m_betRange.Length - 2)) {
				m_threeStateButtons [4].GetComponent<ThreeStateButton> ().enableButtons ();
			}
			m_betTxt.text = m_betRange [m_currentBetIndex].ToString ();
			m_bankTxt.text = "" + (m_bank - m_betRange [m_currentBetIndex]);
		}
	}

	public void onMaxBetBtn (tk2dUIItem thingThatCalledMe) {
		if ((m_gamePlayState == GamePlayState.READYTOPLAY
		    || m_gamePlayState == GamePlayState.AFTER_RESULTS)
		    && thingThatCalledMe.gameObject.GetComponent<ThreeStateButton> ().isEnable) {
		
			m_currentBetIndex = m_betRange.Length - 1;
			m_betTxt.text = m_betRange [m_currentBetIndex].ToString ();

			m_threeStateButtons [4].GetComponent<ThreeStateButton> ().disableButtons ();
			m_threeStateButtons [5].GetComponent<ThreeStateButton> ().enableButtons ();
			m_bankTxt.text = "" + (m_bank - m_betRange [m_currentBetIndex]);
		}
	}

	public void onWipwCardBtn (tk2dUIItem thingThatCalledMe) {
		if ((m_gamePlayState == GamePlayState.READYTOPLAY
		    || m_gamePlayState == GamePlayState.AFTER_RESULTS)
		    && thingThatCalledMe.gameObject.GetComponent<ThreeStateButton> ().isEnable) {
			if (m_bingoBalls.Count > 0) {
				StartCoroutine (dropBallsAndWipeCards ());
			} else {
				wipeCards ();
			}
		}
	}

	public void onQuickPickBtn (tk2dUIItem thingThatCalledMe) {
		if ((m_gamePlayState == GamePlayState.READYTOPLAY
		    || m_gamePlayState == GamePlayState.AFTER_RESULTS)
		    && thingThatCalledMe.gameObject.GetComponent<ThreeStateButton> ().isEnable) {
			if (m_bingoBalls.Count > 0) {
				StartCoroutine (dropBallsAndQuickPickRandom ());
			} else {
				quickPickRandom ();
			}
		}
	}
	public void onHelpBtn (tk2dUIItem thingThatCalledMe) {

	}

	public void onExitBtn (tk2dUIItem thingThatCalledMe) {
		if (thingThatCalledMe.gameObject.GetComponent<ThreeStateButton> ().isEnable) {
			m_exitPopUp.SetActive (true);
			disableAllButtons ();
		}
	}

	public void onExitYesBtn () {
		exitToMenu ();
	}

	public void onExitCancel () {
		m_exitPopUp.SetActive (false);
		enaableAllButtons ();
	}

	void exitToMenu () {
		//		Application.Quit ();
		PlayerPrefs.SetInt("isFromGameScene", 1);
		PlayerPrefs.Save ();
		Debug.Log ("onExitBtn");
		MainController.SwitchScene ("Menu Scene 2");
	}

	void bringRetryPopUp () {
		m_retryPopUp.SetActive (true);
	}

	public void onRetryBtn () {
		Debug.Log ("onRetryBtn");
		startGame ();
	}
	public void onNumberBtn (tk2dUIItem thingThatCalledMe) {

		if (m_gamePlayState != GamePlayState.READYTOPLAY && m_gamePlayState != GamePlayState.AFTER_RESULTS)
			return;
		
		Debug.Log ("thingThatCalledMe.name= " + thingThatCalledMe.name);
//		int.TryParse (thingThatCalledMe.name, out m_currentNumberPressed);
		int result;

		if(int.TryParse (thingThatCalledMe.name, out result)) {
			m_currentNumberPressed = result;
		}

//		if (m_playerPickedNumbers.Count == 0) {
////			wipeCards ();
//			setAllNumberedButtonsToNormal();
//		}

//		thingThatCalledMe.gameObject.GetComponent<NumberedButton> ().selected ();
		processNumberButotnPress(thingThatCalledMe.gameObject.GetComponent<NumberedButton> ());

		if (!m_canUseQuickPick && m_playerPickedNumbers.Count == 0) {
			m_canUseQuickPick = true;
			m_threeStateButtons [3].enableButtons ();
		} else if (m_canUseQuickPick && m_playerPickedNumbers.Count > 0) {
			m_canUseQuickPick = false;
			m_threeStateButtons [3].disableButtons ();
		}

	}

	void setAllNumberedButtonsToNormal () {
		foreach (NumberedButton n in m_numberedButtons) {
			n.normal ();
//			decreasePicks ();
		}
	}

	void wipeCards () {
//		if (m_gamePlayState != GamePlayState.READYTOPLAY)
//			return;
		
		foreach (NumberedButton n in m_numberedButtons) {
			n.normal ();
			decreasePicks ();
		}

//		removeExistingBalls ();

//		m_playerPickedNumbers.RemoveRange (0, m_playerPickedNumbers.Count - 1);
		m_playerPickedNumbers.Clear ();
		m_paysHolderHighlight.gameObject.SetActive (false);
//		Debug.Log ("m_playerPickedNumbers.Count= "+m_playerPickedNumbers.Count);

		m_canUseQuickPick = true;
		m_threeStateButtons [3].enableButtons ();

//		m_mainGameWinAmount.text = m_messages [0];
		setStaticMsg (2);
		m_betTxt.text = m_betRange [0].ToString ();
		m_currentBetIndex = 0;
		m_winTxt.text = "00";
	}

	void quickPickRandom () {
		if (!m_canUseQuickPick) {
			return;
		}
		wipeCards ();
		List<int> result = new List<int>();
		HashSet<int> check = new HashSet<int>();
		for (Int32 i = 0; i < 10; i++) {
			int curValue = Random.Range (1, 80);	// .Next(1, 100000);
			while (check.Contains(curValue)) {
				curValue = Random.Range (1, 80);		//rand.Next(1, 100000);
			}
//			curValue = curValue - 1;
//			if (curValue < 0) {
//				curValue = 0;
//			}
			result.Add(curValue);
			check.Add(curValue);
//			Debug.Log ("curValue= " + curValue);
			int btnID = curValue - 1;
			if (btnID < 0) {
				btnID = 0;
			}

			m_numberedButtons [btnID].selected ();
			m_playerPickedNumbers.Add (curValue);
			increasePicks ();
		}

//		m_canUseQuickPick = false;
//		m_threeStateButtons [3].disableButtons ();

	}

	void processNumberButotnPress (NumberedButton _Button) {
		switch (_Button.m_buttonState) {
		case NumberedButton.ButtonState.SELECTED:
			if (m_gamePlayState == GamePlayState.AFTER_RESULTS) {
				setAllPlayerPickedNumbersToSelected ();
				changeGamePlayState (GamePlayState.READYTOPLAY);
			}
			m_playerPickedNumbers.Remove (int.Parse (_Button.gameObject.name));
			_Button.normal ();
			decreasePicks ();
			break;
		case NumberedButton.ButtonState.WIN:
		case NumberedButton.ButtonState.LOSE:
			if (m_gamePlayState == GamePlayState.AFTER_RESULTS) {
				setAllPlayerPickedNumbersToSelected ();
				changeGamePlayState (GamePlayState.READYTOPLAY);
			} else {
				if (m_playerPickedNumbers.Count < m_maxPicks) {
					m_playerPickedNumbers.Add (int.Parse (_Button.gameObject.name));
					_Button.selected ();
					increasePicks ();
				} else if (m_playerPickedNumbers.Count == m_maxPicks) {
//					m_mainGameWinAmount.text = "You can select up to\n10 numbers";//m_messages [1];
					setTempMessage (0);

				}
			}
			break;
		case NumberedButton.ButtonState.NONE:
		case NumberedButton.ButtonState.NORMAL:
//			Debug.Log ("m_playerPickedNumbers.Count= " + m_playerPickedNumbers.Count);
			if (m_gamePlayState == GamePlayState.AFTER_RESULTS) {
				setAllPlayerPickedNumbersToSelected ();
				changeGamePlayState (GamePlayState.READYTOPLAY);
			}
			if (m_playerPickedNumbers.Count < m_maxPicks) {
				if (m_playerPickedNumbers.Count == (m_maxPicks - 1)) {
//					m_mainGameWinAmount.text = m_messages [2];
					setStaticMsg (0);
				}
				m_playerPickedNumbers.Add (int.Parse (_Button.gameObject.name));
				_Button.selected ();
				increasePicks ();
			} else if (m_playerPickedNumbers.Count == m_maxPicks) {
//				m_mainGameWinAmount.text = "You can select up to\n10 numbers";//m_messages [1];
				setTempMessage (0);
				setAllPlayerPickedNumbersToSelected ();
			}
			break;
		}
	}

	void setAllPlayerPickedNumbersToSelected () {
		if (m_bingoBalls.Count > 0) {
			StartCoroutine (dropBallsAndSetPlayerPickedToSelected ());
		} else {
			foreach (NumberedButton n in m_numberedButtons) {
				n.normal ();
			}
			for (int i = 0; i < m_playerPickedNumbers.Count; i++) {
				int btnID = m_playerPickedNumbers [i] - 1;
				if (btnID < 0) {
					btnID = 0;
				}
				m_numberedButtons [btnID].selected ();
			}
//			removeExistingBalls ();
			m_paysHolderHighlight.gameObject.SetActive (false);
		}
	}

	void increasePicks () {
		m_currentPicks += 1;
		if (m_currentPicks > m_maxPicks) {
			m_currentPicks = m_maxPicks;
//			m_mainGameWinAmount.text = "You can select up to\n10 numbers";//m_messages [1];
			setTempMessage (0);
		} else if(m_currentPicks == m_maxPicks) {
//			m_mainGameWinAmount.text = m_messages [2];
			setStaticMsg (0);
			m_threeStateButtons [7].gameObject.GetComponent<ThreeStateButton> ().startBlink ();
			disableUpDownBttonsForNumberedButtons ();
		}
//		m_picksTxt.text = m_currentPicks.ToString ();
		setUITexts ();
		setPayTableValues ();
	}

	void disableUpDownBttonsForNumberedButtons () {
		for (int i = 0; i < 80; i++) {
//			Debug.Log ("i= " + i);
			m_numberedButtons [i].gameObject.GetComponent<tk2dUIUpDownButton> ().isActive = false;
		}
	}

	void enableUpDownBttonsForNumberedButtons () {
		for (int i = 0; i < 80; i++) {
			m_numberedButtons [i].gameObject.GetComponent<tk2dUIUpDownButton> ().isActive = true;
		}
	}

	void decreasePicks () {
		m_currentPicks -= 1;
		if (m_currentPicks < 0) {
			m_currentPicks = 0;
		}
		if (m_currentPicks < m_maxPicks) {
			m_threeStateButtons [7].gameObject.GetComponent<ThreeStateButton> ().stopBlink ();
			enableUpDownBttonsForNumberedButtons ();
		}
//		m_picksTxt.text = m_currentPicks.ToString ();
		setUITexts ();
		setPayTableValues ();
	}

	void setPayTableValues () {
		for (int i = 0; i < m_payTable.Count; i++) {
			if (m_currentPicks == m_payTable [i].m_picks) {
//				for (int j = 0; j < m_payTable [i].m_hits.Length; j++) {
//					m_hitsTxt [m_hitsTxt.Length - j].text = m_payTable [i].m_multipliers [j].ToString ();
//					float value = 0;
//					if (m_payTable [i].m_multipliers.TryGetValue (m_hitsTxt [m_hitsTxt.Length - j].text, value)) {
//						m_paysTxt [m_paysTxt.Length - j].text = value.ToString ();
//					}
//					m_paysTxt [m_paysTxt.Length - j].text = value.ToString ();

//					m_hitsTxt [m_hitsTxt.Length - j - 1].text = m_payTable [i].m_hits [j];
//					m_paysTxt [m_paysTxt.Length - j - 1].text = m_payTable [i].m_pays [j].ToString ();
//				}
				for (int j = 0; j < m_hitsTxt.Length; j++) {
					if (j < m_payTable [i].m_hits.Length) {
						m_hitsTxt [j].text = m_payTable [i].m_hits [m_payTable [i].m_hits.Length - j - 1];
						m_paysTxt [j].text = m_payTable [i].m_pays [m_payTable [i].m_hits.Length - j - 1].ToString ();
					} else {
						m_hitsTxt [j].text = "";
						m_paysTxt [j].text = "";
					}
				}
				break;
			} else {
				for (int j = 0; j < m_hitsTxt.Length; j++) {
					m_hitsTxt [j].text = "";
					m_paysTxt [j].text = "";
				}
			}
		}
	}

	void clearPayTableText () {
		for (int j = 0; j < m_hitsTxt.Length; j++) {
			m_hitsTxt [j].text = "";
			m_paysTxt [j].text = "";
		}
	}

	void createPlayRequest () {

		if (m_playerPickedNumbers.Count < 2) {
			return;
		}

//		removeExistingBalls ();

		User user = new User ();
		PlayRequest playRequest = new PlayRequest ();
		user.deviceCode = 1;
		playRequest.action = "play";
		playRequest.gameID = 1;
		playRequest.user = user;
		playRequest.playerPicks = m_playerPickedNumbers;
		playRequest.bet = m_betRange [m_currentBetIndex];

		JSONNode N = JSON.Parse (m_playRequestStr);

		N ["action"] = playRequest.action;
		N ["gameID"].AsInt = playRequest.gameID;
		N ["user"] ["deviceCode"].AsInt = 1;
		//		N ["playerPicks"] = listToJSONString (m_playerPickedNumbers);

//		for (int i = 0; i < m_playerPickedNumbers.Count; i++) {
//			N ["playerPicks"] [i].AsInt = m_playerPickedNumbers [i];
//		}

		for (int k = 0; k < N ["playerPicks"].Count; k++) {
			N ["playerPicks"] = N ["playerPicks"].Remove (k);
		}

		JSONNode a = new JSONArray ();

		for (int i = 0; i < m_playerPickedNumbers.Count; i++) {
//			N ["playerPicks"] [i].AsInt = m_playerPickedNumbers [i];

			a [i].AsInt = m_playerPickedNumbers [i];
//			Debug.Log (a.ToJSON (0));
		}
//		N ["playerPicks"].Add (a.AsArray);
//		Debug.Log ("1 N [playerPicks].Value= "+N ["playerPicks"].Value);

		N ["playerPicks"].Value = listToJSONString (m_playerPickedNumbers);
//		Debug.Log ("2 N [playerPicks].Value= "+N ["playerPicks"].Value);

//		for (int j = 0; j < 10; j++) {
//			if (j < m_playerPickedNumbers.Count) {
//				N ["playerPicks"] [j].AsInt = m_playerPickedNumbers [j];
//			} else {
//				N ["playerPicks"] = N ["playerPicks"].Remove(j);
//			}
//		}

//		if (m_playerPickedNumbers.Count >= N ["playerPicks"].Count) {
//			for (int i = 0; i < m_playerPickedNumbers.Count; i++) {
////				N ["playerPicks"] [i].AsInt = m_playerPickedNumbers [i];
//				N ["playerPicks"] [i].AsArray = m_playerPickedNumbers [i];
//			}
//		} else if (m_playerPickedNumbers.Count < N ["playerPicks"].Count) {
//			for (int i = 0; i < N ["playerPicks"].Count; i++) {
//				if (i < m_playerPickedNumbers.Count) {
//					N ["playerPicks"] [i].AsInt = m_playerPickedNumbers [i];
//				} else {
//					N ["playerPicks"].Remove (i);
//				}
//			}
//		}

//		for (int i = 0; i < m_playerPickedNumbers.Count; i++) {
////				N ["playerPicks"] [i].AsInt = m_playerPickedNumbers [i];
////				N ["playerPicks"] [i].AsArray = m_playerPickedNumbers [i];
//			N ["playerPicks"].Add(a[i]);
//			Debug.Log (N ["playerPicks"] [i].AsInt);
//		}
		
		N ["bet"].AsDouble = playRequest.bet;

		m_playRequestStr = N.ToJSON (0);
//		Debug.Log ("m_playRequestStr= " + m_playRequestStr);
		int indx = m_playRequestStr.IndexOf ("\"[");
//		Debug.Log ("m_playRequestStr_2= " + indx);
		m_playRequestStr = m_playRequestStr.Remove (indx, 1);
//		Debug.Log ("m_playRequestStr_3= " + m_playRequestStr);
		indx = m_playRequestStr.IndexOf ("]\"");
		m_playRequestStr = m_playRequestStr.Remove (indx+1, 1);
		Debug.Log ("m_playRequestStr_4= " + m_playRequestStr);

		string response = m_testJson.serverPlay (m_playRequestStr);
		Debug.Log ("response= " + response);
		if (response.Equals ("-1")) {
			bringRetryPopUp ();
		} else {
			m_serverResponsePlay = response;
			//		setPlayResponseResults (response);
			getPickedAndMatchedNumbers (response);
			StartCoroutine (startBingoBalls ());
		}
	}

	void removeExistingBalls () {
		StartCoroutine(dropBalls ());
//		for (int i = 0; i < m_bingoBalls.Count; i++) {
//			Destroy (m_bingoBalls [i]);
//		}
//		m_bingoBalls.Clear ();
	}

	void getPickedAndMatchedNumbers (string _PlayResponse) {
		m_matchedNumbers.Clear ();
		m_serverPickedNumbers.Clear ();

		JSONNode N = JSON.Parse (_PlayResponse);

		for (int i = 0; i < N ["pickedNumbers"].Count; i++) {
//			_PickedNumbers.Add (N ["pickedNumbers"] [i].AsInt);
			m_serverPickedNumbers.Add (N ["pickedNumbers"] [i].AsInt);
			//			m_numberedButtons [N ["pickedNumbers"] [i].AsInt].GetComponent<NumberedButton> ().setSelectedSprite ();
		}

		for (int j = 0; j < N ["matchedNumbers"].Count; j++) {
			Debug.Log ("N [matchedNumbers] [j].AsInt= " + N ["matchedNumbers"] [j].AsInt);
//			_MatchedNumbers.Add (N ["matchedNumbers"] [j].AsInt);
			m_matchedNumbers.Add (N ["matchedNumbers"] [j].AsInt);
			//			m_numberedButtons [N ["matchedNumbers"] [j].AsInt].GetComponent<NumberedButton> ().setWinSprite ();
		}
	}

	void setPlayResponseResults (string _PlayResponse) {
//		m_matchedNumbers.Clear ();
//		m_serverPickedNumbers.Clear ();

		JSONNode N = JSON.Parse (_PlayResponse);

		m_bank = N ["bank"].AsFloat;
		m_bankTxt.text = m_bank.ToString ();
		if ((N ["win"].AsFloat <= 0)) {
			m_winTxt.text = N ["win"].Value.ToString ();
//			m_mainGameWinAmount.text = "Better Luck Next Time\nPlay Again";			//m_messages [3];
//			m_mainGameWinAmount.text = m_staticMessages[1];
			setStaticMsg (1);
		} else {
			m_winTxt.text = N ["win"].Value.ToString ();
			m_mainGameWinAmount.text = "WON $" + m_winTxt.text;
			setTempMessage (1);
		}
		List<int> _PickedNumbers = new List<int> ();
		List<int> _MatchedNumbers = new List<int> ();
		for (int i = 0; i < N ["pickedNumbers"].Count; i++) {
			_PickedNumbers.Add (N ["pickedNumbers"] [i].AsInt);
//			m_serverPickedNumbers.Add (N ["pickedNumbers"] [i].AsInt);
//			m_numberedButtons [N ["pickedNumbers"] [i].AsInt].GetComponent<NumberedButton> ().setSelectedSprite ();
		}

		for (int j = 0; j < N ["matchedNumbers"].Count; j++) {
			Debug.Log ("N [matchedNumbers] [j].AsInt= " + N ["matchedNumbers"] [j].AsInt);
			_MatchedNumbers.Add (N ["matchedNumbers"] [j].AsInt);
//			m_matchedNumbers.Add (N ["matchedNumbers"] [j].AsInt);
//			m_numberedButtons [N ["matchedNumbers"] [j].AsInt].GetComponent<NumberedButton> ().setWinSprite ();
		}

//		for (int k = 1; k <= m_numberedButtons.Length; k++) {
//			if (_MatchedNumbers.Contains (k)) {
//				m_numberedButtons [k - 1].GetComponent<NumberedButton> ().win ();
//			} else if (m_playerPickedNumbers.Contains (k)) {
//				m_numberedButtons [k - 1].GetComponent<NumberedButton> ().lose ();
//			} else {
//				m_numberedButtons [k - 1].GetComponent<NumberedButton> ().normal ();
//			}
//		}
//		m_playerPickedNumbers.Clear ();

		for (int l = 0; l < m_hitsTxt.Length; l++) {
			if (m_hitsTxt [l].text == m_matchedNumbers.Count.ToString ()) {
				m_paysHolderHighlight.gameObject.SetActive (true);
				m_paysHolderHighlight.transform.position = new Vector3 (m_paysHolderHighlight.transform.position.x,
					m_hitsTxt [l].gameObject.transform.position.y,
					m_paysHolderHighlight.transform.position.z);
				m_paysHolderHighlight.gameObject.GetComponent<BlinkObject> ().startBlink ();
				break;
			} else if (l == (m_hitsTxt.Length - 1)) {
				m_paysHolderHighlight.gameObject.SetActive (false);
			}
		}
	}

	string listToJSONString (List<int> _List) {
		string json = "";
		if (_List != null && _List.Count > 0) {
			json = json + "[";
			for (int i = 0; i < _List.Count; i++) {
				json = json + _List [i];
				if (i < (_List.Count - 1)) {
					json = json + ",";
				} else {
					json = json + "]";
				}
			}
		}
//		Debug.Log ("json= " + json);
		return json;
	}

	void setPickedNumbersStatusFor (int _number) {
//		Debug.Log ("m_serverPickedNumbers ["+_number+"]= "+m_serverPickedNumbers [_number]);
		int Number = m_serverPickedNumbers [_number];
		if (m_matchedNumbers.Contains (Number)) {
			m_numberedButtons [Number-1].GetComponent<NumberedButton> ().win ();
		} else {
			m_numberedButtons [Number-1].GetComponent<NumberedButton> ().lose ();
		}
	}

	IEnumerator showTemproryMessage () {
		yield return new WaitForSeconds (0.1f);
//		m_mainGameWinAmount.text = 
		yield return new WaitForSeconds (5f);
	}

	int m_ballsGenerationCount = 0;
	GameObject m_lastBall;
	IEnumerator startBingoBalls () {
		m_ballsGenerationCount++;
		if (m_ballsGenerationCount >= 21) {
			m_ballsGenerationCount = 0;
//			changeGamePlayState (GamePlayState.READYTOPLAY);
			changeGamePlayState (GamePlayState.AFTER_RESULTS);
			enaableAllButtons ();
			setPlayResponseResults (m_serverResponsePlay);
			yield return new WaitForSeconds (0.5f);
		} else {
			float yForce = 95f;	// * Random.Range (1f, 3f);
			GameObject temp;
			if (m_ballsGenerationCount % 2 == 0) {
				temp = (GameObject)Instantiate (m_bingoBallPrefabEven, m_ballStartPos.position, Quaternion.identity);
			} else {
				temp = (GameObject)Instantiate (m_bingoBallPrefabOdd, m_ballStartPos.position, Quaternion.identity);
			}
			temp.transform.parent = this.transform;
			if (m_ballsGenerationCount < 20) {
				temp.gameObject.GetComponent<Rigidbody> ().AddForce (800f, yForce, 0);		// (Vector3.right * 300f);
			}
			if (m_ballsGenerationCount < 21) {
				temp.gameObject.GetComponent<ParticleSystem> ().Stop ();
			}
			Renderer ballRenderer = temp.GetComponentInChildren<MeshRenderer> (false);

			ballRenderer.material.mainTexture = Resources.Load ("BallTextures/bingoBall" + m_serverPickedNumbers [m_ballsGenerationCount - 1]) as Texture;

			m_lastBall = temp;

			m_bingoBalls.Add (temp);
			setPickedNumbersStatusFor (m_ballsGenerationCount-1);
			yield return new WaitForSeconds (0.5f);

			if (m_ballsGenerationCount == 20) {
				startLastBall ();
			} else {
				StartCoroutine (startBingoBalls ());
			}
		}
		yield return new WaitForSeconds (0.1f);

	}

	IEnumerator pauseLastBallAtLastNumber () {
		
		yield return new WaitForSeconds (0.1f);

		toLastBallFinalPos = true;
	}

	IEnumerator dropBalls () {
		Vector3 rotationPoint = new Vector3 (1.801f, -4.965f, -0.1f);
		m_tubeBaseCollider.SetActive (false);
//		m_ballStopper.transform.RotateAround (rotationPoint, Vector3.forward, 270f);
		m_ballStopper.gameObject.SetActive (false);
		for (int i = 0; i < m_bingoBalls.Count; i++) {
//			m_bingoBalls [i].GetComponent<Rigidbody> ().mass = 1000f;
			m_bingoBalls [i].GetComponent<Rigidbody> ().AddForce (0, -500f, 0);
		}

		yield return new WaitForSeconds (1f);
		m_tubeBaseCollider.SetActive (true);
//		m_ballStopper.transform.RotateAround (rotationPoint, Vector3.forward, 270);
		m_ballStopper.gameObject.SetActive (true);

		for (int i = 0; i < m_bingoBalls.Count; i++) {
			Destroy (m_bingoBalls [i]);
		}
		m_bingoBalls.Clear ();
		yield return new WaitForSeconds (0.1f);
	}

	IEnumerator dropBallsAndPlay () {
		Vector3 rotationPoint = new Vector3 (1.801f, -4.965f, -0.1f);
		m_tubeBaseCollider.SetActive (false);
		//		m_ballStopper.transform.RotateAround (rotationPoint, Vector3.forward, 270f);
		m_ballStopper.gameObject.SetActive (false);
		for (int i = 0; i < m_bingoBalls.Count; i++) {
			//			m_bingoBalls [i].GetComponent<Rigidbody> ().mass = 1000f;
			m_bingoBalls [i].GetComponent<Rigidbody> ().AddForce (0, -500f, 0);
		}

		yield return new WaitForSeconds (1f);
		m_tubeBaseCollider.SetActive (true);
		//		m_ballStopper.transform.RotateAround (rotationPoint, Vector3.forward, 270);
		m_ballStopper.gameObject.SetActive (true);

		for (int i = 0; i < m_bingoBalls.Count; i++) {
			Destroy (m_bingoBalls [i]);
		}
		m_bingoBalls.Clear ();
		yield return new WaitForSeconds (0.1f);
		createPlayRequest ();
	}

	IEnumerator dropBallsAndWipeCards () {
		Vector3 rotationPoint = new Vector3 (1.801f, -4.965f, -0.1f);
		m_tubeBaseCollider.SetActive (false);
		//		m_ballStopper.transform.RotateAround (rotationPoint, Vector3.forward, 270f);
		m_ballStopper.gameObject.SetActive (false);
		for (int i = 0; i < m_bingoBalls.Count; i++) {
			//			m_bingoBalls [i].GetComponent<Rigidbody> ().mass = 1000f;
			m_bingoBalls [i].GetComponent<Rigidbody> ().AddForce (0, -500f, 0);
		}

		yield return new WaitForSeconds (1f);
		m_tubeBaseCollider.SetActive (true);
		//		m_ballStopper.transform.RotateAround (rotationPoint, Vector3.forward, 270);
		m_ballStopper.gameObject.SetActive (true);

		for (int i = 0; i < m_bingoBalls.Count; i++) {
			Destroy (m_bingoBalls [i]);
		}
		m_bingoBalls.Clear ();
		yield return new WaitForSeconds (0.1f);
		wipeCards ();
	}

	IEnumerator dropBallsAndSetPlayerPickedToSelected () {
		Vector3 rotationPoint = new Vector3 (1.801f, -4.965f, -0.1f);
		m_tubeBaseCollider.SetActive (false);
		//		m_ballStopper.transform.RotateAround (rotationPoint, Vector3.forward, 270f);
		m_ballStopper.gameObject.SetActive (false);
		for (int i = 0; i < m_bingoBalls.Count; i++) {
			//			m_bingoBalls [i].GetComponent<Rigidbody> ().mass = 1000f;
			m_bingoBalls [i].GetComponent<Rigidbody> ().AddForce (0, -500f, 0);
		}

		yield return new WaitForSeconds (1f);
		m_tubeBaseCollider.SetActive (true);
		//		m_ballStopper.transform.RotateAround (rotationPoint, Vector3.forward, 270);
		m_ballStopper.gameObject.SetActive (true);

		for (int i = 0; i < m_bingoBalls.Count; i++) {
			Destroy (m_bingoBalls [i]);
		}
		m_bingoBalls.Clear ();
		yield return new WaitForSeconds (0.1f);
		setAllPlayerPickedNumbersToSelected ();
	}

	IEnumerator dropBallsAndQuickPickRandom () {
		Vector3 rotationPoint = new Vector3 (1.801f, -4.965f, -0.1f);
		m_tubeBaseCollider.SetActive (false);
		//		m_ballStopper.transform.RotateAround (rotationPoint, Vector3.forward, 270f);
		m_ballStopper.gameObject.SetActive (false);
		for (int i = 0; i < m_bingoBalls.Count; i++) {
			//			m_bingoBalls [i].GetComponent<Rigidbody> ().mass = 1000f;
			m_bingoBalls [i].GetComponent<Rigidbody> ().AddForce (0, -500f, 0);
		}

		yield return new WaitForSeconds (1f);
		m_tubeBaseCollider.SetActive (true);
		//		m_ballStopper.transform.RotateAround (rotationPoint, Vector3.forward, 270);
		m_ballStopper.gameObject.SetActive (true);

		for (int i = 0; i < m_bingoBalls.Count; i++) {
			Destroy (m_bingoBalls [i]);
		}
		m_bingoBalls.Clear ();
		yield return new WaitForSeconds (0.1f);
		quickPickRandom ();
	}
}

//Better Luck Next Time
//Play Again

//result= {"user":{"bank":9976.25},"gameConfig":{"minPicks":2,"maxPicks":10,"paytable":[{"picks":2,"multipliers":[{"1":0.25},{"2":1.5}]},{"picks":3,"multipliers":[{"2":0.5},{"3":6.75}]},{"picks":4,"multipliers":[{"2":0.25},{"3":1.5},{"4":12.5}]},{"picks":5,"multipliers":[{"2":0.25},{"3":0.5},{"4":2.5},{"5":25}]},{"picks":6,"multipliers":[{"3":0.5},{"4":1.75},{"5":7.75},{"6":37.5}]},{"picks":7,"multipliers":[{"3":0.25},{"4":1},{"5":3.5},{"6":15},{"7":50}]},{"picks":8,"multipliers":[{"3":0.25},{"4":0.5},{"5":1.5},{"6":5.25},{"7":25},{"8":125}]},{"picks":9,"multipliers":[{"4":0.5},{"5":1},{"6":4},{"7":22.5},{"8":50},{"9":250}]},{"picks":10,"multipliers":[{"4":0.25},{"5":1},{"6":2},{"7":6.25},{"8":31.25},{"9":125},{"10":250}]}]}}

//{ 
//	"action": "play", 
//	"gameID": 1, 
//	"user": { 
//		"deviceCode": 1
//	}, 
//	"playerPicks": [ 
//		33, 
//		5, 
//		22, 
//		9, 
//		49
//	], 
//	"bet": 2
//}