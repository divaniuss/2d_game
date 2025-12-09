using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class LevelMusic : MonoBehaviour
{
    public static LevelMusic instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Не выключай музыку при перезагрузке уровня
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            Destroy(gameObject);
            instance = null; 
        }
    }
}