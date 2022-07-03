using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    public Slider slider;
    public EnergyBar energyBar;
    public bool visible = false;

    public void Start()
    {
        SetMaxEnergy(Settings.NPC_DEFAULT_ENERGY);
    }

    public void SetEnergy(int energy)
    {
        slider.value = energy;
    }

    public void SetMaxEnergy(int maxEnergy)
    {
        slider.maxValue = maxEnergy;
        slider.value = maxEnergy;
    }

    public void substractEnergy(int maxEnergy)
    {
        slider.value -= maxEnergy;
    }

    public void setInactive()
    {
        visible = false;
        gameObject.SetActive(false);
    }

    public void setActive()
    {
        visible = true;
        gameObject.SetActive(true);
    }

    public bool isActive()
    {
        return visible;
    }
}