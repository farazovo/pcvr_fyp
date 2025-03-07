using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using ViveSR.anipal.Lip;

public class ViveSRLipTrackingService : ILipTrackingService
{
    public override string DeviceName()
    {
        throw new System.NotImplementedException();
    }

    internal override string FileHeader()
    {
        throw new System.NotImplementedException();
    }

    internal override string GetData()
    {
        throw new System.NotImplementedException();
    }

    private static Thread lipThread;
    private static bool _runningGather;

    private static Dictionary<string, float> lipWeightings;
    private static LipData ld;
    private static ViveSR.Error LastUpdateResult;
    public const int WeightingCount = (int)LipShape.Max;

    // Start is called before the first frame update
    void Start()
    {
        lipWeightings = new Dictionary<string, float>();
        ld = new LipData();
    }

    private void StartThreads()
    {
        if (SRanipal_Lip_Framework.Status != SRanipal_Lip_Framework.FrameworkStatus.WORKING)
        {
            Debug.LogError("SRanipal Lip Framework is not working. Please check the SRanipal Manager.");
            return;
        }
        _runningGather = true;
        lipThread = new Thread(GatherLipData);
        lipThread.Start();
    }

    private void OnDisable()
    {
        _runningGather = false;
        if (lipThread != null)
            lipThread.Join();
        lipThread = null;
    }

    private static bool UpdateData()
    {
        LastUpdateResult = SRanipal_Lip_API.GetLipData(ref ld);
        if (LastUpdateResult == ViveSR.Error.WORK)
        {
            for (int i = 0; i < WeightingCount; ++i) lipWeightings[((LipShape)i).ToString()] = ld.prediction_data.blend_shape_weight[i];
        }
        return LastUpdateResult == ViveSR.Error.WORK;
    }

    private void GatherLipData()
    {
        lipWeightings = new Dictionary<string, float>();
        for (int i = 0; i < WeightingCount; ++i) lipWeightings.Add(((LipShape)i).ToString(), 0.0f);

        while (_runningGather)
        {
            if (UpdateData())
            {
                latestLipTrackingData.currLipWeightings = lipWeightings;
            }
            else
            {
                Debug.Log("Invalid Lip Data");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SRanipal_Lip_Framework.Status != SRanipal_Lip_Framework.FrameworkStatus.WORKING)
        {
            Debug.LogError("SRanipal Lip Framework is not working. Please check the SRanipal Manager.");
            return;
        }

        if (!_runningGather)
            StartThreads();
    }
}
