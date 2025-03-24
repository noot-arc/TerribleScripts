using UnityEngine;
using FistVR;

namespace nootarc.WeaponModifications;

public class IntegratedMagazineButtonReplenish : MonoBehaviour
{
    [Tooltip("The round class that should be replenished by default if the gun spawns empty")] public FireArmRoundClass FallbackRoundClass;
    public FVRFireArm firearm;
    [Tooltip("Delay in seconds between loading rounds")] public float replenishRate = 0.066f;
    public bool ShouldMakeSoundOnIndividualRoundReplenish;
    [HideInInspector] public bool isReplenishing = true;
    [HideInInspector] public float replenishTimer;
    [HideInInspector] public FireArmRoundClass RoundClass;
    private FVRViveHand hand;

    public void Awake()
    {
        RoundClass = FallbackRoundClass;
    }
    public void Update()
    {
        hand = firearm.m_hand;
        if (RoundClass != firearm.FChambers[0].GetRound().RoundClass && firearm.FChambers[0].GetRound() != null)
        {
            RoundClass = firearm.FChambers[0].GetRound().RoundClass;
        }
        if (hand != null)
        {
            if (hand.IsInStreamlinedMode && hand.Input.AXButtonDown || 
                !hand.IsInStreamlinedMode && hand.Input.TouchpadDown && Vector2.Angle(hand.Input.TouchpadAxes, Vector2.down) < 45f)
            {
                isReplenishing = true;
            }
            else if (hand.IsInStreamlinedMode && !hand.Input.AXButtonDown || 
                     !hand.IsInStreamlinedMode && !hand.Input.TouchpadDown)
            {
                isReplenishing = false;
            }
            if (replenishTimer > 0)
            {
                this.replenishTimer -= Time.deltaTime;
            }
            if (isReplenishing && this.replenishTimer <= 0)
            {
                this.firearm.Magazine.AddRound(RoundClass, ShouldMakeSoundOnIndividualRoundReplenish, false);
                replenishTimer = replenishRate;
            }
        }
    }
}