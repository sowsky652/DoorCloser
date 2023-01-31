using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject attackTarget { get; set; }
    public GunStat gun;
    bool isReload;
    private void Update()
    {
        if (attackTarget != null)
        {
            StartCoroutine(Shoot());
        }
        if (attackTarget == null)
        {
            StopCoroutine(Shoot());
        }
    }

    void Reload()
    {
        isReload = true;        
    }

    public void Fire()
    {
        --gun.magReamning;
    }

    IEnumerator Shoot()
    {
        while (true)
        {
            if (gun.magReamning < 0)
            {                
                if (!isReload)
                {
                    Reload();
                }                
            }
            Fire();
            yield return new WaitForSeconds(gun.shotdelay);

        }
    }

}
