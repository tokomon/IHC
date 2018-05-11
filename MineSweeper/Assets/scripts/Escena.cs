using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using AssemblyCSharp;
using UnityEngine.UI;
using System;

public class Escena : MonoBehaviour {
    public void cambiarescena(string nombre) {
        InputField IPField = (InputField)GameObject.Find("IP").GetComponent<InputField>();
        InputField PuertoField = (InputField)GameObject.Find("Puerto").GetComponent<InputField>();
        int puerto;
        Int32.TryParse(PuertoField.text, out puerto);

        int opcion = 1;
        Toggle optoggle = (Toggle)GameObject.Find("Opcion").GetComponent<Toggle>();

        if (optoggle.isOn)
            opcion = 2;

        bool data = AssemblyCSharp.PlayerInfo.Instance.startConnection(IPField.text, puerto, opcion);

        if (data)
            SceneManager.LoadScene(1);
        else
            Debug.Log("Imposible conectar al servidor, pruebe otra vez PE");
        if (data)
            SceneManager.LoadScene(1);
        else
            Debug.Log("Imposible conectar al servidor, pruebe otra vez PE");
	}
}
