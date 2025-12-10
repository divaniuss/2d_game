using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Обязательно для таймера (IEnumerator)

public class coins_collect : MonoBehaviour
{
    [Header("Настройки Монет")]
    [SerializeField] private int CoinsCounter;
    [SerializeField] private Text CoinsCounterText;
    [SerializeField] private AudioClip CoinSound;
    [Range(0f, 1f)] [SerializeField] private float coinVolume = 0.5f;

    [Header("Настройки Финала (Сундук)")]
    [SerializeField] private GameObject levelCompletePanel; 
    [SerializeField] private AudioClip chestPickupSound;    // Короткий звук взятия сундука
    [SerializeField] private AudioClip victoryMusic;        // Длинная мелодия победы
    [SerializeField] private GameObject CoinsInChest; 
    private AudioSource audioSource;
    private bool levelIsFinished = false; // Чтобы не взять сундук дважды (если их вдруг несколько)

    void Start()
    {
        this.audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("coin"))
        {
            CoinsCounter++;
            UpdateUI();
            PlaySound(CoinSound, coinVolume);
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("runduk") && !levelIsFinished)
        {
            levelIsFinished = true; 
            CoinsCounter += 100;
            UpdateUI();

            PlaySound(chestPickupSound, 1f);

            if (victoryMusic != null && audioSource != null)
            {
                audioSource.PlayOneShot(victoryMusic, 1f);
            }

            Destroy(CoinsInChest);
            StartCoroutine(ShowWinPanelRoutine());
        }
    }

    void UpdateUI()
    {
        if (CoinsCounterText != null)
            CoinsCounterText.text = CoinsCounter.ToString();
    }

    void PlaySound(AudioClip clip, float vol)
    {
        if (this.audioSource != null && clip != null)
        {
            this.audioSource.PlayOneShot(clip, vol);
        }
    }

    // --- ТАЙМЕР ПЕРЕД ПОБЕДОЙ ---
    IEnumerator ShowWinPanelRoutine()
    {
        // Ждем 5 секунд (пока играет музыка)
        yield return new WaitForSeconds(5f);

        // Включаем панель
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }
    }
}