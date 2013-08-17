using Microsoft.SPOT;
using GT = Gadgeteer;
using GTI = Gadgeteer.Interfaces;

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
            _joe.RightArm.StartExercising();
            _joe.LeftArm.StartExercising();
            _joe.RightLeg.StartExercising();
            _joe.LeftLeg.StartExercising();
            _joe.Head.StartExercising();
        }
    }
}
