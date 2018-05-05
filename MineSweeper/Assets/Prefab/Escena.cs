using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using AssemblyCSharp;

public class Escena : MonoBehaviour {
    public void cambiarescena(string nombre) {
        bool data = AssemblyCSharp.PlayerInfo.Instance.startConnection("127.0.0.1", 1001, 1);
        if (data)
            SceneManager.LoadScene(1);
        else
            Debug.Log("Imposible conectar al servidor, pruebe otra vez PE");
	}
}
