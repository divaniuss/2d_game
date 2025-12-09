using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class pause_script : MonoBehaviour
{
    public GameObject PausePanel;

    [Header("Звуки")]
    [SerializeField] private AudioClip pauseSound;    // Звук открытия паузы
    [SerializeField] private AudioClip continueSound; // Звук кнопки "Продолжить"
    [SerializeField] private AudioClip menuSound;     // Звук кнопки "В меню"

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // ВАЖНО: Разрешаем этому звуку играть, даже когда время = 0
        audioSource.ignoreListenerPause = true; 
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PausePanel.activeSelf)
            {
                Continue();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        if (pauseSound != null) audioSource.PlayOneShot(pauseSound);
        
        PausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Continue()
    {
        // Сначала звук, потом действия
        if (continueSound != null) audioSource.PlayOneShot(continueSound);

        PausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void GoMenu()
    {
        // Тут запускаем корутину, чтобы звук клика успел проиграть перед сменой сцены
        StartCoroutine(PlaySoundAndLoadMenu(menuSound));
    }

    IEnumerator PlaySoundAndLoadMenu(AudioClip clip)
    {
        // Перед выходом в меню обязательно возвращаем время!
        Time.timeScale = 1f; 

        if (clip != null) audioSource.PlayOneShot(clip);

        float delay = (clip != null) ? clip.length : 0f;
        if (delay > 0.5f) delay = 0.5f;

        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene("Menu");
    }
}