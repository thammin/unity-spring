namespace UnitySpring.SemiImplicitEuler
{
    /// <summary>
    /// Semi-implicit Euler method
    /// https://en.wikipedia.org/wiki/Semi-implicit_Euler_method
    ///
    /// Proof and derived from http://box2d.org/files/GDC2015/ErinCatto_NumericalMethods.pdf
    /// </summary>
    public class Spring : SpringBase
    {
        bool isFirstStep = true;

        public override void Reset()
        {
            currentValue = startValue;
            currentVelocity = initialVelocity;
        }

        public override void UpdateEndValue(float value, float velocity)
        {
            endValue = value;
            currentVelocity = velocity;
        }

        public override float Evaluate(float deltaTime)
        {
            if (isFirstStep)
            {
                Reset();
                isFirstStep = false;
            }

            var c = damping;
            var m = mass;
            var k = stiffness;

            var x = currentValue;
            var v = currentVelocity;

            // springForce = -k * (x - endValue)
            // dampingForce = -c * v
            var a = (-k * (x - endValue) + -c * v) / m;
            v = v + a * deltaTime;
            x = x + v * deltaTime;

            currentValue = x;
            currentVelocity = v;

            return currentValue;
        }
    }
}