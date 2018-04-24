using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using AssemblyCSharp;

public class Escena : MonoBehaviour {
    public void cambiarescena(string nombre) {
        bool data = AssemblyCSharp.PlayerInfo.Instance.startConnection("192.168.137.151", 8888, 1);
        if (data)
            SceneManager.LoadScene(1);
        else
            Debug.Log("Imposible conectar al servidor, pruebe otra vez PE");
	}
}
