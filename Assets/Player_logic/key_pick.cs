using UnityEngine;

public class key_pick : MonoBehaviour
{
    [SerializeField] private GameObject OnGround; 
    [SerializeField] private GameObject OnPlayer; 
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("key"))
        {
            OnGround.SetActive(false);
            OnPlayer.SetActive(true);
        }
    }
}
