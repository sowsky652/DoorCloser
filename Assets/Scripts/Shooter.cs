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
    public ParticleSystem muzzleFlash;
    public bool isAlive { get; set; }
    private int hp { get; set; }

    private void Update()
    {
        if (hp <= 0)
        {
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
        StartCoroutine("ReLoading");
    }

    public void Fire()
    {
        --gunstat.magReamning;
        animator.SetTrigger("Fire");
        muzzleFlash.Play();
    }

    //IEnumerator ReLoading()
    //{
    //    yield return new WaitForSeconds(gunstat.reloadTime);
    //    isReload = false;
    //    //gunstat.magReamning=gunstat.;
    //        yield return 0;
    //}
    IEnumerator Shoot()
    {
        while (true)
        {
            //if (gunstat.magReamning <= 0)
            //{
            //    if (!isReload)
            //    {
            //        Reload();
            //    }
            //}
            if (!isReload)
            {
                Fire();
            }
            yield return new WaitForSeconds(gunstat.shotdelay);
        }
    }

}
