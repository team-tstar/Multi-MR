using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOrJoinRoomCanvas : MonoBehaviour
{

    [SerializeField]
    private GameObject CreateOrJoinRoomcanvas;
    [SerializeField]
    private GameObject CurrentRoomCanvas;
    // Start is called before the first frame update
    void Active()
    {
        CreateOrJoinRoomcanvas.SetActive(true);
        CurrentRoomCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
