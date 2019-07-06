using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class standBy : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update

    // Update is called once per frame

    public GameObject a;
    public GameObject b;
   // public GameObject c;

    private void Start()
    {
        a.SetActive(false);
    }

    void Update()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
   //         a.SetActive(true);
    //        b.SetActive(false);
       //     c.SetActive(false);
        }
    }
}
