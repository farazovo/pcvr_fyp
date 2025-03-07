using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using ViveSR.anipal.Eye;

public class ViveSREyeTrackingService : IEyeTrackingService
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

    public static Vector3 tobiiDir;
    public static Vector3 sRanipalDir;
    public static EyeTrackingData eyeData;

    private static bool isBlinking = false;
    private static float current_blinkDuration = 0.0f;
    private static float current_IBI = 0f;
    private static bool registered = false;
    private static DateTime lastTime;
    private static bool lastTimeSet;

    // Required for IL2CPP
    internal class MonoPInvokeCallbackAttribute : System.Attribute
    {
        public MonoPInvokeCallbackAttribute() { }
    }
    [MonoPInvokeCallback]
    private static void EyeCallback(ref EyeData SranipalEyeTrackingData)
    {
        DateTime curTime = DateTime.Now;
        if (!lastTimeSet)
        {
            lastTime = DateTime.Now;
            lastTimeSet = true;
        }

        TimeSpan deltaSpan = curTime.Subtract(lastTime);
        float deltaTime = deltaSpan.Seconds;

        Vector3 SranipalGazeOriginCombinedLocal, SranipalGazeDirectionCombinedLocal;

        if (SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out SranipalGazeOriginCombinedLocal, out SranipalGazeDirectionCombinedLocal, SranipalEyeTrackingData)) { }
        else if (SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out SranipalGazeOriginCombinedLocal, out SranipalGazeDirectionCombinedLocal, SranipalEyeTrackingData)) { }
        else if (SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out SranipalGazeOriginCombinedLocal, out SranipalGazeDirectionCombinedLocal, SranipalEyeTrackingData)) { }

        if (eyeData.eyeClosedLeft && eyeData.eyeClosedRight)  // FIXME should this be &&?
        {
            if (isBlinking)
            {
                current_blinkDuration += deltaTime;
            }
            else
            {
                isBlinking = true;
                current_blinkDuration = deltaTime / 2;
                current_IBI += deltaTime / 2;
                eyeData.current_interBlinkInterval = current_IBI;
            }
        }
        else
        {
            if (isBlinking)
            {
                isBlinking = false;
                //store blink duration.
                if (current_blinkDuration <= 0.7f)
                {
                    current_blinkDuration += deltaTime / 2;
                    eyeData.current_blinkDuration = current_blinkDuration;
                    current_IBI = deltaTime / 2;
                }
                else
                {
                    //handle invalid blink
                    current_IBI = deltaTime + current_blinkDuration;
                }
            }
            else
                current_IBI += deltaTime;
        }

        eyeData.EyeGazeDirLocal = SranipalGazeDirectionCombinedLocal;
        eyeData.EyeGazePosLocal = SranipalGazeOriginCombinedLocal;

        Vector3 GazeOrigin, GazeDirection;
        SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out GazeOrigin, out GazeDirection, SranipalEyeTrackingData);
        eyeData.LeftEyeGazeDirLocal = GazeDirection; eyeData.LeftEyeGazePosLocal = GazeOrigin;
        SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out GazeOrigin, out GazeDirection, SranipalEyeTrackingData);
        eyeData.RightEyeGazeDirLocal = GazeDirection; eyeData.RightEyeGazePosLocal = GazeOrigin;

        if (SranipalEyeTrackingData.verbose_data.left.pupil_diameter_mm > 0)
            eyeData.pupilDilationLeft = SranipalEyeTrackingData.verbose_data.left.pupil_diameter_mm;
        else
            eyeData.pupilDilationLeft = -1.0f;

        if (SranipalEyeTrackingData.verbose_data.right.pupil_diameter_mm > 0)
            eyeData.pupilDilationRight = SranipalEyeTrackingData.verbose_data.right.pupil_diameter_mm;
        else
            eyeData.pupilDilationRight = -1.0f;

        //tobiiDir = TobiiEyeTrackingDataLocal.GazeRay.Direction;
        sRanipalDir = SranipalGazeDirectionCombinedLocal;

        lastTime = curTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
            SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

        if (Input.GetKeyDown(KeyCode.E))
            SRanipal_Eye.LaunchEyeCalibration();

        if (!registered)
        {
            SRanipal_Eye.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate<SRanipal_Eye.CallbackBasic>(EyeCallback));
            registered = true;
        }

        latestEyeTrackingData = eyeData;
    }
}
