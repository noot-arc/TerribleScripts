using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;


namespace TerribleScripts.Animations
{
    public class AnimPlayOnFire : MonoBehaviour
    {
        [Header("NOTE: i forgot the note that i was supposed to put here")]
        [Tooltip("Firearm to apply animations to")]
        public FVRFireArm Firearm;
        [Tooltip("Animation script thing with all the animations (should be applied to the object)")]
        public Animation Animation;
        [Tooltip("Animation to play on shot fired")]
        public string FireAnim;
        [Tooltip("True: Animation should reset on another shot during the already-playing animation runtime. Continues to play on false")]
        public bool Reset;
        
        public void Hook() { GM.CurrentSceneSettings.ShotFiredEvent += OnShotFired; }
        public void Awake() { Hook(); }
        
        public void OnShotFired(FVRFireArm firearm)
        {
            if (firearm == Firearm)
            {
                if (!Reset && Animation.IsPlaying(FireAnim)) return;
                Animation.Play(FireAnim); //thats literally it. all the """Fun""" bits are in the other scripts!
            }
        }
    }
}