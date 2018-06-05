using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using System;

using AssemblyCSharp;

public class Client : MonoBehaviour {
	bool connected = false;
	InputField IPField = null;
	InputField PuertoField = null;
	Toggle optoggle = null;
	// Use this for initialization
	void Start () {
		IPField = (InputField)GameObject.Find("IP").GetComponent<InputField>();
		IPField.text = "127.0.0.1";
		PuertoField = (InputField)GameObject.Find ("Puerto").GetComponent<InputField> ();
		PuertoField.text = "8000";
		optoggle = (Toggle)GameObject.Find ("Opcion").GetComponent<Toggle> ();
	}
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.S) && !connected){
			int puerto;
			Int32.TryParse (PuertoField.text, out puerto);
			int opcion = 0;
			if (optoggle.isOn)
				opcion = 1;
			connected = AssemblyCSharp.PlayerInfo.Instance.startConnection(IPField.text, puerto, opcion);
		}
        /*else if (Input.GetKeyDown(KeyCode.Q) && !connected)
            connected = AssemblyCSharp.PlayerInfo.Instance.startConnection("192.168.43.177", 8000, 1);*/
        //connected = AssemblyCSharp.PlayerInfo.Instance.startConnection ("192.168.43.177", 8803, 1);
        //connected = AssemblyCSharp.PlayerInfo.Instance.startConnection ("192.168.43.177", 5000, 1);
        else if (Input.GetKeyDown (KeyCode.Space) && connected)
			AssemblyCSharp.PlayerInfo.Instance.sendFreeSpace (1, 1);
		else if (Input.GetKeyDown (KeyCode.W) && connected)
			AssemblyCSharp.PlayerInfo.Instance.sendFreeSpace (0, 0);
		else if (Input.GetKeyDown (KeyCode.L) && connected)
			AssemblyCSharp.PlayerInfo.Instance.sendFreeSpace (2, 2);
		else if ((Input.GetKeyDown (KeyCode.D) || AssemblyCSharp.PlayerInfo.Instance.read_winner) && connected) {
			Debug.LogFormat ("Winner: {0}", AssemblyCSharp.PlayerInfo.Instance.player_winner);
			AssemblyCSharp.PlayerInfo.Instance.endConnection ();
			Debug.Log ("Disconnected");
			connected = false;
		} else if (AssemblyCSharp.PlayerInfo.Instance.alertar_forzado) {
			Debug.Log (AssemblyCSharp.PlayerInfo.Instance.message.ToString ());
			AssemblyCSharp.PlayerInfo.Instance.endConnection ();
			Debug.Log ("Disconnected");
			connected = false;
		} else if (AssemblyCSharp.PlayerInfo.Instance.actualizar_map) {
			Debug.LogFormat ("Liberar X: {0}", AssemblyCSharp.PlayerInfo.Instance.liberar_x);
			Debug.LogFormat ("Liberar Y: {0}", AssemblyCSharp.PlayerInfo.Instance.liberar_y);
			AssemblyCSharp.PlayerInfo.Instance.actualizar_map = false;
		} else if (AssemblyCSharp.PlayerInfo.Instance.alertar_jugador) {
			Debug.Log ("MAL PE");
			AssemblyCSharp.PlayerInfo.Instance.alertar_jugador = false;
		}
		if (connected && AssemblyCSharp.PlayerInfo.Instance.hayJugada ()) {
			Vector3 jugada = AssemblyCSharp.PlayerInfo.Instance.get_jugada ();
			Debug.Log (jugada.ToString ());
			// Liberar esa jugada donde jugada.x es X, jugada.y es Y y jugada.z es el ID del jugador.
		}
		AssemblyCSharp.PlayerInfo.Instance.sendPosition ();
		/*else if (Input.GetKeyDown (KeyCode.M) && connected)
			AssemblyCSharp.PlayerInfo.Instance.mineGame ();*/
	}
}
