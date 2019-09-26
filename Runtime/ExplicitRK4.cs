namespace UnitySpring.ExplicitRK4
{
    /// <summary>
    /// Explicit Runge-Kutta 4th order aka RK4
    /// https://en.wikipedia.org/wiki/Runge%E2%80%93Kutta_methods
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
            var _x = currentValue;
            var _v = currentVelocity;

            // springForce = -k * (x - endValue)
            // dampingForce = -c * v

            var a_v = _v;
            var a_a = (-k * (_x - endValue) - c * _v) / m;
            _x = x + a_v * deltaTime / 2;
            _v = v + a_a * deltaTime / 2;

            var b_v = _v;
            var b_a = (-k * (_x - endValue) - c * _v) / m;
            _x = x + b_v * deltaTime / 2;
            _v = v + b_a * deltaTime / 2;

            var c_v = _v;
            var c_a = (-k * (_x - endValue) - c * _v) / m;
            _x = x + c_v * deltaTime / 2;
            _v = v + c_a * deltaTime / 2;

            var d_v = _v;
            var d_a = (-k * (_x - endValue) - c * _v) / m;
            _x = x + c_v * deltaTime / 2;
            _v = v + c_a * deltaTime / 2;

            var dxdt = (a_v + 2 * (b_v + c_v) + d_v) / 6;
            var dvdt = (a_a + 2 * (b_a + c_a) + d_a) / 6;

            x += dxdt * deltaTime;
            v += dvdt * deltaTime;

            currentValue = x;
            currentVelocity = v;

            return currentValue;
        }
    }
}