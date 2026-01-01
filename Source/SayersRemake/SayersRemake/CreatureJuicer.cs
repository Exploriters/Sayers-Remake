using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using Verse.AI;
using static SayersRemake.SayersRemakeBase;

namespace SayersRemake
{
    public class CompProperties_CompCreatureJuicer : CompProperties
    {
        public CompProperties_CompCreatureJuicer()
        {
            compClass = typeof(Comp_CompCreatureJuicer);
        }
    }

    public class Comp_CompCreatureJuicer : ThingComp
    {
        public Corpse targetCorpse;
        public float progressTick;
        public bool isActive;
        public bool wasCancelled;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            progressTick = 0f;
            isActive = false;
            wasCancelled = false;
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
            if (!isActive)
            {
                yield return new Command_Action
                {
                    defaultLabel = "投喂榨汁机",
                    defaultDesc = "将一具尸体投入榨汁机中",
                    icon = TexCommand.Install,
                    action = delegate ()
                    {
                        List<FloatMenuOption> options = new List<FloatMenuOption>();
                        foreach (Thing thing in parent.Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse))
                        {
                            Corpse corpse = thing as Corpse;
                            if (IsVaildCorpse(corpse))
                            {
                                options.Add(new FloatMenuOption(corpse.LabelCap, () => SelectCorpse(corpse), MenuOptionPriority.Default));
                            }
                        }
                        if(options.Count <= 0)
                        {
                            Messages.Message("没有可用的尸体", MessageTypeDefOf.RejectInput);
                            return;
                        }
                        Find.WindowStack.Add(new FloatMenu(options));
                    }
                };
            }
            else
            {
                yield return new Command_Action
                {
                    defaultLabel = "取消投喂榨汁机",
                    defaultDesc = "让榨汁机停止运作\n注意：这不会返还尸体，也不会产生任何产物",
                    icon = TexCommand.Install,
                    action = delegate ()
                    {
                        isActive = false;
                        wasCancelled = true;
                        GenerateBloodEffects();
                        targetCorpse = null;
                        progressTick = 0f;
                    }
                };
            }
        }
   
        private bool IsVaildCorpse(Corpse corpse)
        {
            if (corpse == null) return false;
            CompRottable rot = corpse.GetComp<CompRottable>();
            if (rot != null && rot.Stage != RotStage.Fresh)
            {
                return false;
            }
            if (corpse.InnerPawn.RaceProps.IsMechanoid)
            {
                return false;
            }
            return true;
        }

        private void SelectCorpse(Corpse corpse)
        {
            Job job = JobMaker.MakeJob(Job_HaulToJuicer_Sayers, corpse, parent);
            job.count = 1;
            Pawn p = FindHauler(corpse);
            if(p != null)
            {
                p.jobs.TryTakeOrderedJob(job, JobTag.MiscWork);
                targetCorpse = corpse;
            }
            else
            {
                Messages.Message("好像没人可以投喂榨汁机……", MessageTypeDefOf.RejectInput);
            }
        }

        private Pawn FindHauler(Corpse corpse)
        {
            Pawn p = null;
            float Bscore = 0f;
            foreach (Pawn pawn in parent.Map.mapPawns.FreeColonistsSpawned)
            {
                if(pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Moving))
                {
                    float score = 1f;
                    float distance = (pawn.Position - corpse.Position).LengthManhattan;
                    score *= 1f / (1f + distance * 0.1f);
                    if (pawn.workSettings.WorkIsActive(WorkTypeDefOf.Hauling))
                    {
                        score *= 1.5f;
                    }
                    if (score > Bscore)
                    {
                        Bscore = score;
                        p = pawn;
                    }
                }
            }
            return p;
        }

        public void ReceiveCorpse(Corpse corpse)
        {
            targetCorpse = corpse;
            isActive = true;
            progressTick = 0f;
            wasCancelled = false;
            corpse.Destroy(DestroyMode.Vanish);
        }

        private void GenerateBloodEffects()
        {
            Map map = parent.Map;
            for (int i = 0; i < 10; i++)
            {
                IntVec3 bloodPos = parent.Position + IntVec3Utility.RandomHorizontalOffset(2f);
                if (bloodPos.InBounds(map))
                {
                    FilthMaker.TryMakeFilth(
                        bloodPos,
                        map,
                        ThingDefOf.Filth_Blood,
                        count: 1,
                        shouldPropagate: false
                    );
                }
            }
        }
    }

    public class JobDriver_HaultToJuicer : JobDriver
    {
        private const TargetIndex CorpseInd = TargetIndex.A;
        private const TargetIndex JuicerInd = TargetIndex.B;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(CorpseInd);
            this.FailOnDespawnedOrNull(JuicerInd);
            this.FailOn(delegate(){
                Comp_CompCreatureJuicer comp = job.GetTarget(JuicerInd).Thing.TryGetComp<Comp_CompCreatureJuicer>();
                return comp == null || comp.isActive || comp.targetCorpse != null;
            });
            yield return Toils_Goto.GotoThing(CorpseInd, PathEndMode.Touch);
            yield return Toils_Haul.StartCarryThing(CorpseInd);
            yield return Toils_Goto.GotoThing(JuicerInd, PathEndMode.Touch);
            Toil placeCorpse = new Toil();
            placeCorpse.initAction = delegate ()
            {
                Pawn actor = placeCorpse.actor;
                Thing carriedThing = actor.carryTracker.CarriedThing;
                if(carriedThing is Corpse corpse)
                {
                    Comp_CompCreatureJuicer comp = job.GetTarget(JuicerInd).Thing.TryGetComp<Comp_CompCreatureJuicer>();
                    if(comp != null)
                    {
                        comp.ReceiveCorpse(corpse);
                        actor.carryTracker.innerContainer.Remove(carriedThing);
                    }
                }
            };
            placeCorpse.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return placeCorpse;
        }
    }
}
