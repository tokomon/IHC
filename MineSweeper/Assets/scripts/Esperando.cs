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
        if (AssemblyCSharp.PlayerInfo.Instance.option == 8)
            SceneManager.LoadScene(3);
        AssemblyCSharp.PlayerInfo.Instance.sendPosition();
    }
}
