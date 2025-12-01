using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShieldSpellUI : MonoBehaviour
{
    public Button shieldButton;
    public Image iconImage;
    public TextMeshProUGUI cooldownText;

    private Color normalColor = Color.white;
    private Color disabledColor = new Color(1, 1, 1, 0.4f);

    void Update()
    {
        int cooldown = PlayerHealth.Instance.arcaneShieldCooldown;

        // Update UI based on cooldown
        if (cooldown > 0)
        {
            shieldButton.interactable = false;
            iconImage.color = disabledColor;

            cooldownText.text = cooldown.ToString();
        }
        else
        {
            shieldButton.interactable = true;
            iconImage.color = normalColor;

            cooldownText.text = ""; // hide cooldown
        }
    }
}
