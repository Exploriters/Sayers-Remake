using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using static SayersRemake.SayersRemakeBase;

namespace SayersRemake
{
    public class CompProperties_StopPoisioningGizmo : CompProperties
    {
        public CompProperties_StopPoisioningGizmo()
        {
            compClass = typeof(Comp_StopPoisioningGizmo);
        }
    }

    public class Comp_StopPoisioningGizmo : ThingComp
    {
        private bool StopPoisoning = false;
        public bool StopPoisoningValue
        {
            get => StopPoisoning;
            set => StopPoisoning = value;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref StopPoisoning, "毒素禁注", true);
        }
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (base.CompGetGizmosExtra() != null)
            {
                foreach (var gizmo in base.CompGetGizmosExtra())
                {
                    if (gizmo != null)
                    {
                        yield return gizmo;
                    }
                }
            }
            if(parent is Pawn p && p.def == AlienSayersDef)
            {
                yield return new Command_Toggle
                {
                    defaultLabel = "毒素禁注",
                    defaultDesc = "控制此塞尔斯是否允许在攻击时对对手注入毒液。",
                    icon = StopPoisoningIcon,
                    isActive = () => StopPoisoningValue,
                    toggleAction = delegate ()
                    {
                        List<Tool> tentacles = new List<Tool>();
                        Tool fang = new Tool();
                        foreach (var tool in p.Tools)
                        {
                            if(tool.untranslatedLabel == "触手注射头")
                            {
                                tentacles.Add(tool);
                            }
                            if (tool.untranslatedLabel == "獠牙")
                            {
                                fang = tool;
                            }
                        }
                        StopPoisoningValue = !StopPoisoningValue;
                        if (StopPoisoningValue)
                        {
                            foreach (var t in tentacles)
                            {
                                List<ToolCapacityDef> cap = new List<ToolCapacityDef>();
                                cap.Add(DefDatabase<ToolCapacityDef>.GetNamed("Stab"));
                                t.capacities = cap;
                            }
                            List<ToolCapacityDef> cap2 = new List<ToolCapacityDef>();
                            cap2.Add(DefDatabase<ToolCapacityDef>.GetNamed("Bite"));
                            fang.capacities = cap2;
                        }
                        else
                        {
                            foreach (var t in tentacles)
                            {
                                List<ToolCapacityDef> cap = new List<ToolCapacityDef>();
                                cap.Add(Capacity_Sayers_Injection);
                                t.capacities = cap;
                            }
                            List<ToolCapacityDef> cap2 = new List<ToolCapacityDef>();
                            cap2.Add(DefDatabase<ToolCapacityDef>.GetNamed("ToxicBite"));
                            fang.capacities = cap2;
                        }
                        p.verbTracker?.InitVerbsFromZero();
                    }
                };
            }
            yield break;
        }
    }
}
