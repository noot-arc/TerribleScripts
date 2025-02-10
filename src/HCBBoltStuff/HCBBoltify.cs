using BepInEx;
using BepInEx.Logging;
using FistVR;
using UnityEngine;

namespace nootarc
{

    [BepInAutoPlugin]
    [BepInProcess("h3vr.exe")]
    public partial class HCBBoltify : BaseUnityPlugin
    {
        public FVRFireArm FireArm;
        public GameObject Bolt;
        
        void Hook()
        {
            GM.CurrentSceneSettings.ShotFiredEvent += OnShotFired;
            //Logger.LogMessage($"HCBBoltify hook successful!");
        }

        private void OnShotFired(FVRFireArm firearm)
        {
            //Logger.LogMessage("OnShotFired called!");
            if(FireArm != null && firearm == FireArm)
            {
                GameObject gameObject = Object.Instantiate<GameObject>(Bolt, firearm.MuzzlePos.position, firearm.MuzzlePos.rotation);
                HCBBolt component = gameObject.GetComponent<HCBBolt>();
                component.Fire(firearm.MuzzlePos.forward, firearm.MuzzlePos.position, 1f);
                //Logger.LogMessage("Fire!");
                component.SetCookedAmount(1f);
            }
        }
        private void Awake()
        {
            Logger = base.Logger;
            Hook();
            //Logger.LogMessage($"HCBBoltify is awake!");
        }
        internal new static ManualLogSource Logger { get; private set; }
    }
}
