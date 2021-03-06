﻿using System.Threading;
using Microsoft.SPOT;
using GT = Gadgeteer;
using Gadgeteer.SocketInterfaces;
using Gadgeteer.Modules.GHIElectronics;

namespace JoeTheGadgeteer
{
    public partial class Program
    {
        private DigitalOutput _heartPin;
        private PwmOutput _headServoPwm;
        private PwmOutput _rightArmServoPwm;
        private PwmOutput _leftArmServoPwm;
        private PwmOutput _rightLegServoPwm;
        private PwmOutput _leftLegServoPwm;
        private Human _joe;
        private bool _isSlackingOff;

        private int _tempPos;
        private int _humidityPos;
        private bool _isTakingTHMeasurements;

        private GT.Timer _colorSensorTimer;

        private void ProgramStarted()
        {
            Debug.Print("Program Started");

            // Initialize the hardware pins.
            _heartPin = upperServos.CreateDigitalOutput(GT.Socket.Pin.Three, false);
            _headServoPwm = upperServos.CreatePwmOutput(GT.Socket.Pin.Eight);
            _rightArmServoPwm = upperServos.CreatePwmOutput(GT.Socket.Pin.Nine);
            _leftArmServoPwm = upperServos.CreatePwmOutput(GT.Socket.Pin.Seven);
            _rightLegServoPwm = lowerServos.CreatePwmOutput(GT.Socket.Pin.Eight);
            _leftLegServoPwm = lowerServos.CreatePwmOutput(GT.Socket.Pin.Seven);

            // Create Joe and make him exercise all his body parts.
            _joe = new Human(_heartPin, _leftArmServoPwm, _rightArmServoPwm, _leftLegServoPwm, _rightLegServoPwm,
                _headServoPwm, distanceSensor);
            _joe.StandAtAttention();

            temperatureHumidity.MeasurementComplete += temperatureHumidity_MeasurementComplete;

            StartScanningForBoss();
        }

        private void DoWork()
        {
            _isSlackingOff = false;
            MonitorTemperatureAndHumidity();
            MonitorColor();
        }

        private void MonitorTemperatureAndHumidity()
        {
            temperatureHumidity.StartTakingMeasurements();
            _isTakingTHMeasurements = true;
        }

        private void StopMonitoringTemperatureAndHumidty()
        {
            if (!_isTakingTHMeasurements) return;
            temperatureHumidity.StopTakingMeasurements();
            _isTakingTHMeasurements = false;
        }

        private void temperatureHumidity_MeasurementComplete(TempHumidity sender, TempHumidity.MeasurementCompleteEventArgs args)
        {
            if (_isSlackingOff) return;

            Debug.Print("Temp = " + args.Temperature + "   Humidity = " + args.RelativeHumidity);

            // Show the temperature with the left arm.
            _tempPos = _joe.LeftArm.MaxPosition - (int) ((args.Temperature/100)*(_joe.LeftArm.MaxPosition - _joe.LeftArm.MinPosition));
            _joe.LeftArm.Move(_tempPos);

            // Show the relative humidity with the right arm.
            _humidityPos = _joe.RightArm.MinPosition + (int) ((args.RelativeHumidity/100)*(_joe.RightArm.MaxPosition - _joe.RightArm.MinPosition));
            _joe.RightArm.Move(_humidityPos);
        }

        private void MonitorColor()
        {
            ColorSense.ColorData color;

            if (_colorSensorTimer == null)
            {
                _colorSensorTimer = new GT.Timer(1000);
                _colorSensorTimer.Tick += t =>
                {
                    if (_isSlackingOff) return;

                    // Take a color reading.
                    colorSensor.LedEnabled = true;
                    Thread.Sleep(200);
                    color = colorSensor.ReadColor();
                    colorSensor.LedEnabled = false;
                    Debug.Print("R: " + color.Red + "  G: " + color.Green + "  B: " + color.Blue);

                    // Raise left leg if the color is (mostly) blue.
                    if (color.Green > color.Red && color.Green > color.Blue & color.Green > 50)
                    {
                        if (_joe.LeftLeg.CurrentPosition != _joe.LeftLeg.MinPosition)
                        {
                            _joe.LeftLeg.Move(_joe.LeftLeg.MinPosition);
                        }
                    }
                    else if (_joe.LeftLeg.CurrentPosition != _joe.LeftLeg.RestPosition)
                    {
                        _joe.LeftLeg.Move(_joe.LeftLeg.RestPosition);
                    }

                    // Raise right leg if the color is (mostly) red.
                    if (color.Red > color.Blue && color.Red > color.Green && color.Red > 50)
                    {
                        if (_joe.RightLeg.CurrentPosition != _joe.RightLeg.MaxPosition)
                        {
                            _joe.RightLeg.Move(_joe.RightLeg.MaxPosition);
                        }
                    }
                    else if (_joe.RightLeg.CurrentPosition != _joe.RightLeg.RestPosition)
                    {
                        _joe.RightLeg.Move(_joe.RightLeg.RestPosition);
                    }
                };
            }
            if (!_colorSensorTimer.IsRunning) _colorSensorTimer.Start();
        }

        /// Visually scan the area for your boss and do work when you see him.
        /// Otherwise, slack off.
        /// </summary>
        private void StartScanningForBoss()
        {
            _joe.VisualObjectLocated += (o, d) =>
            {
                Debug.Print("Visual object located (" + d + ")");
                DoWork();
            };
            _joe.VisualObjectLost += (o, d) =>
            {
                Debug.Print("Visual object lost (" + d + ")");
                SlackOff();
            };
            _joe.StartVisualObjectScan();
        }

        /// <summary>
        /// Don't do the work your boss asked you to do.
        /// </summary>
        void SlackOff()
        {
            _isSlackingOff = true;

            StopMonitoringTemperatureAndHumidty();

            if (_colorSensorTimer != null && _colorSensorTimer.IsRunning)
            {
                _colorSensorTimer.Stop();
            }
            Thread.Sleep(1000);
            _joe.StandAtAttention(true);
        }
    }
}