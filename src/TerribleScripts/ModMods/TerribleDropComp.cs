using System;
using System.Collections.Generic;
using UnityEngine;
using FistVR;
using UnityEngine.UI;

namespace TerribleScripts.ModMods 
{
    public class TerribleDropComp : MonoBehaviour // HERE BE DRAGONS. TODO: ranging varies with round power, low power = earlier drop consistently, unacceptable on 40x46
    {
        public LayerMask LM; //hit thing
        public int CastCount = 1000; //how often (and how accurate) curve thing
        public PIPScopeController ScopeController; //control do zero thing value thing
        public TerribleRangefinder DisplayOverride;
        public Text NameDisplay; //was mostly for testing but its useful enough
        [HideInInspector] public FVRFireArm FVRFirearm;
        [HideInInspector] public Transform Muzzle;
        [HideInInspector] public AnimationCurve DDCurve;
        [HideInInspector] public string RoundName;
        [HideInInspector] public float FinalDistance;
        
        private void FixedUpdate()
        {
            if (FVRFirearm == null) return; //big check but its worth it, the ONE THOUSAND default castcount overshadows it anyway
            Muzzle = FVRFirearm.GetMuzzle(); //also a big check, we need it to ensure curve is always pointing down
            transform.SetPositionAndRotation(Muzzle.position, Quaternion.Euler(Muzzle.rotation.eulerAngles.x, Muzzle.rotation.eulerAngles.y, 0f));
            FinalDistance = 1001f;
            for (var i = 0; i < CastCount; i++)
            {
                var ydistance = (float)i / CastCount;
                var ynextdist = ydistance + ((float)i + 1) / CastCount;
                var zdistance = (float)i;
                var znextdist = i + 1000f / CastCount; //we know that curves are always 1000m and we need the time to be accurate to that
                var vector1 = transform.position + transform.forward * zdistance + transform.up * DDCurve.Evaluate(ydistance); //these two took a while
                var vector2 = transform.position + transform.forward * znextdist + transform.up * DDCurve.Evaluate(ynextdist);
                Debug.DrawLine(vector1, vector2, Color.magenta, 0.02f); // need scalpel for debug
                RaycastHit RaycastHit;
                var Hit = Physics.Linecast(vector1, vector2, out RaycastHit, LM);
                if (!Hit) continue;
                FinalDistance = Vector3.Distance(Muzzle.position, RaycastHit.point); // naive, but should lead to accurate zeroing
                break;
            }
            ScopeController.FixedBaseZero = FinalDistance;
            ScopeController.UpdateScopeParams();
            if (DisplayOverride != null) DisplayOverride.Distance = FinalDistance;
        }
        public void UpdateData() // let user do the update themselves. no other way i see besides big checks in update()
        { // oh also most of this is from the scope script. i am a bad developer
            if (ScopeController.Attachment != null && ScopeController.Attachment.curMount != null &&
                ScopeController.Attachment.curMount.Parent is FVRFireArm) 
            {
                FVRFirearm = ScopeController.Attachment.curMount.Parent as FVRFireArm;
            }
            if (FVRFirearm == null) return;
            var RoundType = FVRFirearm.RoundType;
            Muzzle = FVRFirearm.GetMuzzle();
            if (AM.SRoundDisplayDataDic.ContainsKey(RoundType))
            {
                DDCurve = AM.SRoundDisplayDataDic[RoundType].BulletDropCurve;
                RoundName = AM.SRoundDisplayDataDic[RoundType].DisplayName;
                if (NameDisplay != null) NameDisplay.text = $"{RoundName}";
            }
        }
    }
}