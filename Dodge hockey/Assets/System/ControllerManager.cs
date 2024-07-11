using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ControllerManager : MonoBehaviour
{
    private Dictionary<int, InputDevice> connectedControllers = new Dictionary<int, InputDevice>();
    private int nextControllerId = 1;

    void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        print("onDeviceChange");
        switch (change)
        {
            case InputDeviceChange.Added:
                if (device is Gamepad)
                {
                    AssignUniqueId(device);
                }
                break;
            case InputDeviceChange.Removed:
                if (device is Gamepad)
                {
                    RemoveController(device);
                }
                break;
        }
    }

    private void AssignUniqueId(InputDevice device)
    {
        connectedControllers[nextControllerId] = device;
        Debug.Log($"ê⁄ë±Ç≥ÇÍÇΩÇÒÇ≤: {device.displayName}, Assigned ID: {nextControllerId}");
        nextControllerId++;
    }

    private void RemoveController(InputDevice device)
    {
        int idToRemove = -1;
        foreach (var kvp in connectedControllers)
        {
            if (kvp.Value == device)
            {
                idToRemove = kvp.Key;
                break;
            }
        }
        if (idToRemove != -1)
        {
            connectedControllers.Remove(idToRemove);
            Debug.Log($"êÿífÇ≥ÇÍÇΩÇÒÇ≤: {device.displayName}, Removed ID: {idToRemove}");
        }
    }
}
