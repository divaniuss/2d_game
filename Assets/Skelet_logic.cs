using UnityEngine;
using System.Collections;

public class Skelet_logic : MonoBehaviour
{
    [SerializeField] private int HealthPoints = 3;
    [SerializeField] private Color damageColor = new Color(0.776f, 0.051f, 0f); // red
    [SerializeField] private float damageFlashDuration = 0.2f; // 0.2sec

    private SpriteRenderer[] allRenderers;

    void Awake()
    {
        allRenderers = this.GetComponentsInChildren<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("fireball"))
        {
            HealthPoints -= 1;


            if (HealthPoints <= 0)
            {
                StartCoroutine(FlashAndDie());
            }
            else
            {
                StartCoroutine(FlashDamage());
            }

            Destroy(collision.gameObject);
        }
    }

    IEnumerator FlashDamage()
    {
        Color[] originalColors = new Color[allRenderers.Length];
        for (int i = 0; i < allRenderers.Length; i++)
        {
            originalColors[i] = allRenderers[i].color;
            allRenderers[i].color = damageColor;
        }

        yield return new WaitForSeconds(damageFlashDuration);

        for (int i = 0; i < allRenderers.Length; i++)
        {
            allRenderers[i].color = originalColors[i];
        }
    }

    IEnumerator FlashAndDie()
    {
        // black
        Color[] originalColors = new Color[allRenderers.Length];
        for (int i = 0; i < allRenderers.Length; i++)
        {
            originalColors[i] = allRenderers[i].color;
            allRenderers[i].color = Color.black;
        }

        yield return new WaitForSeconds(damageFlashDuration);

        Destroy(this.gameObject);
    }
}