using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;
using System;
public class Shooter : MonoBehaviour
{
    [Serializable]
    public class guns
    {
        public GunStat gun;
        public List<int> mag;
        public int curmag;
    }

    public GameObject attackTarget { get; set; }
    [Header("ÃÑ Á¤º¸")]
    public List<guns> gunList;
    bool isReload;
    private int curGun;
    public Animator animator;
    public ParticleSystem muzzleFlash;
    public GameObject activeSlider;
    public bool isAlive { get; set; }
    private int hp { get; set; }
    private float reloadtime;

    private void Start()
    {
        
    }
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
        if (isReload && gunList[curGun].gun.reloadTime > reloadtime)
        {
            reloadtime += Time.deltaTime;
            activeSlider.GetComponent<Slider>().value = reloadtime / gunList[curGun].gun.reloadTime;

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
        gunList[curGun].mag[gunList[curGun].curmag] -= 1;
        //animator.SetTrigger("Fire");
        muzzleFlash.Play();
    }

    IEnumerator ReLoading()
    {
        activeSlider.SetActive(true);
        reloadtime = 0;
        yield return new WaitForSeconds(gunList[curGun].gun.reloadTime);
        isReload = false;
        gunList[curGun].curmag++;
        if (gunList[curGun].curmag >= gunList[curGun].mag[gunList[curGun].curmag])
        {
            gunList[curGun].curmag = 0;
        }
        activeSlider.GetComponent<Slider>().value = 0;

        activeSlider.SetActive(false);
        yield break;
    }
    IEnumerator Shoot()
    {
        while (true)
        {

            if (gunList[curGun].mag[gunList[curGun].curmag] <= 0)
            {
                if (!isReload)
                {
                    Reload();
                }
            }
            if (!isReload)
            {
                Fire();
                Debug.Log(gunList[curGun].mag[gunList[curGun].curmag]);

            }
            yield return new WaitForSeconds(gunList[curGun].gun.shotdelay);
        }
    }

}
