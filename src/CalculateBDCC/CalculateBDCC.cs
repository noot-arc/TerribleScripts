using UnityEngine;

//deprecated
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