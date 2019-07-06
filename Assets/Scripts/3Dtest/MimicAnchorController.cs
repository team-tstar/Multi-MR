using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class MimicAnchorController : MonoBehaviour, IPunObservable
{
    private string s = "empty";
    private bool shouldChangeColor = false;
    private bool firstTime = true;
    void Update()
    {
        if (shouldChangeColor && firstTime)
        {
            Debug.Log("colorChange");
            gameObject.GetComponent<RawImage>().color = Color.green;
            //shouldChangeColor = false;
            firstTime = false;
        }
    }
    public void onClick()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Clicked on master" + s);
            s = "hello";
            shouldChangeColor = true;
            GameObject.Find("ControllerMimic").GetComponent<ControllerMimic>().callBackInOtherScript();
            Debug.Log("Clicked on master" + s);
        }
        else
        {
            Debug.Log("Clicked on slave" + s);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(shouldChangeColor);
            stream.SendNext(s);
        }
        else
        {
            shouldChangeColor = (bool)stream.ReceiveNext();
            s = (string)stream.ReceiveNext();
        }
    }
}
