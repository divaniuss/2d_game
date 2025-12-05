using UnityEngine;
using UnityEngine.SceneManagement;

public class menu_buttons : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
