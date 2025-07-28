using FistVR;
using UnityEngine;
// This is an evil script. nothing here has been written in a sober state of mind. :)
// It's also horrendously optimised. I can't see any immediate ways to optimise it but its been done under a time crunch
// sowwy
namespace TerribleScripts.WeaponModifications
{
    public class SparksAndHeat : MonoBehaviour
    {
        [Header("99.5% chance you don't need this script. If you hit the 0.5%, I'm so sorry. At least you can just pick one effect")]
        [Tooltip("Firearm with all the effects")]
        public FVRFireArm FireArm;
        [Tooltip("Round Class that will enable all the effects")]
        public FireArmRoundClass RoundClass;
        [Tooltip("Particle System that will enable and disable on Round Class loaded")]
        public ParticleSystem Particles;
        [Tooltip("Audio Source that will play and stop")]
        public AudioSource Audio;
        [Tooltip("Mesh to modify the Emissions Weight of (Kinda like Object Temperature without the Object Temperature)")]
        public MeshRenderer Mesh;
        [Tooltip("Emissions Weight when the Round Class is Loaded")]
        public float LoadedWeight = 1f;
        [Tooltip("Vice versa as above")]
        public float UnloadedWeight;
        [HideInInspector] public int EmissionWeight;
        [HideInInspector] public bool JustPlayed;
        [HideInInspector] public bool MeshAvailable;
        [HideInInspector] public bool AudioAvailable;
        [HideInInspector] public bool ParticlesAvailable;

        public void Awake()
        {
            EmissionWeight = Shader.PropertyToID("_EmissionWeight"); // name checks get expensive
            if (Mesh != null) MeshAvailable = true;
            if (Audio != null) AudioAvailable = true;
            if (Particles != null) ParticlesAvailable = true;
        }

        public void Update()
        {
                var PropBlock = new MaterialPropertyBlock();
                    if (FireArm.Magazine != null && FireArm.Magazine.LoadedRounds[0] != null && FireArm.Magazine.LoadedRounds[0].LR_Class == RoundClass) // im not explaining this one
                    {
                        if (JustPlayed) return; // this ensures that the effects are only fired on one frame
                        PropBlock.SetFloat(EmissionWeight, LoadedWeight);
                        if (MeshAvailable) Mesh.SetPropertyBlock(PropBlock);
                        if (ParticlesAvailable) Particles.Play();
                        if (AudioAvailable) Audio.Play();
                        JustPlayed = true;
                    }
                    else if (FireArm.Magazine == null || FireArm.Magazine.m_numRounds == 0 || FireArm.Magazine.LoadedRounds[0].LR_Class != RoundClass || FireArm.m_hand == null) // nor am i explaining this one
                    {
                        if (!JustPlayed) return; // vice versa; disabled only in one frame (for multiple frames presumably)
                        PropBlock.SetFloat(EmissionWeight, UnloadedWeight);
                        if (MeshAvailable) Mesh.SetPropertyBlock(PropBlock);
                        if (ParticlesAvailable) Particles.Stop();
                        if (AudioAvailable) Audio.Stop();
                        JustPlayed = false;
                    }
        }
    }
}