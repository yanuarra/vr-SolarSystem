using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ControllerButtonHandler : MonoBehaviour
{
    public UIDisplayHandler displayHandler;
    public InputAction inputAction;
    public InputActionReference togglePlanetinfoAction;
    public InputActionReference muteAction;
    public InputActionReference nextAction;
    public InputActionReference togglePlanetListUIAction;
    public UnityEvent nextEvent;

    private void Awake()
    {
        togglePlanetinfoAction.action.Enable();
        muteAction.action.Enable();
        nextAction.action.Enable();
        togglePlanetinfoAction.action.performed += TogglePlanetInfo;
        muteAction.action.performed += MuteAudio;
        nextAction.action.performed += NextAction;
        togglePlanetListUIAction.action.performed += ToggleHandMenu;
        InputSystem.onDeviceChange += OnDeviceChange;   
    }

    private void OnDestroy()
    {
        togglePlanetinfoAction.action.Disable();  
        muteAction.action.Disable();
        nextAction.action.Disable();
        togglePlanetinfoAction.action.performed -= TogglePlanetInfo;
        muteAction.action.performed -= MuteAudio;
        nextAction.action.performed -= NextAction;
        togglePlanetListUIAction.action.performed -= ToggleHandMenu;
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    void TogglePlanetInfo(InputAction.CallbackContext context)
    {
        displayHandler.TogglePlanetInfoOverlay();
    }

    void MuteAudio(InputAction.CallbackContext context)
    {
        AudioHandler.Instance.ToggleMuteAllAudio();
    }

    void NextAction(InputAction.CallbackContext context)
    {
        nextEvent.Invoke();
    }

    void ToggleHandMenu(InputAction.CallbackContext context)
    {
        displayHandler.TogglePlanetListOverlay();
    }

    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                break;
            case InputDeviceChange.Removed:
                break;
            case InputDeviceChange.Disconnected:
                togglePlanetinfoAction.action.Disable();
                muteAction.action.Disable();
                nextAction.action.Disable();
                togglePlanetListUIAction.action.Disable();
                togglePlanetinfoAction.action.performed -= TogglePlanetInfo;
                muteAction.action.performed -= MuteAudio;
                nextAction.action.performed -= NextAction; ;
                togglePlanetListUIAction.action.performed -= ToggleHandMenu;
                break;
            case InputDeviceChange.Reconnected:
                togglePlanetinfoAction.action.Enable();
                muteAction.action.Enable();
                nextAction.action.Enable();
                togglePlanetListUIAction.action.Enable();
                togglePlanetinfoAction.action.performed += TogglePlanetInfo;
                muteAction.action.performed += MuteAudio;
                nextAction.action.performed += NextAction; ;
                togglePlanetListUIAction.action.performed += ToggleHandMenu;
                break;
            case InputDeviceChange.Enabled:
                break;
            case InputDeviceChange.Disabled:
                break;
            case InputDeviceChange.UsageChanged:
                break;
            case InputDeviceChange.ConfigurationChanged:
                break;
            case InputDeviceChange.SoftReset:
                break;
            case InputDeviceChange.HardReset:
                break;
            default:
                break;
        }
    }
}
