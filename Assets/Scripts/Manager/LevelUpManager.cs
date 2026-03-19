using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    [SerializeField]
    private Button[] buttons;
    [SerializeField]
    private Image[] icons;
    [SerializeField]
    private TextMeshProUGUI[] titles;
    [SerializeField]
    private TextMeshProUGUI[] descs;

    [SerializeField]
    private List<StatUIData> statUIDatas;

    private Dictionary<StatType, StatUIData> statUIMap;
    private Player _player;

    private void EnsureInitialized()
    {
        if (statUIMap != null)
            return;

        statUIMap = new Dictionary<StatType, StatUIData>();

        foreach (var data in statUIDatas)
        {
            statUIMap[data.statType] = data;
        }
    }

    public void Init(Player player)
    {
        _player = player;
    }

    public void Open()
    {
        EnsureInitialized();

        List<StatType> options = GetRandomOptions();

        if (options.Count == 0)
        {
            Debug.Log("모든 스탯이 최대 레벨임");
            return;
        }

        SetButtons(options);

        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    private List<StatType> GetRandomOptions()
    {
        List<StatType> candidates = _player.RunTimeStats.GetAvailableStats();

        if (candidates.Count == 0)
        {
            return new List<StatType>();
        }

        Shuffle(candidates);

        int count = Mathf.Min(buttons.Length, candidates.Count);
        List<StatType> result = new List<StatType>();

        for (int i = 0; i < count; i++)
        {
            result.Add(candidates[i]);
        }

        return result;
    }

    private void Shuffle(List<StatType> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private void SetButtons(List<StatType> options)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.RemoveAllListeners();

            if (i < options.Count)
            {
                StatType statType = options[i];
                StatUIData data = statUIMap[statType];

                buttons[i].gameObject.SetActive(true);
                icons[i].sprite = data.icon;
                titles[i].text = data.title;

                float current = _player.RunTimeStats.GetStat(statType);
                float next = _player.RunTimeStats.GetNextStat(statType);
                float delta = _player.RunTimeStats.GetDelta(statType);

                string deltaText = delta > 0 ? $"+{delta}" : $"{delta}";
                descs[i].text = $"{data.description}\n{current} → {next} ({deltaText})";

                buttons[i].onClick.AddListener(() => SelectOption(statType));
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }
    }

    private void SelectOption(StatType statType)
    {
        _player.RunTimeStats.LevelUp(statType);
        Close();
    }
}