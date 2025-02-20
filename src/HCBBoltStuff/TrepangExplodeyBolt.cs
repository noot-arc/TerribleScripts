using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace nootarc.HCBBoltStuff;

public class TrepangExplodeyBolt : MonoBehaviour
{
    public ParticleSystem fuseParticles;
    public AudioEvent fuseSound;
    public float fuseTime = 3f;
    public float startFuseTime = 3f;
    public List<GameObject> ExplosionSpawns;
    public int IFF;
    public float fuseTick;
    
    public void Update()
    {
        fuseTime -= Time.deltaTime;
        float num2 = Mathf.Clamp(1f - fuseTime / startFuseTime, 0f, 1f);
        num2 = Mathf.Pow(num2, 2f);
        if (fuseTick <= 0f)
        {
            fuseTick = Mathf.Lerp(0.3f, 0.01f, num2);
            float num3 = Mathf.Lerp(1.8f, 3.7f, num2);
            SM.PlayCoreSoundOverrides(FVRPooledAudioType.Generic, fuseSound, transform.position, new Vector2(1f, 1f), new Vector2(num3, num3));
            fuseParticles.Emit(2);
        }
        else
        {
            fuseTick -= Time.deltaTime;
        }
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

            Object.Destroy(base.gameObject);
        }
    }
}