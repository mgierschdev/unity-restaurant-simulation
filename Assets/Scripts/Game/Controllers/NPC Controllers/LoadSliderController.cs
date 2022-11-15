using UnityEngine;
using UnityEngine.UI;

public class LoadSliderController : MonoBehaviour
{
    private Slider slider;
    private float currentEnergy, energyBarSpeed;

    public void Start()
    {
        energyBarSpeed = 20f;
        slider = GetComponent<Slider>();

        if (slider == null)
        {
            GameLog.LogWarning("EnergyBarController/Slider null");
        }
        SetMaxEnergy(Settings.NpcDefaultEnergy);
    }

    public void SetEnergy(int energy)
    {
        slider.value = energy;
    }

    private void SetMaxEnergy(int maxEnergy)
    {
        slider.maxValue = maxEnergy;
        slider.value = maxEnergy;
    }

    public void SetInactive()
    {

        gameObject.SetActive(false);
    }

    public void SetActive(float speed)
    {
        currentEnergy = 0;
        energyBarSpeed = speed;
        gameObject.SetActive(true);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public void UpdateLoadSlider()
    {
        // EnergyBar controller, only if it is active
        if (IsActive())
        {
            if (currentEnergy <= 100)
            {
                currentEnergy += Time.fixedDeltaTime * energyBarSpeed;
                SetEnergy((int)currentEnergy);
            }
            else
            {
                SetInactive();
            }
        }
    }

    public void ResetCurrentEnergy()
    {
        currentEnergy = 0;
    }

    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }
}
