using System;
using Verse;
using UnityEngine;
using System.Collections.Generic;
using static SayersRemake.SayersRemakeBase;
using System.Linq;

namespace SayersRemake
{
    public class CompProperties_PawnEmbeddedWeapons : CompProperties
    {
        public CompProperties_PawnEmbeddedWeapons() : base(typeof(CompPawnEmbeddedWeapons))
        {
        }
        public CompProperties_PawnEmbeddedWeapons(Type cc) : base(cc)
        {
        }
    }

    [StaticConstructorOnStartup]
    public class CompPawnEmbeddedWeapons : ThingComp
    {
        public Pawn Pawn => parent as Pawn;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }
    }
}
