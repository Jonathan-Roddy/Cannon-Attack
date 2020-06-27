using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CannonAttack
{
    //We have made this class sealed so that it is not inherited by anything
    public sealed class Cannon
    {
        private readonly string CANNONID = "Human";
        private string CannonID;
        public static readonly int MAXANGLE = 90;
        public static readonly int MINANGLE = 1;
        private readonly int MAXVELOCITY = 300000000;
        private int distanceOfTarget;
        private readonly double GRAVITY = 9.8;
        private readonly int MAXDISTANCEOFTARGET = 20000;
        private readonly int BURSTRADIUS = 50;

        public int DistanceOfTarget
        {
            get { return distanceOfTarget; }
            set { distanceOfTarget = value; }
        }


        public string ID
        {
            get
            {
                return (string.IsNullOrWhiteSpace(CannonID)) ? CANNONID : CannonID ;
            }
            set {
                CannonID = value;
            }

        }

        public int Shots
        {
            get
            {
                return shots;
            }
        }

        private int shots;

        public void Reset()
        {
            shots = 0;
        }

        private static Cannon cannonSingletonInstance;

        // The block inside the lock ensures that only one thread enters this block at any given time. 
        // Given the importance of determining if there is an instance or not, 
        // we should definitely use the lock.

        static readonly object padlock = new object();

        private Cannon()
        {
            //by default we setup a random target
            Random r = new Random();
            SetTarget(r.Next((int)MAXDISTANCEOFTARGET));
        }

        public static Cannon GetInstance()
        {
            lock (padlock)
            {
                if (cannonSingletonInstance == null)
                {
                    cannonSingletonInstance = new Cannon();
                }
                return cannonSingletonInstance;
            }
            
        }

        public Tuple<bool, string> Shoot(int angle, int velocity)
        {
            if (angle > MAXANGLE || angle < MINANGLE) //Angle must be between 0 and 90 degress
            {
                return Tuple.Create(false, "Angle Incorrect");
            }
            if (velocity > MAXVELOCITY)
            {
                return Tuple.Create(false, "Velocity of the cannon travel faster than the speed of light");
            }
            shots++;
            string message;
            bool hit;
            int distanceOfShot = CalculateDistanceOfCannonShot(angle, velocity);
            if (distanceOfShot.WithinRange(this.distanceOfTarget, BURSTRADIUS))
            {
                message = String.Format("Hit - {0} Shots", shots);
                hit = true;
            }
            else
            {
                message = String.Format("Missed cannonball landed at {0} meters", distanceOfShot);
                hit = false;
            }
            return Tuple.Create(hit, message); // Not going to do the calculations just yet
        }

        public int CalculateDistanceOfCannonShot(int angle, int velocity)
        {
            int time = 0;
            double height = 0;
            double distance = 0;
            double angleInRadians = (3.1415926536 / 180) * angle;
            while (height >= 0)
            {
                time++;
                distance = velocity + Math.Cos(angleInRadians) * time;
                height = (velocity + Math.Cos(angleInRadians) * time) - (GRAVITY * Math.Pow(time, 2)) / 2;
            }
            return (int)distance;
        }

        public void SetTarget(int distanceOfTarget)
        {
            if (!distanceOfTarget.Between(0, MAXDISTANCEOFTARGET))
            {
                throw new ApplicationException(string.Format("Target distance must be between 1 and 0 {0} meters", MAXDISTANCEOFTARGET));
            }
            this.distanceOfTarget = distanceOfTarget;
        }

    }
}
