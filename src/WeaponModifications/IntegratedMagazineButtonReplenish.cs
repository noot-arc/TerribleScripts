using UnityEngine;
using FistVR;

namespace nootarc.WeaponModifications;

public class IntegratedMagazineButtonReplenish : MonoBehaviour
{
    public FireArmRoundClass RoundClass;
    public FVRFireArm firearm;
    public float replenishRate;
    [HideInInspector] public bool isReplenishing = false;
    [HideInInspector] public float replenishTimer = 0;
    private FVRViveHand hand;

    public void Update()
    
    {
        hand = firearm.m_hand;
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
        }
        if (replenishTimer > 0)
        {
            this.replenishTimer -= Time.deltaTime;
        }
        if (isReplenishing && this.replenishTimer <= 0)
        {
            this.firearm.Magazine.AddRound(RoundClass, false, false);
            replenishTimer = replenishRate;
        }
    }
}