﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectionSetter : MonoBehaviourPunCallbacks
{
    void Start()
    {

        PhotonNetwork.AutomaticallySyncScene = true;
       
       
    }

}
