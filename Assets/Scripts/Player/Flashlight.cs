using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField, Range(0, 100)] private float batteryValue = 100f;
    [SerializeField] private GameObject lightsource;
    [SerializeField] private float drainSpeed = 5f;
    [SerializeField] private AudioClip flashlightSound;

    private void Start()
    {
        lightsource.SetActive(false);
    }

    private void Update()
    {
        FlashlightToggle();
    }

    private void FlashlightToggle()
    {
        if (Input.GetButtonDown("Flashlight") && batteryValue > 0f)
        {
            lightsource.SetActive(!lightsource.activeSelf);
            //Flashlight sound
            AudioManager.instance.PlaySFX(flashlightSound);
        }

        if (!lightsource.activeSelf)
            return;

        batteryValue -= drainSpeed * Time.deltaTime;
        batteryValue = Mathf.Clamp(batteryValue, 0f, 100f);

        if (batteryValue <= 0f)
        {
            lightsource.SetActive(false);
        }

    }

    public void AddBattery(float value)
    {
        batteryValue += value;
        batteryValue = Mathf.Clamp(batteryValue, 0f, 100f);
    }
}
