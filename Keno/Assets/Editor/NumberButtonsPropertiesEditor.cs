using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(NumberButtonsProperties))]
public class NumberButtonsPropertiesEditor : Editor {

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		NumberButtonsProperties myProp = (NumberButtonsProperties)target;
		if (GUILayout.Button ("Change Down Image")){
			myProp.getNumberedButton ();
		}
	}
}