/* This driver is based on the work of Chris Seto and has been adapted for Gadgeteer. 
    https://www.ghielectronics.com/community/codeshare/entry/53
 */

using Gadgeteer.SocketInterfaces;

namespace JoeTheGadgeteer
{
    /// <summary>
    /// Servo driver
    /// </summary>
    public class Servo
    {
        private readonly PwmOutput _servo;
        private readonly int _minTiming;
        private readonly int _maxTiming;
        private readonly int _maxDeg;

        /// <summary>
        /// Set servo inversion
        /// </summary>
        public bool Inverted = false;

        /// <summary>
        /// Used to keep track of where we are
        /// </summary>
        private double _internalDegree;

        /// <summary>
        /// Create the PWM pin, set it low and configure timings
        /// </summary>
        public Servo(PwmOutput pwm, int minTiming = 1000, int maxTiming = 2000, int maxDegrees = 180)
        {
            _servo = pwm;
            //servo.Set(5000, 0);

            _minTiming = minTiming;
            _maxTiming = maxTiming;
            _maxDeg = maxDegrees;
        }

        /// <summary>
        /// Disengage the servo. 
        /// The servo motor will stop trying to maintain an angle
        /// </summary>
        public void Disengage()
        {
            _servo.Set(0, 0);
        }

        /// <summary>
        /// Set the servo position
        /// </summary>
        public double Position
        {
            set
            {
                // Range checks
                if (value > _maxDeg) value = _maxDeg;
                if (value < 0) value = 0;

                // Are we inverted?
                if (Inverted) value = _maxDeg - value;

                // Set up so that we know where we are
                _internalDegree = value;

                // Set the pulse
                _servo.Set(20000000, (uint)Map((long)value, 0, _maxDeg, _minTiming, _maxTiming) * 1000, PwmScaleFactor.Nanoseconds);
            }

            get
            {
                return _internalDegree;
            }
        }

        /// <summary>
        /// Used internally
        /// </summary>
        private static long Map(long x, long inMin, long inMax, long outMin, long outMax)
        {
            return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }
    }
}
