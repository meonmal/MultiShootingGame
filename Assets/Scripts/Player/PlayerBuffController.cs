using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffController : MonoBehaviour
{
    private readonly List<ActiveBuff> activeBuffs = new List<ActiveBuff>();

    private readonly Dictionary<StatType, float> addCache = new Dictionary<StatType, float>();
    private readonly Dictionary<StatType, float> multiplyCache = new Dictionary<StatType, float>();

    private bool isDirty = true;

    private void Update()
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            bool isExpired = activeBuffs[i].UpdateTime(Time.deltaTime);

            if (isExpired)
            {
                activeBuffs.RemoveAt(i);
                isDirty = true;
            }
        }
    }

    public void AddBuff(BuffData buffData)
    {
        if (buffData == null)
            return;

        activeBuffs.Add(new ActiveBuff(buffData));
        isDirty = true;
    }

    public float GetAddValue(StatType statType)
    {
        RebuildCacheIfNeeded();

        if (addCache.TryGetValue(statType, out float value))
            return value;

        return 0f;
    }

    public float GetMultiplyValue(StatType statType)
    {
        RebuildCacheIfNeeded();

        if (multiplyCache.TryGetValue(statType, out float value))
            return value;

        return 1f;
    }

    private void RebuildCacheIfNeeded()
    {
        if (!isDirty)
            return;

        addCache.Clear();
        multiplyCache.Clear();

        foreach (ActiveBuff activeBuff in activeBuffs)
        {
            StatType statType = activeBuff.Data.statType;

            if (!addCache.ContainsKey(statType))
                addCache[statType] = 0f;

            if (!multiplyCache.ContainsKey(statType))
                multiplyCache[statType] = 1f;

            if (activeBuff.Data.valueType == BuffValueType.Add)
            {
                addCache[statType] += activeBuff.Data.value;
            }
            else if (activeBuff.Data.valueType == BuffValueType.Multiply)
            {
                multiplyCache[statType] *= activeBuff.Data.value;
            }
        }

        isDirty = false;
    }
}
