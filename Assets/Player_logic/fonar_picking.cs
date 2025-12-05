using UnityEngine;

public class fonar_picking : MonoBehaviour
{
    [SerializeField] private GameObject OnGround;
    [SerializeField] private GameObject InHands;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("fonar"))
        {
            OnGround.SetActive(false);
            InHands.SetActive(true);
        }
    }
}
