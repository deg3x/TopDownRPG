using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void NewGame() => SceneManager.LoadScene("DemoScene", LoadSceneMode.Single);
    public void ExitGame() => Application.Quit();
}
