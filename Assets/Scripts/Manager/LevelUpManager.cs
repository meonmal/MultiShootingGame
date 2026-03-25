using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    /// <summary>
    /// 레벨업 선택지 버튼 배열.
    /// 각 버튼은 하나의 스탯 업그레이드를 의미한다.
    /// </summary>
    [SerializeField]
    private Button[] buttons;

    /// <summary>
    /// 각 선택지에 표시될 아이콘 이미지.
    /// </summary>
    [SerializeField]
    private Image[] icons;

    /// <summary>
    /// 선택지의 제목 텍스트.
    /// (예: 공격력, 이동속도 등)
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI[] titles;

    /// <summary>
    /// 선택지의 상세 설명 텍스트.
    /// 현재 값, 다음 값, 증가량 등을 표시한다.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI[] descs;

    /// <summary>
    /// UI에 표시할 스탯 데이터 목록.
    /// 아이콘, 이름, 설명 등의 정보를 포함한다.
    /// </summary>
    [SerializeField]
    private List<StatUIData> statUIDatas;

    /// <summary>
    /// StatType을 키로 하여 UI 데이터를 빠르게 찾기 위한 Dictionary.
    /// </summary>
    private Dictionary<StatType, StatUIData> statUIMap;

    /// <summary>
    /// 레벨업 대상이 되는 플레이어 참조.
    /// </summary>
    private Player _player;

    /// <summary>
    /// statUIMap이 초기화되어 있지 않다면 생성하고 데이터를 채우는 함수.
    /// 배열 데이터를 Dictionary로 변환하여 빠르게 접근할 수 있도록 한다.
    /// </summary>
    private void EnsureInitialized()
    {
        if (statUIMap != null)
        {
            return;
        }

        statUIMap = new Dictionary<StatType, StatUIData>();

        foreach (var data in statUIDatas)
        {
            statUIMap[data.statType] = data;
        }
    }

    /// <summary>
    /// 레벨업 시스템에 사용할 플레이어를 설정하는 함수.
    /// </summary>
    /// <param name="player">레벨업 대상 플레이어</param>
    public void Init(Player player)
    {
        _player = player;
    }

    /// <summary>
    /// 레벨업 UI를 열고 선택지를 생성하는 함수.
    /// 랜덤으로 스탯을 뽑아 버튼에 세팅한 뒤 게임을 일시정지한다.
    /// </summary>
    public void Open()
    {
        EnsureInitialized();

        List<StatType> options = GetRandomOptions();

        // 선택 가능한 스탯이 없다면 종료 (모든 스탯이 최대 레벨)
        if (options.Count == 0)
        {
            Debug.Log("모든 스탯이 최대 레벨임");
            return;
        }

        SetButtons(options);

        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 레벨업 UI를 닫고 게임을 다시 진행시킨다.
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 플레이어가 업그레이드 가능한 스탯 목록 중에서
    /// 랜덤으로 선택지를 생성하는 함수.
    /// </summary>
    /// <returns>랜덤으로 선택된 스탯 리스트</returns>
    private List<StatType> GetRandomOptions()
    {
        // 현재 레벨업 가능한 스탯 목록 가져오기
        List<StatType> candidates = _player.RunTimeStats.GetAvailableStats();

        if (candidates.Count == 0)
        {
            return new List<StatType>();
        }

        // 리스트를 섞어서 랜덤성 확보
        Shuffle(candidates);

        // 버튼 개수만큼만 선택
        int count = Mathf.Min(buttons.Length, candidates.Count);
        List<StatType> result = new List<StatType>();

        for (int i = 0; i < count; i++)
        {
            result.Add(candidates[i]);
        }

        return result;
    }

    /// <summary>
    /// 리스트를 랜덤하게 섞는 함수.
    /// Fisher-Yates 방식으로 셔플한다.
    /// </summary>
    /// <param name="list">섞을 리스트</param>
    private void Shuffle(List<StatType> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    /// <summary>
    /// 전달받은 스탯 목록을 기반으로 버튼 UI를 세팅하는 함수.
    /// 각 버튼에 아이콘, 텍스트, 클릭 이벤트를 설정한다.
    /// </summary>
    /// <param name="options">표시할 스탯 목록</param>
    private void SetButtons(List<StatType> options)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            // 기존 이벤트 제거 (중복 방지)
            buttons[i].onClick.RemoveAllListeners();

            if (i < options.Count)
            {
                StatType statType = options[i];
                StatUIData data = statUIMap[statType];

                // UI 활성화 및 기본 정보 세팅
                buttons[i].gameObject.SetActive(true);
                icons[i].sprite = data.icon;
                titles[i].text = data.title;

                // 현재 값, 다음 값, 증가량 계산
                float current = _player.GetBaseStats(statType);
                float next = _player.GetNextBaseStats(statType);
                float delta = _player.GetBaseDeltaStats(statType);

                // 증가량 텍스트 처리 (+ 붙이기)
                string deltaText = delta > 0 ? $"+{delta:F1}" : $"{delta:F1}";

                // 설명 텍스트 구성
                descs[i].text = $"{data.description}\n{current:F1} → {next:F1} ({deltaText})";

                // 버튼 클릭 시 해당 스탯 레벨업 실행
                buttons[i].onClick.AddListener(() => SelectOption(statType));
            }
            else
            {
                // 사용할 선택지가 없으면 버튼 비활성화
                buttons[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 선택된 스탯을 레벨업시키고 UI를 닫는 함수.
    /// </summary>
    /// <param name="statType">선택된 스탯 타입</param>
    private void SelectOption(StatType statType)
    {
        _player.LeveUp(statType);
        Close();
    }
}