using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NameGetter : MonoBehaviour
{
    public GameObject namae;
    public void OnClicked()
    {
        namae.GetComponent<PhotonNameSetter>().SetPlayerName(namae.GetComponent<InputField>().text);
    }
}
