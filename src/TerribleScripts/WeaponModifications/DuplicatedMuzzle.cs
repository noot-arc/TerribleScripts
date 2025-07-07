using FistVR;
using UnityEngine;
//this script sucks and all of the code is just the exact same way that vanilla instantiates rounds
//use it if you wanna add more muzzles that fire at the same time as the others without actually going through
//anything that would be needed to achieve that
namespace TerribleScripts.WeaponModifications
{
    public class DuplicatedMuzzle : MonoBehaviour
    {
        public FVRFireArm FireArm;
        public void Hook() //i still dont know what this does
        {
            GM.CurrentSceneSettings.ShotFiredEvent += OnShotFired;
        }

        public void Awake()
        {
            Hook();
        }

        public void OnShotFired(FVRFireArm firearm)
        {
            if (firearm == FireArm) //yeah this is basically the only addition
            {
                float chamberVelMult = AM.GetChamberVelMult(firearm.FChambers[0].RoundType, Vector3.Distance(firearm.FChambers[0].transform.position, firearm.CurrentMuzzle.position));
                float num = firearm.GetCombinedFixedDrop(firearm.AccuracyClass) * 0.0166667f;
                Vector2 vector = firearm.GetCombinedFixedDrift(firearm.AccuracyClass) * 0.0166667f;
                for (int i = 0; i < firearm.FChambers[0].GetRound().NumProjectiles; i++)
                {
                    float num2 = firearm.FChambers[0].GetRound().ProjectileSpread + firearm.m_internalMechanicalMOA + firearm.GetCombinedMuzzleDeviceAccuracy();
                    if (firearm.FChambers[0].GetRound().BallisticProjectilePrefab != null)
                    {
                        Vector3 vector2 = transform.forward * 0.005f;
                        GameObject instantiate = Instantiate(firearm.FChambers[0].GetRound().BallisticProjectilePrefab, transform.position - vector2, transform.rotation);
                        Vector2 vector3 = (Random.insideUnitCircle + Random.insideUnitCircle + Random.insideUnitCircle) * 0.33333334f * num2;
                        instantiate.transform.Rotate(new Vector3(vector3.x + vector.y + num, vector3.y + vector.x, 0f));
                        BallisticProjectile component = instantiate.GetComponent<BallisticProjectile>();
                        component.Fire(component.MuzzleVelocityBase * firearm.FChambers[0].ChamberVelocityMultiplier * 1f * chamberVelMult, instantiate.transform.forward, firearm);
                    }
                }
            }
        }
    }
}