using System;
using System.Numerics;
using Dalamud.Game.ClientState.Actors;
using Dalamud.Plugin;
using ImGuiNET;
using static SamplePlugin.GizmoUtil;

namespace SimpleCompass
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class PluginUI : IDisposable
    {
        private const float PI = (float) Math.PI;

        private Configuration configuration;

        private bool drawCompass;
        private bool drawInterCompass;
        private bool drawSubInterCompass;
        
        private uint mainColor;
        private uint interColor;
        private uint subInterColor;

        private float mainSize;
        private float interSize;
        private float subInterSize;

        private float mainDistance;
        private float interDistance;
        private float subInterDistance;

        // this extra bool exists for ImGui, since you can't ref a property
        private bool debugVisible;

        public bool DebugVisible
        {
            get => debugVisible;
            set => debugVisible = value;
        }

        private bool settingsVisible;

        public bool SettingsVisible
        {
            get => settingsVisible;
            set => settingsVisible = value;
        }

        private bool overlayVisible = true;

        public bool OverlayVisible
        {
            get => overlayVisible;
            set => overlayVisible = value;
        }

        public ActorTable actors;
        public Vector2 actorScreenPos;
        private DalamudPluginInterface pi;
        private Vector3 compassPos;


        // passing in the image here just for simplicity
        public PluginUI(Configuration configuration, DalamudPluginInterface pi /*, ImGuiScene.TextureWrap goatImage*/)
        {
            this.configuration = configuration;

            drawCompass = configuration.DrawCompass;
            drawInterCompass = configuration.DrawInterCompass;
            drawSubInterCompass = configuration.DrawSubInterCompass;

            mainColor = configuration.MainColor;
            interColor = configuration.InterColor;
            subInterColor = configuration.SubInterColor;

            mainSize = configuration.MainSize;
            interSize = configuration.InterSize;
            subInterSize = configuration.SubInterSize;

            mainDistance = configuration.MainDistance;
            interDistance = configuration.InterDistance;
            subInterDistance = configuration.SubInterDistance;

            this.pi = pi;
        }

        public void Dispose()
        {
        }

        public void Draw()
        {
            DrawDebugWindow();
            DrawSettingsWindow();
            DrawOverlay();
            SaveProperties();
        }

        private void SaveProperties()
        {
            bool needSave = false;

            if (configuration.DrawCompass != drawCompass)
            {
                configuration.DrawCompass = drawCompass;
                needSave = true;
            }

            if (configuration.DrawInterCompass != drawInterCompass)
            {
                configuration.DrawInterCompass = drawInterCompass;
                needSave = true;
            }

            if (configuration.DrawSubInterCompass != drawSubInterCompass)
            {
                configuration.DrawSubInterCompass = drawSubInterCompass;
                needSave = true;
            }
            
            if (configuration.MainColor != mainColor)
            {
                configuration.MainColor = mainColor;
                needSave = true;
            }
            
            if (configuration.InterColor != interColor)
            {
                configuration.InterColor = interColor;
                needSave = true;
            }
            
            if (configuration.SubInterColor != subInterColor)
            {
                configuration.SubInterColor = subInterColor;
                needSave = true;
            }
            
            if (configuration.MainSize != mainSize)
            {
                configuration.MainSize = mainSize;
                needSave = true;
            }
            
            if (configuration.InterSize != interSize)
            {
                configuration.InterSize = interSize;
                needSave = true;
            }
            
            if (configuration.SubInterSize != subInterSize)
            {
                configuration.SubInterSize = subInterSize;
                needSave = true;
            }
            
            if (configuration.MainDistance != mainDistance)
            {
                configuration.MainDistance = mainDistance;
                needSave = true;
            }
            
            if (configuration.InterDistance != interDistance)
            {
                configuration.InterDistance = interDistance;
                needSave = true;
            }
            
            if (configuration.SubInterDistance != subInterDistance)
            {
                configuration.SubInterDistance = subInterDistance;
                needSave = true;
            }

            if (needSave) configuration.Save();
        }

        private static readonly Matrix4x4 rot451 = Matrix4x4.CreateRotationZ(PI / 4);
        private static readonly Matrix4x4 rot452 = Matrix4x4.CreateRotationZ(PI / 4 * 3);
        private static readonly Matrix4x4 rot453 = Matrix4x4.CreateRotationZ(PI / 4 * 5);
        private static readonly Matrix4x4 rot454 = Matrix4x4.CreateRotationZ(PI / 4 * 7);

        private static readonly Matrix4x4 rot2251 = Matrix4x4.CreateRotationZ(PI / 8);
        private static readonly Matrix4x4 rot2252 = Matrix4x4.CreateRotationZ(PI / 8 * 3);
        private static readonly Matrix4x4 rot2253 = Matrix4x4.CreateRotationZ(PI / 8 * 5);
        private static readonly Matrix4x4 rot2254 = Matrix4x4.CreateRotationZ(PI / 8 * 7);
        private static readonly Matrix4x4 rot2255 = Matrix4x4.CreateRotationZ(PI / 8 * 9);
        private static readonly Matrix4x4 rot2256 = Matrix4x4.CreateRotationZ(PI / 8 * 11);
        private static readonly Matrix4x4 rot2257 = Matrix4x4.CreateRotationZ(PI / 8 * 13);
        private static readonly Matrix4x4 rot2258 = Matrix4x4.CreateRotationZ(PI / 8 * 15);

        private static readonly Vector3 v451 = Vector3.Transform(Vector3.UnitY, rot451);
        private static readonly Vector3 v452 = Vector3.Transform(Vector3.UnitY, rot452);
        private static readonly Vector3 v453 = Vector3.Transform(Vector3.UnitY, rot453);
        private static readonly Vector3 v454 = Vector3.Transform(Vector3.UnitY, rot454);

        private static readonly Vector3 v2251 = Vector3.Transform(Vector3.UnitY, rot2251);
        private static readonly Vector3 v2252 = Vector3.Transform(Vector3.UnitY, rot2252);
        private static readonly Vector3 v2253 = Vector3.Transform(Vector3.UnitY, rot2253);
        private static readonly Vector3 v2254 = Vector3.Transform(Vector3.UnitY, rot2254);
        private static readonly Vector3 v2255 = Vector3.Transform(Vector3.UnitY, rot2255);
        private static readonly Vector3 v2256 = Vector3.Transform(Vector3.UnitY, rot2256);
        private static readonly Vector3 v2257 = Vector3.Transform(Vector3.UnitY, rot2257);
        private static readonly Vector3 v2258 = Vector3.Transform(Vector3.UnitY, rot2258);

        public void DrawOverlay()
        {
            if (!OverlayVisible)
            {
                return;
            }

            ImGui.SetNextWindowPos(new Vector2(0, 0), ImGuiCond.Always);
            ImGui.SetNextWindowSize(ImGui.GetMainViewport().Size);
            if (ImGui.Begin("OverlayWindow", ref overlayVisible,
                ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoInputs))
            {
                if (actors != null && actors[0] != null)
                {
                    if (configuration.DrawCompass || configuration.DrawInterCompass ||
                        configuration.DrawSubInterCompass)
                    {

                        Vector3 pcPos = new Vector3(actors[0].Position.X, actors[0].Position.Y, actors[0].Position.Z);

                        compassPos = pcPos;

                        if (configuration.DrawCompass)
                        {
                            WriteText(compassPos - Vector3.UnitY * mainDistance, "N", mainColor, mainSize, 2, centered: true);
                            WriteText(compassPos + Vector3.UnitY * mainDistance, "S", mainColor, mainSize, 2, centered: true);
                            WriteText(compassPos + Vector3.UnitX * mainDistance, "E", mainColor, mainSize, 2, centered: true);
                            WriteText(compassPos - Vector3.UnitX * mainDistance, "W", mainColor, mainSize, 2, centered: true);
                        }

                        if (configuration.DrawInterCompass)
                        {
                            WriteText(compassPos - v451 * interDistance, "NE", interColor, interSize, 2, centered: true);
                            WriteText(compassPos - v452 * interDistance, "SE", interColor, interSize, 2, centered: true);
                            WriteText(compassPos - v453 * interDistance, "SW", interColor, interSize, 2, centered: true);
                            WriteText(compassPos - v454 * interDistance, "NW", interColor, interSize, 2, centered: true);
                        }

                        if (configuration.DrawSubInterCompass)
                        {
                            WriteText(compassPos - v2251 * subInterDistance, "NNE", subInterColor, subInterSize, 2, centered: true);
                            WriteText(compassPos - v2252 * subInterDistance, "ENE", subInterColor, subInterSize, 2, centered: true);
                            WriteText(compassPos - v2253 * subInterDistance, "ESE", subInterColor, subInterSize, 2, centered: true);
                            WriteText(compassPos - v2254 * subInterDistance, "SSE", subInterColor, subInterSize, 2, centered: true);
                            WriteText(compassPos - v2255 * subInterDistance, "SSW", subInterColor, subInterSize, 2, centered: true);
                            WriteText(compassPos - v2256 * subInterDistance, "WSW", subInterColor, subInterSize, 2, centered: true);
                            WriteText(compassPos - v2257 * subInterDistance, "WNW", subInterColor, subInterSize, 2, centered: true);
                            WriteText(compassPos - v2258 * subInterDistance, "NNW", subInterColor, subInterSize, 2, centered: true);
                        }
                    }
                }
            }
        }

        //To be drawn by setting hardcoded value to true
        public void DrawDebugWindow()
        {
            if (!DebugVisible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(375, 330), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSizeConstraints(new Vector2(375, 330), new Vector2(float.MaxValue, float.MaxValue));
            if (ImGui.Begin("Debug Window", ref this.debugVisible, ImGuiWindowFlags.NoScrollWithMouse))
            {
                ImGui.Spacing();

                if (actors != null && actors[0] != null)
                {
                    var actor = actors[0];
                    ImGui.Text(actor.Name);
                    ImGui.Text(actor.Position.X.ToString());
                    ImGui.Text(actor.Position.Y.ToString());
                    ImGui.Text(actor.Position.Z.ToString());
                    ImGui.Text(actorScreenPos.ToString());
                    long milliNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    ImGui.Text(milliNow.ToString());
                    if (actor.StatusEffects.Length > 0)
                        ImGui.LabelText("EffectId", actor.StatusEffects[0].EffectId.ToString());
                    ImGui.LabelText("TerritoryType", pi.ClientState.TerritoryType.ToString());
                    ImGui.Text(Convert.ToInt32(Convert.ToString((uint) 0x4001EB2F, 16), 16).ToString());
                    ImGui.Text(((uint) 0x4001EB2F).ToString());
                }
            }

            ImGui.End();
        }
        
        public void DrawSettingsWindow()
        {
            if (!SettingsVisible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(232, 300), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("Compass Configuration", ref settingsVisible,
                ImGuiWindowFlags.NoCollapse))
            {
                ImGui.Text("Overlay settings");
                ImGui.Checkbox("Compass", ref drawCompass);
                if (drawCompass)
                {
                    Vector4 v4Col = ColorUIntToVector4(mainColor);
                    
                    ImGui.ColorEdit4("Color##main", ref v4Col);
                    ImGui.DragFloat("Size##main", ref mainSize, 0.01f);
                    ImGui.DragFloat("Distance##main", ref mainDistance, 0.01f);

                    mainColor = Vector4ToUIntColor(v4Col);
                }

                ImGui.NewLine();
                ImGui.Checkbox("Inter-cardinal Compass", ref drawInterCompass);
                if (drawInterCompass)
                {
                    Vector4 v4Col = ColorUIntToVector4(interColor);

                    ImGui.ColorEdit4("Color##inter", ref v4Col);
                    ImGui.DragFloat("Size##inter", ref interSize, 0.01f);
                    ImGui.DragFloat("Distance##inter", ref interDistance, 0.01f);

                    interColor = Vector4ToUIntColor(v4Col);
                }

                ImGui.NewLine();
                ImGui.Checkbox("Sub-Inter-Cardinal Compass", ref drawSubInterCompass);
                if (drawSubInterCompass)
                {
                    Vector4 v4Col = ColorUIntToVector4(subInterColor);

                    ImGui.ColorEdit4("Color##subInter", ref v4Col);
                    ImGui.DragFloat("Size##subInter", ref subInterSize, 0.01f);
                    ImGui.DragFloat("Distance##subInter", ref subInterDistance, 0.01f);

                    subInterColor = Vector4ToUIntColor(v4Col);
                }

            }

            ImGui.End();
        }

        private Vector4 ColorUIntToVector4(uint col)
        {
            Vector4 v4Col = Vector4.Zero;
            v4Col.W = ((col & 0xFF000000) >> 24)/255f;
            v4Col.Z = ((col & 0x00FF0000) >> 16)/255f;
            v4Col.Y = ((col & 0x0000FF00) >> 8)/255f;
            v4Col.X = ((col & 0x000000FF) >> 0)/255f;
            return v4Col;
        }

        private uint Vector4ToUIntColor(Vector4 v4)
        {
            uint col = 0;
            col += (uint)(v4.W * 255) << 24;
            col += (uint)(v4.Z * 255) << 16;
            col += (uint)(v4.Y * 255) << 8;
            col += (uint)(v4.X * 255);
            return col;
        }
    }
}