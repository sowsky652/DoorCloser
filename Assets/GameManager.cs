using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool Stop { get; set; }
    private static GameManager m_instance; // �̱����� �Ҵ�� static ����
    delegate void pause();
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
        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
        if (instance != this)
        {
            // �ڽ��� �ı�
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
