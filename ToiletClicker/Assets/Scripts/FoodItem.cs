using UnityEngine;


[CreateAssetMenu(fileName = "NewFood", menuName = "Food/FoodItem")]
public class FoodItem : ScriptableObject
{
    public string foodName;
    public Sprite foodImage;
    public int cost;
    public AudioClip foodEatingSound;
    public float fatContent;
    public float sugarContent;
    public float fibersContent;
    

    private const float fatCoefficient = 0.025f;
    private const float sugarCoefficient = 0.025f;
    private const float fibersCoefficient = 0.25f;
    private float foodWeightGain => (fatContent * fatCoefficient) + (sugarContent * sugarCoefficient) - (fibersContent * fibersCoefficient);

    public float FoodWeightGain => foodWeightGain;

}
