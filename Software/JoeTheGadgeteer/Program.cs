using System.Threading;
using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTI = Gadgeteer.Interfaces;
using GTM = Gadgeteer.Modules;

namespace JoeTheGadgeteer
{
    public partial class Program
    {
        private GTI.DigitalOutput _heartPin;
        private GTI.PWMOutput _headServoPwm;
        private GTI.PWMOutput _rightArmServoPwm;
        private GTI.PWMOutput _leftArmServoPwm;
        private GTI.PWMOutput _rightLegServoPwm;
        private GTI.PWMOutput _leftLegServoPwm;
        private Human _joe;

        private GT.Timer _tempHumidityTimer;
        private int _tempPos;
        private int _humidityPos;

        private GT.Timer _colorSensorTimer;

        void ProgramStarted()
        {
            Debug.Print("Program Started");

            // Initialize the hardware pins.
            _heartPin = upperServos.SetupDigitalOutput(GT.Socket.Pin.Three, false);
            _headServoPwm = upperServos.SetupPWMOutput(GT.Socket.Pin.Eight);
            _rightArmServoPwm = upperServos.SetupPWMOutput(GT.Socket.Pin.Nine);
            _leftArmServoPwm = upperServos.SetupPWMOutput(GT.Socket.Pin.Seven);
            _rightLegServoPwm = lowerServos.SetupPWMOutput(GT.Socket.Pin.Eight);
            _leftLegServoPwm = lowerServos.SetupPWMOutput(GT.Socket.Pin.Seven);

            // Create Joe and make him exercise all his body parts.
            _joe = new Human(_heartPin, _leftArmServoPwm, _rightArmServoPwm, _leftLegServoPwm, _rightLegServoPwm, _headServoPwm);
            _joe.StandAtAttention();

            temperatureHumidity.MeasurementComplete += temperatureHumidity_MeasurementComplete;

            DoWork();
        }

        void DoWork()
        {
            MonitorTemperatureAndHumidity();
            MonitorColor();
        }

        private void MonitorColor()
        {
            ColorSense.ColorChannels color;

            if (_colorSensorTimer == null)
            {
                _colorSensorTimer = new GT.Timer(1000);
                _colorSensorTimer.Tick += t =>
                {
                    // Take a color reading.
                    colorSensor.ToggleOnboardLED(true);
                    Thread.Sleep(200);
                    color = colorSensor.ReadColorChannels();
                    colorSensor.ToggleOnboardLED(false);
                    Debug.Print("R: " + color.Red + "  G: " + color.Green + "  B: " + color.Blue);

                    // Raise left leg if the color is (mostly) blue.
                    if (color.Blue > color.Red && color.Blue > color.Green & color.Blue > 50)
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

        /// <summary>
        /// Setup a timer that will continuously monitor the temperature & humidity.
        /// </summary>
        /// <param name="sampleFrequency">The frequency in milliseconds at which to make a measurement.</param>
        private void MonitorTemperatureAndHumidity(int sampleFrequency = 500)
        {
            if (_tempHumidityTimer == null)
            {
                _tempHumidityTimer = new GT.Timer(sampleFrequency);
                _tempHumidityTimer.Tick += t => temperatureHumidity.RequestMeasurement();
            }
            if (!_tempHumidityTimer.IsRunning) _tempHumidityTimer.Start();
        }

        void temperatureHumidity_MeasurementComplete(GTM.Seeed.TemperatureHumidity sender, double temperature, double relativeHumidity)
        {
            Debug.Print("Temp = " + temperature + "   Humidity = " + relativeHumidity);

            // Show the temperature with the left arm.
            _tempPos = _joe.LeftArm.MaxPosition - (int)((temperature / 100) * (_joe.LeftArm.MaxPosition - _joe.LeftArm.MinPosition));
            _joe.LeftArm.Move(_tempPos);

            // Show the relative humidity with the right arm.
            _humidityPos = _joe.RightArm.MinPosition + (int)((relativeHumidity / 100) * (_joe.RightArm.MaxPosition - _joe.RightArm.MinPosition));
            _joe.RightArm.Move(_humidityPos);
        }
    }
}
