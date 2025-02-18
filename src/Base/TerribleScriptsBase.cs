using BepInEx;
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
            var customFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "TerribleScripts.cfg"), true);
        }
    }
}
