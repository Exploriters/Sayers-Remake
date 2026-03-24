using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using static SayersRemake.SayersRemakeBase;

namespace SayersRemake
{
    public class Need_PhysiologicalCycle_Sayers : Need
    {
        public enum PhysiologicalCycleCategory : byte
        {
            Danger,
            SeriousWarning,
            ModerateWarning,
            MildWarning,
            Normal,
            Stable
        }

        public PhysiologicalCycleCategory CurCategory
        {
            get
            {
                if (this.CurLevel < 0.15f)
                {
                    return PhysiologicalCycleCategory.Danger;
                }
                if (this.CurLevel < 0.25f)
                {
                    return PhysiologicalCycleCategory.SeriousWarning;
                }
                if (this.CurLevel < 0.4f)
                {
                    return PhysiologicalCycleCategory.ModerateWarning;
                }
                if (this.CurLevel < 0.5f)
                {
                    return PhysiologicalCycleCategory.MildWarning;
                }
                if (this.CurLevel < 0.8f)
                {
                    return PhysiologicalCycleCategory.Normal;
                }
                return PhysiologicalCycleCategory.Stable;
            }
        }
        protected override bool IsFrozen
        {
            get
            {
                return this.pawn.def != AlienSayersDef || base.IsFrozen;
            }
        }
        public override bool ShowOnNeedList
        {
            get
            {
                return this.pawn.def == AlienSayersDef && base.ShowOnNeedList;
            }
        }
        public Need_PhysiologicalCycle_Sayers(Pawn pawn) : base(pawn)
        {
            this.threshPercents = new List<float>
            {
                0.15f,
                0.25f,
                0.4f,
                0.5f,
                0.8f
            };
        }
        public override void SetInitialLevel()
        {
            this.CurLevel = 1f;
        }
        public override void NeedInterval()
        {
            Pawn pawn = this.pawn;
            Map map = pawn.Map;
            if (!this.IsFrozen && this.pawn.def == AlienSayersDef && this.pawn.Spawned)
            {
                float ChangeValue = 0.01f;
                // Fall on dangrous environment
                CellRect cellRect = CellRect.CenteredOn(pawn.Position, 1);
                foreach (IntVec3 cell in cellRect)
                {
                    if (!cell.InBounds(map))
                    {
                        continue;
                    }
                    Plant plant = cell.GetPlant(map);
                    bool firstBlighted = false;
                    if (plant != null && plant.Blighted)
                    {
                        if (!firstBlighted)
                        {
                            firstBlighted = true;
                            ChangeValue = 0f;
                        }
                        ChangeValue -= 0.005f;
                    }
                }
                this.CurLevel = Clamp(this.CurLevel + ChangeValue, 0f, 1f);
            }
        }
    }
}
