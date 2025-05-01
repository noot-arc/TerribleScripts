using UnityEngine;

//This code was made to increase parity between the Display Data & actual ballistics of HCB Bolts by recalculating the
//BDCC on both; the vanilla implementation is to manually adjust the DD's speed and weight values. and something else.
//not recommended for usage since it Will break stuff
namespace nootarc
{
    [CreateAssetMenu(menuName = "Misc/CalculateBDCC", fileName = "NewBDCCurve")]
    public class ScaleBDCC : ScriptableObject
    {
        [ContextMenu("Scale Curve")]
        public void ScaleCurve()
        {
            {
                for (int i = 0; i < curve.keys.Length; i++)
                {
                    Keyframe keyframe = curve.keys[i];
                    keyframe.value = curve.keys[i].value * Height;
                    keyframe.time = curve.keys[i].time * Width;
                    keyframe.inTangent = curve.keys[i].inTangent * Height / Width;
                    keyframe.outTangent = curve.keys[i].outTangent * Height / Width;

                    scaledCurve.AddKey(keyframe);
                }
                return;
            }
        }
        public AnimationCurve curve;
        public AnimationCurve scaledCurve;
        public float Width = 1f;
        public float Height = 0.1f;
    }
}