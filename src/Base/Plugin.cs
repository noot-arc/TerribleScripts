using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using System.IO;
using System.Drawing.Text;
using UnityEngine.Windows.Speech;
using Steamworks;

namespace nootarc
{
    [BepInAutoPlugin]
    [BepInProcess("h3vr.exe")]
    public partial class TerribleScriptsBase : BaseUnityPlugin
    {
        private ConfigEntry<bool> configDebug;
        internal static ManualLogSource Log;
        private void Awake()
        {
            configDebug = Config.Bind("General", "DebugState", false, "Enable debug messages?");
            if (configDebug.Value)
            {
                Log = base.Logger;
            }
            var customFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "TerribleScripts.cfg"), true);
        }
        
        internal new static ManualLogSource Logger { get; private set; }
    }
}
