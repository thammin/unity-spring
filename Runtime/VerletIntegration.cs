using UnityEngine;

namespace UnitySpring.VerletIntegration
{
    /// <summary>
    /// Verlet Integration
    /// https://en.wikipedia.org/wiki/Verlet_integration
    /// </summary>
    public class Spring : SpringBase
    {
        float stepSize = 1f / 61f; // stable if < deltaTime && < 1/60
        bool isFirstEvaluate = true;
        float currentAcceleration = 0.0f;

        public override void Reset()
        {
            currentValue = startValue;
            currentVelocity = initialVelocity;
            currentAcceleration = 0.0f;
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
            var a = currentAcceleration;

            var _stepSize = deltaTime > stepSize ? stepSize : deltaTime - 0.001f;
            var steps = Mathf.Ceil(deltaTime / _stepSize);
            for (var i = 0; i < steps; i++)
            {
                var dt = i == steps - 1 ? deltaTime - i * _stepSize : _stepSize;

                x += v * dt + a * (dt * dt * 0.5f);
                // springForce = -k * (x - endValue)
                // dampingForce = -c * v
                var _a = (-k * (x - endValue) + -c * v) / m;
                v += (a + _a) * (dt * 0.5f);
                a = _a;
            }

            currentValue = x;
            currentVelocity = v;
            currentAcceleration = a;

            return currentValue;
        }
    }
}