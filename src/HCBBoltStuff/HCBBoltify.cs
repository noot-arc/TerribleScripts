using FistVR;
using UnityEngine;
using BepInEx;

namespace nootarc.HCBBoltStuff
{
    public class HCBBoltify : MonoBehaviour
    {
        public FVRFireArm FireArm;
        public FVRFireArmChamber Chamber;
        
        void Hook()
        {
            GM.CurrentSceneSettings.ShotFiredEvent += OnShotFired;
        }

        private void OnShotFired(FVRFireArm firearm)
        {
            if(FireArm != null && firearm == FireArm)
            {

                FVRFireArmRound fVRFireArmRound = Chamber.GetRound();
                RoundHCBBoltType roundBoltType = fVRFireArmRound.GetComponent<RoundHCBBoltType>();
                GameObject Bolt = roundBoltType.RoundHCBBolt;
                GameObject gameObject = Object.Instantiate<GameObject>(Bolt, firearm.CurrentMuzzle.position, firearm.CurrentMuzzle.rotation);
                HCBBolt component = gameObject.GetComponent<HCBBolt>();
                component.SetCookedAmount(1f);
                component.Fire(firearm.MuzzlePos.forward, firearm.MuzzlePos.position, 1f);
            }
        }
        public void Awake()
        {
            Hook();
        }
    }
}
