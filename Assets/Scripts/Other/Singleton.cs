using System.Collections;
using UnityEngine;


public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static readonly object lockObject = new object();
    private static bool applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            // Если приложение завершается, возвращаем null и выводим предупреждение
            if (applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance of {typeof(T)} already destroyed. Returning null.");
                return null;
            }

            // Блокировка на случай вызова из нескольких потоков (хотя в Unity это редкость)
            lock (lockObject)
            {
                if (instance == null)
                {
                    // Пытаемся найти существующий экземпляр в сцене
                    instance = FindObjectOfType<T>();

                    // Если не нашли – создаём новый объект
                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject($"{typeof(T)} (Singleton)");
                        instance = singletonObject.AddComponent<T>();

                        // Указываем, что объект не должен уничтожаться при загрузке новой сцены
                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // Уничтожаем лишний объект, оставляя оригинальный
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            applicationIsQuitting = true;
        }
    }

    protected virtual void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }
}