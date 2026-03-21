using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlayBgm(BgmType.Game);
    }
}
