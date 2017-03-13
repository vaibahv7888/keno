using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using SimpleJSON;
using System;

public class TestJson : MonoBehaviour {

	public string m_postURL;
	public string m_jsonInit;

	string m_jsonResponse = "-1";

	UTF8Encoding m_encoding = new UTF8Encoding ();
//	Hashtable m_hashtable = new Hashtable ();
	Dictionary<string, string> m_postHeader = new Dictionary<string, string> ();

	static TestJson instance;

	public static TestJson sharedInstance () {
		if (instance == null) {
			instance = new TestJson ();
		}
		return instance;
	}

	// Use this for initialization
	void Start () {
//		m_hashtable.Add("Content-Type", "text/json");
//		m_hashtable.Add("Content-Length", m_jsonInit.Length);
		m_postHeader.Add ("Content-Type", "text/json");
//		m_postHeader.Add ("Content-Length", m_jsonInit.Length.ToString());
//		Debug.Log ("m_jsonInit= "+m_jsonInit);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

//	public void serverInit () {
////		StartCoroutine (checkServerResponce ());
//		callInit();
//	}

	IEnumerator checkServerResponce () {

		WWW www = new WWW (m_postURL, m_encoding.GetBytes (m_jsonInit), m_postHeader);		//, m_hashtable);

		Debug.Log ("www.url= "+www.url);

		yield return www;

		if (!string.IsNullOrEmpty(www.error)) {
			print(www.error);
		}
		else {
			print("Server Responce= "+www.text);
		}
	}

	public string serverInit () {
		var httpWebRequest = (HttpWebRequest)WebRequest.Create(m_postURL);
		httpWebRequest.ContentType = "application/json";		//"text/json";
		httpWebRequest.Method = "POST";

		try {
		using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
		{
			string json = "{\"user\":\"test\"," +
				"\"password\":\"bla\"}";

			streamWriter.Write(m_jsonInit);
//			Debug.Log ("streamWriter.ToString ()= "+streamWriter.ToString ());
			streamWriter.Flush();
			streamWriter.Close();
		}
		} catch (Exception ex) {
			return "-1";
		}

//		Debug.Log ("httpWebRequest.ToString ()= " + httpWebRequest.ToString ());

		try {
		var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
		using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
		{
			var result = streamReader.ReadToEnd();
			m_jsonResponse = result;
			Debug.Log ("result= "+result);
//			parseJsonResponce ();
		}
		} catch (Exception ex) {
			return "-1";
		}

		return m_jsonResponse;
	}

	public string serverPlay (string _PostString) {
		string m_playResponse = "-1";
		var httpWebRequest = (HttpWebRequest)WebRequest.Create(m_postURL);
		httpWebRequest.ContentType = "application/json";		//"text/json";
		httpWebRequest.Method = "POST";

		try {
		using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
		{
//			string json = "{\"user\":\"test\"," +
//				"\"password\":\"bla\"}";

			streamWriter.Write(_PostString);
			streamWriter.Flush();
			streamWriter.Close();
		}
		} catch (Exception ex) {
			return "-1";
		}

		try {
		var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
		using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
		{
			var result = streamReader.ReadToEnd();
			m_playResponse = result;
			Debug.Log ("result= "+result);
			//			parseJsonResponce ();
		}
		} catch (Exception ex) {
			return "-1";
		}
		return m_playResponse;
	}

	void parseJsonResponce () {
		JSONNode N = JSON.Parse (m_jsonResponse);
		string bank = N ["user"] ["bank"].Value;
		Debug.Log ("bank amount= "+bank);
	}
}
