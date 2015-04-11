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

        private int _tempPos;
        private int _humidityPos;

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
                _headServoPwm);
            _joe.StandAtAttention();

            temperatureHumidity.MeasurementComplete += temperatureHumidity_MeasurementComplete;

            DoWork();
        }

        private void DoWork()
        {
            temperatureHumidity.StartTakingMeasurements();
        }

        private void temperatureHumidity_MeasurementComplete(TempHumidity sender, TempHumidity.MeasurementCompleteEventArgs args)
        {
            Debug.Print("Temp = " + args.Temperature + "   Humidity = " + args.RelativeHumidity);

            // Show the temperature with the left arm.
            _tempPos = _joe.LeftArm.MaxPosition - (int) ((args.Temperature/100)*(_joe.LeftArm.MaxPosition - _joe.LeftArm.MinPosition));
            _joe.LeftArm.Move(_tempPos);

            // Show the relative humidity with the right arm.
            _humidityPos = _joe.RightArm.MinPosition + (int) ((args.RelativeHumidity/100)*(_joe.RightArm.MaxPosition - _joe.RightArm.MinPosition));
            _joe.RightArm.Move(_humidityPos);
        }
    }
}