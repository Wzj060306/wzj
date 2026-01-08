using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // 加载小孩场景
    public void LoadKidScene()
    {
        SceneManager.LoadScene("KidScene");
    }

    // 加载科学家场景
    public void LoadScientistScene()
    {
        SceneManager.LoadScene("ScientistScene");
    }
}