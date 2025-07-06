using UnityEngine;
using FistVR;

namespace TerribleScripts.WeaponModifications
{
    public class ReplenishFirearmMagazineAutomagically : MonoBehaviour
    {
        [Header("NOTE: This script is used with firearms, not magazines!")]
        [Tooltip("The round class that should be replenished by default if the gun spawns empty")] public FireArmRoundClass FallbackRoundClass = FireArmRoundClass.FMJ;
        public FVRFireArm firearm;
        [Tooltip("Makes the magazine only replenish when the firearm is being held and the AX button is being pressed. If false, firearm replenishes when *not* held, and stops when held. ")] public bool HoldToReplenish;
        [Tooltip("*Only functions when HoldToReplenish is false* Clicking the AX button toggles the replenish-when-not-held feature on and off.")] public bool ToggleToReplenish;
        [Tooltip("Should the trigger make the firearm stop replenishing")] public bool TriggerBlocksReplenish;
        [Header("Delays")]
        [Tooltip("Delay in seconds between loading rounds at the fastest")] public float minReplenishRate = 0.066f;
        [Tooltip("Delay in seconds between loading rounds at the slowest")] public float maxReplenishRate = 5f;
        [Tooltip("Smaller the number, the longer the delay, and vice versa. 0 to bypass lerp and replenish at max, 1 to replenish at min")] public float lerpFactor = 0.5f;
        [Header("Audio")]
        [Tooltip("Makes the mag insert round sound every time a round is replenished")] public bool IndividualRoundSound;
        [Tooltip("OPTIONAL: audio source w/ clip that will play when the gun is replenishing, e.g. if you want to have a quiet loop instead of CLICK CLICK CLICK CLICK CLICK")] public AudioSource replenishAudioSource;
        [HideInInspector] public bool isReplenishing;
        [HideInInspector] public float replenishTimer;
        [HideInInspector] public FireArmRoundClass RoundClass;
        [HideInInspector] public FVRViveHand hand;

        public void Awake()
        {
            RoundClass = FallbackRoundClass; //so nothing breaks if the gun isn't pre-loaded
        }
        
        //not a fan of the entire thing being in Update()
        public void Update()
        {
            if (firearm.FChambers[0].GetRound() != null && RoundClass != firearm.FChambers[0].GetRound().RoundClass)
            {
                //wish this could be done outside of Update(), not aware of a Chamber Update event or similar
                RoundClass = firearm.FChambers[0].GetRound().RoundClass;
            }
            
            if (isReplenishing && replenishTimer > 0)
            {
                replenishTimer -= Time.deltaTime;
            }
            else if (isReplenishing && replenishTimer <= 0)
            {
                firearm.Magazine.AddRound(RoundClass, IndividualRoundSound, false);
                replenishTimer = Mathf.Lerp(replenishTimer, minReplenishRate, lerpFactor);
            }

            if (replenishAudioSource != null)
            {
                //this probably doesn't work. no clue how to make them play once, repeat while rplnsh & stop immediately
                if (isReplenishing && !replenishAudioSource.isPlaying) replenishAudioSource.Play();
                else replenishAudioSource.Stop();
            }

            if (HoldToReplenish) isReplenishing = false;
            //↑ the duo of these lines is the worst part of this script. knowing exactly where to return so everything
            //↓ works the way its supposed to without frame-length buffers where the player can break the functionality
            if (firearm.m_hand == null && (HoldToReplenish || ToggleToReplenish)) return;


            if (HoldToReplenish)
            {
                hand = firearm.m_hand;
                if (TriggerBlocksReplenish && hand.Input.TriggerFloat < 0.1 || !TriggerBlocksReplenish)
                {
                    if (hand.Input.AXButtonDown) isReplenishing = true; // todo: classic
                    else if (hand.Input.AXButtonUp) isReplenishing = false;
                }
                else isReplenishing = false;
            }
            else if (ToggleToReplenish)
            {
                hand = firearm.m_hand;
                if (hand.Input.AXButtonDown) isReplenishing = !isReplenishing; // simple toggle
            }
            else isReplenishing = !firearm.IsHeld; // this feels... wrong...
        }
    
    }
}