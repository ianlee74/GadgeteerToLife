using System.Threading;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using GTI = Gadgeteer.SocketInterfaces;

namespace JoeTheGadgeteer
{
    public partial class Program
    {
        private GTI.DigitalOutput _heartPin;

        void ProgramStarted()
        {
            Debug.Print("Program Started");

            _heartPin = upperServos.CreateDigitalOutput(GT.Socket.Pin.Three, false);

            // Start the heart beating.
            var heartState = false;
            var heartTimer = new GT.Timer(200);
            heartTimer.Tick += (t) =>
            {
                heartState = !heartState;
                _heartPin.Write(heartState);
            };
            heartTimer.Start();
        }
    }
}
