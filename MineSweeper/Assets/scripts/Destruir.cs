using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruir : MonoBehaviour {
    GameObject d; 
	// Use this for initialization
	void Start () {
        d = GameObject.Find("Cube07");
        Destroy(d, (float)0);
        Destroy(this.gameObject, (float)1); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
