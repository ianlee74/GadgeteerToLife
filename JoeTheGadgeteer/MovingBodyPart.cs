using GT = Gadgeteer;
using Gadgeteer.SocketInterfaces;
using Microsoft.SPOT;

namespace JoeTheGadgeteer
{
    public class MovingBodyPart
    {
        private readonly Servo _servo;
        private GT.Timer _exerciseTimer;
        private int _lastDirection = 1;

        /// <summary>
        /// The minimum servo position that this body part is allowed to move.
        /// </summary>
        public int MinPosition { get; private set; }

        /// <summary>
        /// The maximum servo position that this body part is allowed to move.
        /// </summary>
        public int MaxPosition { get; private set; }

        /// <summary>
        /// The position that the body part should go to when resting.
        /// </summary>
        public int RestPosition { get; set; }

        /// <summary>
        /// The body part's current position.
        /// </summary>
        public int CurrentPosition { get; private set; }

        /// <summary>
        /// Is the body part currently exercising?
        /// </summary>
        public bool IsExercising { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="servoSignal">The PWM output pin used to control the servo.</param>
        /// <param name="minPosition">The minimum servo position that this body part is allowed to move.</param>
        /// <param name="maxPosition">The maximum servo position that this body part is allowed to move.</param>
        /// <param name="restPosition">The position that the body part should go to when resting.</param>
        public MovingBodyPart(PwmOutput servoSignal, int minPosition, int maxPosition, int restPosition = 0)
        {
            _servo = new Servo(servoSignal, 390, 2350);
            MinPosition = minPosition;
            MaxPosition = maxPosition;
            RestPosition = restPosition;
        }

        /// <summary>
        /// Moves the body part.
        /// </summary>
        /// <param name="position">The distance to move from the bottom of the movement range.</param>
        public void Move(int position)
        {
            if (position > MaxPosition) position = MaxPosition;
            _servo.Position = position;
            CurrentPosition = position;
        }

        /// <summary>
        /// Starts the body part moving through the full range of motion.
        /// </summary>
        /// <param name="moveIncrement">The distance to move in each step.</param>
        /// <param name="timeBetweenSteps">The time (in ms) between step motions.</param>
        public void StartExercising(int moveIncrement = 1, int timeBetweenSteps = 50)
        {
            // Position can't be < 0.
            var position = CurrentPosition > 0 ? CurrentPosition : 0;

            // Initialize a timer.
            if (_exerciseTimer == null)
            {
                _exerciseTimer = new GT.Timer(timeBetweenSteps);
                _exerciseTimer.Tick += t =>
                {
                    // Limit the max position.
                    if (position > MaxPosition)
                    {
                        _lastDirection = -1;
                        position = MaxPosition - moveIncrement;
                    }

                    // Limit the lower position.
                    if (position < MinPosition)
                    {
                        _lastDirection = 1;
                        position = MinPosition + moveIncrement;
                    }

                    // Move a little every time the timer ticks.
                    Move(position);
                    position += _lastDirection * moveIncrement;
                };
            }
            IsExercising = true;
            _exerciseTimer.Start();
        }

        /// <summary>
        /// Stop exercising the body part.
        /// </summary>
        public void StopExercising()
        {
            IsExercising = false;
            if (_exerciseTimer != null) _exerciseTimer.Stop();
        }
    }
}
