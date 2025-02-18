using FistVR;
using UnityEngine;

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
            if(firearm == FireArm && (Chamber.GetRound().GetComponent<RoundHCBBoltType>() != null))
            {

                FVRFireArmRound fVRFireArmRound = Chamber.GetRound();
                RoundHCBBoltType roundBoltType = fVRFireArmRound.GetComponent<RoundHCBBoltType>();
                GameObject Bolt = roundBoltType.RoundHCBBolt;
                GameObject instantiate = Object.Instantiate(Bolt, firearm.CurrentMuzzle.position, firearm.CurrentMuzzle.rotation);
                HCBBolt component = instantiate.GetComponent<HCBBolt>();
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
