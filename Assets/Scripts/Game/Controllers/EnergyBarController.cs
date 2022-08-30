using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarController : MonoBehaviour
{
    public Slider Slider { get; set; }
    public EnergyBarController EnergyBar { get; set; }
    public bool Visible { get; set; }
    public TextMeshProUGUI SliderText;

    public void Start()
    {
        Visible = false;
        Slider = GetComponent<Slider>();
        SliderText = gameObject.transform.Find(Settings.NPC_ENERGY_BAR_TEXT).gameObject.GetComponent<TextMeshProUGUI>();

        if (Slider == null)
        {
            Debug.LogWarning("EnergyBarController/Slider null");
        }

        if (SliderText == null)
        {
            Debug.LogWarning("EnergyBarController/SliderText null");
        }
        SetMaxEnergy(Settings.NPC_DEFAULT_ENERGY);
    }

    public void SetEnergy(int energy)
    {
        Slider.value = energy;
        SliderText.text = energy+"%";
    }

    public void SetMaxEnergy(int maxEnergy)
    {
        Slider.maxValue = maxEnergy;
        Slider.value = maxEnergy;
    }

    public void SubstractEnergy(int maxEnergy)
    {
        Slider.value -= maxEnergy;
    }

    public void SetInactive()
    {
        Visible = false;
        gameObject.SetActive(false);
    }

    public void SetActive()
    {
        Visible = true;
        gameObject.SetActive(true);
    }

    public bool IsActive()
    {
        return Visible;
    }
}