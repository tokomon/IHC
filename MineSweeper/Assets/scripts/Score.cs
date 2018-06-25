using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {
    static public List<string> scoreList = new List<string>();
    public GameObject scoreText;
 

    // Use this for initialization
    void Start () {
        scoreText = GameObject.Find("textDinamic");//.GetComponent<Text>();


        for (int i = 0; i < scoreList.Count; i++)
        {
            scoreText.GetComponent<Text>().text = scoreList[i] + '\n';
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
