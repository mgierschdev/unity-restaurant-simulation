using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadSliderController : MonoBehaviour
{
    private Slider slider;
    private float currentEnergy, energyBarSpeed = 30f, defaultMaxEnergy = 100;
    private Image sliderImage;

    public void Awake()
    {
        slider = GetComponent<Slider>();
        GameObject fillAreaObject = transform.Find("FillArea/Fill").gameObject;
        Util.IsNull(fillAreaObject, "LoadSliderController/fillAreaObject is null ");
        sliderImage = fillAreaObject.GetComponent<Image>();
        slider.value = 0;

        if (slider == null)
        {
            GameLog.LogWarning("EnergyBarController/Slider null");
        }
        SetMaxEnergy(Settings.NpcDefaultEnergy);
    }

    public void Update()
    {
        // EnergyBar controller, only if it is active
        if (gameObject.activeSelf)
        {
            if (currentEnergy <= defaultMaxEnergy)
            {
                currentEnergy += Time.deltaTime * energyBarSpeed;
                SetEnergy((int)currentEnergy);
            }
            else
            {
                SetInactive();
            }
        }
    }

    public void SetDefaultFillSpeed(float speed)
    {
        energyBarSpeed = speed;
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
        if (!gameObject.activeSelf)
        {
            currentEnergy = 0;
            energyBarSpeed = speed;
            gameObject.SetActive(true);
        }
    }

    public void SetActive()
    {
        if (!gameObject.activeSelf)
        {
            currentEnergy = 0;
            SetEnergy(0);
            gameObject.SetActive(true);
        }
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public void ResetCurrentEnergy()
    {
        currentEnergy = 0;
    }

    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }

    public bool IsEnergyFull()
    {
        return currentEnergy >= defaultMaxEnergy;
    }

    public void SetSliderFillMethod(Image.FillMethod method)
    {
        sliderImage.fillMethod = method;
    }
}