using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace HatsuMuv.DynamixelForUnity.Example
{
    public class D4UExampleController : MonoBehaviour
    {
#if UNITY_64
        public x64.DynamixelForUnity d4u;
#else
        public x86.DynamixelForUnity d4u;
#endif
        public List<DynamixelMotor> motors;

        public byte[] ids;

        private CancellationTokenSource cancellationTokenSource;
        private Task metricsLoop;

        public bool Ready { get { return d4u.ConnectStatus; } }

        public bool NeedWait { get; private set; }


        private async void Start()
        {
            NeedWait = true;


            if (d4u == null)
            {
                Debug.LogError("[DynamixelForUnitySample] DynamixelForUnity is not assigned!");
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();

            var connectResult = await Task.Run(() => InitializeDynamixel(cancellationTokenSource.Token));
            if (connectResult) StartMotorMetrics();

            NeedWait = false;
        }

        public async Task RefleshData()
        {
            NeedWait = true;

            await Task.Run(() => StopMotorMetrics());

            cancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();

            var connectResult = await Task.Run(() => InitializeDynamixel(cancellationTokenSource.Token));
            if (connectResult) StartMotorMetrics();

            NeedWait = false;
        }

        private async Task<bool> InitializeDynamixel(CancellationToken ct)
        {
            if (d4u == null)
            {
                Debug.LogError("[DynamixelForUnitySample] DynamixelForUnity is not assigned!");
                return false;
            }

            if (!d4u.ConnectStatus)
            {
                var connectResult = d4u.ConnectDynamixel();
                if (!connectResult)
                {
                    Debug.LogError("[DynamixelForUnitySample] Dynamixel is not connected.Connect it first.");
                    return false;
                }

                await d4u.SetUpAsync(cancellationToken: ct);
            }

            motors = new List<DynamixelMotor>();
            ids = d4u.IDsForUse;

            if (ids.Length == 0) return false;

            await GetAllMotorProperty(ct);
            return true;
        }

        private async Task GetAllMotorProperty(CancellationToken ct)
        {
            OperatingMode[] operatingModes =
                await Task.Run(() =>
                    d4u.SyncGetDataGroupMultiModels("OperatingMode")
                        .Select(d => (OperatingMode)d)
                        .ToArray(),
                    ct);

            float[] homingOffsets = await Task.Run(() => d4u.SyncGetSignedDataGroupMultiModels("HomingOffset").ToActualNumber(UNIT.POSITION).ToFloatArray(), ct);

            bool[] torqueEnable = await Task.Run(() => d4u.SyncGetDataGroupMultiModels("TorqueEnable").ToBoolArray(), ct);
            bool[] ledEnable = await Task.Run(() => d4u.SyncGetDataGroupMultiModels("LED").ToBoolArray(), ct);
            float[] goalPositions = await Task.Run(() => d4u.SyncGetSignedDataGroupMultiModels("GoalPosition").ToActualNumber(UNIT.POSITION).ToFloatArray(), ct);
            float[] goalVelocities = await Task.Run(() => d4u.SyncGetSignedDataGroupMultiModels("GoalVelocity").ToActualNumber(UNIT.VELOCITY).ToFloatArray(), ct);
            float[] goalCurrents = await Task.Run(() => d4u.SyncGetSignedDataGroupMultiModels("GoalCurrent").ToActualNumber(UNIT.CURRENT).ToFloatArray(), ct);
            float[] goalPWMs = await Task.Run(() => d4u.SyncGetSignedDataGroupMultiModels("GoalPWM").ToActualNumber(UNIT.PWM).ToFloatArray(), ct);
            byte[] movingStatuses = await Task.Run(() => d4u.SyncGetDataGroupMultiModels("MovingStatus").ToByteArray(), ct);
            float[] presentPositions = await Task.Run(() => d4u.SyncGetSignedDataGroupMultiModels("PresentPosition").ToActualNumber(UNIT.POSITION).ToFloatArray(), ct);
            float[] presentVelocities = await Task.Run(() => d4u.SyncGetSignedDataGroupMultiModels("PresentVelocity").ToActualNumber(UNIT.VELOCITY).ToFloatArray(), ct);
            float[] presentCurrents = await Task.Run(() => d4u.SyncGetSignedDataGroupMultiModels("PresentCurrent").ToActualNumber(UNIT.CURRENT).ToFloatArray(), ct);
            float[] presentPWMs = await Task.Run(() => d4u.SyncGetSignedDataGroupMultiModels("PresentPWM").ToActualNumber(UNIT.PWM).ToFloatArray(), ct);
            byte[] presentTemperatures = await Task.Run(() => d4u.SyncGetDataGroupMultiModels("PresentTemperature").ToByteArray(), ct);

            for (int i = 0; i < ids.Length; i++)
            {
                DynamixelMotor m;
                if (motors.Count < i + 1)
                {
                    m = new DynamixelMotor(ids[i]);
                    motors.Add(m);
                }
                else
                {
                    m = motors[i];
                }
                m.OperatingMode = operatingModes[i];
                m.HomingOffset = homingOffsets[i];

                m.TorqueEnable = torqueEnable[i];
                m.LED = ledEnable[i];
                m.GoalPosition = goalPositions[i];
                m.GoalVelocity = goalVelocities[i];
                m.GoalCurrent = goalCurrents[i];
                m.GoalPWM = goalPWMs[i];
                m.MovingStatus = movingStatuses[i];
                m.PresentPosition = presentPositions[i];
                m.PresentVelocity = presentVelocities[i];
                m.PresentCurrent = presentCurrents[i];
                m.PresentPWM = presentPWMs[i];
                m.PresentTemperature = presentTemperatures[i];
            }
        }

        private async Task MotorMetrics(CancellationToken ct)
        {
            if (!Ready) return;

            byte[] movingStatuses = await Task.Run(() => d4u.SyncGetDataGroupMultiModels("MovingStatus").ToByteArray(), ct);

            float[] presentPositions = await Task.Run(() => d4u.SyncGetSignedDataGroupMultiModels("PresentPosition").ToActualNumber(UNIT.POSITION).ToFloatArray(), ct);

            float[] presentVelocities = await Task.Run(() => d4u.SyncGetSignedDataGroupMultiModels("PresentVelocity").ToActualNumber(UNIT.VELOCITY).ToFloatArray(), ct);

            float[] presentCurrents = await Task.Run(() => d4u.SyncGetSignedDataGroupMultiModels("PresentCurrent").ToActualNumber(UNIT.CURRENT).ToFloatArray(), ct);

            float[] presentPWMs = await Task.Run(() => d4u.SyncGetSignedDataGroupMultiModels("PresentPWM").ToActualNumber(UNIT.PWM).ToFloatArray(), ct);

            byte[] presentTemperatures = await Task.Run(() => d4u.SyncGetDataGroupMultiModels("PresentTemperature").ToByteArray(), ct);

            for (int i = 0; i < ids.Length; i++)
            {
                DynamixelMotor m;
                if (motors.Count < i + 1)
                {
                    m = new DynamixelMotor(ids[i]);
                    motors.Add(m);
                }
                else
                {
                    m = motors[i];
                }
                m.MovingStatus = movingStatuses[i];
                m.PresentPosition = presentPositions[i];
                m.PresentVelocity = presentVelocities[i];
                m.PresentCurrent = presentCurrents[i];
                m.PresentPWM = presentPWMs[i];
                m.PresentTemperature = presentTemperatures[i];
            }
        }

        private void StartMotorMetrics()
        {
            if (!d4u.ConnectStatus)
                return;

            cancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            metricsLoop = MotorMetricsLoop(cancellationToken);
        }

        private async Task MotorMetricsLoop(CancellationToken ct)
        {
            while (true)
            {
                if (ct.IsCancellationRequested)
                    return;

                Debug.Log("[SampleController] Metrics");
                try
                {
                    await MotorMetrics(ct);
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("[SampleController] Metrics canceled");
                    return;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return;
                }
            }
        }

        private async Task StopMotorMetrics()
        {
            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;

                if(metricsLoop != null)
                    await metricsLoop;
            }
        }

        private async void OnApplicationQuit()
        {
            await StopMotorMetrics();
        }

        public async Task<bool> SetID(uint data, byte id)
        {
            if (!Ready) return false;
            await StopMotorMetrics();

            var result = d4u.SetData("ID", data, id);
            if (result)
            {
                motors.Where(m => m.ID == id).First().ID = (byte)data;
                d4u.SetIDsForUse(motors.Select(m => m.ID).ToArray());
            }
            StartMotorMetrics();
            return result;
        }
        public async Task<bool> SetOperatingMode(uint data, byte id)
        {
            if (!Ready) return false;
            await StopMotorMetrics();

            var result = d4u.SetData("OperatingMode", data, id);
            if (result)
            {
                motors.Where(m => m.ID == id).First().OperatingMode = (OperatingMode)data;
            }
            StartMotorMetrics();
            return result;
        }
        public async Task<bool> SetHomingOffset(int data, byte id)
        {
            if (!Ready) return false;
            await StopMotorMetrics();

            var result = d4u.SetSignedData("HomingOffset", data, id);
            if (result)
                motors.Where(m => m.ID == id).First().HomingOffset = (int)data * (float)UNIT.POSITION;
            StartMotorMetrics();
            return result;
        }
        public async Task<bool> SetTorque(uint data, byte id)
        {
            if (!Ready) return false;
            await StopMotorMetrics();

            var result = d4u.SetData("TorqueEnable", data, id);
            if (result)
                motors.Where(m => m.ID == id).First().TorqueEnable = data == 1;
            StartMotorMetrics();
            return result;
        }
        public async Task<bool> SetLED(uint data, byte id)
        {
            if (!Ready) return false;
            await StopMotorMetrics();

            var result = d4u.SetData("LED", data, id);
            if (result)
                motors.Where(m => m.ID == id).First().LED = data == 1;
            StartMotorMetrics();
            return result;
        }
        public async Task<bool> SetGoalPosition(int data, byte id)
        {
            if (!Ready) return false;
            await StopMotorMetrics();

            var result = d4u.SetSignedData("GoalPosition", data, id);
            if (result)
                motors.Where(m => m.ID == id).First().GoalPosition = (int)data * (float)UNIT.POSITION;
            StartMotorMetrics();
            return result;
        }
        public async Task<bool> SetGoalVelocity(int data, byte id)
        {
            if (!Ready) return false;
            await StopMotorMetrics();

            var result = d4u.SetSignedData("GoalVelocity", data, id);
            if (result)
                motors.Where(m => m.ID == id).First().GoalVelocity = (int)data * (float)UNIT.VELOCITY;
            StartMotorMetrics();
            return result;
        }
        public async Task<bool> SetGoalCurrent(int data, byte id)
        {
            if (!Ready) return false;
            await StopMotorMetrics();

            var result = d4u.SetSignedData("GoalCurrent", data, id);
            if (result)
                motors.Where(m => m.ID == id).First().GoalCurrent = (int)data * (float)UNIT.CURRENT;
            StartMotorMetrics();
            return result;
        }
        public async Task<bool> SetGoalPWM(int data, byte id)
        {
            if (!Ready) return false;
            await StopMotorMetrics();

            var result = d4u.SetSignedData("GoalPWM", data, id);
            if (result)
                motors.Where(m => m.ID == id).First().GoalPWM = (int)data * (float)UNIT.PWM;
            StartMotorMetrics();
            return result;
        }
    }

    [Serializable]
    public class DynamixelMotor
    {
        public byte ID = 1;
        public OperatingMode OperatingMode = OperatingMode.Position;
        public float HomingOffset = 0;

        public bool TorqueEnable = false;
        public bool LED = false;
        public float GoalPosition = 0;
        public float GoalVelocity = 0;
        public float GoalCurrent = 0;
        public float GoalPWM = 0;
        public byte MovingStatus = 0b0000_0000;
        public float PresentPosition = 0;
        public float PresentVelocity = 0;
        public float PresentCurrent = 0;
        public float PresentPWM = 0;
        public byte PresentTemperature;

        public DynamixelMotor(byte id)
        {
            this.ID = id;
        }
    }
}