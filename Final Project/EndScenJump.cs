using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneJump : MonoBehaviour
{
    // 填写完整路径（和Build Settings一致）
    public string targetSceneName = "Scenes/End_";

    public void JumpToEndScene()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}