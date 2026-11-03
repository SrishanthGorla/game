using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    public CarController car;
    public TextMeshProUGUI speedText;

    void Start()
    {
        if (speedText == null)
        {
            speedText = GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (car != null && speedText != null)
        {
            int speedKmh = Mathf.RoundToInt(car.GetCurrentSpeedKmh());
            speedText.text = speedKmh + " KM/H";
        }
    }
}