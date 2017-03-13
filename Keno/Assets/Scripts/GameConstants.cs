using UnityEngine;
using System.Collections;

public class GameConstants : MonoBehaviour {

	public static GameConstants sharedInstance;

	public static GameConstants getSharedInstance () {
		if (sharedInstance == null) {
			sharedInstance = new GameConstants ();
		}
		return sharedInstance;
	}

	public tk2dFontData m_dubNumbersNormal60;
	public tk2dFontData m_dubNumbersSelect60;
	public tk2dFontData m_dubNumbersWin60;
	public tk2dFontData m_dubNumbersDead60;
	public tk2dFontData m_ballNumbersNormal60;
	public tk2dFontData m_valuePayRed60;
	public tk2dFontData m_valuePayYellow60;
	public tk2dFontData m_valueRed60;
	public tk2dFontData m_valueYellow60;
	public tk2dFontData m_winningNumbers90;

//	public static tk2dFont m_winningNumbers90_;

	// Use this for initialization
//	void Start () {
//	
//	}
	
	// Update is called once per frame
//	void Update () {
//	
//	}
}
