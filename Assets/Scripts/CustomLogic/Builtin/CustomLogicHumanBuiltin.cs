using ApplicationManagers;
using Cameras;
using Characters;
using GameManagers;
using Settings;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace CustomLogic
{
    [CLType(Static = false, InheritBaseMembers = true)]
    class CustomLogicHumanBuiltin : CustomLogicCharacterBuiltin
    {
        public Human Human;

        public CustomLogicHumanBuiltin(Human human) : base(human, "Human")
        {
            Human = human;
        }

        // Add CLProperties for the above setField/getField 
        [CLProperty(description: "The weapon the human is using")]
        public string Weapon
        {
            get => Human.Setup.Weapon.ToString();
            set
            {
                // TODO: Why do we need this?
                if (Human.IsMine())
                {
                    var gameManager = (InGameManager)SceneLoader.CurrentGameManager;
                    if (gameManager.CurrentCharacter != null && gameManager.CurrentCharacter is Human)
                    {
                        var miscSettings = SettingsManager.InGameCurrent.Misc;
                        if (!Human.Dead)
                        {
                            List<string> loadouts = new List<string>();
                            if (miscSettings.AllowBlades.Value)
                                loadouts.Add(HumanLoadout.Blades);
                            if (miscSettings.AllowAHSS.Value)
                                loadouts.Add(HumanLoadout.AHSS);
                            if (miscSettings.AllowAPG.Value)
                                loadouts.Add(HumanLoadout.APG);
                            if (miscSettings.AllowThunderspears.Value)
                                loadouts.Add(HumanLoadout.Thunderspears);
                            if (loadouts.Count == 0)
                                loadouts.Add(HumanLoadout.Blades);
                            if (loadouts.Contains(value) && value != SettingsManager.InGameCharacterSettings.Loadout.Value)
                            {
                                SettingsManager.InGameCharacterSettings.Loadout.Value = value;
                                var manager = (InGameManager)SceneLoader.CurrentGameManager;
                                Human = (Human)gameManager.CurrentCharacter;
                                Human.ReloadHuman(manager.GetSetHumanSettings());
                            }
                        }
                    }
                }
            }
        }

        [CLProperty(description: "The current special the human is using")]
        public string CurrentSpecial => Human.CurrentSpecial;

        [CLProperty(description: "The cooldown of the special")]
        public float SpecialCooldown
        {
            get => Human.Special == null ? 0f : Human.Special.Cooldown;
            set
            {
                if (Human.Special == null) return;
                var v = Mathf.Max(0f, value);
                Human.Special.Cooldown = v;
            }
        }

        [CLProperty(description: "The live time of the shifter special")]
        public float ShifterLiveTime
        {
            get
            {
                if (Human.Special != null && Human.Special is ShifterTransformSpecial special)
                    return special.LiveTime;
                return 0f;
            }
            set
            {
                if (Human.Special != null && Human.Special is ShifterTransformSpecial special)
                    special.LiveTime = value;
            }
        }

        [CLProperty(description: "The ratio of the special cooldown")]
        public float SpecialCooldownRatio => Human.Special == null ? 0f : Human.Special.GetCooldownRatio();

        [CLProperty(description: "The current gas of the human")]
        public float CurrentGas
        {
            get => Human.Stats.CurrentGas;
            set => Human.Stats.CurrentGas = Mathf.Min(Human.Stats.MaxGas, value);
        }

        [CLProperty(description: "The max gas of the human")]
        public float MaxGas
        {
            get => Human.Stats.MaxGas;
            set => Human.Stats.MaxGas = value;
        }

        [CLProperty(description: "The acceleration of the human")]
        public int Acceleration
        {
            get => Human.Stats.Acceleration;
            set => Human.Stats.Acceleration = value;
        }

        [CLProperty(description: "The speed of the human")]
        public int Speed
        {
            get => Human.Stats.Speed;
            set => Human.Stats.Speed = value;
        }

        [CLProperty(description: "The speed of the horse")]
        public float HorseSpeed
        {
            get => Human.Stats.HorseSpeed;
            set => Human.Stats.HorseSpeed = value;
        }

        [CLProperty(description: "The current blade durability")]
        public float CurrentBladeDurability
        {
            get
            {
                if (Human.Weapon is BladeWeapon bladeWeapon)
                    return bladeWeapon.CurrentDurability;
                return 0f;
            }
            set
            {
                if (Human.Weapon is BladeWeapon bladeWeapon)
                    bladeWeapon.CurrentDurability = Mathf.Min(bladeWeapon.MaxDurability, value);
            }
        }

        [CLProperty(description: "The max blade durability")]
        public float MaxBladeDurability
        {
            get
            {
                if (Human.Weapon is BladeWeapon bladeWeapon)
                    return bladeWeapon.MaxDurability;
                return 0f;
            }
            set
            {
                if (Human.Weapon is BladeWeapon bladeWeapon)
                    bladeWeapon.MaxDurability = value;
            }
        }

        [CLProperty(description: "The current blade")]
        public int CurrentBlade
        {
            get
            {
                if (Human.Weapon is BladeWeapon bladeWeapon)
                    return bladeWeapon.BladesLeft;
                return 0;
            }
            set
            {
                if (Human.Weapon is BladeWeapon bladeWeapon)
                    bladeWeapon.BladesLeft = Mathf.Min(value, bladeWeapon.MaxBlades);
            }
        }

        [CLProperty(description: "The max number of blades held")]
        public int MaxBlade
        {
            get
            {
                if (Human.Weapon is BladeWeapon bladeWeapon)
                    return bladeWeapon.MaxBlades;
                return 0;
            }
            set
            {
                if (Human.Weapon is BladeWeapon bladeWeapon)
                    bladeWeapon.MaxBlades = value;
            }
        }

        [CLProperty(description: "The current ammo round")]
        public int CurrentAmmoRound
        {
            get
            {
                if (Human.Weapon is AmmoWeapon ammoWeapon)
                    return ammoWeapon.RoundLeft;
                return 0;
            }
            set
            {
                if (Human.Weapon is AmmoWeapon ammoWeapon)
                    ammoWeapon.RoundLeft = Mathf.Min(ammoWeapon.MaxRound, value);
            }
        }

        [CLProperty(description: "The max ammo round")]
        public int MaxAmmoRound
        {
            get
            {
                if (Human.Weapon is AmmoWeapon ammoWeapon)
                    return ammoWeapon.MaxRound;
                return 0;
            }
            set
            {
                if (Human.Weapon is AmmoWeapon ammoWeapon)
                    ammoWeapon.MaxRound = value;
            }
        }

        [CLProperty(description: "The current ammo left")]
        public int CurrentAmmoLeft
        {
            get
            {
                if (Human.Weapon is AmmoWeapon ammoWeapon)
                    return ammoWeapon.AmmoLeft;
                return 0;
            }
            set
            {
                if (Human.Weapon is AmmoWeapon ammoWeapon)
                    ammoWeapon.AmmoLeft = Mathf.Min(ammoWeapon.MaxAmmo, value);
            }
        }

        [CLProperty(description: "The max total ammo")]
        public int MaxAmmoTotal
        {
            get
            {
                if (Human.Weapon is AmmoWeapon ammoWeapon)
                    return ammoWeapon.MaxAmmo;
                return 0;
            }
            set
            {
                if (Human.Weapon is AmmoWeapon ammoWeapon)
                    ammoWeapon.MaxAmmo = value;
            }
        }

        [CLProperty(description: "Whether the left hook is enabled")]
        public bool LeftHookEnabled
        {
            get => Human.HookLeft.Enabled;
            set => Human.HookLeft.Enabled = value;
        }

        [CLProperty(description: "Whether the right hook is enabled")]
        public bool RightHookEnabled
        {
            get => Human.HookRight.Enabled;
            set => Human.HookRight.Enabled = value;
        }

        [CLProperty(description: "Whether the human is mounted")]
        public bool IsMounted => Human.MountState == HumanMountState.MapObject;

        [CLProperty(description: "The map object the human is mounted on")]
        public CustomLogicMapObjectBuiltin MountedMapObject
        {
            get
            {
                if (Human.MountedMapObject == null)
                    return null;
                return new CustomLogicMapObjectBuiltin(Human.MountedMapObject);
            }
        }

        [CLProperty(description: "The transform the human is mounted on")]
        public CustomLogicTransformBuiltin MountedTransform
        {
            get
            {
                if (Human.MountedTransform == null)
                    return null;
                return new CustomLogicTransformBuiltin(Human.MountedTransform);
            }
        }

        [CLProperty(description: "Whether the human auto refills gas")]
        public bool AutoRefillGas
        {
            get => Human != null && Human.IsMine() && SettingsManager.InputSettings.Human.AutoRefillGas.Value;
            set
            {
                if (Human != null && Human.IsMine())
                    SettingsManager.InputSettings.Human.AutoRefillGas.Value = value;
            }
        }

        [CLProperty(description: "The state of the human")]
        public string State => Human.State.ToString();

        [CLProperty(description: "Whether the human can dodge")]
        public bool CanDodge
        {
            get => Human.CanDodge;
            set => Human.CanDodge = value;
        }

        [CLProperty(description: "Whether the human is invincible")]
        public bool IsInvincible
        {
            get => Human.IsInvincible;
            set => Human.IsInvincible = value;
        }

        [CLProperty(description: "The time left for invincibility")]
        public float InvincibleTimeLeft
        {
            get => Human.InvincibleTimeLeft;
            set => Human.InvincibleTimeLeft = value;
        }

        // Add CLMethods for the above setField/getField
        [CLMethod(description: "Refills the gas of the human")]
        public bool Refill()
        {
            if (Human.IsMine() && Human.NeedRefill(true))
                return Human.Refill();
            return false;
        }

        [CLMethod(description: "Refills the gas of the human immediately")]
        public void RefillImmediate()
        {
            if (Human.IsMine())
                Human.FinishRefill();
        }

        [CLMethod(description: "Clears all hooks")]
        public void ClearHooks()
        {
            if (Human.IsMine())
            {
                Human.HookLeft.DisableAnyHook();
                Human.HookRight.DisableAnyHook();
            }
        }

        [CLMethod(description: "Clears the left hook")]
        public void ClearLeftHook()
        {
            if (Human.IsMine())
                Human.HookLeft.DisableAnyHook();
        }

        [CLMethod(description: "Clears the right hook")]
        public void ClearRightHook()
        {
            if (Human.IsMine())
                Human.HookRight.DisableAnyHook();
        }

        [CLMethod(description: "Mounts the human on a map object")]
        public void MountMapObject(CustomLogicMapObjectBuiltin mapObject, CustomLogicVector3Builtin positionOffset, CustomLogicVector3Builtin rotationOffset)
        {
            if (Human.IsMine())
                Human.Mount(mapObject.Value, positionOffset.Value, rotationOffset.Value);
        }

        [CLMethod(description: "Mounts the human on a transform")]
        public void MountTransform(CustomLogicTransformBuiltin transform, CustomLogicVector3Builtin positionOffset, CustomLogicVector3Builtin rotationOffset)
        {
            if (Human.IsMine())
                Human.Mount(transform.Value, positionOffset.Value, rotationOffset.Value);
        }

        [CLMethod(description: "Unmounts the human")]
        public void Unmount()
        {
            if (Human.IsMine())
                Human.Unmount(true);
        }

        [CLMethod(description: "Sets the special of the human")]
        public void SetSpecial(string special)
        {
            if (Human.IsMine())
                Human.SetSpecial(special);
        }

        [CLMethod(description: "Activates the special of the human")]
        public void ActivateSpecial()
        {
            if (Human.IsMine() && Human.Special != null)
            {
                Human.Special.SetInput(true);
                Human.Special.SetInput(false);
            }
        }

        [CLMethod(description: "Sets the weapon of the human")]
        public void SetWeapon(string weapon)
        {
            if (!Human.IsMine())
                return;
            var gameManager = (InGameManager)SceneLoader.CurrentGameManager;
            if (gameManager.CurrentCharacter != null && gameManager.CurrentCharacter is Human && Human.IsMine())
            {
                var miscSettings = SettingsManager.InGameCurrent.Misc;
                if (!Human.Dead)
                {
                    List<string> loadouts = new List<string>();
                    if (miscSettings.AllowBlades.Value)
                        loadouts.Add(HumanLoadout.Blades);
                    if (miscSettings.AllowAHSS.Value)
                        loadouts.Add(HumanLoadout.AHSS);
                    if (miscSettings.AllowAPG.Value)
                        loadouts.Add(HumanLoadout.APG);
                    if (miscSettings.AllowThunderspears.Value)
                        loadouts.Add(HumanLoadout.Thunderspears);
                    if (loadouts.Count == 0)
                        loadouts.Add(HumanLoadout.Blades);
                    if (loadouts.Contains(weapon) && weapon != SettingsManager.InGameCharacterSettings.Loadout.Value)
                    {
                        SettingsManager.InGameCharacterSettings.Loadout.Value = weapon;
                        var manager = (InGameManager)SceneLoader.CurrentGameManager;
                        Human = (Human)gameManager.CurrentCharacter;
                        Human.ReloadHuman(manager.GetSetHumanSettings());
                    }
                }
            }
        }

        [CLMethod(description: "Disables all perks of the human")]
        public void DisablePerks()
        {
            if (Human.IsMine())
                Human.Stats.DisablePerks();
        }
        
    }
}
