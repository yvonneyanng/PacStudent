using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] string level1SceneName = "Level_Manual"; 

    public void LoadLevel1() => SceneManager.LoadScene(level1SceneName);
    public void LoadStart() => SceneManager.LoadScene("StartScene");
}
