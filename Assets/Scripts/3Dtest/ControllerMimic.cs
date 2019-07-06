using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ControllerMimic : MonoBehaviour
{
    
    void Start()
    {
        gameObject.name = "ControllerMimic";
        if(PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Instantiated on Network");
            PhotonNetwork.Instantiate("Canvas", Vector3.zero, Quaternion.identity);
        }
    }

    
    // Update is called once per frame
    public void callBackInOtherScript()
    {
        Debug.Log("it's working");
    }
}
