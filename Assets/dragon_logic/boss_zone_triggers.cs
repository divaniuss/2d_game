using UnityEngine;

public enum ZoneType
{
    Ground, 
    Green,  
    Blue,   
    Purple, 
    Red     
}

public class BossZoneTrigger : MonoBehaviour
{
    public ZoneType zoneType;       
    public Transform standPoint;    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Просто сообщаем боссу: "Игрок в моей зоне!"
            dragon_logic boss = FindFirstObjectByType<dragon_logic>(); // Unity 2023+ (или FindObjectOfType для старых)
            if (boss != null)
            {
                boss.UpdatePlayerPosition(this);
            }
        }
    }
}