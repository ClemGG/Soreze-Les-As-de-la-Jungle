using UnityEngine;
using System.Collections;
// We need this one for importing our IOS functions
using System.Runtime.InteropServices;

public class iosAirprintPlugin
{
	// Use this #if so that if you run this code on a different platform, you won't get errors.
	#if UNITY_IOS
	[DllImport ("__Internal")]
	private static extern int _pow2(int x);
	
	// For the most part, your imports match the function defined in the iOS code, except char* is replaced with string here so you get a C# string.    
	[DllImport ("__Internal")]
	private static extern string _helloWorldString();
	
	[DllImport ("__Internal")]
	private static extern string  _combineStrings(string cString1, string cString2);

	[DllImport ("__Internal")]
	private static extern void _print();
	
	#endif
	
	// Now make methods that you can provide the iOS functionality
	
	static public int Pow2(int x)
	{
		int pow = 0;
		// We check for UNITY_IOS again so we don't try this if it isn't iOS platform.
		#if UNITY_IOS
		// Now we check that it's actually an iOS device/simulator, not the Unity Player. You only get plugins on the actual device or iOS Simulator.
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			pow = _pow2(x);
		}
		#endif
		// TODO:  You could test for Android, PC, Mac, Web, etc and do something with a plugin for them here.
		
		return pow;
	}
	
	static public string HelloWorldString()
	{
		string helloWorld = "";
		// We check for UNITY_IOS again so we don't try this if it isn't iOS platform.
		#if UNITY_IOS
		// Now we check that it's actually an iOS device/simulator, not the Unity Player. You only get plugins on the actual device or iOS Simulator.
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			helloWorld = _helloWorldString();
		}
		#endif
		// TODO:  You could test for Android, PC, Mac, Web, etc and do something with a plugin for them here.
		
		return helloWorld;
	}
	
	static public string CombineStrings(string string1, string string2)
	{
		string combinedString = "";
		// We check for UNITY_IOS again so we don't try this if it isn't iOS platform.
		#if UNITY_IOS
		// Now we check that it's actually an iOS device/simulator, not the Unity Player. You only get plugins on the actual device or iOS Simulator.
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			combinedString = _combineStrings(string1, string2);
		}
		#endif
		// TODO:  You could test for Android, PC, Mac, Web, etc and do something with a plugin for them here.
		
		return combinedString;
	}

	static public void PrintOut()
	{
		#if UNITY_IOS
		// Now we check that it's actually an iOS device/simulator, not the Unity Player. You only get plugins on the actual device or iOS Simulator.
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_print();
		}
		#endif
	}
}