using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace TerribleScripts.Animations
{
    public class AnimPlayOnAXPress : MonoBehaviour
    {
        [Tooltip("Put your animations in a sequential list; they will play on every press and loop back around to first position")]
        public List<string> Animations;
        [Tooltip("Animation controller thingy on your object")]
        public Animation Animation;
        [Tooltip("Object you're pressing the buttons on")]
        public FVRInteractiveObject Object;
        [Tooltip("Whether the animation should reset on every button press. Leave true if using with fire selectors/other things")]
        public bool Reset = true;
        [Tooltip("Should the animation play a FireSelector sound when it starts?")]
        public bool PlaySound;
        private int i;
        private FVRFireArm Firearm;

        public void Awake()
        {
            if (Object is FVRFireArm)
            { 
                Firearm = Object as FVRFireArm;
            }
        }
        
        public void Update()
        {
            if (Object.m_hand != null)
            {
                if (Object.m_hand.Input.AXButtonDown ||
                    (Object.m_hand.Input.TouchpadDown && Object.m_hand.Input.TouchpadAxes.magnitude > 0.1f &&
                     Vector2.Angle(Object.m_hand.Input.TouchpadAxes, Vector2.left) <= 45f)) //check for both streamlined and non-streamlined inputs
                {
                    for (var j = 0; j < Animations.Count; j++)
                    { if (!Reset && Animation.IsPlaying(Animations[j])) return; } //if one of the animations is playing, don't play another one until its done
                    Animation.Play(Animations[i]);
                    i++;
                    if (PlaySound && Object is FVRFireArm)
                    { Firearm.PlayAudioEvent(FirearmAudioEventType.FireSelector); }
                    if (i >= Animations.Count) i = 0; //make sure the animations played are sequential and looping
                }
            }
        }
    }
}