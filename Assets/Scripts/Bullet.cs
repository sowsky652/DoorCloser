using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    private IObjectPool<Bullet> objectPool;
    private int dmg;
    Vector3 lastpos;
    private GameObject owner;

    public GameObject Owner { set; get; }

    public void SetDamage(int damage)
    {
        dmg = damage;
    }
    private void OnEnable()
    {
        //   StartCoroutine("DeleteBullet");
        lastpos = transform.position;
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.green;

        Handles.DrawLine(transform.position, transform.forward*1000);

    }

    private void FixedUpdate()
    {
        Vector3 dir = lastpos- transform.position;
        RaycastHit hit;
        if(Owner==null)
            objectPool.Release(this);

        if (Physics.Raycast(Owner.transform.position, transform.position, out hit))
        {
            if (hit.collider.gameObject.tag=="Enemy" || hit.collider.gameObject.tag == "Player"|| hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                
                Debug.Log(hit.collider.tag);
                Debug.Log(Owner.tag);
                Debug.Log("\n");
                if (hit.transform.GetComponent<Shooter>() != null &&
                hit.transform.tag != Owner.tag)
                {

                    hit.transform.GetComponent<Shooter>().OnDamage(dmg);
                }


                objectPool.Release(this);
            }
        }
        lastpos = transform.position;


    }
    IEnumerator DeleteBullet()
    {
        yield return new WaitForSeconds(1f);
        objectPool.Release(this);
        yield break;

    }

    public void SetPool(IObjectPool<Bullet> pool)
    {
        objectPool = pool;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.tag == "Player")
        {
            Debug.Log("hit");
            if (other.transform.GetComponent<Shooter>() != null &&
                other.tag != owner.tag)
            {

                other.transform.GetComponent<Shooter>().OnDamage(dmg);
            }

            objectPool.Release(this);
        }

    }
}
