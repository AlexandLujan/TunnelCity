using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneScript : MonoBehaviour
{
    public void LoadSceneFunc(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
