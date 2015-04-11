using Gadgeteer.SocketInterfaces;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

namespace JoeTheGadgeteer
{
    public class Human
    {
        private readonly DigitalOutput _heartPin;
        private readonly GT.Timer _heartTimer;

        public MovingBodyPart LeftArm { get; private set; }
        public MovingBodyPart RightArm { get; private set; }
        public MovingBodyPart LeftLeg { get; private set; }
        public MovingBodyPart RightLeg { get; private set; }
        public MovingBodyPart Head { get; private set; }

        public Human(DigitalOutput heart, PwmOutput leftArm, PwmOutput rightArm,
                     PwmOutput leftLeg, PwmOutput rightLeg, PwmOutput head)
        {
            _heartPin = heart;

            // Initialize the body parts with their calibrated min, max, & rest positions.
            LeftArm = new MovingBodyPart(leftArm, 3, 115, 115);
            RightArm = new MovingBodyPart(rightArm, 2, 145, 2);
            LeftLeg = new MovingBodyPart(leftLeg, 130, 180, 180);
            RightLeg = new MovingBodyPart(rightLeg, 2, 60, 2);
            Head = new MovingBodyPart(head, 20, 160, 90);

            // Start the heart beating.
            var heartState = false;
            _heartTimer = new GT.Timer(200);
            _heartTimer.Tick += t =>
            {
                heartState = !heartState;
                _heartPin.Write(heartState);
            };
            _heartTimer.Start();
        }

        /// <summary>
        /// Moves all body parts to their rest position.
        /// </summary>
        public void StandAtAttention(bool keepHeadPosition = false)
        {
            if (!keepHeadPosition) Head.Move(Head.RestPosition);
            LeftArm.Move(LeftArm.RestPosition);
            RightArm.Move(RightArm.RestPosition);
            LeftLeg.Move(LeftLeg.RestPosition);
            RightLeg.Move(RightLeg.RestPosition);
        }
    }
}
