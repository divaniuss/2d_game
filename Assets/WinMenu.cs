using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
public class WinMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created 
    [Header("Звук кнопки")]
    [SerializeField] private AudioClip restartSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.ignoreListenerPause = true;
    }

    public void RestartGame()
    {
        // 1. Размораживаем время сразу
        Time.timeScale = 1f;
        GameSession.ClearAll();
        // PlayerPrefs.DeleteAll();

        // 2. Создаем "Бессмертный звук", если клип есть
        if (restartSound != null)
        {
            // Создаем пустой объект
            GameObject soundObj = new GameObject("RestartSound_Persistent");
            
            // Вешаем на него плеер
            AudioSource src = soundObj.AddComponent<AudioSource>();
            src.clip = restartSound;
            src.volume = 1f; // Или audioSource.volume, если хочешь ту же громкость
            src.Play();

            // ГЛАВНОЕ: Говорим Unity НЕ уничтожать этот объект при смене сцены
            DontDestroyOnLoad(soundObj);

            // Заказываем уничтожение этого объекта ровно через длину звука (например, 7 сек)
            Destroy(soundObj, restartSound.length);
        }
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

}
