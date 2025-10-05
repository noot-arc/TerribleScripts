using System;
using System.Collections.Generic;
using UnityEngine;
using FistVR;
using UnityEngine.UI;

namespace TerribleScripts.ModMods 
{
    public class TerribleDropComp : MonoBehaviour // HERE BE DRAGONS. TODO: ranging varies with round power, low power = earlier drop consistently, unacceptable on 40x46
    {
        
        [Tooltip("DEFAULT VALUES: Default, AgentMeleeWeapon, Environment, AgentBody; no Smoke since its not mimicking a laser")] public LayerMask LM;
        [Tooltip("Large performance impact, divide by 1000 to get precision in meters")] public int CastCount = 1000; //determines accuracy, divide by 1000 to get metre-precision
        [Tooltip("its yo scopes controller")] public PIPScopeController ScopeController; //controller does zeroing and scope updates
        public Text DistanceDisplay; //for testing
        public Text NameDisplay; //was mostly for testing but its useful enough
        [HideInInspector] public FVRFireArm FVRFirearm; //the scripts firearm calls and such can be cleaned up but they shouldnt impact the end result right now
        [HideInInspector] public Transform Muzzle; //transform for rotation reasons
        [HideInInspector] public AnimationCurve DDCurve; //reason why this whole thing doesnt work
        [HideInInspector] public string RoundName;
        [HideInInspector] public float FinalDistance;
        
        private void FixedUpdate()
        {
            if (FVRFirearm == null) return; //big check but its worth it, the ONE THOUSAND default castcount overshadows it anyway
            Muzzle = FVRFirearm.GetMuzzle(); //also a big check, we need it to ensure curve is always pointing down
            transform.SetPositionAndRotation(Muzzle.position, Quaternion.Euler(Muzzle.rotation.eulerAngles.x, Muzzle.rotation.eulerAngles.y, 0f)); //curve should always be aligned on the Z axis
            FinalDistance = 9999f; //for testing, make sure we know when it doesnt hit anything
            for (var i = 0; i < CastCount; i++)
            {
                var ydistance = (float)i / CastCount;
                var ynextdist = ydistance + ((float)i + 1) / CastCount;
                var zdistance = (float)i;
                var znextdist = i + 1000f / CastCount; //we know that curves are always 1000m and we need the time to be accurate to that
                var vector1 = transform.position + transform.forward * zdistance + transform.up * DDCurve.Evaluate(ydistance); //these two took a while
                var vector2 = transform.position + transform.forward * znextdist + transform.up * DDCurve.Evaluate(ynextdist);
                Debug.DrawLine(vector1, vector2, Color.magenta, 0.02f); //need scalpel for debugging
                RaycastHit RaycastHit;
                var Hit = Physics.Linecast(vector1, vector2, out RaycastHit, LM);
                if (!Hit) continue;
                FinalDistance = Vector3.Distance(Muzzle.position, RaycastHit.point); // naive, i+RaycastHit.Distance will calculate the distance along the curve. both lead to the same issue
                break;
            }
            ScopeController.FixedBaseZero = FinalDistance; //doesnt get much simpler than this
            ScopeController.UpdateScopeParams(); //this is required expense unless you modify the PIPScope's reticle rotation yourself
            if (DistanceDisplay != null) DistanceDisplay.text = $"{FinalDistance:F1}"; //for testing
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