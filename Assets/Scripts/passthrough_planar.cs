using System.Collections.Generic;
using UnityEngine;
using VIVE.OpenXR.CompositionLayer;
using VIVE.OpenXR.Passthrough;
using VIVE.OpenXR.Samples;
using XrPassthroughHTC = VIVE.OpenXR.Passthrough.XrPassthroughHTC;
public class passthrough_planar : MonoBehaviour
{
    private LayerType currentActiveLayerType = LayerType.Underlay;
    private XrPassthroughHTC activePassthroughID = 0;
    private float alpha = 1.0f;
    private bool passthroughActive
    {
        get
        {
            List<XrPassthroughHTC> currentLayerIDs = PassthroughAPI.GetCurrentPassthroughLayerIDs();
            if (currentLayerIDs != null && currentLayerIDs.Contains(activePassthroughID)) //Layer is active
            {
                //Debug.Log("passthroughActive: true");
                return true;
            }
            //Debug.Log("passthroughActive: false");
            return false;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateClassicUnderlayPassthrough();
    }

    public void CreateClassicUnderlayPassthrough()
    {
        CreateClassicPassthrough(currentActiveLayerType);
    }

    private void CreateClassicPassthrough(LayerType layerType)
    {
        if (passthroughActive) return;

        PassthroughAPI.CreatePlanarPassthrough(out activePassthroughID, layerType, PassthroughSessionDestroyed, alpha);
    }

    private void PassthroughSessionDestroyed(XrPassthroughHTC passthrough) //Handle destruction of passthrough layer when OpenXR session is destroyed
    {
        PassthroughAPI.DestroyPassthrough(passthrough);
        activePassthroughID = 0;
    }

}

