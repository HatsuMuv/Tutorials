using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HatsuMuv.DynamixelForUnity.x64;
using HatsuMuv.DynamixelForUnity;

public class SingleMotorDemo : MonoBehaviour
{
    private DynamixelForUnity d4u;
    public byte id;
    public int goalPosition = 1024;

    // Start is called before the first frame update
    void Start()
    {
        d4u = GetComponent<DynamixelForUnity>();
        d4u.ConnectDynamixel();
        d4u.SetUp();

        id = d4u.IDsForUse[0];

        d4u.SetSignedData("OperatingMode", (int)OperatingMode.Position, id);

        d4u.SetSignedData("TorqueEnable", 1, id);
    }

    public void SetGoalPosition()
    {
        if (!d4u.SetSignedData("GoalPosition", goalPosition, id)) { Debug.LogError("fail"); };
    }

    public void GetPresentPosition()
    {
        int presentPosition = d4u.GetSignedData("PresentPosition", id);
        Debug.Log($"ID: {id}, PresentPosition: {presentPosition}");
    }

    private void OnApplicationQuit()
    {
        d4u.SetSignedData("TorqueEnable", 0, id);
        d4u.DisconnectDynamixel();
    }
}