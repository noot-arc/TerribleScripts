using FistVR;
using UnityEngine;

namespace nootarc.HCBBoltStuff
{
    public class HCBBoltify : MonoBehaviour
    {
        public FVRFireArm FireArm;
        public FVRFireArmChamber Chamber;
        
        void Hook() // i dont know why this works, because it doesnt in any other scripts. oh well
        {
            GM.CurrentSceneSettings.ShotFiredEvent += OnShotFired;
        }

        private void OnShotFired(FVRFireArm firearm)
        {
            //round needs to have the RoundHCBBoltType component, the companion script to this one.
            //cant be bothered to implement it a-la vanilla (as that would require accounting for double-shots when both
            //the round's ballistic projectile and the gun's inherent hcb fire. lazy
            if(firearm == FireArm && (Chamber.GetRound().GetComponent<RoundHCBBoltType>() != null))
            {

                FVRFireArmRound fVRFireArmRound = Chamber.GetRound();
                RoundHCBBoltType roundBoltType = fVRFireArmRound.GetComponent<RoundHCBBoltType>();
                GameObject Bolt = roundBoltType.RoundHCBBolt;
                GameObject instantiate = Object.Instantiate(Bolt, firearm.CurrentMuzzle.position, firearm.CurrentMuzzle.rotation);
                HCBBolt component = instantiate.GetComponent<HCBBolt>();
                component.SetCookedAmount(1f); //HCB Bolts inherently have the sustenance's cooking mechanic. bypass it like vanilla
                component.Fire(firearm.MuzzlePos.forward, firearm.MuzzlePos.position, 1f);
            }
        }
        public void Awake()
        {
            Hook();
        }
    }
}
