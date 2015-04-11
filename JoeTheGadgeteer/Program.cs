using Gadgeteer.SocketInterfaces;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

namespace JoeTheGadgeteer
{
    public partial class Program
    {
        private DigitalOutput _heartPin;
        private PwmOutput _rightArmServoPwm;
        private MovingBodyPart _rightArm;

        void ProgramStarted()
        {
            Debug.Print("Program Started");

            _heartPin = upperServos.CreateDigitalOutput(GT.Socket.Pin.Three, false);

            // Start the heart beating.
            var heartState = false;
            var heartTimer = new GT.Timer(200);
            heartTimer.Tick += t =>
            {
                heartState = !heartState;
                _heartPin.Write(heartState);
            };
            heartTimer.Start();

            // Wave Joe's right arm.
            _rightArmServoPwm = upperServos.CreatePwmOutput(GT.Socket.Pin.Nine);
            _rightArm = new MovingBodyPart(_rightArmServoPwm, 0, 145, 0);
            _rightArm.StartExercising();
        }
    }
}
