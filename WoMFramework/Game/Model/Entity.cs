namespace WoMFramework.Game.Model
{
    using Actions;
    using Enums;
    using Generator;
    using GoRogue;
    using GoRogue.MapViews;
    using Learnable;
    using Mogwai;
    using Random;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class Entity : Combatant
    {
        public int Gender { get; set; }
        public string GenderStr => ((GenderType)Gender).ToString();

        public SizeType SizeType { get; set; }

        #region abilities

        public int BaseStrength { get; set; }
        public int Strength => BaseStrength + MiscStrength + TempStrength;
        public int StrengthMod => Modifier(Strength);
        public int MiscStrength => AccumulateMiscModifiers(this, ModifierType.Strength);
        public int TempStrength => AccumulateTempModifiers(this, ModifierType.Strength);

        public int BaseDexterity { get; set; }
        public int Dexterity => BaseDexterity + MiscDexterity + TempDexterity;
        public int DexterityMod => Modifier(Dexterity);
        public int MiscDexterity => AccumulateMiscModifiers(this, ModifierType.Dexterity);
        public int TempDexterity => AccumulateTempModifiers(this, ModifierType.Dexterity);

        public int BaseConstitution { get; set; }
        public int Constitution => BaseConstitution + MiscConstitution + TempConstitution;
        public int ConstitutionMod => Modifier(Constitution);
        public int MiscConstitution => AccumulateMiscModifiers(this, ModifierType.Constitution);
        public int TempConstitution => AccumulateTempModifiers(this, ModifierType.Constitution);

        public int BaseIntelligence { get; set; }
        public int Intelligence => BaseIntelligence + MiscIntelligence + TempIntelligence;
        public int IntelligenceMod => Modifier(Intelligence);
        public int MiscIntelligence => AccumulateMiscModifiers(this, ModifierType.Intelligence);
        public int TempIntelligence => AccumulateTempModifiers(this, ModifierType.Intelligence);

        public int BaseWisdom { get; set; }
        public int Wisdom => BaseWisdom + MiscWisdom + TempWisdom;
        public int WisdomMod => Modifier(Wisdom);
        public int MiscWisdom => AccumulateMiscModifiers(this, ModifierType.Wisdom);
        public int TempWisdom => AccumulateTempModifiers(this, ModifierType.Wisdom);

        public int BaseCharisma { get; set; }
        public int Charisma => BaseCharisma + MiscCharisma + TempCharisma;
        public int CharismaMod => Modifier(Charisma);
        public int MiscCharisma => AccumulateMiscModifiers(this, ModifierType.Charisma);
        public int TempCharisma => AccumulateTempModifiers(this, ModifierType.Charisma);

        private int Modifier(int ability) => (int)Math.Floor((ability - 10) / 2.0);

        #endregion

        public int CurrentLevel { get; set; } = 1;

        // base speed
        public int BaseSpeed { get; set; }

        // calculate encumbrance and stuff like that ...
        public int Speed => BaseSpeed + AccumulateMiscModifiers(this, ModifierType.Speed) + AccumulateTempModifiers(this, ModifierType.Speed);

        public int NaturalArmor { get; set; }

        // armorclass = 10 + armor bonus + shield bonus + dex modifier + size modifier + natural armor + deflection + misc modifier
        public int ArmorClass => 10 + Equipment.ArmorBonus + Equipment.ShieldBonus + DexterityMod + SizeType.Modifier() + NaturalArmor + MiscArmorClass + TempArmorClass;
        public int MiscArmorClass => AccumulateMiscModifiers(this, ModifierType.ArmorClass);
        public int TempArmorClass => AccumulateTempModifiers(this, ModifierType.ArmorClass);

        // hitpoints
        public int HitPointDice { get; set; }
        public int[] HitPointDiceRollEvent { get; set; }
        public List<int> HitPointLevelRolls { get; }
        public int MaxHitPoints => HitPointDice + HitPointLevelRolls.Sum();
        public int CurrentHitPoints { get; set; }

        // initiative = dex modifier + misc modifier
        public int Initiative => DexterityMod + MiscInitiative + TempInitiative;
        public int MiscInitiative => AccumulateMiscModifiers(this, ModifierType.Initiative);
        public int TempInitiative => AccumulateTempModifiers(this, ModifierType.Initiative);

        #region saving throws

        //saving throw = basesave + ability modifier + misc modifier + magic modifier + temp modifier
        public int FortitudeBaseSave { get; set; }
        public int Fortitude => FortitudeBaseSave + ConstitutionMod + MiscFortitude + TempFortitude;
        public int MiscFortitude => AccumulateMiscModifiers(this, ModifierType.Fortitude);
        public int TempFortitude => AccumulateTempModifiers(this, ModifierType.Fortitude);

        public int ReflexBaseSave { get; set; }
        public int Reflex => ReflexBaseSave + DexterityMod + MiscReflex + TempReflex;
        public int MiscReflex => AccumulateMiscModifiers(this, ModifierType.Reflex);
        public int TempReflex => AccumulateTempModifiers(this, ModifierType.Reflex);

        public int WillBaseSave { get; set; }
        public int Will => WillBaseSave + WisdomMod + MiscWill + TempWill;
        public int MiscWill => AccumulateMiscModifiers(this, ModifierType.Will);
        public int TempWill => AccumulateTempModifiers(this, ModifierType.Will);

        #endregion

        // base attack bonus = class dependent value
        public int[] BaseAttackBonus { get; set; } = { 0 };

        // attackbonus = base attack bonus + strength modifier + size modifier
        public int AttackBonus(int attackIndex) => BaseAttackBonus[attackIndex] + StrengthMod + (int)SizeType + MiscAttackBonus + TempAttackBonus;
        public int MiscAttackBonus => AccumulateMiscModifiers(this, ModifierType.AttackBonus);
        public int TempAttackBonus => AccumulateMiscModifiers(this, ModifierType.AttackBonus);

        // attack roll
        public int[] AttackRolls(int attackIndex, int criticalMinRoll = 21)
        {
            var rolls = new List<int>();
            for (var i = 0; i < 3; i++)
            {
                var lastRoll = Dice.Roll(DiceType.D20);
                rolls.Add(lastRoll + AttackBonus(attackIndex));
                if (lastRoll < criticalMinRoll)
                    break;
            }

            return rolls.ToArray();
        }

        // initiative roll
        public int InitiativeRoll => Dice.Roll(DiceType.D20) + Initiative;

        // damage
        public int DamageRoll(Weapon weapon, Dice dice)
        {
            var damage = dice.Roll(weapon.DamageRoll) + (weapon.WeaponEffortType == WeaponEffortType.TwoHanded ? (int)Math.Floor(1.5 * StrengthMod) : StrengthMod);
            return damage < 1 ? 1 : damage;
        }

        public override bool CanAct => (int)HealthState >= 0;
        public override bool IsAlive => HealthState != HealthState.Dead;
        public override bool IsDisabled => HealthState == HealthState.Disabled;
        public override bool IsInjured => HealthState == HealthState.Injured;
        public override bool IsDying => HealthState == HealthState.Dying;
        public override bool IsDead => HealthState == HealthState.Dead;

        // injury and death
        public HealthState HealthState
        {
            get
            {
                if (CurrentHitPoints == MaxHitPoints)
                {
                    return HealthState.Healthy;
                }

                if (CurrentHitPoints > 0)
                {
                    return HealthState.Injured;
                }

                if (CurrentHitPoints == 0)
                {
                    return HealthState.Disabled;
                }

                if (CurrentHitPoints > -10)
                {
                    return HealthState.Dying;
                }

                return HealthState.Dead;
            }
        }

        // equipment
        public Equipment Equipment { get; }

        // wealth
        public Wealth Wealth { get; set; }

        // dice
        public virtual Dice Dice { get; set; }

        //public Classes.Classes CurrentClass => Classes.Count > 0 ? Classes[0] : null;
        public List<Classes.Classes> Classes { get; }

        public int GetClassLevel(ClassType classType)
        {
            var classes = Classes.FirstOrDefault(p => p.ClassType == classType);
            return classes?.ClassLevel ?? 0;
        }

        public EnvironmentType[] EnvironmentTypes { get; set; }

        public List<Feat> Feats { get; set; }

        public List<Spell> Spells { get; set; }

        public List<BaseItem> Inventory { get; }

        /// <summary>
        /// 
        /// </summary>
        protected Entity() : base(false, false, 1)
        {
            // initialize
            HitPointLevelRolls = new List<int>();
            Equipment = new Equipment();
            Wealth = new Wealth();
            Classes = new List<Classes.Classes>();

            EngagedEnemies = new List<Entity>();

            // add basic actions
            CombatActions = new List<CombatAction>
            {
                CombatAction.CreateMove(this)
            };

            // initialize skills list
            Feats = new List<Feat>();

            // initialize spells list
            Spells = new List<Spell>();

            // initialize inventory
            Inventory = new List<BaseItem>();

            // unlooted state
            LootState = LootState.Unlooted;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dice"></param>
        public void LevelClass(Dice dice)
        {
            BaseAttackBonus = CalculateBaseAttackBonus(Classes.Sum(p => p.ClassAttackBonus));
            FortitudeBaseSave = Classes.Sum(p => p.FortitudeBaseSave);
            ReflexBaseSave = Classes.Sum(p => p.ReflexBaseSave);
            WillBaseSave = Classes.Sum(p => p.WillBaseSave);
            HitPointDiceRollEvent = Classes[0].HitPointDiceRollEvent;
            HitPointLevelRolls.Add(dice.Roll(HitPointDiceRollEvent));

            Classes.ForEach(p => p.Learnables.ForEach(q => Learn(q)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="learnable"></param>
        /// <returns></returns>
        public bool Learn(ILearnable learnable)
        {
            if (!learnable.CanLearn(this))
            {
                return false;
            }

            return learnable.Learn(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attackBonus"></param>
        /// <returns></returns>
        private int[] CalculateBaseAttackBonus(int attackBonus)
        {
            var currentBaseAttackBonus = attackBonus;

            var baseAttackBonusList = new List<int> { currentBaseAttackBonus };

            for (var i = currentBaseAttackBonus - 5; i > 0; i = i - 5)
            {
                baseAttackBonusList.Add(i);
            }

            return baseAttackBonusList.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseItem"></param>
        /// <returns></returns>
        public bool AddToInventory(BaseItem baseItem)
        {
            // TODO check if we are not encumbered!

            Inventory.Add(baseItem);

            baseItem.ChangeOwner(this);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseItem"></param>
        /// <returns></returns>
        public bool RemoveFromInventory(BaseItem baseItem)
        {
            if (!Inventory.Contains(baseItem))
            {
                return false;
            }

            Inventory.Remove(baseItem);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="baseItem"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public bool CanEquipItem(SlotType slotType, BaseItem baseItem, out EquipmentSlot slot, int slotIndex = 0)
        {
            slot = Equipment.Slots.Where(p => p.SlotType == slotType).ElementAtOrDefault(slotIndex);

            return Inventory.Contains(baseItem)
                   && baseItem.SlotType == slotType
                   && slot != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="baseItem"></param>
        /// <returns></returns>
        public bool EquipItem(SlotType slotType, BaseItem baseItem, int slotIndex = 0)
        {
            if (!CanEquipItem(slotType, baseItem, out var slot, slotIndex))
            {
                return false;
            }

            // removed old item
            if (slot.BasicItem != null)
            {
                var oldItem = slot.BasicItem;
                slot.BasicItem = null;

                if (!AddToInventory(oldItem))
                {
                    return false;
                }

                // unlearn old item
                oldItem.UnLearn(this);
            }

            // add new item
            if (!RemoveFromInventory(baseItem))
            {
                return false;
            }

            slot.BasicItem = baseItem;

            // learn new item
            baseItem.Learn(this);

            return true;
        }

        public bool CanEquipWeapon(SlotType slotType, Weapon weapon, int slotIndex, out WeaponSlot slot)
        {
            slot = Equipment.GetWeaponSlot(slotIndex);

            return Inventory.Contains(weapon)
                   && weapon.SlotType == slotType
                   && slot != null;
        }

        public bool EquipWeapon(Weapon baseItem, Weapon secondaryWeapon = null, int slotIndex = 0)
        {
            if (!CanEquipWeapon(SlotType.Weapon, baseItem, slotIndex, out var slot))
            {
                return false;
            }

            // removed old item
            if (slot.PrimaryWeapon != null)
            {
                var oldItem = slot.PrimaryWeapon;
                slot.PrimaryWeapon = null;

                if (!AddToInventory(oldItem))
                {
                    return false;
                }

                // unlearn old item
                oldItem.UnLearn(this);
            }

            // add new item
            if (!RemoveFromInventory(baseItem))
            {
                return false;
            }

            slot.PrimaryWeapon = baseItem;

            // learn new item
            baseItem.Learn(this);

            // standard attack
            //CombatActions.Add(CombatAction.CreateWeaponAttack(this, baseItem, false));

            // full attack
            //CombatActions.Add(CombatAction.CreateWeaponAttack(this, baseItem, true));

            slot.PrimaryWeapon = baseItem;

            if (secondaryWeapon != null)
            {
                // TODO implement secondary weapon
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attackRolls"></param>
        /// <param name="armorClass"></param>
        /// <param name="criticalCount"></param>
        /// <returns></returns>
        private int AttackRoll(int[] attackRolls, int armorClass, out int criticalCount)
        {
            var attack = attackRolls[attackRolls.Length - 1];
            if (attack > armorClass)
            {
                criticalCount = attackRolls.Length - 1;
                return attack;
            }

            criticalCount = attackRolls.Length > 2 ? attackRolls.Length - 2 : 0;
            return attack;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gold"></param>
        public virtual void AddGold(int gold)
        {
            // nothing here ..
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="monster"></param>
        public virtual void AddExp(double exp, Monster.Monster monster = null)
        {
            // nothing here ..
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="healAmount"></param>
        /// <param name="healType"></param>
        public void Heal(int healAmount, HealType healType)
        {
            var missingHealth = MaxHitPoints - CurrentHitPoints;
            if (missingHealth <= 0 || healAmount <= 0)
            {
                return;
            }

            if (missingHealth < healAmount)
            {
                healAmount = missingHealth;
            }
            var activity = ActivityLog.Create(ActivityLog.ActivityType.Heal, ActivityLog.ActivityState.None, new int[] { healAmount, (int)healType }, null);
            Mogwai.Mogwai.History.Add(LogType.Info, activity);
            Adventure?.Enqueue(AdventureLog.Info(this, null, activity));

            CurrentHitPoints += healAmount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damageAmount"></param>
        /// <param name="damageType"></param>
        public void Damage(int damageAmount, DamageType damageType)
        {
            // no damage return or same for dead entities
            if (damageAmount <= 0 || IsDead)
            {
                return;
            }

            var activity = ActivityLog.Create(ActivityLog.ActivityType.Damage, ActivityLog.ActivityState.None, new int[] { damageAmount, (int)damageType }, null);
            Mogwai.Mogwai.History.Add(LogType.Info, activity);
            Adventure?.Enqueue(AdventureLog.Info(this, null, activity));
            CurrentHitPoints -= damageAmount;

            if (!CanAct)
            {
                activity = ActivityLog.Create(ActivityLog.ActivityType.HealthState, ActivityLog.ActivityState.None, new int[] { (int)HealthState }, null);
                Mogwai.Mogwai.History.Add(LogType.Info, activity);
                Adventure?.Enqueue(AdventureLog.Info(this, null, activity));
            }

            if (IsDead)
            {
                activity = ActivityLog.Create(ActivityLog.ActivityType.HealthState, ActivityLog.ActivityState.None, new int[] { (int)HealthState }, null);
                Mogwai.Mogwai.History.Add(LogType.Info, activity);
                Adventure?.Enqueue(AdventureLog.Info(this, null, activity));

                Map.DeadEntity(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void Loot(AdventureEntity entity)
        {
            // only allow mogwais to loot
            if (!(this is Mogwai.Mogwai mogwai))
            {
                return;
            }

            if (entity.LootState == LootState.None
             || entity.LootState == LootState.Looted)
            {
                return;
            }

            var activity = ActivityLog.Create(ActivityLog.ActivityType.Loot, ActivityLog.ActivityState.None, new int[] { }, null);
            Mogwai.Mogwai.History.Add(LogType.Info, activity);
            Adventure?.Enqueue(AdventureLog.Info(this, entity, activity));

            if (entity.Treasure != null)
            {
                activity = ActivityLog.Create(ActivityLog.ActivityType.Treasure, ActivityLog.ActivityState.Success, new int[] { }, null);
                Mogwai.Mogwai.History.Add(LogType.Info, activity);
                Adventure?.Enqueue(AdventureLog.Info(this, entity, activity));
                mogwai.AddGold(entity.Treasure.Gold);
            }
            else
            {
                activity = ActivityLog.Create(ActivityLog.ActivityType.Treasure, ActivityLog.ActivityState.Fail, new int[] { }, null);
                Mogwai.Mogwai.History.Add(LogType.Info, activity);
                Adventure?.Enqueue(AdventureLog.Info(this, entity, activity));
            }

            entity.LootState = LootState.Looted;
        }

        #region AdventureEntity

        public override bool TakeAction(EntityAction entityAction)
        {
            if (!entityAction.IsExecutable)
            {
                return false;
            }

            // weapon attack
            if (entityAction is WeaponAttack weaponAttack)
            {
                Attack(weaponAttack);
                return true;
            }

            // cast spell
            if (entityAction is SpellCast spellCast)
            {
                return Cast(spellCast);
            }

            // move
            if (entityAction is MoveAction moveAction)
            {
                return Move(moveAction.Destination, true);
            }

            return false;
        }

        #endregion

        #region Combatant

        public bool LootablesInSight(out List<AdventureEntity> lootableEntities)
        {
            lootableEntities = new List<AdventureEntity>();
            foreach (var fovCoord in FovCoords)
            {
                if (Map.EntityMap[fovCoord] != null)
                {
                    foreach (var entity in Map.EntityMap[fovCoord].Entities)
                    {
                        if (entity != null
                         && this != entity
                         && entity.IsLootable
                         && entity.HasLoot
                         && (!(entity is Combatant combatant) || combatant.IsDead))
                        {
                            lootableEntities.Add(entity);
                        }
                    }
                }
            }

            return lootableEntities.Any();
        }

        public override bool CanSee(AdventureEntity entity)
        {
            return entity != null && FovCoords.Any(p => entity.Coordinate == p);
        }

        public override bool IsInReach(AdventureEntity entity)
        {
            return Distance.EUCLIDEAN.Calculate(entity.Coordinate - Coordinate) <= 2d;
        }

        /// <summary>
        /// Reset all adventure and combat stats
        /// </summary>
        public override void Reset()
        {
            CombatState = CombatState.None;
            EngagedEnemies.Clear();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spellCast"></param>
        /// <returns></returns>
        private bool Cast(SpellCast spellCast)
        {
            var spell = spellCast.Spell;

            if (spellCast.Target is Entity target)
            {
                // TODO: https://github.com/WorldOfMogwais/WoMNetCore/issues/22
                var concentrateRoll = 10;

                Adventure.Enqueue(AdventureLog.Info(this, target, ActivityLog.Create(ActivityLog.ActivityType.Cast, ActivityLog.ActivityState.Init, concentrateRoll, spell)));

                if (concentrateRoll > 1 && concentrateRoll > spell.Level)
                {
                    Adventure.Enqueue(AdventureLog.Info(this, target, ActivityLog.Create(ActivityLog.ActivityType.Cast, ActivityLog.ActivityState.Success, concentrateRoll, spell)));
                    spell.SpellEffect(this, target);
                }
                else
                {
                    Adventure.Enqueue(AdventureLog.Info(this, target, ActivityLog.Create(ActivityLog.ActivityType.Cast, ActivityLog.ActivityState.Fail, concentrateRoll, spell)));
                }

                Adventure.Enqueue(AdventureLog.Attacked(this, target));

                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weaponAttack"></param>
        /// <returns></returns>
        private void Attack(WeaponAttack weaponAttack)
        {
            var attackTimes = weaponAttack.ActionType == ActionType.Full ? BaseAttackBonus.Length : 1;
            var weapon = weaponAttack.Weapon;
            var target = weaponAttack.Target as Entity;

            //Console.WriteLine($"{Name}: is attacking {attackTimes} times");

            // all attacks are calculated
            for (var attackIndex = 0; attackIndex < attackTimes; attackIndex++)
            {
                // break when target is null or dead, no more attacks on dead monsters.
                if (target == null || target.IsDead)
                {
                    break;
                }

                var attackRolls = AttackRolls(attackIndex, weapon.CriticalMinRoll);
                var attack = AttackRoll(attackRolls, target.ArmorClass, out var criticalCounts);

                Adventure.Enqueue(AdventureLog.Info(this, target, ActivityLog.Create(ActivityLog.ActivityType.Attack, ActivityLog.ActivityState.Init, new int[] { attackIndex, attack, criticalCounts }, weaponAttack)));

                if (attack > target.ArmorClass || criticalCounts > 0)
                {
                    var damage = DamageRoll(weapon, Dice);
                    var criticalDamage = 0;
                    if (criticalCounts > 0)
                    {
                        for (var i = 0; i < weapon.CriticalMultiplier - 1; i++)
                        {
                            criticalDamage += DamageRoll(weapon, Dice);
                        }
                    }

                    Adventure.Enqueue(AdventureLog.Info(this, target, ActivityLog.Create(ActivityLog.ActivityType.Attack, ActivityLog.ActivityState.Success, new int[] { attackIndex, attack, criticalCounts, damage, criticalDamage, (int)DamageType.Weapon }, weaponAttack)));
                    target.Damage(damage + criticalDamage, DamageType.Weapon);
                }
                else
                {
                    Adventure.Enqueue(AdventureLog.Info(this, target, ActivityLog.Create(ActivityLog.ActivityType.Attack, ActivityLog.ActivityState.Fail, new int[] { attackIndex, attack, criticalCounts }, weaponAttack)));
                }

                Adventure.Enqueue(AdventureLog.Attacked(this, target));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="checkForEnemies"></param>
        /// <returns></returns>
        private bool Move(Coord destination, bool checkForEnemies = false)
        {
            // initialise
            var moveRange = Speed / 5;

            var path = Algorithms.AStar(Coordinate, destination, Map);

            if (path == null)
                return false;

            var i = 1;
            var diagonalCount = 0;

            // TODO: Implement the exact movement rule in the manual
            while (moveRange > 0)
            {
                // check if we have an enemy in fov
                if (checkForEnemies && CombatState == CombatState.None)
                {
                    // check field of view positions for enemies
                    foreach (var entity in Map.EntitiesOnCoords(FovCoords).Where(p => p.IsAlive))
                    {
                        if (entity.Faction == Faction.None) continue;
                        if (Faction != entity.Faction)
                        {
                            CombatState = CombatState.Initiation;
                            entity.CombatState = CombatState.Initiation;
                        }
                    }

                    // break if we have found an enemy
                    if (CombatState == CombatState.Initiation)
                    {
                        break;
                    }
                }

                if (path.Length <= i)
                {
                    break;
                }

                var next = path[i++];
                if (next == destination && !Map.WalkabilityMap[next])
                {
                    break;
                }

                MoveAtomic(next, ref moveRange, ref diagonalCount);
            }

            return true;
        }

        protected bool MoveAtomic(Coord next, ref int moveRange, ref int diagonalCount)
        {
            // Should check the destination is adjacent tile

            // Check it is diagonal or cardinal
            if (Distance.EUCLIDEAN.Calculate(Coordinate, next) > 1)
            {
                diagonalCount++;
                if (diagonalCount % 2 == 1)
                {
                    Map.MoveEntity(this, next);
                    moveRange--;
                }
                else
                {
                    if (moveRange == 1)
                    {
                        var newNext = next.Translate(0, -(next - Coordinate).Y);  // Prefer X direction for now; can be randomized
                        if (Map.WalkabilityMap[newNext])
                            Map.MoveEntity(this, newNext);
                        else
                        {
                            newNext = next.Translate(-(next - Coordinate).X, 0);
                            if (Map.WalkabilityMap[newNext])
                                Map.MoveEntity(this, newNext);
                            else
                            {
                                // we dismiss the last point, as probably it's a diag
                                return true;
                            }
                        }

                        return false;
                    }

                    Map.MoveEntity(this, next);
                    moveRange -= 2;
                }
            }
            else
            {
                Map.MoveEntity(this, next);
                moveRange--;
            }

            return true;
        }

        public FOV PrivateFov;
        private Coord[] _lastPath;
        private int _pathIndex = -1;
        private bool _foundLootable;
        private AdventureEntity _currentLoot;

        public void ExploreDungeon(bool checkEnemies)
        {
            // expMap
            // -1 : impassable
            //  0 : uncharted   (WHITE)
            //  1 : observed through FOV (GREY)
            //  2 : Visited or having no explorable tiles (BLACK)

            var expMap = Map.ExplorationMap;

            var moveRange = Speed / 5;
            var diagonalCount = 0;

            //bool foundLootable = false;
            //AdventureEntity currentLoot = null;

            while (moveRange > 0)
            {
                // check if we have an enemy in fov
                if (checkEnemies && CombatState == CombatState.None)
                {
                    // check field of view positions for enemies
                    foreach (var entity in Map.EntitiesOnCoords(FovCoords).Where(p => p.IsAlive))
                    {
                        if (entity.Faction == Faction.None) continue;

                        if (Faction != entity.Faction)
                        {
                            CombatState = CombatState.Initiation;
                            entity.CombatState = CombatState.Initiation;
                        }
                    }

                    // break if we have found an enemy
                    if (CombatState == CombatState.Initiation)
                    {
                        break;
                    }
                }

                // check if we have lootable entities in fov
                if (!_foundLootable && LootablesInSight(out var lootableEntities))
                {
                    // Searching for the nearest reachable loot.
                    Coord[] nearestPath = null;
                    foreach (var loot in lootableEntities)
                    {
                        //System.Console.WriteLine($"Is in reach {loot.Name}? {IsInReach(loot)}");
                        if (IsInReach(loot))
                        {
                            //System.Console.WriteLine($"looting {loot.Name}");
                            Loot(loot);
                            continue;
                        }

                        var path = Algorithms.AStar(Coordinate, loot.Coordinate, Map);


                        if (nearestPath == null)
                        {
                            nearestPath = path;
                            _currentLoot = loot;
                        }
                        else if (path != null && nearestPath.Length > path.Length)
                        {
                            nearestPath = path;
                            _currentLoot = loot;
                        }
                    }

                    if (nearestPath != null)
                    {
                        // Set a path for the objective.
                        _foundLootable = true;
                        _lastPath = nearestPath;
                        _pathIndex = 2;

                        if (!MoveAtomic(nearestPath[1], ref moveRange, ref diagonalCount))
                            return;

                        continue;
                    }
                }
                else if (_foundLootable && IsInReach(_currentLoot))
                {
                    Loot(_currentLoot);
                    _foundLootable = false;
                    _currentLoot = null;
                }

                // check if already have a path
                if (_pathIndex > 0)
                {
                    if (_pathIndex != _lastPath.Length)
                    {
                        if (Coordinate == _lastPath[_pathIndex - 1])
                        {
                            if (!MoveAtomic(_lastPath[_pathIndex++], ref moveRange, ref diagonalCount))
                                return;

                            continue;
                        }
                    }
                    else if (_foundLootable)
                    {
                        // This branch means this entity follows a proper path to the current loot,
                        // and in terms of path there is only 1 step left to the loot
                        // but the one step is diagonal so that it is not reachable from the current coordinate.
                        // To handle this problem, set the next coordinate as one of the cardinally adjacent tiles
                        // of the loot.
                        if (Math.Abs(Coord.EuclideanDistanceMagnitude(_currentLoot.Coordinate - Coordinate) - 2) < float.Epsilon)
                        {
                            var loot = _currentLoot.Coordinate;

                            var projected = loot.Translate(0, -(loot - Coordinate).Y); // Prefer X direction for now; can be randomized
                            if (Map.WalkabilityMap[projected])
                                MoveAtomic(projected, ref moveRange, ref diagonalCount);
                            else
                            {
                                projected = loot.Translate(-(loot - Coordinate).X, 0);
                                if (Map.WalkabilityMap[projected])
                                    MoveAtomic(projected, ref moveRange, ref diagonalCount);
                                else
                                {
                                    _foundLootable = false;
                                    _currentLoot = null;
                                }
                            }
                        }
                        else
                        {
                            _foundLootable = false;
                            _currentLoot = null;
                        }
                    }
                }

                _pathIndex = -1;

                // Atomic movement; consider adjacent tiles first
                var adjs = Algorithms.GetReachableNeighbors(Map.WalkabilityMap, Coordinate);
                var max = 0;
                var maxIndex = -1;
                for (var i = 0; i < adjs.Length; i++)
                {
                    // Not consider Black tiles
                    if (expMap[adjs[i]] == 2) continue;

                    var c = Map.ExpectedFovNum[adjs[i].X, adjs[i].Y];
                    // Calculate which adjacent tile has the greatest number of expected FOV tiles
                    if (max < c)
                    {
                        max = c;
                        maxIndex = i;
                    }
                }

                Coord next;

                // If all adjacent tiles are Black, explorer have to find the nearest grey tile.
                if (maxIndex < 0)
                {
                    Coord[] nearest;

                    // Consider grey tiles in FOV first. If not any, check the whole map
                    var inFov = FovCoords
                        .Where(c => expMap[c] == 1)
                        .ToArray();

                    if (inFov.Length > 0)
                    {
                        nearest = inFov
                            .Select(c => Algorithms.AStar(Coordinate, c, Map))
                            .OrderBy(p => p.Length)
                            .First();
                    }
                    else
                    {
                        nearest = expMap.Positions()
                            .Where(c => expMap[c] == 1)
                            .OrderBy(p => Distance.EUCLIDEAN.Calculate(Coordinate, p))
                            .Take(5)
                            .Where(p => p != Coord.NONE)
                            .Select(p => Algorithms.AStar(Coordinate, p, Map))
                            .OrderBy(p => p.Length)
                            .First();
                    }

                    if (nearest?.Length < 2)
                    {
                        for (int i = 0; i < expMap.Width; i++)
                            for (int j = 0; j < expMap.Height; j++)
                                expMap[i, j] = 2;

                        return;
                    }

                    next = nearest[1];

                    // Save the path
                    _lastPath = nearest;
                    _pathIndex = 2;
                }
                else
                {
                    next = adjs[maxIndex];
                }

                if (!MoveAtomic(next, ref moveRange, ref diagonalCount))
                    return;
            }
        }

        public void WalkTo(Coord coord, bool checkForEnemies)
        {
            // expMap
            // -1 : impassable
            //  0 : uncharted   (WHITE)
            //  1 : observed through FOV (GREY)
            //  2 : Visited or having no explorable tiles (BLACK)

            var moveRange = Speed / 5;
            var diagonalCount = 0;

            while (moveRange > 0)
            {
                // check if we have an enemy in fov
                if (checkForEnemies && CombatState == CombatState.None)
                {
                    // check field of view positions for enemies
                    foreach (var entity in Map.EntitiesOnCoords(FovCoords).Where(p => p.IsAlive))
                    {
                        if (entity.Faction == Faction.None) continue;

                        if (Faction != entity.Faction)
                        {
                            CombatState = CombatState.Initiation;
                            entity.CombatState = CombatState.Initiation;
                        }
                    }

                    // break if we have found an enemy
                    if (CombatState == CombatState.Initiation)
                    {
                        break;
                    }
                }

                // check if already have a path
                if (_pathIndex > 0 && _pathIndex != _lastPath.Length)
                {
                    if (Coordinate == _lastPath[_pathIndex - 1])
                    {
                        if (!MoveAtomic(_lastPath[_pathIndex++], ref moveRange, ref diagonalCount))
                            return;

                        continue;
                    }
                }

                _pathIndex = -1;

                var nearest = Algorithms.AStar(Coordinate, coord, Map);

                var next = nearest[1];

                // Save the path
                _lastPath = nearest;
                _pathIndex = 2;

                if (!MoveAtomic(next, ref moveRange, ref diagonalCount))
                    return;
            }
        }

        public int GetRequirementValue(RequirementType requirementType, object addValue = null)
        {
            switch (requirementType)
            {
                case RequirementType.Strength:
                    return Strength;
                case RequirementType.Dexterity:
                    return Dexterity;
                case RequirementType.Constitution:
                    return Constitution;
                case RequirementType.Intelligence:
                    return Intelligence;
                case RequirementType.Wisdom:
                    return Wisdom;
                case RequirementType.Charisma:
                    return Charisma;
                case RequirementType.Skill:
                    throw new NotImplementedException();
                case RequirementType.Level:
                    return CurrentLevel;
                case RequirementType.CasterLevel:
                    var casterClasses = Classes.Where(p => p.CanCast).ToArray();
                    return casterClasses.Any() ? casterClasses.Max(p => p.ClassLevel) : 0;
                case RequirementType.FighterLevel:
                    var fighterClasses = Classes.Where(p => !p.CanCast).ToArray();
                    return fighterClasses.Any() ? fighterClasses.Max(p => p.ClassLevel) : 0;
                case RequirementType.BaseAttack:
                    return BaseAttackBonus[0];
                default:
                    throw new ArgumentOutOfRangeException(nameof(requirementType), requirementType, null);
            }
        }
    }
}
