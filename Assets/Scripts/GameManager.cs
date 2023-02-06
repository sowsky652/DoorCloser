using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{
    bool Stop { get; set; }
    private static GameManager m_instance; // �̱����� �Ҵ�� static ����
    delegate void pause();
    public Bullet bulletPrefab;
    private IObjectPool<Bullet> bullet;


    Bullet InstantiateObject()
    {
        Bullet temp = Instantiate(bulletPrefab);
        temp.SetPool(bullet);
        return temp;
    }

    public void OnObject(Bullet obj)
    {
        obj.gameObject.SetActive(true);
    }

    public void OnReleased(Bullet obj)
    {
        obj.gameObject.SetActive(false);
    }

    public bool IsStop()
    {
        return Stop;
    }

    public static GameManager instance
    {
        get
        {
            // ���� �̱��� ������ ���� ������Ʈ�� �Ҵ���� �ʾҴٸ�
            if (m_instance == null)
            {
                // ������ GameManager ������Ʈ�� ã�� �Ҵ�
                m_instance = FindObjectOfType<GameManager>();
            }

            // �̱��� ������Ʈ�� ��ȯ
            return m_instance;
        }
    }



    public void GameStop(bool value)
    {
        Stop = value;
    }

    private void Start()
    {
    }

    private void Awake()
    {
        bullet = new ObjectPool<Bullet>(InstantiateObject, OnObject, OnReleased);

        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
        if (instance != this)
        {
            // �ڽ��� �ı�
            Destroy(gameObject);
        }
    }
    public void MakeNoise(GameObject noiseMaker, float radius)
    {
        Collider[] colls = Physics.OverlapSphere(noiseMaker.transform.position, radius);

        foreach (var coll in colls)
        {

            if (coll.tag != noiseMaker.transform.tag &&
                coll.GetComponent<Shooter>() != null)
            {
                if (coll.GetComponent<Shooter>().Status == Status.Idle)
                {
                    coll.transform.LookAt(noiseMaker.transform.position);
                    coll.GetComponent<Shooter>().Status = Status.Fight;  
                }
            }
        }

    }
    public Bullet GetBullet()
    {
        return bullet.Get();

    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!Stop)
                BroadcastMessage("Stop", SendMessageOptions.DontRequireReceiver);
            else
                BroadcastMessage("Resume", SendMessageOptions.DontRequireReceiver);

        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Camera.main.fieldOfView -= 1;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.fieldOfView += 1;
        }
    }
}
