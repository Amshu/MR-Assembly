using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SYS_HyboxMaxi : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] TextMeshProUGUI screenText;
    [SerializeField] Transform gaugeNeedle;
    [SerializeField] SYS_Switch switchScript;

    bool _isRunning;

    private void Awake()
    {
        screenText.text = "";
    }


    public void PowerOnMachine(bool toggle)
    {
        if (_isRunning == toggle || !switchScript.IsInOnPosition) return;

        _isRunning = toggle;

        if (toggle)
        {
            audioSource.Play();
            screenText.text = "Ready To Start";
            gaugeNeedle.Rotate(30.0f, 0f, 0f);
        }
        else
        {
            audioSource.Stop();
            screenText.text = "";
            gaugeNeedle.Rotate(0.0f, 0.0f, 0.0f);
        }
    }


    public void ControlRelease()
    {
        if (!_isRunning) return;

        screenText.text = "Ready To Start";
    }


    public void Extend()
    {
        if (!_isRunning) return;

        screenText.text = "Extending";

    }


    public void Retract()
    {
        if (!_isRunning) return;

        screenText.text = "Retracting";

    }


    public void Raise()
    {
        if (!_isRunning) return;

        screenText.text = "Raising";

    }


    public void Lower()
    {
        if (!_isRunning) return;

        screenText.text = "Ready To Start";

    }
}
