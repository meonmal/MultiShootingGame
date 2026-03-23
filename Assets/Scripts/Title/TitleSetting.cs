using UnityEngine;

public class TitleSetting : MonoBehaviour
{
    [SerializeField]
    private GameObject settingPanel;

    public void SettingOn()
    {
        settingPanel.gameObject.SetActive(true);
    }
}
