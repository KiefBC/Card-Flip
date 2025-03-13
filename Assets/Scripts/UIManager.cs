using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreDisplay;

    public void UpdateScore(int newScore)
    {
        scoreDisplay.text = "Score: " + newScore;
    }
}
