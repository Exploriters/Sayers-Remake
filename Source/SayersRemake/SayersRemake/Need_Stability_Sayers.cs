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
    public class Need_Stability_Sayers : Need
    {
        public enum StabilityCategory : byte
        {
            Danger,
            SeriousWarning,
            ModerateWarning,
            MildWarning,
            Normal,
            Stable
        }
        public StabilityCategory CurCategory
        {
            get
            {
                if(this.CurLevel < 0.15f)
                {
                    return StabilityCategory.Danger;
                }
                if(this.CurLevel < 0.25f)
                {
                    return StabilityCategory.SeriousWarning;
                }
                if(this.CurLevel < 0.4f)
                {
                    return StabilityCategory.ModerateWarning;
                }
                if(this.CurLevel < 0.5f)
                {
                    return StabilityCategory.MildWarning;
                }
                if(this.CurLevel < 0.8f)
                {
                    return StabilityCategory.Normal;
                }
                return StabilityCategory.Stable;
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
        public Need_Stability_Sayers(Pawn pawn) : base(pawn)
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
            if (!this.IsFrozen && this.pawn.def == AlienSayersDef)
            {
                if(Find.CurrentMap != null)
                {
                    bool droneing = false;
                    foreach(GameCondition condition in Find.CurrentMap.gameConditionManager.ActiveConditions)
                    {
                        if(condition is GameCondition_PsychicEmanation con && con.gender == Gender.None)
                        {
                            droneing = true;
                            switch (con.level) {
                                case PsychicDroneLevel.BadLow:
                                    this.CurLevel = Clamp(this.CurLevel - 0.005f,0f, 1f);
                                    break;
                                case PsychicDroneLevel.BadMedium:
                                    this.CurLevel = Clamp(this.CurLevel - 0.01f,0f, 1f);
                                    break;
                                case PsychicDroneLevel.BadHigh:
                                    this.CurLevel = Clamp(this.CurLevel - 0.05f,0f, 1f);
                                    break;
                                case PsychicDroneLevel.BadExtreme:
                                    this.CurLevel = Clamp(this.CurLevel - 0.1f,0f, 1f);
                                    break;
                            }
                        }
                    }
                    if (!droneing && this.pawn.needs.mood.CurLevel > this.pawn.GetStatValue(StatDefOf.MentalBreakThreshold, true, -1))
                    {
                        Need_Stability_Sayers stability = (this.pawn.needs != null) ? this.pawn.needs.TryGetNeed<Need_Stability_Sayers>() : null;
                        if (stability.CurCategory == StabilityCategory.Stable)
                        {
                            this.CurLevel = Clamp(this.CurLevel + 0.05f, 0f, 1f);
                        }
                        else
                        {
                            this.CurLevel = Clamp(this.CurLevel + 0.01f, 0f, 1f);
                        }
                    }
                }
            }
        }
    }
}
