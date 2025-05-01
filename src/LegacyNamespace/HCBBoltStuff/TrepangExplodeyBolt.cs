using System.Collections.Generic;
using FistVR;
using UnityEngine;

//i forgot how this code works, aside from having a bunch of expensive calls.
//appropriated the sound and particle stuff from the multi-timing hand grenade. i think.
namespace nootarc.HCBBoltStuff
{
    public class TrepangExplodeyBolt : MonoBehaviour
    {
        public ParticleSystem fuseParticles;
        public AudioEvent fuseSound;
        [HideInInspector] public float fuseTime = 3f;
        public float startFuseTime = 3f;
        public List<GameObject> ExplosionSpawns;
        [HideInInspector] public int IFF;
        [HideInInspector] public float fuseTick;

        public void StartExplosion()
        {
            fuseTime -= Time.deltaTime;
            var num2 = Mathf.Clamp(1f - fuseTime / startFuseTime, 0f, 1f);
            num2 = Mathf.Pow(num2, 2f);
            if (fuseTick <= 0f)
            {
                fuseTick = Mathf.Lerp(0.3f, 0.01f, num2);
                var num3 = Mathf.Lerp(1.8f, 3.7f, num2);
                SM.PlayCoreSoundOverrides(FVRPooledAudioType.Generic, fuseSound, transform.position, new Vector2(1f, 1f),
                    new Vector2(num3, num3));
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
                    var instantiate =
                        Instantiate(t, transform.position, Quaternion.identity);
                    var component = instantiate.GetComponent<Explosion>();
                    if (component != null) component.IFF = IFF;
                    var component2 = instantiate.GetComponent<ExplosionSound>();
                    if (component2 != null) component2.IFF = IFF;
                }

                Destroy(gameObject);
            }
        }

        public void Update()
        {
            if (!gameObject.GetComponent<HCBBolt>() || !gameObject.GetComponent<HCBBolt>().m_isFlying)
                StartExplosion();
        }
    }
}