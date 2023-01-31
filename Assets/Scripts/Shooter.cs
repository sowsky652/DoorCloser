using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject attackTarget { get; set; }
    public GunStat gunstat;
    bool isReload;
    public Animator animator;
    public bool isAlive { get; set; }
    private int hp { get; set; }

    private void Update()
    {
        if (hp <= 0) {
            //place regdoll and Destory gameobject
        }
        if (attackTarget == null)
        {
            StopCoroutine("Shoot");
        }
    }

    public void OnShoot()
    {
        StartCoroutine("Shoot");
    }

    public void OnDamage(int damage)
    {
        hp -= damage;
        
    }

    public void StopShoot()
    {
        StopCoroutine("Shoot");
    }

    void Reload()
    {
        isReload = true;        
    }

    public void Fire()
    {
        --gunstat.magReamning;
        animator.SetTrigger("Fire");
    }

    IEnumerator Shoot()
    {
        while (true)
        {
            //if (gun.magReamning < 0)
            //{                
            //    if (!isReload)
            //    {
            //        Reload();
            //    }                
            //}
            Fire();
            Debug.Log("shoot!!!!");
            yield return new WaitForSeconds(gunstat.shotdelay);

        }
    }

}
