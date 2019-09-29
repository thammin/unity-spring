namespace UnitySpring
{
    public abstract class SpringBase
    {
        // Default to critically damped
        public float damping = 26f;
        public float mass = 1f;
        public float stiffness = 169f;
        public float startValue;
        public float endValue;
        public float initialVelocity;

        protected float currentValue;
        protected float currentVelocity;

        /// <summary>
        /// Reset all values to initial states.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Update the end value in the middle of motion.
        /// This reuse the current velocity and interpolate the value smoothly afterwards.
        /// </summary>
        /// <param name="value">End value</param>
        public virtual void UpdateEndValue(float value) => UpdateEndValue(value, currentVelocity);

        /// <summary>
        /// Update the end value in the middle of motion but using a new velocity.
        /// </summary>
        /// <param name="value">End value</param>
        /// <param name="velocity">New velocity</param>
        public abstract void UpdateEndValue(float value, float velocity);

        /// <summary>
        /// Advance a step by deltaTime(seconds).
        /// </summary>
        /// <param name="deltaTime">Delta time since previous frame</param>
        /// <returns>Evaluated value</returns>
        public abstract float Evaluate(float deltaTime);
    }
}