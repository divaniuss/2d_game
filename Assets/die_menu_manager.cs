using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class die_menu_manager : MonoBehaviour
{
    [Header("Звуки")]
    [SerializeField] private AudioClip restartSound;
    [SerializeField] private AudioClip menuSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.ignoreListenerPause = true;
    }

    // --- ОБНОВЛЕННЫЙ МЕТОД RESTART ---
    public void RestartGame()
    {
        // 1. Размораживаем время сразу
        Time.timeScale = 1f;

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

        // 3. Загружаем сцену (можно сразу, не дожидаясь конца звука)
        // Если хочешь микро-задержку для визуального клика, можно через корутину, 
        // но обычно рестарт делают мгновенным.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        // Для меню оставляем старую логику (ждем и выходим)
        StartCoroutine(WaitAndLoadString("Menu", menuSound));
    }

    // Корутина теперь нужна только для Меню
    IEnumerator WaitAndLoadString(string sceneName, AudioClip clip)
    {
        Time.timeScale = 1f; 

        if (clip != null) audioSource.PlayOneShot(clip);

        float delay = (clip != null) ? clip.length : 0f;
        if (delay > 0.5f) delay = 0.5f;

        yield return new WaitForSecondsRealtime(delay);

        SceneManager.LoadScene(sceneName);
    }
}