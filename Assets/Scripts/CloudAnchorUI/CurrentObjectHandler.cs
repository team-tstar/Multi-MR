using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentObjectHandler : MonoBehaviour
{

   // public int currentObjNumber;
  //  public GameObject selected;
 //   public GameObject lord;
//    public GameObject chick;
//    public GameObject bass;
//    public GameObject mage;
    public string selected;   
    /// <summary>
    /// replace "Andy" with selected I have placed the req objects in the resource folder
    /// </summary>


    public selectScreenToggle sst;

   

    private void Update()
    {
        if(sst.selec==0)
        {
            selected = "lord";
        }

        if (sst.selec == 1)
        {
            selected = "bass";
        }

        if (sst.selec == 2)
        {
            selected = "mage";
        }

        if (sst.selec == 3)
        {
            selected = "chick";
        }
    }
}
