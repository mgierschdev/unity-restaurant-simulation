using UnityEngine;
using UnityEngine.UI;

public class EnergyBarController : MonoBehaviour
{
    private Slider slider { get; set; }
    private EnergyBarController energyBar { get; set; }
    private bool visible { get; set; }

    public void Start()
    {
        visible = false;
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

    public void SubstractEnergy(int maxEnergy)
    {
        slider.value -= maxEnergy;
    }

    public void SetInactive()
    {
        visible = false;
        gameObject.SetActive(false);
    }

    public void SetActive()
    {
        visible = true;
        gameObject.SetActive(true);
    }

    public bool IsActive()
    {
        return visible;
    }
}