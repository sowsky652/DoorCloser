using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Scriptable/GunStat",fileName ="GunStat")]
public class GunStat : ScriptableObject
{
    public AudioClip shotClip;
    public AudioClip reloadClip;
    public int damage;
    public float shotdelay;
    public int magCapacity;
    public float reloadTime;
    public int magReamning;
   
}
