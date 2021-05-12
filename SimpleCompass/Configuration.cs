using System;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace SimpleCompass
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public bool DrawCompass { get; set; } = false;
        public bool DrawInterCompass { get; set; } = false;
        public bool DrawSubInterCompass { get; set; } = false;
        
        public uint MainColor { get; set; } = 0xFFFFFFFF;
        public uint InterColor { get; set; } = 0xFFDDDDDD;
        public uint SubInterColor { get; set; } = 0xFF999999;

        public float MainSize { get; set; } = 32;
        public float InterSize { get; set; } = 20;
        public float SubInterSize { get; set; } = 16;

        public float MainDistance { get; set; } = 1;
        public float InterDistance { get; set; } = 1;
        public float SubInterDistance { get; set; } = 1;


        // the below exist just to make saving less cumbersome

        [NonSerialized]
        private DalamudPluginInterface pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.pluginInterface.SavePluginConfig(this);
        }
    }
}
