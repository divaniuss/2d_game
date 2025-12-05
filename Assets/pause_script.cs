using UnityEngine;
using UnityEngine.SceneManagement;
public class pause_script : MonoBehaviour
{

    public GameObject PausePanel;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PausePanel.activeSelf)
            {
                Continue();
            }
            else
            {
                Pause();
            }

        }
    }

    public void Pause()
    {
        PausePanel.SetActive(true);
        Time.timeScale = 0f;
    }
    public void Continue()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void GoMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
