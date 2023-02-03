using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{
    bool Stop { get; set; }
    private static GameManager m_instance; // 싱글톤이 할당될 static 변수
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
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<GameManager>();
            }

            // 싱글톤 오브젝트를 반환
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

        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
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

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Camera.main.fieldOfView -= 1;
        }else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.fieldOfView += 1;
        }
    }
}
