using UnityEngine;
using TMPro;

public class LevelTextUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;

    void Update()
    {
        if (PlayerStats.Instance == null) return;

        levelText.text = "Level " + PlayerStats.Instance.level;
    }
}
