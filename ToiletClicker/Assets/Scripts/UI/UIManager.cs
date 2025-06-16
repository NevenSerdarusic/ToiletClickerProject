using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text coinSaldo;
    [SerializeField] private TMP_Text weightText;


    public void UpdateCoins(int amount)
    {
        coinSaldo.text = amount.ToString();
    }

    public void UpdateWeight(float amount)
    {
        weightText.text = $"{amount:F2} kg";
    }

    public void ShowGameOverScreen()
    {

    }
}
