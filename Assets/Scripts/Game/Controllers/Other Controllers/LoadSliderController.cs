using UnityEngine;
using UnityEngine.UI;

public class LoadSliderController : MonoBehaviour
{
    private Slider slider;
    private float currentEnergy, energyBarTime = 3f, seconds;
    private Image sliderImage;
    private bool finished;

    public void Awake()
    {
        slider = GetComponent<Slider>();
        GameObject fillAreaObject = transform.Find("FillArea/Fill").gameObject;
        Util.IsNull(fillAreaObject, "LoadSliderController/fillAreaObject is null ");
        sliderImage = fillAreaObject.GetComponent<Image>();
        slider.value = 0;
        seconds = 0;
        finished = false;

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
            seconds += Time.unscaledDeltaTime;

            if (seconds <= energyBarTime)
            {
                currentEnergy = seconds * 100 / energyBarTime;
                SetEnergy((int)currentEnergy);
            }
            else
            {
                finished = true;
                seconds = 0;
                SetInactive();
            }
        }
    }

    public void SetDefaultFillTime(float time)
    {
        energyBarTime = time;
    }

    private void SetEnergy(int energy)
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

    public void SetActive(float seconds)
    {
        if (!gameObject.activeSelf)
        {
            finished = false;
            currentEnergy = 0;
            energyBarTime = seconds;
            gameObject.SetActive(true);
        }
    }

    public void SetActive()
    {
        if (!gameObject.activeSelf)
        {
            finished = false;
            currentEnergy = 0;
            SetEnergy(0);
            gameObject.SetActive(true);
        }
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    private void ResetCurrentEnergy()
    {
        currentEnergy = 0;
    }

    private float GetCurrentEnergy()
    {
        return currentEnergy;
    }

    public bool IsFinished()
    {
        return finished;
    }

    public void SetSliderFillMethod(Image.FillMethod method)
    {
        sliderImage.fillMethod = method;
    }
}