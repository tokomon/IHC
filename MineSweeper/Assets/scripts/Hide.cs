using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : MonoBehaviour {

    void OnMouseDown()
    {

        int Pos_x = AssemblyCSharp.PlayerInfo.Instance.pos_x;
        int Pos_y = AssemblyCSharp.PlayerInfo.Instance.pos_y;
        System.Int32.TryParse(gameObject.name[0].ToString(), out Pos_x);
        System.Int32.TryParse(gameObject.name[2].ToString(), out Pos_y);
        Debug.Log(gameObject.name.ToString());
        Debug.LogFormat("X: {0}", Pos_x);
        Debug.LogFormat("Y: {0}", Pos_y);
        AssemblyCSharp.PlayerInfo.Instance.sendFreeSpace(Pos_x,Pos_y);
        /*
        if (gameObject.name[gameObject.name.Length-1] != '1'){
            Destroy(gameObject);
                    SoundManagerScript.PlaySound("cube");

           // this.GetComponent<Renderer>().material = tablero.matPiso;
            //  gameObject.GetComponent<Renderer>().enabled = false;
        }
        else
        {
            this.GetComponent<Renderer>().material = tablero.matPared;
        }*/

    }
}
