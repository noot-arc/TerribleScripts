using FistVR;
using UnityEngine;

namespace nootarc.WeaponModStuff;

//Drop Hammer Continuously on Specific Round Type In Chamber. Idk how else to name it. It also sucks. Help.
public class DropHammerContOnRnd : MonoBehaviour
{
    public FVRFireArmRound Round;
    public FVRFireArmChamber Chamber;
    public int Counter = 3;

    public void DropHammer()
    {
        while (true)
        {
            if (Chamber.GetRound() == Round && Counter > 0)
            {
                Counter--;
                continue;
            }
            break;
        }
    }
}