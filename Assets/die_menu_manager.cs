using UnityEngine;
using UnityEngine.SceneManagement;

public class die_menu_manager : MonoBehaviour
{
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Метод для кнопки "Menu"
    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
