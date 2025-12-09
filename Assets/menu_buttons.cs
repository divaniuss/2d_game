using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 

[RequireComponent(typeof(AudioSource))]
public class menu_buttons : MonoBehaviour
{
    [Header("Звуки кнопок")]
    [SerializeField] private AudioClip startSound;
    [SerializeField] private AudioClip exitSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // --- ОБНОВЛЕННЫЙ МЕТОД START GAME ---
    public void StartGame()
    {
        GameSession.ClearAll(); 

        if (startSound != null)
        {
            GameObject soundObj = new GameObject("StartGameSound_Persistent");
            
            AudioSource src = soundObj.AddComponent<AudioSource>();
            src.clip = startSound;
            src.volume = 1f; 
            src.Play();

            // Говорим Unity НЕ уничтожать этот объект при загрузке уровня
            DontDestroyOnLoad(soundObj);

            // Удаляем его, когда звук закончится
            Destroy(soundObj, startSound.length);
        }

        // 2. Грузим сцену МГНОВЕННО (без задержек)
        SceneManager.LoadScene("SampleScene");
    }

    // --- МЕТОД EXIT ОСТАВЛЯЕМ КАК БЫЛ ---
    // Потому что если приложение закроется, звук играть будет негде :)
    public void ExitGame()
    {
        StartCoroutine(PlaySoundAndQuit(exitSound));
    }

    // Корутина нужна теперь только для выхода
    IEnumerator PlaySoundAndQuit(AudioClip clip)
    {
        if (clip != null) audioSource.PlayOneShot(clip);

        float delay = (clip != null) ? clip.length : 0f;
        if (delay > 0.5f) delay = 0.5f; // Ждем не больше полсекунды

        yield return new WaitForSecondsRealtime(delay);

        Application.Quit();
    }
}