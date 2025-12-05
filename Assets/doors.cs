using Unity.VisualScripting;
using UnityEngine;

public class doors : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject ClosedDoor;
    [SerializeField] private GameObject OpendedDoor;

    [SerializeField] private GameObject KeyInHands;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("door") == true)
        {
            ClosedDoor.SetActive(false);
            OpendedDoor.SetActive(true);
            Destroy(KeyInHands);
        }
    }
}
