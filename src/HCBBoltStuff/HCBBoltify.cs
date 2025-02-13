using BepInEx;
using BepInEx.Logging;
using FistVR;
using UnityEngine;

namespace nootarc
{

    [BepInAutoPlugin]
    [BepInProcess("h3vr.exe")]
    public partial class HCBBoltify : MonoBehaviour
    {
        public FVRFireArm FireArm;
        public GameObject Bolt;
        
        void Hook()
        {
            GM.CurrentSceneSettings.ShotFiredEvent += OnShotFired;
            TerribleScriptsBase.Log.LogInfo("HCBBoltify hook successful!");
        }

        private void OnShotFired(FVRFireArm firearm)
        {
            TerribleScriptsBase.Log.LogInfo("OnShotFired called!");
            if(FireArm != null && firearm == FireArm)
            {
                GameObject gameObject = Object.Instantiate<GameObject>(Bolt, firearm.MuzzlePos.position, firearm.MuzzlePos.rotation);
                HCBBolt component = gameObject.GetComponent<HCBBolt>();
                component.SetCookedAmount(1f);
                component.Fire(firearm.MuzzlePos.forward, firearm.MuzzlePos.position, 1f);
                TerribleScriptsBase.Log.LogInfo($"Fire @ {firearm.MuzzlePos.position}!");
            }
        }
        private void Awake()
        {
            Hook();
        }
    }
}
