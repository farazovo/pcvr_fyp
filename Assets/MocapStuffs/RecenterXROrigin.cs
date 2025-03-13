using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class RecenterXROrigin : MonoBehaviour
{
    public Transform target;
    private XROrigin xrOrigin;

    void Start()
    {
        // Assuming this script is attached to the same GameObject as the XROrigin component
        xrOrigin = GetComponent<XROrigin>();
    }


    void Update()
    {
        // Check for input to trigger recentering
        if (Input.GetKeyDown(KeyCode.R))
        {
            Recenter();
        }
    }

    void Recenter()
    {
        var xrSettings = XRGeneralSettings.Instance;
        if (xrSettings == null)
        {
            Debug.Log($"XRGeneralSettings is null.");
            return;
        }

        var xrManager = xrSettings.Manager;
        if (xrManager == null)
        {
            Debug.Log($"XRManagerSettings is null.");
            return;
        }
        var xrLoader = xrManager.activeLoader;
        if (xrLoader == null)
        {
            Debug.Log($"XRLoader is null.");
            return;
        }
        var xrInput = xrLoader.GetLoadedSubsystem<XRInputSubsystem>();
        if (xrInput != null)
        {
            if (xrInput.TryRecenter()) Debug.Log("Recentered");
            else Debug.LogError("Recenter Failed!");
        }

        /*
        xrOrigin.MoveCameraToWorldLocation(target.position);

        // Match the origin's up and forward direction to the target object's up and forward
        xrOrigin.MatchOriginUpCameraForward(target.up, -target.right);*/
    }
}
