using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private bool open;
    private Vector3 originRot;
    // Start is called before the first frame update
    void Start()
    {
        originRot = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("door hit");
        if(!open&&other.transform.GetComponent<Shooter>() != null)
        {
            open = true;
            transform.rotation = Quaternion.Euler(originRot + new Vector3(0, 90,0));
        }
    }
}
