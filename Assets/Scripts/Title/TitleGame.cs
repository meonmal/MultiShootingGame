using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleGame : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlayBgm(BgmType.Title);
    }

    public void GameScene()
    {
        SceneManager.LoadScene("GameScene");
        SoundManager.Instance.PlayBgm(BgmType.Game);
    }

    public void GameExit()
    {
        #if UNITY_EDITOR
        // 에디터에서 플레이 모드 종료
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // 빌드된 게임 종료
        Application.Quit();
        #endif
    }
}
