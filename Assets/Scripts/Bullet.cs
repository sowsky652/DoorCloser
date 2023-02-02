using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    private IObjectPool<Bullet> objectPool;
    private int dmg;

    public void SetDamage(int damage) {
        dmg = damage;
    }
    private void OnEnable()
    {
     //   StartCoroutine("DeleteBullet");
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

    private void ontrigger(Collision collision)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")||other.gameObject.tag=="Player") {

            if (other.transform.GetComponent<Shooter>() != null)
            {
                other.transform.GetComponent<Shooter>().OnDamage(dmg);
            }

            objectPool.Release(this);
        }

    }
}
