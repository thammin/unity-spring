using System;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUI;
using System.Linq;
using System.Reflection;

namespace UnitySpring.Editor
{
    public class Visualizer : EditorWindow
    {
        // Graph
        static readonly Rect Box = new Rect(100, 100, 1000, 500);
        int graphOffsetY = 25;
        int cellSize = 20;
        int offset = 10;
        Color gridColor = new Color(0.5f, 0.5f, 0.5f, 0.1f);
        Color axisColor = Color.gray;
        float gridSizeX => Visualizer.Box.width - offset * 2;
        float gridSizeY => Visualizer.Box.height - offset * 2 - graphOffsetY;
        int gridCountX => Mathf.CeilToInt(gridSizeX / cellSize);
        int gridCountY => Mathf.CeilToInt(gridSizeY / cellSize);
        int axisY => gridCountY / 2;

        // Spring Types
        Type[] springTypes = new Type[]
        {
            typeof(ClosedForm.Spring),
            typeof(SemiImplicitEuler.Spring),
            typeof(ExplicitRK4.Spring),
            typeof(VerletIntegration.Spring)
        };
        String[] springTypeOptions => springTypes
            .Select(t => t.FullName)
            .Select(s => s.Split('.')[1])
            .ToArray();

        Type currentType = typeof(ClosedForm.Spring);

        (float damping, float startValue, float endValue, float initialVelocity)[] dataset
            = new (float damping, float startValue, float endValue, float initialVelocity)[]
            {
                (26 , 10, 0, 0  ), // critically damped
                (5  , 10, 0, 0  ), // under damped
                (100, 10, 0, 0  ), // over damped
                (5  ,  0, 0, 100)  // under damped with initial velocity
            };

        Color[] colors = new Color[]
        {
            Color.red,    // critically damped
            Color.cyan,   // under damped
            Color.yellow, // over damped
            Color.magenta // under damped with initial velocity
        };

        // caches
        SpringBase[] springs;
        FieldInfo stepSizeField;
        int stepSizeFps;
        int fps = 60;
        float graphTime = 2f;

        [MenuItem("Tools/UnitySpring/Visualizer")]
        static void ShowWindow()
        {
            GetWindowWithRect<Visualizer>(Box, true, "Unity Spring Visualizer", true);
        }

        void OnEnable() => SetupSprings();

        void SetupSprings()
        {
            springs = dataset.Select(d =>
            {
                var spring = Activator.CreateInstance(currentType) as SpringBase;
                spring.damping = d.damping;
                spring.startValue = d.startValue;
                spring.endValue = d.endValue;
                spring.initialVelocity = d.initialVelocity;
                return spring;
            }).ToArray();

            stepSizeField = currentType.GetField("stepSize", BindingFlags.NonPublic | BindingFlags.Instance);
            if (stepSizeField != null)
            {
                stepSizeFps = Mathf.CeilToInt(1f / (float)stepSizeField.GetValue(springs[0]));
            }
        }

        void OnGUI()
        {
            DrawController();
            DrawGrid();
            PlotGraph();
        }

        void DrawGrid()
        {
            for (var x = 0; x < gridCountX + 1; x++)
            {
                var color = x == 0 ? axisColor : gridColor;
                drawLine(x, 0, 1, gridCountY * cellSize, color);
            }

            for (var y = 0; y < gridCountY + 1; y++)
            {
                var color = y == axisY ? axisColor : gridColor;
                drawLine(0, y, gridCountX * cellSize, 1, color);
            }

            void drawLine(float x, float y, float w, float h, Color c)
            {
                x = x * cellSize + offset - 0.5f;
                y = y * cellSize + offset - 0.5f + graphOffsetY;
                DrawRect(new Rect(x, y, w, h), c);
            }
        }

        void PlotGraph()
        {
            var step = 1f / fps;
            var dt = step / (graphTime / gridSizeX);

            foreach (var s in springs) s.Reset();

            // start values
            for (var i = 0; i < springs.Length; i++)
            {
                drawPoint(0, springs[i].startValue, colors[i]);
            }

            // draw until end of axis x
            var t = dt;
            while (t < gridSizeX)
            {
                for (var i = 0; i < springs.Length; i++)
                {
                    drawPoint(t, springs[i].Evaluate(step), colors[i]);
                }
                t += dt;
            }

            void drawPoint(float x, float y, Color c)
            {
                var n = Mathf.FloorToInt(dt / 2);
                n = Mathf.Clamp(n, 1, 5);
                DrawRect(
                    new Rect(
                        x + offset - 0.5f * n,
                        (axisY - y) * cellSize + offset - 0.5f * n + graphOffsetY,
                        n,
                        n
                    ),
                    c
                );
            }
        }

        void DrawController()
        {
            EditorGUIUtility.labelWidth = 70;
            var sliderWidth = GUILayout.Width(230);

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(10);

                // spring types
                BeginChangeCheck();
                {
                    var index = Array.IndexOf(springTypes, currentType);
                    index = EditorGUILayout.Popup("Spring Type:", index, springTypeOptions);
                    currentType = springTypes[index];
                }
                if (EndChangeCheck()) SetupSprings();

                GUILayout.Space(10);

                // fps
                fps = EditorGUILayout.IntSlider("FPS:", fps, 10, 120, sliderWidth);

                GUILayout.Space(10);

                // step size
                if (stepSizeField != null)
                {
                    BeginChangeCheck();
                    {
                        stepSizeFps = EditorGUILayout.IntSlider("Step FPS:", stepSizeFps, fps, 120, sliderWidth);
                    }
                    if (EndChangeCheck())
                    {
                        foreach (var s in springs) stepSizeField.SetValue(s, 1f / stepSizeFps);
                    }
                }

                GUILayout.Space(10);

                // graph time
                graphTime = EditorGUILayout.Slider("Graph Time:", graphTime, 0.1f, 5f, sliderWidth);

                GUILayout.Space(10);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = 0;
        }
    }
}