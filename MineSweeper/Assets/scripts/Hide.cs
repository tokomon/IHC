using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : MonoBehaviour {

    void OnMouseDown()
    {
        int pos_x;
        int pos_y;
        System.Int32.TryParse(gameObject.name[0].ToString(), out pos_x);
        System.Int32.TryParse(gameObject.name[2].ToString(), out pos_y);
        Debug.Log(gameObject.name.ToString());
        Debug.LogFormat("X: {0}", pos_x);
        Debug.LogFormat("Y: {0}", pos_y);
        AssemblyCSharp.PlayerInfo.Instance.sendFreeSpace(pos_x,pos_y);
        /*
        if (gameObject.name[gameObject.name.Length-1] != '1'){
            Destroy(gameObject);
           // this.GetComponent<Renderer>().material = tablero.matPiso;
            //  gameObject.GetComponent<Renderer>().enabled = false;
        }
        else
        {
            this.GetComponent<Renderer>().material = tablero.matPared;
        }*/

    }
}
