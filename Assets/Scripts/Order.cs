using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Order : MonoBehaviour 
{
    public GameObject arrow { get { return arrow; } set { arrow.SetActive(true); } }
    public CTMgr ct { get; set; }
    public bool swap { get; set; }
    public bool reload { get; set; }
    public Vector3 rot { get; set; }
    public bool flash { get; set; }

    public void BookedReload()
    {
       reload = true;
       transform.Find("OrderMenu").gameObject.SetActive(false);
    }

    public void BookedSwap()
    {
        swap= true;
        transform.Find("OrderMenu").gameObject.SetActive(false);
    }

    public void Delete()
    {
        Destroy(gameObject);
    }
}
