using UnityEngine;
using UnityEngine.SceneManagement;
public class pause_script : MonoBehaviour
{

    public GameObject PausePanel;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausePanel.SetActive(!PausePanel.activeSelf);
        }
    }

    public void Continue()
    {
        PausePanel.SetActive(false);
    }

    public void GoMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
