using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EsperandoObs : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (AssemblyCSharp.PlayerInfo.Instance.read_map && AssemblyCSharp.PlayerInfo.Instance.option >= 20)
            SceneManager.LoadScene(3);
    }
}
