using Microsoft.MixedReality.Toolkit.UI;
using System;
using UnityEngine;

public class SYS_Switch: MonoBehaviour
{
    public string LockTag;
    public event Action<bool> OnSwitchClick;

    //[SerializeField] private ObjectManipulator manipulator;
    [SerializeField] private Interactable interactable;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    private bool _isLocked;

    public Action<bool> OnSwitchToggle;


    private void OnEnable()
    {
        //manipulator.OnManipulationStarted.AddListener(OnManipulationStarted);
        //manipulator.OnManipulationEnded.AddListener(OnManipulationEnded);
        //manipulator.enabled = false;

        interactable.OnClick.AddListener(OnSwitchClicked);
    }

    private void OnDisable()
    {
        interactable.OnClick.RemoveListener(OnSwitchClicked);

        //manipulator.OnManipulationStarted.RemoveListener(OnManipulationStarted);
        //manipulator.OnManipulationEnded.RemoveListener(OnManipulationEnded);
    }

    private void OnSwitchClicked()
    {
        if (_isLocked) return;

        OnSwitchClick?.Invoke(interactable.IsToggled);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(LockTag))
        {
            _isLocked = true;
            //manipulator.enabled = false;
            interactable.IsEnabled = false;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals(LockTag))
        {
            _isLocked = false;
            interactable.IsEnabled = true;
        }
    }


    public void ChangeSwitchState(bool toggle)
    {
        // Change Position
        transform.localRotation = (toggle) ? Quaternion.Euler(-65, -90, 0) : Quaternion.Euler(0, -90, 0);

        audioSource.PlayOneShot(audioClip);
    } 
}