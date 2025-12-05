using UnityEngine;

public class portal_back : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.position += new Vector3(38, -9f, 0); 
        }
    }
}
