using UnityEngine;
using UnityEngine.UI;

public class LoadSliderController : MonoBehaviour
{
    private Slider slider;
    private float currentEnergy, energyBarTime, seconds;
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
            gameObject.SetActive(true);
            RestartState();
            energyBarTime = seconds;
        }
    }

    public void SetActive()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            energyBarTime = energyBarTime == 0 ? 3 : energyBarTime;
            RestartState();
        }
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    private float GetCurrentEnergy()
    {
        return currentEnergy;
    }

    public bool IsFinished()
    {
        return finished;
    }

    public void RestartState()
    {
        finished = false;
        currentEnergy = 0;
        seconds = 0;
        SetEnergy(0);
    }

    public void SetSliderFillMethod(Image.FillMethod method)
    {
        sliderImage.fillMethod = method;
    }
}