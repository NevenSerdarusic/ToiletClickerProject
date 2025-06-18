using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text coinSaldo;
    [SerializeField] private TMP_Text weightText;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TMP_Text gameOverText;


    private void Start()
    {
        gameOverScreen.SetActive(false);
    }

    public void UpdateCoins(int amount)
    {
        coinSaldo.text = amount.ToString();
    }

    public void UpdateWeight(float amount)
    {
        weightText.text = $"{amount:F2} kg";
    }

    public void ShowGameOverScreen(GameOverReason reason)
    {
        if (gameOverScreen == null || gameOverText == null)
        {
            Debug.LogWarning("GameOverScreen or GameOverText is not set!");
            return;
        }

        gameOverScreen.SetActive(true);

        switch (reason)
        {
            case GameOverReason.WeightLimit:
                gameOverText.text = "Game Over!\nYou've reached the ultimate weight!\nMaybe it's time for a healthy change?";
                break;

            case GameOverReason.PressureOverload:
                gameOverText.text = "Game Over!\nYou spent too long on the toilet!\nRemember: moderation is key!";
                break;
        }
    }

    
}
