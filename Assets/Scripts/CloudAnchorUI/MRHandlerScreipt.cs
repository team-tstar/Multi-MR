using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MRHandlerScreipt : MonoBehaviour
{

    public GameObject cam;
    public GameObject UIele;
    public GameObject mainCam;
    public GameObject but;

    public void switchPress()
    {
        cam.SetActive(false);
        UIele.SetActive(false);
        but.SetActive(true);
        mainCam.GetComponent<Camera>().rect = new Rect(0, 0, 1f, 1f);
        mainCam.GetComponent<Camera>().depth = 0;
        mainCam.GetComponent<Camera>().farClipPlane = 200;
        mainCam.GetComponent<Camera>().allowMSAA = true;


    }

    public void SwitchtoMR()
    {
        cam.SetActive(true);
        UIele.SetActive(true);
        but.SetActive(false);
       
        mainCam.GetComponent<Camera>().rect = new Rect(0, 0, 0.5f, 1f);
        mainCam.GetComponent<Camera>().depth = -2;
        mainCam.GetComponent<Camera>().farClipPlane = 2000;
        mainCam.GetComponent<Camera>().allowMSAA = false;
    }
}
