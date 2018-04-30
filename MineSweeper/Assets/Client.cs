using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using AssemblyCSharp;

public class Client : MonoBehaviour {
	PlayerInfo playerInfo;
	bool connected = false;
	// Use this for initialization
	void Start () {
		playerInfo = new AssemblyCSharp.PlayerInfo ();
		connected = playerInfo.startConnection ("192.168.137.151", 8888, 1);
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space) && connected)
			playerInfo.sendFreeSpace (1, 1);
		/*else if (Input.GetKeyDown (KeyCode.M) && connected)
			playerInfo.mineGame ();*/
		playerInfo.sendPosition ();
	}
}
