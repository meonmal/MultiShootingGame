using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Player player;
    [SerializeField]
    private BuffPopup popupPrefab;
    [SerializeField]
    private Transform popupParent;

    [SerializeField]
    private Slider playerHPSlider;
    [SerializeField]
    private Slider playerEXPSlider;
    [SerializeField]
    private TextMeshProUGUI playerHPText;
    [SerializeField]
    private TextMeshProUGUI playerEXPText;
    [SerializeField]
    private TextMeshProUGUI playerLevelText;

    private void Update()
    {
        playerHPSlider.value = player.CurrentHP / player.GetStats(StatType.PlayerHP);
        playerEXPSlider.value = player.PlayerExperience.CurrentExp / player.PlayerExperience.RequiredExp;

        playerHPText.text = $"체력 : {player.CurrentHP} / {player.GetStats(StatType.PlayerHP)}";
        playerEXPText.text = $"경험치 : {player.PlayerExperience.CurrentExp} / {player.PlayerExperience.RequiredExp}";
        playerLevelText.text = $"Lv : {player.PlayerExperience.CurrentLevel + 1}";
    }

    public void ShowBuffPopup(string message, Vector3 worldPosition)
    {
        BuffPopup popup = Instantiate(popupPrefab, popupParent);
        popup.Init(message, worldPosition);
    }
}
