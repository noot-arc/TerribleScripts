using FistVR;
using UnityEngine;

namespace TerribleScripts.WeaponModifications
{
    public class EjectMagOnEmpty : MonoBehaviour 
    // this is a very simple script. i honestly dont like it just because of that.
    // feels like all of this could be in the ammo count stuff. oh well 
    {
        [Tooltip("Firearm :)")]
        public FVRFireArm FireArm;
        [HideInInspector] public float CurrentAmount;
        [HideInInspector] public int LoadedChambers;
        [HideInInspector] public bool MagIn;
        private void Update()
        {
            for (var i = 0; i < FireArm.FChambers.Count; i++)
            {
                if (FireArm.FChambers[i].IsFull)
                {
                    LoadedChambers++;
                }
            }
            if (FireArm.Magazine != null)
            {
                CurrentAmount = FireArm.Magazine.m_numRounds + LoadedChambers;
                MagIn = true;
            }

            if (MagIn && CurrentAmount <= 0)
            {
                FireArm.EjectMag();
            }

            LoadedChambers = 0;
        }
    }
}