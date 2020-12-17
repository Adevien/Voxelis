using UnityEngine;
using UnityEngine.UI;

public class FpsMeter : MonoBehaviour
{
    private Text Meter;

    private float Timer;

    private void Start() => Meter = GetComponent<Text>();

    private void Update()
    {
        if (Time.unscaledTime > Timer)
        {
            Meter.text = $"{(int)(1f / Time.unscaledDeltaTime)}";
            Timer = Time.unscaledTime + 1;
        }
    }
}
