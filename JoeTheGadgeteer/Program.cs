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

        void ProgramStarted()
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
            _joe = new Human(_heartPin, _leftArmServoPwm, _rightArmServoPwm, _leftLegServoPwm, _rightLegServoPwm, _headServoPwm);
            _joe.StandAtAttention();
            _joe.RightArm.StartExercising();
            _joe.LeftArm.StartExercising();
            _joe.RightLeg.StartExercising();
            _joe.LeftLeg.StartExercising();
            _joe.Head.StartExercising();
        }
    }
}
