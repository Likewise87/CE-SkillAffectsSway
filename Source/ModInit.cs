using System;
using UnityEngine;
using Verse;

namespace TSA.CE.SkillAffectsSway
{
    public enum SliderFormat { Fixed0, Fixed1, Fixed2, Multiplier, Percent, PercentDecimal }

    public class ModInit : Mod
    {
        private SkillAffectsSway_Settings settings;
        private Vector2 scrollPos = Vector2.zero;

        private const float SliderMin = 0.1f;
        private const float SliderMax = 3.0f;

        public ModInit(ModContentPack content) : base(content)
        {
            settings = GetSettings<SkillAffectsSway_Settings>();
            SkillAffectsSway_Settings.Current = settings;
        }

        public override string SettingsCategory() => "CE Skill Affects Sway";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var s = settings;
            float contentWidth = inRect.width - 24f;
            Rect viewRect = new Rect(0f, 0f, contentWidth, 400f);

            Widgets.BeginScrollView(inRect, ref scrollPos, viewRect);
            var l = new Listing_Standard();
            l.Begin(viewRect);

            DrawResetButton(l, () =>
            {
                s.factor0 = 1.25f;
                s.factor5 = 1.10f;
                s.factor10 = 1.00f;
                s.factor15 = 0.75f;
                s.factor20 = 0.50f;
                s.factor25 = 0.40f;
                s.factor30 = 0.35f;
            });

            l.Label("Shooting skill scales SwayFactor (aim wobble). Lower factor = less sway.");
            l.Gap(6f);

            s.factor0 = LabeledSlider(l, "Shooting skill 0", s.factor0, SliderMin, SliderMax,
                "Sway factor at shooting skill 0.", 0.01f, SliderFormat.Multiplier, 1.25f);
            s.factor5 = LabeledSlider(l, "Shooting skill 5", s.factor5, SliderMin, SliderMax,
                "Sway factor at shooting skill 5.", 0.01f, SliderFormat.Multiplier, 1.10f);
            s.factor10 = LabeledSlider(l, "Shooting skill 10", s.factor10, SliderMin, SliderMax,
                "Sway factor at shooting skill 10.", 0.01f, SliderFormat.Multiplier, 1.00f);
            s.factor15 = LabeledSlider(l, "Shooting skill 15", s.factor15, SliderMin, SliderMax,
                "Sway factor at shooting skill 15.", 0.01f, SliderFormat.Multiplier, 0.75f);
            s.factor20 = LabeledSlider(l, "Shooting skill 20", s.factor20, SliderMin, SliderMax,
                "Sway factor at shooting skill 20.", 0.01f, SliderFormat.Multiplier, 0.50f);
            s.factor25 = LabeledSlider(l, "Shooting skill 25", s.factor25, SliderMin, SliderMax,
                "Sway factor at shooting skill 25.", 0.01f, SliderFormat.Multiplier, 0.40f);
            s.factor30 = LabeledSlider(l, "Shooting skill 30", s.factor30, SliderMin, SliderMax,
                "Sway factor at shooting skill 30.", 0.01f, SliderFormat.Multiplier, 0.35f);

            l.Gap(8f);
            l.Label("Final SwayFactor = base sway × this multiplier × other effects.");

            l.End();
            Widgets.EndScrollView();

            s.EnforceBoundsAndMonotone();
        }

        // ----- WD-style helpers (kept in ModInit; no separate library file) -----

        private static void DrawResetButton(Listing_Standard l, Action onReset)
        {
            if (onReset == null) return;
            l.Gap(4f);
            Rect row = l.GetRect(30f);
            float resetW = Mathf.Min(250f, row.width);
            if (resetW < 120f) resetW = Mathf.Max(100f, row.width * 0.35f);
            Rect resetRect = new Rect(row.x, row.y, resetW, row.height);
            Color prev = GUI.color;
            GUI.color = new Color(1f, 0.5f, 0.5f);
            if (Widgets.ButtonText(resetRect, "Restore defaults")) onReset();
            GUI.color = prev;
            l.Gap(6f);
        }

        private static float LabeledSlider(Listing_Standard l, string label, float val, float min, float max, string tooltip = null, float step = 0.1f, SliderFormat format = SliderFormat.Fixed1, float? defaultValue = null)
        {
            l.Gap(2f);
            Rect r = l.GetRect(24f);
            if (!tooltip.NullOrEmpty()) TooltipHandler.TipRegion(r, TooltipWithDefault(tooltip, defaultValue, format));
            Widgets.Label(r.LeftPart(0.5f), $"{label}: {FormatValue(val, format).Colorize(Color.cyan)}");
            return Widgets.HorizontalSlider(r.RightPart(0.5f), val, min, max, false, null, null, null, step);
        }

        private static string TooltipWithDefault(string tooltip, float? defaultValue, SliderFormat format = SliderFormat.Fixed1)
        {
            if (!defaultValue.HasValue) return tooltip;
            return AppendDefault(tooltip, FormatValue(defaultValue.Value, format));
        }

        private static string AppendDefault(string tooltip, string def)
        {
            string line = $"Default: {def}";
            return tooltip.NullOrEmpty() ? line : tooltip + "\n" + line;
        }

        private static string FormatValue(float val, SliderFormat format) => format switch
        {
            SliderFormat.Fixed0 => val.ToString("F0"),
            SliderFormat.Fixed1 => val.ToString("F1"),
            SliderFormat.Fixed2 => val.ToString("F2"),
            SliderFormat.Multiplier => val.ToString("F2") + "x",
            SliderFormat.Percent => (val * 100f).ToString("F0") + " %",
            SliderFormat.PercentDecimal => (val * 100f).ToString("F1") + " %",
            _ => val.ToString()
        };
    }
}
