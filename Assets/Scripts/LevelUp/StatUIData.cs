using UnityEngine;

/// <summary>
/// 레벨업을 하면 뜨게 될 UI의 정보를 담은 클래스
/// </summary>
[System.Serializable]
public class StatUIData
{
    public StatType statType;
    public Sprite icon;
    public string description;
    public string title;
}
