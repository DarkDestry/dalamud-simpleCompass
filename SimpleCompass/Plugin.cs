using System.Collections.Generic;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using SamplePlugin;

namespace SimpleCompass
{
    public class Plugin : IDalamudPlugin
    {
        public string Name => "Simple Compass";

        private const string commandName = "/compass";
        private const string commandHelp = "Open configuration window";

        private DalamudPluginInterface pi;
        private Configuration configuration;
        private PluginUI ui;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pi = pluginInterface;
            GizmoUtil.pi = pi;

            this.configuration = this.pi.GetPluginConfig() as Configuration ?? new Configuration();
            this.configuration.Initialize(this.pi);

            this.ui = new PluginUI(this.configuration, pi);

            this.pi.CommandManager.AddHandler(commandName, new CommandInfo((s, arguments) => DrawConfigUI())
            {
                HelpMessage = commandHelp
            });

            this.pi.UiBuilder.OnBuildUi += DrawUI;
            this.pi.UiBuilder.OnOpenConfigUi += (sender, args) => DrawConfigUI();
        }

        public void Dispose()
        {
            this.ui.Dispose();
            
            this.pi.CommandManager.RemoveHandler(commandName);

            this.pi.Dispose();
        }

        private void DrawUI()
        {
            this.ui.Draw();

            ui.actors = pi.ClientState.Actors;
        }

        private void DrawConfigUI()
        {
            this.ui.SettingsVisible = true;
        }
    }
}
