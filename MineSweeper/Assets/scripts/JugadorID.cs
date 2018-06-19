using UnityEngine;
using UnityEngine.UI;
using AssemblyCSharp;

public class JugadorID : MonoBehaviour {

    public Text m_MyText;
    // Use this for initialization
    void Start () {
        m_MyText.text = "Jugador: " + AssemblyCSharp.PlayerInfo.Instance.player_id  ;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
