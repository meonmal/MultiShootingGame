using UnityEngine;

public enum StageState
{
    Ready,
    Playing,
    BossSpawned,
    Clear,
    GameOver,
}

public class GameSceneManager : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlayBgm(BgmType.Game);
    }
}
