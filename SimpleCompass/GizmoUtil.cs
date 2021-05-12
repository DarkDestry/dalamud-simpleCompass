using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Actors;
using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;

namespace SamplePlugin
{
    static class GizmoUtil
    {
        public static DalamudPluginInterface pi;

        private const float PI = (float) Math.PI;

        public static Vector2 WorldToScreen(Position3 pos)
        {
            pi.Framework.Gui.WorldToScreen(
                new SharpDX.Vector3(pos.X, pos.Z, pos.Y),
                out SharpDX.Vector2 screenPos);
            return new Vector2(screenPos.X, screenPos.Y);
            // pi.Framework.Gui.WorldToScreen(
            //     new SharpDX.Vector3(actor.Position.X, actor.Position.Z, actor.Position.Y), 
            //     out SharpDX.Vector2 screenPos)
        }
        public static Vector2 WorldToScreen(Vector3 pos)
        {
            if (pi.Framework.Gui.WorldToScreen(
                new SharpDX.Vector3(pos.X, pos.Z, pos.Y),
                out SharpDX.Vector2 screenPos)) return new Vector2(screenPos.X, screenPos.Y);
            return Vector2.Zero;
            // pi.Framework.Gui.WorldToScreen(
            //     new SharpDX.Vector3(actor.Position.X, actor.Position.Z, actor.Position.Y), 
            //     out SharpDX.Vector2 screenPos)
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="col">0xAABBGGRR</param>
        /// <param name="thickness"></param>
        public static void DrawLine(Vector3 start, Vector3 end, uint col = 0xFFFFFFFF, float thickness = 1)
        {
            Vector2 startV2 = WorldToScreen(start), endV2 = WorldToScreen(end);
            if (startV2 == Vector2.Zero || endV2 == Vector2.Zero) return;
            ImGui.GetWindowDrawList().AddLine(startV2, endV2, col, thickness);
        }

        public static void DrawArrow(Vector3 start, Vector3 end, uint col = 0xFFFFFFFF, float thickness = 1)
        {
            DrawLine(start, end, col, thickness);
            Vector3 right = Vector3.Cross(end-start, Vector3.UnitZ);
            DrawLine(end, end + Vector3.Transform(right * 0.5f, Matrix4x4.CreateRotationZ(-PI / 4)), col, thickness);
            DrawLine(end, end + Vector3.Transform(right * 0.5f, Matrix4x4.CreateRotationZ(-PI / 4 * 3)), col, thickness);
        }

        public static void DrawCircle(Vector3 center, float radius, uint col = 0xFFFFFFFF, float thickness = 1, int segments = 32)
        {
            for (int i = 0; i < segments; i++)
            {
                Vector3 start, end;
                start = Vector3.Transform(Vector3.UnitX * radius, Matrix4x4.CreateRotationZ(PI*2 / segments * i));
                end = Vector3.Transform(Vector3.UnitX * radius, Matrix4x4.CreateRotationZ(PI*2 / segments * (i-1)));
                DrawLine(center + start, center + end, col, thickness);
            }
        }

        public static void DrawQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, uint col = 0xFFFFFFFF)
        {
            Vector2 p1V2, p2V2, p3V2, p4V2;
            p1V2 = WorldToScreen(p1);
            p2V2 = WorldToScreen(p2);
            p3V2 = WorldToScreen(p3);
            p4V2 = WorldToScreen(p4);
            if (p1V2 == Vector2.Zero || p2V2 == Vector2.Zero || p3V2 == Vector2.Zero || p4V2 == Vector2.Zero) return;
            ImGui.GetWindowDrawList().AddQuadFilled(p1V2,p2V2,p3V2,p4V2, col);
        }

        public static void DrawPoly(Vector3[] points, uint col = 0xFFFFFFFF)
        {
            Vector2[] scrPoints = points.Select(p => WorldToScreen(p)).Where(p => p != Vector2.Zero).ToArray();

            if (scrPoints.Length == 0)
            {
                scrPoints = new []{Vector2.Zero, Vector2.UnitX*1000, Vector2.One*1000, Vector2.UnitY*1000};
            }

            if (scrPoints.Length > 0) ImGui.GetWindowDrawList().AddConvexPolyFilled(ref scrPoints[0], scrPoints.Length, col);
        }

        public static void DrawQuad(Vector3 pos, Vector3 forward, float width, float distance, uint col = 0xFFFFFFFF, float increments = 0.1f)
        {
            Vector3 right = -Vector3.Cross(forward, Vector3.UnitZ);

            float maxLeft, maxRight;
            maxLeft = -1;
            maxRight = -1;
            float maxDistance = distance;
            for (float i = 0; i < distance; i += increments)
            {
                maxDistance = distance - i;
                Vector2 p3, p4;
                p3 = WorldToScreen(pos + right * width + forward * (distance - i));
                p4 = WorldToScreen(pos + right * -width + forward * (distance - i));
                if (p3 != Vector2.Zero && maxRight == -1) maxRight = distance - i;
                if (p4 != Vector2.Zero && maxLeft == -1) maxLeft = distance - i;
                if (p3 != Vector2.Zero && p4 != Vector2.Zero) break;
            }

            
            
            DrawLine(pos + right * width, pos + right * width + forward * maxRight, 0xFF000000 | col);
            DrawLine(pos + right * -width, pos + right * -width + forward * maxLeft, 0xFF000000 | col);
            DrawQuad(pos + right * width, 
                pos + right * -width,
                pos + right * -width + forward * maxLeft,
                pos + right * width + forward * maxRight, col);
        }

        public static void DrawCircle(Vector3 center, float radius, uint col, int segments = 32)
        {
            // DrawCircle(center, 10, 0xFF000000 | col, 2, segments);

            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < segments; i++)
            {
                Vector3 start, end;
                start = Vector3.Transform(Vector3.UnitX * radius, Matrix4x4.CreateRotationZ(PI*2 / segments * i));
                end = Vector3.Transform(Vector3.UnitX * radius, Matrix4x4.CreateRotationZ(PI*2 / segments * (i-1)));
                points.Add(center + start);
                DrawLine(center + start, center + end, 0xFF000000 | col, 2);
            }
            DrawPoly(points.ToArray(), col);
        }

        public static void WriteText(Vector3 pos, string text, uint col = 0xFFFFFFFF, float size = 20, float shadow = -1, uint shadowCol = 0xFF000000, bool centered = false)
        {
            Vector2 posV2 = WorldToScreen(pos);
            if (posV2 == Vector2.Zero) return;
            if (centered)
            {
                posV2 -= Vector2.UnitY * (size / 2);
                posV2 -= Vector2.UnitX * (size / 2 * text.Length);
            }
            if (shadow > 0)
            {
                ImGui.GetWindowDrawList().AddText(UiBuilder.DefaultFont, size * ImGui.GetIO().FontGlobalScale + shadow*2, posV2 - Vector2.One * shadow, shadowCol, text);
            }
            ImGui.GetWindowDrawList().AddText(UiBuilder.DefaultFont, size * ImGui.GetIO().FontGlobalScale, posV2, col, text);
        }
    }
}
