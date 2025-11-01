using UnityEngine;
using FistVR;
using UnityEngine.UI;

namespace TerribleScripts.ModMods 
{
    public class TerribleDropComp : MonoBehaviour // HERE BE DRAGONS.
    {
        [Header("NOTE: Put this on dedicated GameObject anywhere in hierarchy")]
        [Tooltip("Your PIPScopeController.")]                                                                        public PIPScopeController ScopeController;
        [Tooltip("Max count of simulated frames. 90 = 1s.")]                                                         public int CalcFrames = 900;
        [Tooltip("Artificial distance limit for drop compensation.")]                                                public float MaxDistance = 999f;
        [Header("OPTIONAL BIT")]
        [Tooltip("OPTIONAL: Text object that displays the distance between the approximate hit point and the muzzle. 1 metre precision. If you need any other precision or style of output feel free to bug me about it")] public Text DistanceDisplay;
        [Tooltip("String that will display after the numerical output.")]                                            public string Suffix = "m";
        [Tooltip("String that will display if the calculated distance is over the MaxDisplayDistance")]              public string MaxDistanceString = "---m";
        [Tooltip("Flag this if you want your DistanceDisplay to have the same color as the current reticle color.")] public bool TextColorParity;
        public GameObject AuxWepDisplay;
        private FVRFireArm Firearm;
        private BallisticProjectile Projectile;
        private Vector3 Velocity;
        private Vector3 Dimensions;
        private Material MatInstance;
        private int Color;
        private LayerMask LM;
        private float LastDistance = 100f;
        private bool DDAvailable;
        private bool DataAvailable;
        // projectile data
        private float Timer;
        private float Mass;
        private float Density;
        private float MuzzleVel;
        private float Gravity;
        private float VelocityMultiplier;
        private float GravityMultiplier;
        
        public void UpdateData()
        {
            if (ScopeController.Attachment != null && ScopeController.Attachment.curMount != null && ScopeController.Attachment.curMount.Parent is FVRFireArm) // this check is from the scope script idk why its so large
            { Firearm = ScopeController.Attachment.curMount.Parent as FVRFireArm; }
            if (Firearm == null) { DataAvailable = false; return; }
            var Chamber = Firearm.GetChambers()[0];
            var Magazine = Firearm.Magazine;
            FVRFireArmRound Round;
            if (Chamber != null && Chamber.GetRound() == null) 
            {
                if (Magazine != null && Magazine.LoadedRounds[Magazine.m_numRounds - 1] != null) Round = Magazine.LoadedRounds[Magazine.m_numRounds - 1].LR_ObjectWrapper.GetGameObject().GetComponent<FVRFireArmRound>();
                else {DataAvailable = false; return;}
            }
            else Round = Chamber.GetRound();
            //Set Projectile & Related Data
            LM = AM.PLM; //SET 1: Get Projectile LayerMask
            Projectile = Round.BallisticProjectilePrefab.GetComponent<BallisticProjectile>(); //SET 2: Get Projectile
            MuzzleVel = Projectile.MuzzleVelocityBase * Chamber.ChamberVelocityMultiplier * AM.GetChamberVelMult(Chamber.RoundType, Vector3.Distance(Chamber.transform.position, Firearm.MuzzlePos.position)); //SET 3: Get Projectile Muzzle Velocity
            Mass = Projectile.Mass; //SET 4: Get Projectile Mass (for Air Drag calculations)
            Dimensions = Projectile.Dimensions; //SET 5: Get Projectile Dimensions (for Air Drag calculations)
            Density = 1.225f * Projectile.AirDragMultiplier; //SET 6: Get Projectile Density (for Air Drag calculations)
            VelocityMultiplier = Projectile.FlightVelocityMultiplier; //SET 7: Get Velocity Multiplier (e.g. for correctly calculating HCB Bolt projectile trajectories)
            GravityMultiplier = Projectile.GravityMultiplier; //SET 8: Get Gravity Multiplier (e.g. for correctly calculating HCB Bolt projectile trajectories)
            switch (GM.Options.SimulationOptions.BallisticGravityMode)
            {
                //SET 9: Get current scene Ballistic Gravity
                case SimulationOptions.GravityMode.Realistic:
                    Gravity = 9.81f;
                    break;
                case SimulationOptions.GravityMode.Playful:
                    Gravity = 5f;
                    break;
                case SimulationOptions.GravityMode.OnTheMoon:
                    Gravity = 1.622f;
                    break;
                case SimulationOptions.GravityMode.None:
                    Gravity = 0f;
                    break;
            }
            DataAvailable = true;
        }

        private void Awake()
        {
            //New instance of text object material for color parity reasons
            MatInstance = Instantiate(DistanceDisplay.material);
            Color = Shader.PropertyToID("_Color"); 
            DistanceDisplay.material = MatInstance;
            //Distance Display is optional
            if (DistanceDisplay != null) DDAvailable = true;
        }

        private void Update()
        {
            if (!TextColorParity) return; //it set color :thumbsup:
            var ReticleColor = ScopeController.PScope.reticleIllumination;
            MatInstance.SetColor(Color, ReticleColor);
        }

        private void FixedUpdate()
        {
            if (Firearm == null) DataAvailable = false; //theres definitely a better way to do both of these checks
            if (!DataAvailable) return; //but do i look like a person who can think of one
            transform.position = Firearm.MuzzlePos.transform.position; //Move this object to the muzzle for referencing reasons
            var NextPos = transform.position; //End position of linecast
            Velocity = Firearm.MuzzlePos.transform.forward * MuzzleVel; //Set initial velocity & direction
            for (var i = 0; i <= CalcFrames; i++) //<= might be off-by-one. i stopped bothering a month ago
            {
                Velocity += Vector3.down * (Gravity * Time.deltaTime * GravityMultiplier); //Affect velocity by gravity
                Velocity = ApplyDrag(Velocity, Density, Time.deltaTime); //Affect velocity by air drag
                NextPos += Velocity * (Time.deltaTime * VelocityMultiplier); //Set end position of linecast to predicted bullet location
                RaycastHit Hit;
                var b = Physics.Linecast(transform.position, NextPos, out Hit, LM);
                if (b)
                {
                    transform.position = Hit.point; //Not really required except for some debugging stuff
                    Timer = 0f; // make sure lerping instantly stops as soon as a valid Thing is hit
                    var Distance = Vector3.Distance(Firearm.MuzzlePos.transform.position, Hit.point);
                    LastDistance = Distance;
                    if (Distance >= MaxDistance) goto NOHIT;
                    Timer = 0f; 
                    if (DDAvailable) DistanceDisplay.text = $"{Distance:F0}{Suffix}";
                    // the bit below is shamelessly ripped from the scope script because what would i even do here
                    var UVVector = ScopeController.PScope.ZeroToWorldPoint(Hit.point);
                    var Elevation = ScopeController.ReticleElevationMagnitude * ScopeController.ReticleElevationAdjustmentPerTick;
                    var Windage = ScopeController.ReticleWindageMagnitude * ScopeController.ReticleWindageAdjustmentPerTick;
                    switch (ScopeController.ZeroScaling)
                    {
                        case PipScopeZeroScaling.MOA:
                            Elevation *= 0.016666668f;
                            break;
                        case PipScopeZeroScaling.MIL:
                            Elevation *= 0.05625f;
                            break;
                        case PipScopeZeroScaling.MRAD:
                            Elevation *= 0.05729578f;
                            break;
                    }
                    switch (ScopeController.ZeroScaling)
                    {
                        case PipScopeZeroScaling.MOA:
                            Windage *= 0.016666668f;
                            break;
                        case PipScopeZeroScaling.MIL:
                            Windage *= 0.05625f;
                            break;
                        case PipScopeZeroScaling.MRAD:
                            Windage *= 0.05729578f;
                            break;
                    }
                    ScopeController.PScope.reticleAdjustmentDegrees = new Vector2(UVVector.x + Windage, UVVector.y + Elevation);
                    // end of shamelessly ripped bit
                    return;
                }
                transform.position = NextPos; //Repeat cycle
            }
            NOHIT:
            if (DDAvailable) DistanceDisplay.text = $"{LerpMaxDistance()}";
            ScopeController.FixedBaseZero = 100f;
            ScopeController.UpdateScopeParams();
        }
        private string LerpMaxDistance() // do the thing mgsv does when the distance is over a certain threshold
        {
            Timer += Time.deltaTime;
            if (Timer < 1f)
            {
                return $"{Mathf.Lerp(LastDistance, MaxDistance, Timer):F0}{Suffix}";
            }
            Timer = 1f;
            return MaxDistanceString;
        }
        private void OnDestroy() { Destroy(MatInstance); } //No littering!
        private float GetCurrentDragCoefficient(float velocityMS) { return AM.BDCC.Evaluate(velocityMS * 0.00291545f); } //Stolen from the BallisticProjectile Script
        private Vector3 ApplyDrag(Vector3 hvelocity, float materialDensity, float time) //Stolen from the BallisticProjectile Script
        {
            var num = 3.1415927f * Mathf.Pow(Dimensions.x * 0.5f, 2f);
            var magnitude = hvelocity.magnitude;
            var normalized = hvelocity.normalized;
            var currentDragCoefficient = GetCurrentDragCoefficient(hvelocity.magnitude);
            return normalized * Mathf.Clamp(magnitude - (-hvelocity * ((materialDensity * 0.5f * currentDragCoefficient * num / Mass) * magnitude)).magnitude * time, 0f, magnitude);
        }
    }
}