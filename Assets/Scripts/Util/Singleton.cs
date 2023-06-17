using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; private set; }
    public static bool IsInitialized => Instance != null;

    protected virtual void Awake()
    {
        if (Instance == null) Instance = (T)this;
        else Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
