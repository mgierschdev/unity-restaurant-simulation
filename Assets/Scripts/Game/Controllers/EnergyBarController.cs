
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarController : MonoBehaviour
{
    private Slider Slider { get; set; }
    public EnergyBarController EnergyBar { get; set; }
    private bool Visible { get; set; }

    public void Start()
    {
        Visible = false;
        Slider = GetComponent<Slider>();

        if (Slider == null)
        {
            Debug.LogWarning("EnergyBarController/Slider null");
        }
        SetMaxEnergy(Settings.NPC_DEFAULT_ENERGY);
    }

    public void SetEnergy(int energy)
    {
        Slider.value = energy;
    }

    private void SetMaxEnergy(int maxEnergy)
    {
        Slider.maxValue = maxEnergy;
        Slider.value = maxEnergy;
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