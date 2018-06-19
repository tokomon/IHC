using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Reconexion : MonoBehaviour
{

    public void cambiarescena(string nombre)
    {
        InputField IPField = (InputField)GameObject.Find("IP").GetComponent<InputField>();
        InputField PuertoField = (InputField)GameObject.Find("Puerto").GetComponent<InputField>();
        InputField IDField = (InputField)GameObject.Find("ID").GetComponent<InputField>();

        int puerto;
        Int32.TryParse(PuertoField.text, out puerto);
        int id;
        Int32.TryParse(IDField.text, out id);
        /*
                bool data; //= AssemblyCSharp.PlayerInfo.Instance.startConnection(IPField.text, puerto, opcion);

                if (data)
                {
                    SceneManager.LoadScene(1);
                }
                else
                    Debug.Log("Imposible reconectar al servidor, pruebe otra vez PE");*/
    }
}
