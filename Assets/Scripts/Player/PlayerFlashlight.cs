using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[System.Serializable]
public class PlayerFlashlight
{
    [Header("Flashlight Settings")]
    [SerializeField] private Transform flashlightModel;
    [SerializeField] private Light flashlightLight;
    [SerializeField] private InputActionReference toggleAction;
    [SerializeField] private InputActionReference rechargeBatteryAction;
    [SerializeField] private float maxBattery = 100f;
    [SerializeField] private float drainRate = 20f; // per second
    [SerializeField] private float lowBatteryThreshold = 15f;
    [SerializeField] private float flickerIntensity = 0.2f;
    [SerializeField] private float flickerSpeed = 0.1f;

    private float flashlightStartingIntensity; 
    
    private bool isOn;
    private float currentBattery;
    private Coroutine flickerRoutine;
    private MonoBehaviour coroutineRunner; // to allow coroutines from this serialized class

    public bool IsOn => isOn;
    public float Battery => currentBattery;

    public void Initialize(MonoBehaviour runner)
    {
        coroutineRunner = runner;
        currentBattery = maxBattery;
        flashlightLight.enabled = false;
        flashlightModel.gameObject.SetActive(false);
        flashlightStartingIntensity = flashlightLight.intensity;
        toggleAction.action.performed += ctx => ToggleFlashlight();
        rechargeBatteryAction.action.performed += ctx => RechargeFlashlight();
    }

    public void Update()
    {
        if (!isOn) return;
        if (currentBattery <= 0f)
        {
            TurnOff();
            return;
        }

        currentBattery -= drainRate * Time.deltaTime;

        if (currentBattery <= lowBatteryThreshold)
        {
            if (flickerRoutine == null)
                flickerRoutine = coroutineRunner.StartCoroutine(FlickerEffect());
        }
        else if (flickerRoutine != null)
        {
            coroutineRunner.StopCoroutine(flickerRoutine);
            flickerRoutine = null;
            flashlightLight.intensity = 1f;
        }
    }

    private void ToggleFlashlight()
    {
        if (isOn)
            TurnOff();
        else
            TurnOn();
    }

    public void RechargeFlashlight()
    {
        if (flickerRoutine != null)
        {
            coroutineRunner.StopCoroutine(flickerRoutine);
            flickerRoutine = null;
        }
        currentBattery = maxBattery;
        flashlightLight.intensity = flashlightStartingIntensity;
        // flashlightLight.enabled = true;
    }

    private void TurnOn()
    {
        if (currentBattery <= 0f) return;
        isOn = true;
        flashlightLight.enabled = true;
        flashlightModel.gameObject.SetActive(true);
        AudioManager.Instance.PlaySound("SFX_Flashlight_On");
    }

    private void TurnOff()
    {
        isOn = false;
        flashlightLight.enabled = false;
        flashlightModel.gameObject.SetActive(false);
        if (flickerRoutine != null)
        {
            coroutineRunner.StopCoroutine(flickerRoutine);
            flickerRoutine = null;
        }

        AudioManager.Instance.PlaySound("SFX_Flashlight_Off");
    }

    private IEnumerator FlickerEffect()
    {
        while (isOn && currentBattery <= lowBatteryThreshold)
        {
            flashlightLight.intensity = 1f - Random.Range(0f, flickerIntensity);
            yield return new WaitForSeconds(Random.Range(flickerSpeed * 0.5f, flickerSpeed * 1.5f));
        }

        flashlightLight.intensity = 1f;
    }
}
