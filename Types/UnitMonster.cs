﻿using MapAssist.Helpers;
using MapAssist.Structs;
using System;
using System.Collections.Generic;

namespace MapAssist.Types
{
    public class UnitMonster : UnitAny
    {
        public MonsterData MonsterData { get; set; }
        public MonStats MonsterStats { get; private set; }
        public List<Resist> Immunities { get; set; }
        public Npc Npc => (Npc)TxtFileNo;

        public UnitMonster(IntPtr pUnit) : base(pUnit)
        {
        }

        public new UnitMonster Update()
        {
            if (base.Update())
            {
                using (var processContext = GameManager.GetProcessContext())
                {
                    MonsterData = processContext.Read<MonsterData>(Struct.pUnitData);
                    MonsterStats = processContext.Read<MonStats>(MonsterData.pMonStats);
                    Immunities = GetImmunities();
                }

                return this;
            }

            return null;
        }

        private List<Resist> GetImmunities()
        {
            Stats.TryGetValue(Stat.DamageReduced, out var resistanceDamage);
            Stats.TryGetValue(Stat.MagicResist, out var resistanceMagic);
            Stats.TryGetValue(Stat.FireResist, out var resistanceFire);
            Stats.TryGetValue(Stat.LightningResist, out var resistanceLightning);
            Stats.TryGetValue(Stat.ColdResist, out var resistanceCold);
            Stats.TryGetValue(Stat.PoisonResist, out var resistancePoison);

            var resists = new List<int>
            {
                resistanceDamage,
                resistanceMagic,
                resistanceFire,
                resistanceLightning,
                resistanceCold,
                resistancePoison
            };

            var immunities = new List<Resist>();

            for (var i = 0; i < 6; i++)
            {
                if (resists[i] >= 100)
                {
                    immunities.Add((Resist)i);
                }
            }

            return immunities;
        }

        public float HealthPercentage
        {
            get
            {
                if (Stats.TryGetValue(Stat.Life, out var health) &&
                    Stats.TryGetValue(Stat.MaxLife, out var maxHp) && maxHp > 0)
                {
                    return (float)health / maxHp;
                }
                return 0.0f;
            }
        }
    }
}
