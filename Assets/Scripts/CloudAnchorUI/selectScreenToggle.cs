using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selectScreenToggle : MonoBehaviour
{

    public bool toggs;
    public GameObject selectCanvas;
    public int selec;
    public bool justSelec;
    // Start is called before the first frame update
    void Start()
    {
        toggs= false;
        selectCanvas.SetActive(false);
        justSelec = false;
    }

    public void toggler()
    {
        if(!toggs)
        {
            selectCanvas.SetActive(true);
            toggs=true;
        }
        else
        {
            selectCanvas.SetActive(false);
            toggs=false;
        }
    
    }

    public void t1()
    {
        selec = 1;
        _ShowAndroidToastMessage("chair");
        selectCanvas.SetActive(false);
        toggs = false;
        Debug.Log("Selecte");
        justSelec = true;

    }
    public void t2()
    {
        selec = 2;
        _ShowAndroidToastMessage("boss chair");
        selectCanvas.SetActive(false);
        toggs = false;
        justSelec = true;
    }
    public void t3()
    {
        selec = 3;
        _ShowAndroidToastMessage("Sofa");
        selectCanvas.SetActive(false);
        toggs = false;
        justSelec = true;
    }
    public void t4()
    {
        selec = 4;
        _ShowAndroidToastMessage("Table");
        selectCanvas.SetActive(false);
        toggs = false;
        justSelec = true;
    }


    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject =
                    toastClass.CallStatic<AndroidJavaObject>(
                        "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
