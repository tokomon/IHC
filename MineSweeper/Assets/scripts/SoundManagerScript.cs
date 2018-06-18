using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour {

    public static AudioClip cubeDestroySound;
    static AudioSource audioSrc;
	// Use this for initialization
	void Start () {
        cubeDestroySound = Resources.Load<AudioClip>("cube");
    //    Debug.Log(cubeDestroySound);
        audioSrc = GetComponent<AudioSource>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "cube":
                audioSrc.PlayOneShot(cubeDestroySound);
                Debug.Log("Entra");
                break;
        }
    }
}
