using System.Collections;
using UnityEngine;

namespace Characters
{
    class BladeWeapon : HoldUseable
    {
        public float MaxDurability;
        public float CurrentDurability;
        public int MaxBlades;
        public int BladesLeft;
        private Human _human;

        public BladeWeapon(BaseCharacter owner, float durability, int blades): base(owner)
        {
            BladesLeft = MaxBlades = blades;
            CurrentDurability = MaxDurability = durability;
            _human = (Human)owner;
            _human.ChangeHeldAmmo(BladesLeft, true, false);
        }

        public void UseDurability(float amount)
        {
            CurrentDurability -= amount;
            CurrentDurability = Mathf.Max(CurrentDurability, 0f);
        }

        public override void Reload()
        {
            if (BladesLeft > 0)
            {
                BladesLeft--;
                CurrentDurability = MaxDurability;
            }
            _human.ChangeHeldAmmo(BladesLeft, true, true);
        }

        public override void Reset()
        {
            BladesLeft = MaxBlades;
            CurrentDurability = MaxDurability;
            _human.ChangeHeldAmmo(BladesLeft, true, true);
        }

        public override bool CanUse()
        {
            return base.CanUse() && CurrentDurability > 0f && ((Human)_owner).State == HumanState.Idle;
        }

        protected override void Activate()
        {
            ((Human)_owner).StartBladeSwing();
        }

        protected override void Deactivate()
        {
        }

        protected override void ActiveFixedUpdate()
        {
        }
    }
}
