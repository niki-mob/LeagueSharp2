﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Leblanc.Common;
using SharpDX;

namespace Leblanc.Champion
{
    public static class PlayerSpells
    {
        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q, W, E, R;

        public static Spell Q2, W2, E2;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 720);
            Q.SetTargetted(0.5f, 1500f);

            W = new Spell(SpellSlot.W, 600 + 80);
            W.SetSkillshot(0.6f, 140f, 1450f, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 820);
            E.SetSkillshot(0.3f, 55f, 1650f, true, SkillshotType.SkillshotLine);

            R = new Spell(SpellSlot.R);

            Q2 = new Spell(SpellSlot.R, 660);
            Q2.SetTargetted(0.5f, 1500f);

            W2 = new Spell(SpellSlot.R, 600 + 80);
            W2.SetSkillshot(0.6f, 180f, 1450f, false, SkillshotType.SkillshotCircle);

            E2 = new Spell(SpellSlot.R, 820);
            E2.SetSkillshot(0.3f, 55f, 1650f, true, SkillshotType.SkillshotLine);

            SpellList.AddRange(new[] { Q, W, E });

            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        public static bool IsReadyInt(this Spell spell)
        {
            if (!R.IsReady())
            {
                return false;
            }

            return spell.IsReady();
        }

        private static int LastSeen(Obj_AI_Base t)
        {
            return Common.AutoBushHelper.EnemyInfo.Find(x => x.Player.NetworkId == t.NetworkId).LastSeenForE;
        }

        static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!Modes.ModeSettings.MenuSettingQ.Item("Settings.SpellCast.Active").GetValue<bool>())
            {
                args.Process = true;
            }
            else
            {
                var hero = args.Target as Obj_AI_Hero;
                if (hero != null)
                {
                    var t = hero;
                    if (!t.IsValidTarget(E.Range))
                    {
                        return;
                    }
                    // args.Process = !t.HaveImmortalBuff();

                    args.Process = Environment.TickCount - LastSeen(t) >=
                                   (Modes.ModeSettings.MenuSettingQ.Item("Settings.SpellCast.VisibleDelay")
                                       .GetValue<StringList>()
                                       .SelectedIndex + 1)*250;
                }
            }
        }

        public static void CastQ(Obj_AI_Base t)
        {
            //if (t.HaveImmortalBuff())
            //{
            //    return;
            //}

            Q.CastOnUnit(t);
        }

        public static void CastQ2(Obj_AI_Base t)
        {
            //if (t.HaveImmortalBuff())
            //{
            //    return;
            //}

            if (CommonHelper.SpellRStatus == CommonHelper.SpellRName.R2xQ)
            {
                Q2.CastOnUnit(t);
            }
        }
        public static void CastW(Vector3 t)
        {
            if (W.IsReady() && !W.StillJumped())
            {
                W.Cast(t);
            }
        }

        public static void CastW2(Vector3 t)
        {
            if (W2.IsReady() && !W2.StillJumped())
            {
                W2.Cast(t);
            }
        }

        public static void CastW(Obj_AI_Base t, bool returnBack = false)
        {
            if (W.CanCast(t) && !W.StillJumped())
            {
                W.Cast(t.Position);
            }

            if (returnBack && W.StillJumped() && Modes.ModeConfig.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                W.Cast();
            }
        }

        //public static void CastW2(Obj_AI_Base t, bool returnBack = false)
        //{
        //    if (CommonHelper.SpellRStatus == CommonHelper.SpellRName.R2xW && W2.CanCast(t) && !W2.StillJumped())
        //    {
        //        W2.Cast(t.Position);
        //    }

        //    if (returnBack && W2.StillJumped() && Modes.ModeConfig.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
        //    {
        //        W2.Cast();
        //    }

        //    //if (CommonHelper.SpellRStatus == CommonHelper.SpellRName.R2xW && W2.CanCast(t) && !W2.StillJumped())
        //    //{
        //    //    W2.Cast(t.Position);
        //    //}
        //}

        public static void CastE(Obj_AI_Base t)
        {
            if (E.IsReady() && t.IsValidTarget(Modes.ModeSettings.MaxERange))
            {
                HitChance[] hitChances = new[] { HitChance.VeryHigh, HitChance.High, HitChance.Medium, HitChance.Low };
                E.UpdateSourcePosition(ObjectManager.Player.ServerPosition, t.ServerPosition);
                if (E.GetPrediction(t).Hitchance >= hitChances[Modes.ModeSettings.EHitchance])
                {
                    E.Cast(t);
                }
            }
        }

        public static void CastE2(Obj_AI_Base t)
        {
            if (CommonHelper.SpellRStatus == CommonHelper.SpellRName.R2xE && E.IsReady() && t.IsValidTarget(Modes.ModeSettings.MaxERange))
            {
                HitChance[] hitChances = new[] { HitChance.VeryHigh, HitChance.High, HitChance.Medium, HitChance.Low};
                E.UpdateSourcePosition(ObjectManager.Player.ServerPosition, t.ServerPosition);
                if (E.GetPrediction(t).Hitchance >= hitChances[Modes.ModeSettings.EHitchance])
                {
                    E2.Cast(t);
                }

                //E2.UpdateSourcePosition(ObjectManager.Player.ServerPosition, t.ServerPosition);
                //if (E2.GetPrediction(t).Hitchance >= HitChance.High)
                //{
                //    E2.Cast(t);
                //}
            }
        }
    }

}
