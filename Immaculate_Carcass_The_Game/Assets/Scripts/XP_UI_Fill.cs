using UnityEngine;
using UnityEngine.UI;

public class XP_UI_Fill : MonoBehaviour
{
    public Image fillImage;

    void Update()
    {
        float percent =
            (float)PlayerStats.Instance.xp /
            PlayerStats.Instance.xpToNextLevel;

        fillImage.fillAmount = Mathf.Clamp01(percent);
    }
}
