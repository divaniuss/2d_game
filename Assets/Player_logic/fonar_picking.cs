using UnityEngine;

public class fonar_picking : MonoBehaviour
{
    [SerializeField] private GameObject OnGround;
    [SerializeField] private GameObject InHands;


    [SerializeField] private AudioClip pickupSound; 
    [Range(0f, 1f)] [SerializeField] private float volume = 1f;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("fonar"))
        {
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
            }

            OnGround.SetActive(false);
            InHands.SetActive(true);
        }
    }
}