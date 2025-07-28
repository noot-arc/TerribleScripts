using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace TerribleScripts.WeaponModifications
{
    public class AlternatingMuzzle : MonoBehaviour
    {
        [Header("Note: Firearm should have the first muzzle in this list as the muzzle position")]
        [Tooltip("Firearm with all the muzzle shenanigans")]
        public FVRFireArm FireArm;
        [Tooltip("Your muzzles. Top-down will dictate the order the muzzles are fired in. Note the header!")]
        public List<Transform> Muzzles;
        [HideInInspector] public int Index;

        [Header("Closed Bolt Moving Barrel stuff")]
        [Tooltip("Reciprocating/Moving barrels to do")]
        public List<Transform> Barrels;
        [Tooltip("Forward Positions of the Barrels. Note the order of the list above.")]
        public List<Vector3> BarrelsForward;
        [Tooltip("Same as above, Rearward Positions of the Barrels.")]
        public List<Vector3> BarrelsRearward;
        [HideInInspector] public bool IsClosedBolt;
        [HideInInspector] public ClosedBoltWeapon ClosedBolt;
        [HideInInspector] public bool MovingBarrelsAvailable;
        
        public void Hook() { GM.CurrentSceneSettings.ShotFiredEvent += OnShotFired; }

        public void Awake()
        {
            Hook();
            if (FireArm is ClosedBoltWeapon) // make sure that no weird conversions happen
            {
                ClosedBolt = FireArm as ClosedBoltWeapon;
                IsClosedBolt = true;
                if (Barrels != null) MovingBarrelsAvailable = true; // dont touch the barrels if the weapon is closed bolt but the barrels list hasn't been filled in
            }
        }

        public void OnShotFired(FVRFireArm firearm)
        {
            if (firearm == FireArm)
            {
                FireArm.MuzzlePos = FireArm.CurrentMuzzle = Muzzles[Index]; // stuff in the firearm gets calculated off of CurrentMuzzle. i forgot why MuzzlePos is there
                if (IsClosedBolt && MovingBarrelsAvailable)
                {
                    ClosedBolt.Bolt.Barrel = Barrels[Index];
                    ClosedBolt.Bolt.BarrelForward = BarrelsForward[Index];
                    ClosedBolt.Bolt.BarrelRearward = BarrelsRearward[Index];
                }
                Index++; // we can be confident that OnShotFired is called once per frame. otherwise this would break :)
                if (Index >= Muzzles.Count) Index = 0;
            }
        }
    }
}