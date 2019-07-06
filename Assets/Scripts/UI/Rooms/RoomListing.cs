using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomListing : MonoBehaviour
{

   
    [SerializeField]
    private Text _text;
    public GameObject LoadS;
    public GameObject LoadST;

    public RoomInfo RoomInfo { get; private set; }

    private void Start()
    {

    }
    public void SetRoomInfo(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        _text.text = roomInfo.MaxPlayers + "," + roomInfo.Name;
    }

    public void OnClick_Button()
    {
        PhotonNetwork.JoinRoom(RoomInfo.Name);
        SceneManager.LoadScene(2);

 //      LoadS.SetActive(true);
 //      LoadST.SetActive(false);
    }

}
