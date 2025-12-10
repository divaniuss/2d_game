using UnityEngine;

public class key_pick : MonoBehaviour
{
    [SerializeField] private GameObject OnGround; 
    [SerializeField] private GameObject OnPlayer; 

    [SerializeField] private AudioClip pickupSound; 
    [Range(0f, 1f)] [SerializeField] private float volume = 1f;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("key"))
        {
      
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
            }

            OnGround.SetActive(false);
            OnPlayer.SetActive(true);
        }
    }
}