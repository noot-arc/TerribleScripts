using BepInEx;

namespace TerribleScripts
{
    [BepInAutoPlugin]
    [BepInProcess("h3vr.exe")]
    public partial class TerribleScriptsBase : BaseUnityPlugin
    { 
        public enum StreamlinedButton { AX, BY }
        public enum TouchpadVector { up, down, left, right }
    }
}
