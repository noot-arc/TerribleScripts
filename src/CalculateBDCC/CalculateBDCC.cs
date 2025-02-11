using BepInEx;
using FistVR;
using UnityEngine;

namespace nootarc
{
    public partial class ScaleBDCC : ScriptableObject
    {
        [ContextMenu("CalculateBDCC")]
        public void ScaleCurve(AnimationCurve curve, float maxX, float maxY)
        {
            for (int i = 0; i < curve.keys.Length; i++)
            {
                Keyframe keyframe = curve.keys[i];
                keyframe.value = curve.keys[i].value * maxY;
                keyframe.time = curve.keys[i].time * maxX;
                keyframe.inTangent = curve.keys[i].inTangent * maxY / maxX;
                keyframe.outTangent = curve.keys[i].outTangent * maxY / maxX;

                scaledCurve.AddKey(keyframe);
            }
            this.scaledCurve = new AnimationCurve();
        }
        public AnimationCurve curve;
        public AnimationCurve scaledCurve;
        public float maxX = 1f;
        public float maxY = 0.1f;
    }
}