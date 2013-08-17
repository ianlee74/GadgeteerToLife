using System.Threading;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using GTI = Gadgeteer.Interfaces;

namespace JoeTheGadgeteer
{
    public partial class Program
    {
        private GTI.DigitalOutput _heartPin;
        private GTI.PWMOutput _rightArmServoPwm;
        private MovingBodyPart _rightArm;

        void ProgramStarted()
        {
            Debug.Print("Program Started");

            // Blink Joe's LED heart.
            _heartPin = upperServos.SetupDigitalOutput(GT.Socket.Pin.Three, false);
            var heartTimer = new GT.Timer(1000);
            heartTimer.Tick += t =>
            {
                _heartPin.Write(true);
                Thread.Sleep(200);
                _heartPin.Write(false);
            };
            heartTimer.Start();

            // Wave Joe's right arm.
            _rightArmServoPwm = upperServos.SetupPWMOutput(GT.Socket.Pin.Nine);
            _rightArm = new MovingBodyPart(_rightArmServoPwm, 0, 145, 0); 
            _rightArm.StartExercising();
        }
    }
}
