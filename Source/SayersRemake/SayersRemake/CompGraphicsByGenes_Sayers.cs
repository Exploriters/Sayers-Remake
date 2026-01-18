using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using AlienRace;
using static SayersRemake.SayersRemakeBase;

namespace SayersRemake
{
    /*public class CompProperties_CompGraphicsByGenes_Sayers : CompProperties
    {
        public CompProperties_CompGraphicsByGenes_Sayers()
        {
            compClass = typeof(CompGraphicsByGenes_Sayers);
        }
    }
    public class CompGraphicsByGenes_Sayers : AlienPartGenerator.AlienComp
    {
        public Pawn pawn => (Pawn)parent;

        public override void PostDraw()
        {
            base.PostDraw();
            if(!(pawn.def == AlienSayersDef))
            {
                return;
            }
            Dictionary<string, string> geneGraphics_head = new Dictionary<string, string>()
            {
                {"Sayers_Pattern_None", "Things/Pawn/Sayers/Prototype/Heads/solid"},
                {"Sayers_Pattern_Tabby", "Things/Pawn/Sayers/Prototype/Heads/tabby"},
                {"Sayers_Pattern_Spot", "Things/Pawn/Sayers/Prototype/Heads/spotted"},
            };
            Dictionary<string, string> geneGraphics_body = new Dictionary<string, string>()
            {
                {"Sayers_Pattern_None", "Things/Pawn/Sayers/Prototype/Bodies/solid"},
                {"Sayers_Pattern_Tabby", "Things/Pawn/Sayers/Prototype/Bodies/tabby"},
                {"Sayers_Pattern_Spot", "Things/Pawn/Sayers/Prototype/Bodies/spotted"},
            };
            Dictionary<string, string> geneGraphics_flowers_eye = new Dictionary<string, string>()
            {
                {"Sayers_Flowers_Default", "Things/Pawn/Sayers/Prototype/Flowers/InEye/base"},
                {"Sayers_Flowers_Sharp", "Things/Pawn/Sayers/Prototype/Flowers/InEye/sharp"},
                {"Sayers_Flowers_Round", "Things/Pawn/Sayers/Prototype/Flowers/InEye/round"},
                {"Sayers_Flowers_Triple", "Things/Pawn/Sayers/Prototype/Flowers/InEye/triple"},
                {"Sayers_Flowers_Vine", "Things/Pawn/Sayers/Prototype/Flowers/InEye/vine"},
                {"Sayers_Flowers_Circle", "Things/Pawn/Sayers/Prototype/Flowers/InEye/circle"},
            };
            ThingDef_AlienRace pawnAlien = pawn.def as ThingDef_AlienRace;

            AlienPartGenerator.ExtendedGraphicTop headGraphic = pawnAlien.alienRace.graphicPaths.head;
            AlienPartGenerator.ExtendedGraphicTop bodyGraphic = pawnAlien.alienRace.graphicPaths.body;
            List<AlienPartGenerator.BodyAddon> addons = pawnAlien.alienRace.generalSettings.alienPartGenerator.bodyAddons;
            foreach (Gene gene in pawn.genes.Endogenes)
            {
                headGraphic.path = geneGraphics_head.TryGetValue(gene.def.defName, "Things/Pawn/Sayers/Prototype/Heads/solid");
                bodyGraphic.path = geneGraphics_body.TryGetValue(gene.def.defName, "Things/Pawn/Sayers/Prototype/Bodies/solid");
                addons[0].path = geneGraphics_flowers_eye.TryGetValue(gene.def.defName, "Things/Pawn/Sayers/Prototype/Flowers/InEye/base");
            }
            RegenerateAddonsForced(pawn);
        }
    }*/
}
