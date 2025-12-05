using UnityEngine;
using UnityEngine.UI;

public class coins_collect : MonoBehaviour
{
    [SerializeField] private int CoinsCounter;
    [SerializeField] private Text CoinsCounterText;
    [SerializeField] private AudioClip CoinSound;
    private AudioSource audioSource;

    void Start()
    {
        this.audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("coin"))
        {
            CoinsCounter++;
            CoinsCounterText.text = CoinsCounter.ToString();
            if (this.audioSource != null && this.CoinSound != null)
            {
                this.audioSource.PlayOneShot(this.CoinSound);
            }
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("runduk"))
        {
            CoinsCounter += 100;
            CoinsCounterText.text = CoinsCounter.ToString();
        }
    }
}