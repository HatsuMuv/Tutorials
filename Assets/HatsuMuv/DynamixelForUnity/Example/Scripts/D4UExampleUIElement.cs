using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HatsuMuv.DynamixelForUnity.Example
{
    public class D4UExampleUIElement : MonoBehaviour
    {
        [SerializeField] private InputField idUI;
        [SerializeField] private Dropdown operatingModeUI;
        [SerializeField] private InputField homingOffsetUI;
        [SerializeField] private Toggle torqueUI;
        [SerializeField] private Toggle ledUI;
        [SerializeField] private InputField goalPositionUI;
        [SerializeField] private InputField goalVelocityUI;
        [SerializeField] private InputField goalCurrentUI;
        [SerializeField] private InputField goalPWMUI;
        [SerializeField] private Toggle inPositionUI;
        [SerializeField] private Toggle profileOngoingUI;
        [SerializeField] private Toggle followingErrorUI;
        [SerializeField] private Dropdown velocityProfileUI;
        [SerializeField] private Text presentPositionUI;
        [SerializeField] private Text presentVelocityUI;
        [SerializeField] private Text presentCurrentUI;
        [SerializeField] private Text presentPWMUI;
        [SerializeField] private Text presentTemperatureUI;

        private D4UExampleController controller;
        public byte motorId;
        public DynamixelMotor motor;

        public void Initialize(D4UExampleController controller)
        {
            this.controller = controller;
            operatingModeUI.ClearOptions();
            operatingModeUI.AddOptions(Enum.GetNames(typeof(OperatingMode)).ToList());
            velocityProfileUI.ClearOptions();
            velocityProfileUI.AddOptions(new List<string>() { "Profile not used", "Rectangular Profile", "Triangular Profile", "Trapezoidal Profile" });
            velocityProfileUI.interactable = false;
        }

        public void UpdateParameters(DynamixelMotor motor)
        {
            motorId = motor.ID;
            this.motor = motor;

            if(!idUI.isFocused)
                idUI.text = motor.ID.ToString();

            operatingModeUI.value     = Enum.GetNames(typeof(OperatingMode)).ToList().IndexOf(Enum.GetName(typeof(OperatingMode), motor.OperatingMode));
            if(!homingOffsetUI.isFocused)
                homingOffsetUI.text = motor.HomingOffset.ToString();

            torqueUI.isOn             = motor.TorqueEnable;
            if (motor.TorqueEnable)
            {
                idUI.interactable = false;
                operatingModeUI.interactable = false;
                homingOffsetUI.interactable = false;
            }
            else
            {
                idUI.interactable = true;
                operatingModeUI.interactable = true;
                homingOffsetUI.interactable = true;
            }

            ledUI.isOn                = motor.LED;

            if(!goalPositionUI.isFocused)
                goalPositionUI.text = motor.GoalPosition.ToString();

            if (!goalVelocityUI.isFocused)
                goalVelocityUI.text = motor.GoalVelocity.ToString();

            if (!goalCurrentUI.isFocused)
                goalCurrentUI.text = motor.GoalCurrent.ToString();

            if (!goalPWMUI.isFocused)
                goalPWMUI.text = motor.GoalPWM.ToString();

            inPositionUI.isOn         = 0 != (motor.MovingStatus & 0b0000_0001);
            profileOngoingUI.isOn     = 0 != (motor.MovingStatus & 0b0000_0010);
            followingErrorUI.isOn     = 0 != (motor.MovingStatus & 0b0000_1000);
            velocityProfileUI.value   = (motor.MovingStatus & 0b0011_0000) >> 4;
            presentPositionUI.text    = motor.PresentPosition.ToString();
            presentVelocityUI.text    = motor.PresentVelocity.ToString();
            presentCurrentUI.text     = motor.PresentCurrent.ToString();
            presentPWMUI.text         = motor.PresentPWM.ToString();
            presentTemperatureUI.text = motor.PresentTemperature.ToString();
        }

        public void ChangeID(string s)
        {
            if (motor.ID == byte.Parse(s)) return;
            var result = controller.SetID(byte.Parse(s), motorId);
        }
        public async void ChangeOperatingMode(int index)
        {
            if (motor.OperatingMode == (OperatingMode)(Enum.GetValues(typeof(OperatingMode)).GetValue(index))) return;
            var result = await controller.SetOperatingMode((uint)(int)(Enum.GetValues(typeof(OperatingMode)).GetValue(index)), motorId);
        }
        public void ChangeHomingOffset(string s)
        {
            if (motor.HomingOffset == float.Parse(s) / (float)UNIT.POSITION) return;
            var _ = controller.SetHomingOffset((int)(float.Parse(s) / (float)UNIT.POSITION), motorId);
        }
        public void ChangeTorque(bool b)
        {
            if (motor.TorqueEnable == b) return;
            var _ = controller.SetTorque(b ? 1u : 0u, motorId);
        }
        public void ChangeLED(bool b)
        {
            if (motor.LED == b) return;
            var _ = controller.SetLED(b ? 1u : 0u, motorId);
        }
        public void ChangeGoalPosition(string s)
        {
            if (motor.GoalPosition == float.Parse(s) / (float)UNIT.POSITION) return;
            var _ = controller.SetGoalPosition((int)(float.Parse(s) / (float)UNIT.POSITION), motorId);
        }
        public void ChangeGoalVelocity(string s)
        {
            if (motor.GoalVelocity == float.Parse(s) / (float)UNIT.VELOCITY) return;
            var _ = controller.SetGoalVelocity((int)(float.Parse(s) / (float)UNIT.VELOCITY), motorId);
        }
        public void ChangeGoalCurrent(string s)
        {
            if (motor.GoalCurrent == float.Parse(s) / (float)UNIT.CURRENT) return;
            var _ = controller.SetGoalCurrent((int)(float.Parse(s) / (float)UNIT.CURRENT), motorId);
        }
        public void ChangeGoalPWM(string s)
        {
            if (motor.GoalPWM == float.Parse(s) / (float)UNIT.PWM) return;
            var _ = controller.SetGoalPWM((int)(float.Parse(s) / (float)UNIT.PWM), motorId);
        }
    }
}