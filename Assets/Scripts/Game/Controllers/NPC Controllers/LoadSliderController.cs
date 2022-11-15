using UnityEngine;
using UnityEngine.UI;

public class LoadSliderController : MonoBehaviour
{
    private Slider Slider { get; set; }

    public void Start()
    {

        Slider = GetComponent<Slider>();

        if (Slider == null)
        {
            GameLog.LogWarning("EnergyBarController/Slider null");
        }
        SetMaxEnergy(Settings.NpcDefaultEnergy);
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

        gameObject.SetActive(false);
    }

    public void SetActive()
    {
        gameObject.SetActive(true);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
}
