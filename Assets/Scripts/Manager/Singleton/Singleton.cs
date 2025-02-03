using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance=(T)FindObjectOfType(typeof(T));
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    
                    instance = obj.GetComponent<T>();
                }
            }
            Debug.Log(instance.name);
            return instance;
        }
        
    }

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            // �̹� �����ϴ� �ν��Ͻ��� ������ ���� ������ ������Ʈ�� �ı�
            Destroy(this.gameObject);
            return;
        }

        instance = this as T;
        DontDestroyOnLoad(this.gameObject);

        /*if (transform.parent != null && transform.root != null)
        {
            DontDestroyOnLoad(this.transform.root.gameObject);
        }
        else
        {
            DontDestroyOnLoad (this.gameObject);
        }*/

    }
}
