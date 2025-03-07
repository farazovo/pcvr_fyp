using ShimmerAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class ShimmerGSRService : IGSRService
{
    // New shimmer device.
    private int enabledSensors;
    private ShimmerLogAndStreamSystemSerialPort ShimmerDevice;


    [HideInInspector]
    public double gsrConductance;
    [HideInInspector]
    public double resConductance;

    private double SamplingRate = 128;

    private bool gsrSet = false;
    private int IndexGSR;
    private int IndexRes;
    private SensorData dataGSR;
    private SensorData dataResistance;

    public bool bruteForceConnection = false;

    public override string DeviceName()
    {
        return "Shimmer GSR Device";
    }

    // Start is called before the first frame update
    void Start()
    {
        // Enable GSR sensor.
        enabledSensors = ((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_A_ACCEL | (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_GSR | (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_INT_A13);

        // Add shimmer device with empty COM port.
        ShimmerDevice = new ShimmerLogAndStreamSystemSerialPort("Shimmer", "", SamplingRate, 0, ShimmerBluetooth.GSR_RANGE_AUTO, enabledSensors, false, false, false, 1, 0, Shimmer3Configuration.EXG_EMG_CONFIGURATION_CHIP1, Shimmer3Configuration.EXG_EMG_CONFIGURATION_CHIP2, true);
        ShimmerDevice.UICallback += this.HandleEvent;

        // Disconnect any shimmer device to reduce errors.
        ShimmerDevice.Disconnect();

        // cycle through ports available until connection is established.
        foreach (string port in SerialPort.GetPortNames())
        {
            ShimmerDevice.SetShimmerAddress(port);

            bool connect = true; // check to connect one at a time

            if (ShimmerDevice.GetState() != ShimmerBluetooth.SHIMMER_STATE_CONNECTED)
            {
                if (connect)
                {
                    ShimmerDevice.StartConnectThread();
                    //ShimmerDevice.Connect();
                    connect = false;
                }
            }
        }
    }

    void Update()
    {

        // Set debug display to value.
        if (IsStreaming)
        {
            latestGSRData.gsrConductance = gsrConductance;
            latestGSRData.gsrResistance = resConductance;
        }

        if (bruteForceConnection)
        {
            bruteForceConnection = false;
            foreach (string port in SerialPort.GetPortNames())
            {
                ShimmerDevice.SetShimmerAddress(port);

                bool connect = true; // check to connect one at a time

                if (ShimmerDevice.GetState() != ShimmerBluetooth.SHIMMER_STATE_CONNECTED)
                {
                    if (connect)
                    {
                        ShimmerDevice.StartConnectThread();
                        connect = false;
                    }
                }
            }
        }
    }

    public void HandleEvent(object sender, EventArgs args)
    {
        CustomEventArgs eventArgs = (CustomEventArgs)args;
        int indicator = eventArgs.getIndicator();
        // Deals with what has been received.
        switch (indicator)
        {
            // If state change.
            case (int)ShimmerBluetooth.ShimmerIdentifier.MSG_IDENTIFIER_STATE_CHANGE:
                int state = (int)eventArgs.getObject();

                if (state == (int)ShimmerBluetooth.SHIMMER_STATE_CONNECTED)
                {
                    IsConnected = true;
                    Debug.Log("[ShimmerService]: Connected");
                    ShimmerDevice.StartStreaming();
                }
                else if (state == (int)ShimmerBluetooth.SHIMMER_STATE_CONNECTING)
                {
                    Debug.Log("[ShimmerService]: Establishing connection");
                }
                else if (state == (int)ShimmerBluetooth.SHIMMER_STATE_NONE)
                {
                    IsConnected = false;
                    Debug.Log($"[ShimmerService]: Disconnected from: {ShimmerDevice.GetShimmerAddress()}");
                }
                else if (state == (int)ShimmerBluetooth.SHIMMER_STATE_STREAMING)
                {
                    IsStreaming = true;
                    Debug.Log("[ShimmerService]: Streaming");
                }
                break;

            case (int)ShimmerBluetooth.ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE:
                print("[ShimmerService]: Notification: " + eventArgs.getObject());
                break;
            // If received data.
            case (int)ShimmerBluetooth.ShimmerIdentifier.MSG_IDENTIFIER_DATA_PACKET:
                // this is essential to ensure the object is not a reference
                ObjectCluster objectCluster = new ObjectCluster((ObjectCluster)eventArgs.getObject());
                List<string> names = objectCluster.GetNames();
                List<string> formats = objectCluster.GetFormats();
                List<string> units = objectCluster.GetUnits();
                List<double> data = objectCluster.GetData();

                // Get GSR Conductance rate and store it.
                if (!gsrSet)
                {
                    IndexGSR = objectCluster.GetIndex(Shimmer3Configuration.SignalNames.GSR_CONDUCTANCE, ShimmerConfiguration.SignalFormats.CAL);
                    IndexRes = objectCluster.GetIndex(Shimmer3Configuration.SignalNames.GSR, ShimmerConfiguration.SignalFormats.CAL);
                    gsrSet = true;
                }

                dataGSR = objectCluster.GetData(IndexGSR);
                dataResistance = objectCluster.GetData(IndexRes);
                gsrConductance = dataGSR.Data;
                resConductance = dataResistance.Data;

                break;

            default:
                break;
        }
    }

    // On quit, stop streaming and disconnect shimmer from COM port.
    private void OnApplicationQuit()
    {
        ShimmerDevice.StopStreaming();
        ShimmerDevice.Disconnect();
    }
}
