using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig")]
public class GameConfig : ScriptableObject
{
    public DifficultyConfig easyConfig;
    public DifficultyConfig hardConfig;

    public enum Difficulty
    {
        Easy,
        Hard
    }

    private Difficulty? selectedDifficultyCache;

    public DifficultyConfig Current
    {
        get
        {
            if (!selectedDifficultyCache.HasValue)
            {
                int savedDifficulty = PlayerPrefsHandler.GetDifficulty();
                selectedDifficultyCache = (Difficulty)Mathf.Clamp(savedDifficulty, 0, 1);
            }
            
            return selectedDifficultyCache == Difficulty.Easy ? easyConfig : hardConfig;
        }
    }

    public void RefreshSelectedDifficulty()
    {
        selectedDifficultyCache = null;
    }

}
