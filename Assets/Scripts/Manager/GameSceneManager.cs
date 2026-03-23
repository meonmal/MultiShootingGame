using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField]
    private Player player;
    [SerializeField]
    private StageManager stageManager;

    private void Start()
    {
        SoundManager.Instance.PlayBgm(BgmType.Game);
        stageManager.Init(player);
    }
}
