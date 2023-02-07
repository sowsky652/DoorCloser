using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;

public class Shooter : MonoBehaviour
{
    [Serializable]
    public class guns
    {
        public GunStat gun;
        public List<int> mag;
        public int curmag;
    }

    Status status;

    public Status Status { get { return status; } set { status = value; Debug.Log(status); } }

    public GameObject attackTarget { get; set; }
    [Header("�� ����")]
    public List<guns> gunList;
    bool isReload;
    bool isSwaping;
    public float swapSpeed;
    public float swaping;
    private int curGun;
    public Animator animator;
    public ParticleSystem muzzleFlash;
    public GameObject activeSlider;
    public bool isAlive { get; set; }
    public int hp;
    private float reloadtime;
    public Transform muzzlePos;
    public float accuracy = 0.1f;

    private void Start()
    {
        status = Status.Idle;

    }
    private void Update()
    {
        if (hp <= 0)
        {
            Debug.Log($"{gameObject.name} dead");
            Destroy(gameObject);
        }
        if (attackTarget == null)
        {
            StopCoroutine("Shoot");
            status = Status.Idle;
        }
        if (isReload && gunList[curGun].gun.reloadTime > reloadtime)
        {
            reloadtime += Time.deltaTime;
            activeSlider.GetComponent<Slider>().value = reloadtime / gunList[curGun].gun.reloadTime;

        }
        if (isSwaping && swapSpeed > swaping)
        {
            swaping += Time.deltaTime;
            activeSlider.GetComponent<Slider>().value = swaping / swapSpeed;
        }

    }

    public void Swap()
    {
        isSwaping = true;
        StopCoroutine("Swop");
        StopCoroutine("ReLoading");
        StopCoroutine("Shoot");
        StartCoroutine("Swop");

    }

    IEnumerator Swop()
    {
        activeSlider.SetActive(true);
        swaping = 0;
        yield return new WaitForSeconds(swapSpeed);
        if (curGun == 0)
            curGun = 1;
        else
            curGun = 0;
        isSwaping = false;
        activeSlider.GetComponent<Slider>().value = 0;

        activeSlider.SetActive(false);
        yield break;
    }

    public void OnShoot()
    {
        StartCoroutine("Shoot");
    }

    public void OnDamage(int damage)
    {
        Debug.Log($"{transform.name}has Damage({hp})");
        hp -= damage;

    }

    public void StopShoot()
    {
        StopCoroutine("Shoot");
    }

    public void Reload()
    {
        StopCoroutine("ReLoading");
        isReload = true;
        reloadtime = 0;
        StartCoroutine("ReLoading");
    }

    public void Fire()
    {
        if (gunList[curGun].mag[gunList[curGun].curmag] <= 0)
        {
            if (!isReload)
            {
                Reload();
            }
        }
        if (isReload)
        {
            return;
        }

        gunList[curGun].mag[gunList[curGun].curmag] -= 1;
        GameManager.instance.MakeNoise(gameObject, 10);
        muzzleFlash.Play();
        var temp = GameManager.instance.GetBullet();
        temp.SetDamage(gunList[curGun].gun.damage);
        temp.transform.position = muzzlePos.transform.position;
        float x= UnityEngine.Random.Range(-accuracy, accuracy);
        float y= UnityEngine.Random.Range(-accuracy, accuracy);
        temp.transform.rotation =muzzlePos.transform.rotation;
        temp.transform.Rotate(new Vector3(x, y, 0));
        temp.transform.gameObject.GetComponent<Rigidbody>().AddForce(temp.transform.forward * 40, ForceMode.Impulse);
        temp.Owner = transform.gameObject;
    }

    IEnumerator ReLoading()
    {
        activeSlider.SetActive(true);
        reloadtime = 0;
        yield return new WaitForSeconds(gunList[curGun].gun.reloadTime);
        isReload = false;
        gunList[curGun].curmag++;
        if (gunList[curGun].curmag >= gunList[curGun].mag.Count)
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
            Debug.Log(gunList[curGun].mag[gunList[curGun].curmag]);

            Fire();

            yield return new WaitForSeconds(gunList[curGun].gun.shotdelay);
        }
    }

}
