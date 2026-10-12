using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-10)]
public class Singleton<T> : MonoBehaviour where T : Component
{
    private static bool isQuitting = false;
    private static T _instance;
    public static T Instance
    {
        get 
        {
            if (isQuitting)
            {
                return null;
            }

            if(_instance == null)
            {
                _instance = new GameObject(nameof(T), typeof(T)).GetComponent<T>();
            }
            return _instance;
        }

        private set
        {
            _instance = value;
        }
    }

    public virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }
}
