using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUI;
using Debug = UnityEngine.Debug;
using Spring = UnitySpring.ClosedForm.Spring;

namespace UnitySpring.Editor
{
    public class Visualizer : EditorWindow
    {
        static readonly Rect Box = new Rect(100, 100, 1000, 500);

        int cellSize = 20;
        int offset = 10;
        Color gridColor = new Color(0.5f, 0.5f, 0.5f, 0.1f);
        Color axisColor = Color.gray;

        [MenuItem("Tools/UnitySpring/Visualizer")]
        static void ShowWindow()
        {
            GetWindowWithRect<Visualizer>(Box, true, "Unity Spring Visualizer", true);
        }

        Spring s1 = new Spring() // critically damped
        {
            startValue = 10,
            endValue = 0
        };
        Spring s2 = new Spring() // under damped
        {
            damping = 5.0f,
            startValue = 10,
            endValue = 0
        };
        Spring s3 = new Spring() // over damped
        {
            damping = 100.0f,
            startValue = 10,
            endValue = 0
        };
        Spring s4 = new Spring() // under damped with initial velocity
        {
            damping = 5.0f,
            initialVelocity = 100
        };

        Stopwatch sw = new Stopwatch();

        void OnGUI()
        {
            DrawGrid();
            PlotGraph();
        }

        int getCount(float size) => ((int)size - offset) / cellSize + 1;

        void DrawGrid()
        {
            var box = Visualizer.Box;
            var axisY = getCount(box.height) / 2;

            for (var x = 0; x < getCount(box.width); x++)
            {
                var color = x == 0 ? axisColor : gridColor;
                drawLine(x, 0, 1, box.height, color);
            }

            for (var y = 0; y < getCount(box.height); y++)
            {
                var color = y == axisY ? axisColor : gridColor;
                drawLine(0, y, box.width, 1, color);
            }

            void drawLine(float x, float y, float w, float h, Color c)
            {
                x = x * cellSize + offset;
                y = y * cellSize + offset;
                w = w == 1 ? w : w - cellSize;
                h = h == 1 ? h : h - cellSize;
                DrawRect(new Rect(x, y, w, h), c);
            }
        }

        void PlotGraph()
        {
            var box = Visualizer.Box;
            var axisY = getCount(box.height) / 2;
            var step = 1f / 150f / 1000f; // 150fps

            s1.Reset();
            s2.Reset();
            s3.Reset();
            s4.Reset();

            sw.Reset();
            sw.Start();
            for (var t = 0; t < box.width - cellSize; t++)
            {
                drawPoint(t, s1.Evaluate(t * step), Color.red, s1.endValue);
                drawPoint(t, s2.Evaluate(t * step), Color.cyan, s2.endValue);
                drawPoint(t, s3.Evaluate(t * step), Color.yellow, s3.endValue);
                drawPoint(t, s4.Evaluate(t * step), Color.magenta, s4.endValue);
            }
            sw.Stop();
            Debug.Log($"{1000.0 * (double)sw.ElapsedTicks / Stopwatch.Frequency} ms");

            void drawPoint(float x, float y, Color c, float ev)
            {
                if (Mathf.Approximately(y, ev)) return;
                DrawRect(new Rect(x + offset, (axisY - y) * cellSize + offset, 1, 1), c);
            }
        }
    }
}