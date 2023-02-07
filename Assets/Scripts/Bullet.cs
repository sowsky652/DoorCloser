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
    
    public GameObject Owner { set; get; }

    public void SetDamage(int damage)
    {
        dmg = damage;
    }
    private void OnEnable()
    {
        StartCoroutine("DeleteBullet");
        lastpos = transform.position;
        transform.rotation= Quaternion.identity;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GameManager.instance.MakeNoise(Owner,20);
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.green;

        Handles.DrawLine(lastpos, transform.position);

    }

    private void FixedUpdate()
    {
        if (Owner == null)
        {

            objectPool.Release(this);

        }
        Vector3 dir = (lastpos - transform.position).normalized;
        RaycastHit hit;
        
        if (Physics.Raycast(lastpos, dir, out hit, Vector3.Distance(lastpos,transform.position)))
        {
            if (hit.collider.gameObject.tag == "Enemy" || hit.collider.gameObject.tag == "Player" || hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                {
                    objectPool.Release(this);

                }
                if (hit.transform.GetComponent<Shooter>() != null &&
                hit.transform.tag != Owner.tag)
                {
                    Debug.Log($"hit:{hit.collider.tag}");
                    Debug.Log($"owner:{Owner.tag}");
                    Debug.Log("\n");
                    hit.transform.GetComponent<Shooter>().OnDamage(dmg);
                    objectPool.Release(this);

                }


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
        
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Player")
        {
            Debug.Log($"{other.gameObject.name}hit");
            Debug.Log($"isnull?{other.transform.GetComponent<Shooter>()}");
            if (other.transform.GetComponent<Shooter>() != null &&
                other.tag != Owner.tag)
            {

                other.transform.GetComponent<Shooter>().OnDamage(dmg);
            }

            objectPool.Release(this);
        }

    }
}
