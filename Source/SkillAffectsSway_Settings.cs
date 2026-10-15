using Verse;
using UnityEngine;

namespace TSA.CE.SkillAffectsSway
{
    public class SkillAffectsSway_Settings : ModSettings
    {
        // Lower = less sway (time/factor multipliers)
        public float factor0 = 1.25f;
        public float factor5 = 1.10f;
        public float factor10 = 1.00f;
        public float factor15 = 0.75f;
        public float factor20 = 0.50f;
        // NEW for 30-cap
        public float factor25 = 0.40f;
        public float factor30 = 0.35f;

        public static SkillAffectsSway_Settings Current;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref factor0, "factor0", 1.25f);
            Scribe_Values.Look(ref factor5, "factor5", 1.10f);
            Scribe_Values.Look(ref factor10, "factor10", 1.00f);
            Scribe_Values.Look(ref factor15, "factor15", 0.75f);
            Scribe_Values.Look(ref factor20, "factor20", 0.50f);
            // NEW
            Scribe_Values.Look(ref factor25, "factor25", 0.40f);
            Scribe_Values.Look(ref factor30, "factor30", 0.35f);
        }

        public void EnforceBoundsAndMonotone()
        {
            factor0 = Mathf.Clamp(factor0, 0.1f, 3f);
            factor5 = Mathf.Clamp(factor5, 0.1f, 3f);
            factor10 = Mathf.Clamp(factor10, 0.1f, 3f);
            factor15 = Mathf.Clamp(factor15, 0.1f, 3f);
            factor20 = Mathf.Clamp(factor20, 0.1f, 3f);
            factor25 = Mathf.Clamp(factor25, 0.1f, 3f);
            factor30 = Mathf.Clamp(factor30, 0.1f, 3f);

            // Ensure non-increasing with skill (higher skill ⇒ less sway)
            if (factor5 > factor0) factor5 = factor0;
            if (factor10 > factor5) factor10 = factor5;
            if (factor15 > factor10) factor15 = factor10;
            if (factor20 > factor15) factor20 = factor15;
            if (factor25 > factor20) factor25 = factor20; // NEW
            if (factor30 > factor25) factor30 = factor25; // NEW
        }

        // Piecewise 0–5–10–15–20–25–30; clamp beyond 30
        public float GetFactorForSkill(int lvl)
        {
            EnforceBoundsAndMonotone();

            if (lvl <= 0) return factor0;
            if (lvl <= 5) return Mathf.Lerp(factor0, factor5, lvl / 5f);
            if (lvl <= 10) return Mathf.Lerp(factor5, factor10, (lvl - 5) / 5f);
            if (lvl <= 15) return Mathf.Lerp(factor10, factor15, (lvl - 10) / 5f);
            if (lvl <= 20) return Mathf.Lerp(factor15, factor20, (lvl - 15) / 5f);
            if (lvl <= 25) return Mathf.Lerp(factor20, factor25, (lvl - 20) / 5f); // NEW
            if (lvl <= 30) return Mathf.Lerp(factor25, factor30, (lvl - 25) / 5f); // NEW
            return factor30; // clamp >30
        }
    }
}
