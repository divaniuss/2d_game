using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{    
    private bool isActivated = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Если зашел игрок и точка еще не активирована
        if (collision.CompareTag("Player") && !isActivated)
        {
            ActivateCheckpoint();
        }
    }

    void ActivateCheckpoint()
    {
        isActivated = true;

        // ЗАМЕНЯЕМ ВСЕ PlayerPrefs на GameSession
        GameSession.SetFloat("CheckpointX", transform.position.x);
        GameSession.SetFloat("CheckpointY", transform.position.y);
        GameSession.SetInt("HasCheckpoint", 1);

    }
}