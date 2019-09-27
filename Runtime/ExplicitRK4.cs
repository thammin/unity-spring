using UnityEngine;

namespace UnitySpring.ExplicitRK4
{
    /// <summary>
    /// Explicit Runge-Kutta 4th order aka RK4
    /// https://en.wikipedia.org/wiki/Runge%E2%80%93Kutta_methods
    /// </summary>
    public class Spring : SpringBase
    {
        float stepSize = 1f / 60f; // stable if < 1/36
        bool isFirstEvaluate = true;

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
            if (isFirstEvaluate)
            {
                Reset();
                isFirstEvaluate = false;
            }

            var c = damping;
            var m = mass;
            var k = stiffness;

            var x = currentValue;
            var v = currentVelocity;
            var _x = currentValue;
            var _v = currentVelocity;

            var steps = Mathf.Ceil(deltaTime / stepSize);
            for (var i = 0; i < steps; i++)
            {
                var dt = i == steps - 1 ? deltaTime - i * stepSize : stepSize;

                // springForce = -k * (x - endValue)
                // dampingForce = -c * v
                var a_v = _v;
                var a_a = (-k * (_x - endValue) - c * _v) / m;
                _x = x + a_v * dt / 2;
                _v = v + a_a * dt / 2;

                var b_v = _v;
                var b_a = (-k * (_x - endValue) - c * _v) / m;
                _x = x + b_v * dt / 2;
                _v = v + b_a * dt / 2;

                var c_v = _v;
                var c_a = (-k * (_x - endValue) - c * _v) / m;
                _x = x + c_v * dt / 2;
                _v = v + c_a * dt / 2;

                var d_v = _v;
                var d_a = (-k * (_x - endValue) - c * _v) / m;
                _x = x + c_v * dt / 2;
                _v = v + c_a * dt / 2;

                var dxdt = (a_v + 2 * (b_v + c_v) + d_v) / 6;
                var dvdt = (a_a + 2 * (b_a + c_a) + d_a) / 6;

                x += dxdt * dt;
                v += dvdt * dt;
            }

            currentValue = x;
            currentVelocity = v;

            return currentValue;
        }
    }
}