using System;
using Utility.Units;

namespace Utility
{
    public static class Movement
    {
        public static Duration GetDurationFromDistance(Distance distance, Acceleration acc, Speed maxSpeed)
        {
            var distanceToAccelerateToMaxSpeed = GetDistanceToAccelerateToSpeed(acc, maxSpeed);
            if (distanceToAccelerateToMaxSpeed * 2.0 > distance) // Distance too short to accelerate all the way
            {
                var actualAcceleratingDistance = distance / 2.0;
                return (2.0 * actualAcceleratingDistance / acc).SquareRoot;
            }

            var timeToAccelerateToMaxSpeed = GetDurationToAccelerateToSpeed(acc, maxSpeed);
            var distanceWithMaxSpeed = distance - 2.0 * distanceToAccelerateToMaxSpeed;
            return 2 * timeToAccelerateToMaxSpeed + distanceWithMaxSpeed / maxSpeed;
        }

        public static Distance GetDistanceFromDuration(Duration time, Duration totalTime, Acceleration acc, Speed maxSpeed)
        {
            if (time > totalTime)
            {
                throw new ArgumentOutOfRangeException(nameof(time));
            }

            var timeToAccelerateToMaxSpeed = GetDurationToAccelerateToSpeed(acc, maxSpeed);
            if (timeToAccelerateToMaxSpeed * 2 > totalTime) // Distance too short to accelerate all the way
            {
                var actualAcceleratingTime = totalTime / 2;
                if (time <= actualAcceleratingTime)
                {
                    return GetDistanceFromAcceleration(acc, time);
                }

                var actualMaxSpeed = acc * actualAcceleratingTime;
                return GetDistanceFromAcceleration(acc, actualAcceleratingTime) + GetDistanceFromDeceleration(actualMaxSpeed, acc, time - actualAcceleratingTime);
            }

            if (time <= timeToAccelerateToMaxSpeed)
            {
                return GetDistanceFromAcceleration(acc, time);
            }

            var distanceToAccelerateToMaxSpeed = GetDistanceFromAcceleration(acc, timeToAccelerateToMaxSpeed);

            if (time <= totalTime - timeToAccelerateToMaxSpeed)
            {
                return distanceToAccelerateToMaxSpeed + (time - timeToAccelerateToMaxSpeed) * maxSpeed;
            }

            var timeWithMaxSpeed = totalTime - 2 * timeToAccelerateToMaxSpeed;
            var timeDecelerating = time - timeToAccelerateToMaxSpeed - timeWithMaxSpeed;
            return distanceToAccelerateToMaxSpeed + timeWithMaxSpeed * maxSpeed + GetDistanceFromDeceleration(maxSpeed, acc, timeDecelerating);
        }

        private static Distance GetDistanceToAccelerateToSpeed(Acceleration acc, Speed speed) => GetDistanceFromAcceleration(acc, GetDurationToAccelerateToSpeed(acc, speed));

        private static Duration GetDurationToAccelerateToSpeed(Acceleration acc, Speed speed) => speed / acc;

        private static Distance GetDistanceFromAcceleration(Acceleration acc, Duration dur) => acc * 0.5 * dur * dur;

        private static Distance GetDistanceFromDeceleration(Speed speed, Acceleration acc, Duration dur)
        {
            var timeToDecelerateToZero = GetDurationToAccelerateToSpeed(acc, speed);
            if (dur >= timeToDecelerateToZero)
            {
                return GetDistanceToAccelerateToSpeed(acc, speed);
            }

            var speedOffset = speed - acc * dur;
            return speedOffset * dur + (speed - speedOffset) * dur * 0.5;
        }
    }
}
