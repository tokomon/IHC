using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChange : MonoBehaviour {
    public Material[] material;
    Renderer rend;

    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
        //rend.enabled = true;
        Texture3D texture = (Texture3D)Resources.Load("Assets/textures/piso");
        Material newMat = Resources.Load("Assets/Materials/piso", typeof(Material)) as Material;
        //Debug.Log(newMat);

        rend.material = newMat;


    }


    // Update is called once per frame
    void Update () {
		
	}
}
