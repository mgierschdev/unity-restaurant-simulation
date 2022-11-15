using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Image.FillMethod;

public class LoadSliderController : MonoBehaviour
{
    private float maxEnergy = 100;
    private Slider slider;
    private float currentEnergy, energyBarSpeed = 20f;
    private Image sliderImage;

    public void Start()
    {
        slider = GetComponent<Slider>();
        GameObject fillAreaObject = transform.Find("FillArea/Fill").gameObject;
        sliderImage = fillAreaObject.GetComponent<Image>();

        if (slider == null)
        {
            GameLog.LogWarning("EnergyBarController/Slider null");
        }
        SetMaxEnergy(Settings.NpcDefaultEnergy);
    }

    public void Update()
    {
        // EnergyBar controller, only if it is active
        if (IsActive())
        {
            if (currentEnergy <= maxEnergy)
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
        if (!IsActive())
        {
            currentEnergy = 0;
            energyBarSpeed = speed;
            gameObject.SetActive(true);
        }
    }

    public void SetActive()
    {
        if (!IsActive())
        {
            currentEnergy = 0;
            energyBarSpeed = 30f;
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
        return currentEnergy >= maxEnergy;
    }

    public void SetSliderFillMethod(Image.FillMethod method)
    {
        sliderImage.fillMethod = method;
    }
}
