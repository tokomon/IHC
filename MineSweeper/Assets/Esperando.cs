using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using AssemblyCSharp;
public class Esperando : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        Debug.Log("ijgiu");
        if (AssemblyCSharp.PlayerInfo.Instance.option != 3)
            SceneManager.LoadScene(2);

    }
}
