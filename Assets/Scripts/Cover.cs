using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cover : MonoBehaviour
{       
    Bounds bounds;
    List<Vector3> coverPos=new List<Vector3>();
   

    private void Start()
    {
        bounds = GetComponent<MeshCollider>().bounds;

        coverPos.Add(new Vector3(bounds.min.x - 0.5f, 0, bounds.center.z));
        coverPos.Add(new Vector3(bounds.max.x - 0.5f, 0, bounds.center.z));
        coverPos.Add(new Vector3(bounds.center.x, 0, bounds.max.z + 0.5f));
        coverPos.Add(new Vector3(bounds.center.x, 0, bounds.min.z - 0.5f));
    }

    private void Update()
    {

    }


    //private void OnDrawGizmos()
    //{
    //    rend = GetComponent<Renderer>();
    //    Vector3 center = left;

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireCube(center, rend.bounds.size);
    //}

    public Vector3 FindCover(GameObject opponent)
    {
        RaycastHit hit;
        List<Vector3> cover;

        foreach (var pos in coverPos)
        {
            var temp = Physics.RaycastAll(opponent.transform.position, (pos - opponent.transform.position).normalized, Vector3.Distance(opponent.transform.position, pos));
            if ( temp!= null)
            {
                return pos;
            }
        }
        return default;

    }


}
