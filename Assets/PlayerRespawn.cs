using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    void Awake()
    {
        // ЗАМЕНЯЕМ PlayerPrefs на GameSession
        if (GameSession.GetInt("HasCheckpoint") == 1)
        {
            float x = GameSession.GetFloat("CheckpointX");
            float y = GameSession.GetFloat("CheckpointY");
            transform.position = new Vector2(x, y);
        }
    }
}