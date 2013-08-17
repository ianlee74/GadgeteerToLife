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

        void ProgramStarted()
        {
            Debug.Print("Program Started");

            _heartPin = upperServos.SetupDigitalOutput(GT.Socket.Pin.Three, false);

            var heartTimer = new GT.Timer(1000);
            heartTimer.Tick += t =>
            {
                // Blink the LED (heart).
                _heartPin.Write(true);
                Thread.Sleep(200);
                _heartPin.Write(false);
            };
            heartTimer.Start();
        }
    }
}
