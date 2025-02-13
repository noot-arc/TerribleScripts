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
        }

        private void OnShotFired(FVRFireArm firearm)
        {
            if(FireArm != null && firearm == FireArm)
            {
                
                GameObject gameObject = Object.Instantiate<GameObject>(Bolt, firearm.CurrentMuzzle.position, firearm.CurrentMuzzle.rotation);
                HCBBolt component = gameObject.GetComponent<HCBBolt>();
                component.SetCookedAmount(1f);
                component.Fire(firearm.MuzzlePos.forward, firearm.MuzzlePos.position, 1f);
            }
        }
        private void Awake()
        {
            Hook();
        }
    }
}
