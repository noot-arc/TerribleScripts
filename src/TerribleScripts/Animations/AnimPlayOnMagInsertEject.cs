using FistVR;
using UnityEngine;

namespace TerribleScripts.Animations
{
    public class AnimPlayOnMagInsertEject : MonoBehaviour
    {
        [Tooltip("yo  ur  firearm rr")]
        public FVRFireArm Firearm;
        [Tooltip(" the animation thing controller thing that go into the. firearm")]
        public Animation Animation;
        [Tooltip("Animation that plays when the magazine is inserted.")]
        public string InsertAnim;
        [Tooltip("animnaitnon that plays twhen the mag is eject ted")]
        public string EjectAnim;
        private bool MagIn;

        public void Update()
        {
            if (!MagIn && Firearm.Magazine != null)
            {
                MagIn = true;
                Animation.Play(InsertAnim);
            }
            else if (MagIn && Firearm.Magazine == null)
            {
                MagIn = false;
                Animation.Play(EjectAnim);
            }
        }
    }
}