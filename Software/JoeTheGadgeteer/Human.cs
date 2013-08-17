using System.Threading;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTI = Gadgeteer.Interfaces;
using GTM = Gadgeteer.Modules;

namespace JoeTheGadgeteer
{
    // A delegate type for hooking up visual scan notifications.
    public delegate void VisualObjectChangedEventHandler(object sender, int distance);

    public class Human
    {
        // Heart stuff.
        private readonly GTI.DigitalOutput _heartPin;
        private readonly GT.Timer _heartTimer;

        // Object sensor stuff.
        private readonly GT.Timer _visualObjectScanTimer;
        private readonly GTM.GHIElectronics.Distance_US3 _distanceSensor;

        public event VisualObjectChangedEventHandler VisualObjectLocated;
        public event VisualObjectChangedEventHandler VisualObjectLost;

        public MovingBodyPart LeftArm { get; private set; }
        public MovingBodyPart RightArm { get; private set; }
        public MovingBodyPart LeftLeg { get; private set; }
        public MovingBodyPart RightLeg { get; private set; }
        public MovingBodyPart Head { get; private set; }

        public Human(GTI.DigitalOutput heart, GTI.PWMOutput leftArm, GTI.PWMOutput rightArm, 
                     GTI.PWMOutput leftLeg, GTI.PWMOutput rightLeg, GTI.PWMOutput head,
                     GTM.GHIElectronics.Distance_US3 distanceSensor)
        {
            _heartPin = heart;

            // Initialize the body parts with their calibrated min, max, & rest positions.
            LeftArm = new MovingBodyPart(leftArm, 0, 115, 115);
            RightArm = new MovingBodyPart(rightArm, 0, 145, 0);
            LeftLeg = new MovingBodyPart(leftLeg, 130, 180, 180);
            RightLeg = new MovingBodyPart(rightLeg, 0, 60, 0);
            Head = new MovingBodyPart(head, 20, 160, 90);

            // Start the heart beating.
            var heartState = false;
            _heartTimer = new GT.Timer(200);
            _heartTimer.Tick += (t) =>
                {
                    heartState = !heartState;
                    _heartPin.Write(heartState);
                };
            _heartTimer.Start();

            // Initialize the visual object scanner (head + distance sensor).
            _distanceSensor = distanceSensor;
            _visualObjectScanTimer = new GT.Timer(500);
            _visualObjectScanTimer.Tick += VisualObjectScanTimerOnTick;
        }

        private void VisualObjectScanTimerOnTick(GT.Timer timer)
        {
            const int MAX_BOSS_DISTANCE = 75;

            var distance = _distanceSensor.GetDistanceInCentimeters(5);
            Debug.Print("Distance = " + distance);
            if (distance > 0 && distance < MAX_BOSS_DISTANCE && Head.IsExercising)
            {
                Head.StopExercising();
                OnVisualObjectLocated(distance);
            }
            else if (distance >= MAX_BOSS_DISTANCE && !Head.IsExercising)
            {
                OnVisualObjectLost(distance);
                Head.StartExercising(2);
            }
        }

        // Invoke the VisualObjectLocated event; called whenever an object is spotted by the 
        // range sensor.
        protected virtual void OnVisualObjectLocated(int distance)
        {
            if (VisualObjectLocated != null)
            {
                VisualObjectLocated(this, distance);
            }
        }

        // Invoke the VisualObjectLocated event; called whenever an object is no longer spotted by the 
        // range sensor.
        protected virtual void OnVisualObjectLost(int distance)
        {
            if (VisualObjectLost != null)
            {
                VisualObjectLost(this, distance);
            }
        }

        /// <summary>
        /// Starts moving the head and scanning for objects.
        /// </summary>
        public void StartVisualObjectScan()
        {
            _visualObjectScanTimer.Start();
        }

        /// <summary>
        /// Stops the head from scanning.
        /// </summary>
        public void StopVisualObjectScan()
        {
            _visualObjectScanTimer.Stop();
            Head.StopExercising();
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
