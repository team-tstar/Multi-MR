using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class StartNowScript : MonoBehaviourPunCallbacks
{
    public void StartTheParty()
    {

        PhotonNetwork.LoadLevel(1);
        // SceneManager.LoadScene(2);

    }

}
