using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using FistVR;

namespace TerribleScripts.TerribleScripts.ModMods
{
    public class AmmoCountUIColorShenanigans : MonoBehaviour
    {
        [Header("Note: There should be one gradient per object with them having the same positions (topmost object will use the topmost gradient, for example)")]
        [Tooltip("Graphics to recolor")] 
        public List<Graphic> Graphics;
        [Tooltip("Gradients to recolor the graphics with; key position is the fullness factor (e.g 0.2f is 20% capacity remaining)")]
        public List<Gradient> Gradients;
        [Tooltip("Firearm :)")]
        public FVRFireArm FireArm;
        [Tooltip("Does your firearm have/uses magazines or is it direct chamber-loaded?")]
        public bool UsesMags;
        [HideInInspector] public int MaxCap;
        [HideInInspector] public float CurrentAmount;
        [HideInInspector] public int LoadedChambers;
        [HideInInspector] public float CurrentPercentage;
        [HideInInspector] public Color UIColor;

        public void Update()
        {
            if (FireArm.Magazine != null)
            {
                MaxCap = FireArm.Magazine.m_capacity;
                UsesMags = true;
            }
            else if (UsesMags == false)
            {
                MaxCap = FireArm.FChambers.Count;
            }
            else MaxCap = 1;
            
            for (int i = 0; i < FireArm.FChambers.Count; i++)
            {
                if (FireArm.FChambers[i].IsFull)
                {
                    LoadedChambers++;
                }
            }
            CurrentAmount = FireArm.Magazine.m_numRounds + LoadedChambers;
            var UnclampedPercentage  = CurrentAmount / MaxCap;
            CurrentPercentage = Mathf.Clamp01(UnclampedPercentage);
            for (int i = 0; i < Graphics.Count; i++)
            {
                UIColor = Gradients[i].Evaluate(CurrentPercentage);
                Graphics[i].color = UIColor;
            }
            
            LoadedChambers = 0;
        }
    }
}