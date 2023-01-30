using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool Stop { get; set; }
    private static GameManager m_instance; // 싱글톤이 할당될 static 변수
    delegate void pause();
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
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
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
    }
}
