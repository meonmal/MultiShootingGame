using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    [SerializeField]
    private float requiredExp;
    [SerializeField]
    private float baseExp;
    [SerializeField]
    private float scale;
    [SerializeField]
    private LevelUpManager levelUpManager;

    private int currentLevel;
    private float currentExp;

    public float CurrentExp => currentExp;
    public float RequiredExp => requiredExp;
    public int CurrentLevel => currentLevel;

    public void AddExp(float amount)
    {
        currentExp += amount;

        while (currentExp >= requiredExp)
        {
            currentExp -= requiredExp;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        requiredExp = baseExp + (currentLevel * currentLevel * scale);
        levelUpManager.Open();
    }
}
