using System.Collections.Generic;
using UnityEngine;

public class BuffUI : MonoBehaviour
{
    [SerializeField] private BuffSlotUI[] buffSlots;

    private readonly List<ActiveBuff> activeBuffs = new();

    private void Start()
    {
        RefreshUI();
    }

    private void Update()
    {
        if (activeBuffs.Count == 0)
        {
            return;
        }

        bool isRemoved = UpdateBuffTimes(Time.deltaTime);

        RefreshUI();
    }

    public void AddBuff(BuffData buffData)
    {
        if (buffData == null)
        {
            return;
        }

        for (int i = 0; i < activeBuffs.Count; i++)
        {
            if (activeBuffs[i].Data == buffData)
            {
                activeBuffs[i] = new ActiveBuff(buffData);
                RefreshUI();
                return;
            }
        }

        if (activeBuffs.Count >= buffSlots.Length)
        {
            return;
        }

        ActiveBuff newBuff = new ActiveBuff(buffData);
        activeBuffs.Add(newBuff);

        RefreshUI();
    }

    private bool UpdateBuffTimes(float deltaTime)
    {
        bool removed = false;

        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            bool isFinished = activeBuffs[i].UpdateTime(deltaTime);

            if (isFinished)
            {
                activeBuffs.RemoveAt(i);
                removed = true;
            }
        }

        return removed;
    }

    private void RefreshUI()
    {
        for (int i = 0; i < buffSlots.Length; i++)
        {
            if (i < activeBuffs.Count)
            {
                ActiveBuff buff = activeBuffs[i];
                buffSlots[i].SetBuff(buff.Data.icon, buff.NormalizedTime);
            }
            else
            {
                buffSlots[i].Clear();
            }
        }
    }
}
