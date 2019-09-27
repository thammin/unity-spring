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

        public abstract void Reset();

        public virtual void UpdateEndValue(float value) => UpdateEndValue(value, currentVelocity);

        public abstract void UpdateEndValue(float value, float velocity);

        public abstract float Evaluate(float deltaTime);
    }
}