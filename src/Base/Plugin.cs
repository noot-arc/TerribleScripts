using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using System.IO;

namespace nootarc
{
    [BepInAutoPlugin]
    [BepInProcess("h3vr.exe")]
    public partial class TerribleScriptsBase : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger = base.Logger;
            var customFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "TerribleScripts.cfg"), true);
        }
        
        internal new static ManualLogSource Logger { get; private set; }
    }
}
