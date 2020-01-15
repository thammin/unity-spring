using NUnit.Framework;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace UnitySpring.Tests
{
    public class PerformanceTest
    {
        const int Count = 100000;

        [Test]
        public static void _ClosedForm()
            => Benchmark(CreateSprings<ClosedForm.Spring>(), "ClosedForm");

        [Test]
        public static void _SemiImplicitEuler()
            => Benchmark(CreateSprings<SemiImplicitEuler.Spring>(), "SemiImplicitEuler");

        [Test]
        public static void _ExplicitRK4()
            => Benchmark(CreateSprings<ExplicitRK4.Spring>(), "ExplicitRK4");

        [Test]
        public static void _VerletIntegration()
            => Benchmark(CreateSprings<VerletIntegration.Spring>(), "VerletIntegration");

        [Test]
        public static void UnitySmoothDamp()
            => Benchmark(CreateSprings<UnitySmoothDamp>(), "UnitySmoothDamp");

        static void Benchmark(SpringBase[] springs, string name)
        {
            var sw = new Stopwatch();
            var count = 0;
            var sum = 0.0;

            while (count < 120)
            {
                sum += Step();
                count++;
            }

            Debug.Log($"[{name}] Average time for {Count} spring per frame : {(sum / count).ToString("0.000")} ms");

            double Step()
            {
                sw.Reset();
                sw.Start();
                foreach (var s in springs) s.Evaluate(1 / 60f);
                sw.Stop();
                return GetPreciseTime(sw);
            }
        }

        static T[] CreateSprings<T>() where T : SpringBase, new()
        {
            var springs = new T[Count];
            for (var i = 0; i < springs.Length; i++)
            {
                springs[i] = new T();
            }
            return springs;
        }

        static double GetPreciseTime(Stopwatch sw)
        {
            return 1000.0 * (double) sw.ElapsedTicks / Stopwatch.Frequency;
        }
    }

    class UnitySmoothDamp : SpringBase
    {
        float smoothTime = 0.1f;
        float maxSpeed = Mathf.Infinity;

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
            currentValue = Mathf.SmoothDamp(currentValue, endValue, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
            return currentValue;
        }
    }
}
