using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : MonoBehaviour {

    void OnMouseDown()
    {
<<<<<<< HEAD
        
        int pos_x;
        int pos_y;
        System.Int32.TryParse(gameObject.name[0].ToString(), out pos_x);
        System.Int32.TryParse(gameObject.name[2].ToString(), out pos_y);
        Debug.Log(gameObject.name.ToString());
        Debug.LogFormat("X: {0}", pos_x);
        Debug.LogFormat("Y: {0}", pos_y);
        AssemblyCSharp.PlayerInfo.Instance.sendFreeSpace(pos_x,pos_y);
        
=======

        int Pos_x = AssemblyCSharp.PlayerInfo.Instance.pos_x;
        int Pos_y = AssemblyCSharp.PlayerInfo.Instance.pos_y;
        System.Int32.TryParse(gameObject.name[0].ToString(), out Pos_x);
        System.Int32.TryParse(gameObject.name[2].ToString(), out Pos_y);
        Debug.Log(gameObject.name.ToString());
        Debug.LogFormat("X: {0}", Pos_x);
        Debug.LogFormat("Y: {0}", Pos_y);
        AssemblyCSharp.PlayerInfo.Instance.sendFreeSpace(Pos_x,Pos_y);
        /*
>>>>>>> 8f3704a0d0cf71366d234b02dd22b12fc6589b01
        if (gameObject.name[gameObject.name.Length-1] != '1'){
            Destroy(gameObject);
                    SoundManagerScript.PlaySound("cube");

           // this.GetComponent<Renderer>().material = tablero.matPiso;
            //  gameObject.GetComponent<Renderer>().enabled = false;
        }
        else
        {     SoundManagerScript.PlaySound("error");
            this.GetComponent<Renderer>().material = tablero.matPared;
        }

    }
}
