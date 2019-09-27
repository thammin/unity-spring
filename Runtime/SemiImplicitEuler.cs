using UnityEngine;

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
        float stepSize = 1f / 60f; // stable if < 1/51
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

            var steps = Mathf.Ceil(deltaTime / stepSize);
            for (var i = 0; i < steps; i++)
            {
                var dt = i == steps - 1 ? deltaTime - i * stepSize : stepSize;

                // springForce = -k * (x - endValue)
                // dampingForce = -c * v
                var a = (-k * (x - endValue) + -c * v) / m;
                v += a * dt;
                x += v * dt;
            }

            currentValue = x;
            currentVelocity = v;

            return currentValue;
        }
    }
}