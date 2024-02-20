using System;
using System.Device.Gpio;
using System.IO.Ports;
using Weather.Services.Interfaces;

namespace Weather.Services
{
    public class BLEService : ISingletonService
    {
        private readonly SerialPort blePort;
        private readonly GpioPin bleState;

        public event EventHandler<bool> ConnectionStateChanged;

        private bool _enable;
        public bool Enable
        {
            get => _enable;
            set
            {
                if (_enable != value)
                {
                    _enable = value;
                    if (value)
                    {
                        bleState.ValueChanged += BleState_ValueChanged;
                        blePort.Open();
                    }
                    else
                    {
                        bleState.ValueChanged -= BleState_ValueChanged;
                        blePort.Close();
                    }
                }
            }
        }

        public bool IsConnected { get => Enable && bleState.Read() == PinValue.High; }

        public BLEService(Device device)
        {
            blePort = device.blePort;
            bleState = device.bleState;
            bleState.ValueChanged += BleState_ValueChanged;
            Enable = true;
            blePort.Open();
        }

        private void BleState_ValueChanged(object sender, PinValueChangedEventArgs e)
        {
            if (Enable)
            {
                ConnectionStateChanged.Invoke(this, e.ChangeType == PinEventTypes.Rising);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private void checkEnable()
        {
            if (!Enable || !blePort.IsOpen)
            {
                throw new InvalidOperationException("BLE服务未启用或串口未打开");
            }
            if (!IsConnected)
            {
                throw new InvalidOperationException("蓝牙未连接");
            }
        }

        public void WriteLine(string message)
        {
            checkEnable();
            blePort.WriteLine(message);
        }
    }
}
