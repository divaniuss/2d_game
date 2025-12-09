using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class doors : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject ClosedDoor;
    [SerializeField] private GameObject OpendedDoor;

    [SerializeField] private GameObject KeyInHands;
    [SerializeField] private AudioClip doorOpenSound;
    private AudioSource audioSource;
    private void Start()
    {
        // Получаем AudioSource (он добавится автоматически благодаря RequireComponent)
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false; // Чтобы не заиграл случайно при старте
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("door") == true)
        {
            ClosedDoor.SetActive(false);
            OpendedDoor.SetActive(true);
            if (doorOpenSound != null)
            {
                // Создает временный динамик в точке, где был ключ, играет звук и сам удаляется
                AudioSource.PlayClipAtPoint(doorOpenSound, transform.position);
            }
            Destroy(KeyInHands);
        }
    }
}
