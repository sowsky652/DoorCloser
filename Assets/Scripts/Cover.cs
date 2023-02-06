using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cover : MonoBehaviour
{
    Bounds bounds;
    Transform left;
    Transform right;
    Transform upper;
    Transform lower;
    
    private void Start()
    {
        
        bounds=GetComponent<MeshCollider>().bounds;
       
        left= new Vector3(bounds.min.x-10,0,bounds.center.z);
        right = new Vector3(bounds.max.x - 10, 0, bounds.center.z);
        
        upper = new Vector3(bounds.center.x, 0,bounds.max.z + 10);
        lower = new Vector3(bounds.center.x, 0, bounds.min.z - 10);
    }

    private void Update()
    {
    }

    public List<Vector3> FindCover(GameObject opponent)
    {
        RaycastHit hit;
        if (Physics.Raycast(opponent.transform.position, left - opponent.transform.position, out hit, Vector3.Distance(left, transform.position)))
        {

        }

    }


}
