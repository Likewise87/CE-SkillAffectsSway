using RimWorld;
using Verse;

namespace TSA.CE.SkillAffectsSway
{
    public class StatPart_ShootingSway : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            var pawn = ResolvePawn(req);
            if (pawn == null || pawn.skills == null) return;

            int lvl = pawn.skills.GetSkill(SkillDefOf.Shooting).Level;
            float factor = SkillAffectsSway_Settings.Current?.GetFactorForSkill(lvl) ?? 1f;

            // EXACTLY like warm-up: multiply the stat by the skill-based factor
            val *= factor;
        }

        public override string ExplanationPart(StatRequest req)
        {
            var pawn = ResolvePawn(req);
            if (pawn == null || pawn.skills == null) return null;

            int lvl = pawn.skills.GetSkill(SkillDefOf.Shooting).Level;
            float factor = SkillAffectsSway_Settings.Current?.GetFactorForSkill(lvl) ?? 1f;
            return $"Shooting skill {lvl}: x{factor:0.00} (sway)";
        }

        private Pawn ResolvePawn(StatRequest req)
        {
            // 1) direct pawn on request (info card often sets this)
            if (req.Pawn != null) return req.Pawn;

            // 2) thing is a pawn
            if (req.HasThing && req.Thing is Pawn p1) return p1;

            // 3) walk up the holder chain (weapon/apparel/inventory → trackers → pawn)
            if (req.HasThing && req.Thing is Thing t) return FindPawnFromHolder(t.ParentHolder);

            return null;
        }

        private Pawn FindPawnFromHolder(IThingHolder holder)
        {
            if (holder == null) return null;
            if (holder is Pawn p) return p;
            if (holder is Pawn_EquipmentTracker eq) return eq.pawn;
            if (holder is Pawn_ApparelTracker ap) return ap.pawn;
            if (holder is Pawn_InventoryTracker inv) return inv.pawn;
            if (holder is ThingOwner to && to.Owner != null) return FindPawnFromHolder(to.Owner);
            if (holder is Thing th && th.ParentHolder != null) return FindPawnFromHolder(th.ParentHolder);
            return null;
        }
    }
}
