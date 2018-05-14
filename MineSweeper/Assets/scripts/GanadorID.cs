using UnityEngine;
using UnityEngine.UI;
using AssemblyCSharp;

public class GanadorID : MonoBehaviour
{
    public Text m_MyText;
    
    void Start()
    {
        //Text sets your text to say this message
        m_MyText.text = "Ganador: " + AssemblyCSharp.PlayerInfo.Instance.player_winner;
    }

    void Update()
    {
    }
}