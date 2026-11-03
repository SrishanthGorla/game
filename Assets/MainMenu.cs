using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Loads your racing track scene
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        // Closes the game application
        Debug.Log("Quit Game Clicked!");
        Application.Quit();
    }
}