using UnityEngine;
using System.Collections;

public class TestController: MonoBehaviour {
	bool touched = false;

	// Use this for initialization
	void Start () 
	{
		Debug.Log ("8 x 8 = " + iosAirprintPlugin.Pow2(8));
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.touchCount > 0 && !touched) 
		{
			Debug.Log ("Touch! Then print");
			iosAirprintPlugin.PrintOut ();
			touched = true;
		}
	}
}
