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
    public class GameComponent_ForceIllness : GameComponent
    {
        private bool alreadyChecked = false;
        public GameComponent_ForceIllness(Game game) { }
        public override void LoadedGame()
        {
            base.LoadedGame();
            CheckAndEnforce();
        }
        public override void StartedNewGame()
        {
            base.StartedNewGame();
            CheckAndEnforce();
        }
        private void CheckAndEnforce()
        {
            if (alreadyChecked) return;
            var illnesses = GetAllIllnesses();
            var pawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive;
            var healthySayers = pawns
                .Where(p => p.def == AlienSayersDef
                && !p.story.traits.allTraits.Any(t => illnesses.Contains(t.def))).ToList();
            if (healthySayers.Any())
            {
                Find.WindowStack.Add(new Dialog_ChooseIllness(healthySayers));
                alreadyChecked = true;
            }
        }
    }

    public class Dialog_ChooseIllness : Window
    {
        private List<Pawn> healthyPawns;
        private int currentIndex;
        private Pawn CurrentPawn => healthyPawns[currentIndex];
        private List<TraitDef> allIllness;
        private List<bool> selections;

        public override Vector2 InitialSize => new Vector2(600, 500);

        public Dialog_ChooseIllness(List<Pawn> pawns)
        {
            healthyPawns = pawns;
            currentIndex = 0;
            allIllness = GetAllIllnesses();
            selections = new List<bool>(new bool[allIllness.Count]);

            forcePause = true;
            closeOnCancel = false;
            closeOnAccept = false;
            closeOnClickedOutside = false;
            doCloseButton = false;
            absorbInputAroundWindow = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0, 0, inRect.width, 40),
                "选择特质 - " + CurrentPawn.Name.ToStringShort);
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(0, 45, inRect.width, 30),
               "作为必要的特化和削弱，请至少为这只塞尔斯选择一项疾病！");
            float curY = 80;
            float lineHeight = 30;
            for (int i = 0; i < allIllness.Count; i++)
            {
                Rect rect = new Rect(0, curY, inRect.width - 40, lineHeight);
                bool sel = selections[i];
                Widgets.CheckboxLabeled(rect, allIllness[i].degreeDatas[0].LabelCap, ref sel);
                selections[i] = sel;
                curY += lineHeight;
            }
            float confirmPosY = inRect.height - +40;
            if(Widgets.ButtonText(new Rect(inRect.width/2-100, confirmPosY, 200, 35), "确认"))
            {
                if(currentIndex < healthyPawns.Count)
                {
                    TryApplyAndNext();
                }
            }
        }

        private void TryApplyAndNext()
        {
            if (!selections.Any(s => s))
            {
                Messages.Message("必须至少选择一个特质！", MessageTypeDefOf.RejectInput, false);
                return;
            }
            for(int i = 0; i < selections.Count; i++)
            {
                if (selections[i])
                {
                    CurrentPawn.story.traits.GainTrait(new Trait(allIllness[i]));
                }
            }

            currentIndex++;
            if(currentIndex < healthyPawns.Count)
            {
                selections = new List<bool>(new bool[allIllness.Count]);
            }
            else
            {
                this.Close();
                Find.WindowStack.Add(new Dialog_MessageBox("所有塞尔斯已就绪！", "确认"));
            }
        }
    }
}
