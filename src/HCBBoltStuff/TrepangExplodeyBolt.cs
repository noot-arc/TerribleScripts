using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace nootarc.HCBBoltStuff;

public class TrepangExplodeyBolt : MonoBehaviour
{
    private float fuseTime = 3f;
    private List<GameObject> ExplosionSpawns;
    public int IFF;
    
    public void FVRUpdate()
    {
        fuseTime -= Time.deltaTime;
        if (fuseTime <= 0)
        {
            foreach (var t in ExplosionSpawns)
            {
                GameObject instantiate =
                    Instantiate(t, transform.position, Quaternion.identity);
                Explosion component = instantiate.GetComponent<Explosion>();
                if (component != null)
                {
                    component.IFF = IFF;
                }
                ExplosionSound component2 = instantiate.GetComponent<ExplosionSound>();
                if (component2 != null)
                {
                    component2.IFF = IFF;
                }
            }

            Destroy(this);
        }
    }
}