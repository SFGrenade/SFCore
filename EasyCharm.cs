using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modding;
using UnityEngine;

namespace SFCore
{

    /// <summary>
    /// Serializable state of an EasyCharm
    /// </summary>
    public class EasyCharmState
    {
        /// <summary>
        /// Is this charm equipped by the player
        /// </summary>
        public bool IsEquipped = false;
        /// <summary>
        /// Has this charm been Acquired by the player
        /// </summary>
        public bool GotCharm = false;
        /// <summary>
        /// Is this charm newly Acquired by the player
        /// </summary>
        public bool IsNew = false;
    }
    /// <summary>
    /// An Abstract class representing an EasyCharm
    /// </summary>
    public abstract class EasyCharm
    {
        private Sprite _sprite;
        /// <summary>
        /// The Id of the charm, this is not fixed across sessions.
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Is this charm equipped by the player
        /// </summary>
        public bool IsEquipped { get; protected set; } = false;
        /// <summary>
        /// Has this charm been Acquired by the player
        /// </summary>
        public bool GotCharm { get; protected set; } = false;
        /// <summary>
        /// Is this charm newly Acquired by the player
        /// </summary>
        public bool IsNew { get; protected set; } = false;

        /// <summary>
        /// Method that is used to load the charm sprite
        /// </summary>
        /// <returns>The charm sprite</returns>
        protected abstract Sprite GetSpriteInternal();
        /// <summary>
        /// The Name of the charm
        /// </summary>
        /// <returns>The Name</returns>
        protected abstract string GetName();
        /// <summary>
        /// The Description of the charm
        /// </summary>
        /// <returns>The Description</returns>
        protected abstract string GetDescription();
        /// <summary>
        /// The Cost of the charm
        /// </summary>
        /// <returns>The Cost</returns>
        protected abstract int GetCharmCost();

        /// <summary>
        /// Get the sprite of the charm
        /// </summary>
        /// <returns>The charm sprite</returns>
        public Sprite GetSprite()
        {
            if (_sprite == null)
            {
                _sprite = GetSpriteInternal();
            }
            return _sprite;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public EasyCharm()
        {
            Id = CharmHelper.AddSprites(GetSprite())[0];
            ModHooks.LanguageGetHook += OnLanguageGetHook;
            ModHooks.GetPlayerBoolHook += OnGetPlayerBoolHook;
            ModHooks.SetPlayerBoolHook += OnSetPlayerBoolHook;
            ModHooks.GetPlayerIntHook += OnGetPlayerIntHook;
        }

        private int OnGetPlayerIntHook(string target, int orig)
        {
            // Check if the charm cost is wanted
            if (target == $"charmCost_{Id}")
            {
                return GetCharmCost();
            }
            // Return orig if we don't want to make any changes
            return orig;
        }

        private bool OnSetPlayerBoolHook(string target, bool orig)
        {
            // Check if the charm gotten flag is wanted
            if (target == $"gotCharm_{Id}")
            {
                GotCharm = orig;
            }
            // Check if the charm new flag is wanted
            if (target == $"newCharm_{Id}")
            {
                IsNew = orig;
            }
            // Check if the charm equipped flag is wanted
            if (target == $"equippedCharm_{Id}")
            {
                IsEquipped = orig;
            }
            // Always return orig in set hooks, unless you specifically want to change what is saved
            return orig;
        }

        private bool OnGetPlayerBoolHook(string target, bool orig)
        {
            // Check if the charm gotten flag is wanted
            if (target == $"gotCharm_{Id}")
            {
                return GotCharm;
            }
            // Check if the charm new flag is wanted
            if (target == $"newCharm_{Id}")
            {
                return IsNew;
            }
            // Check if the charm equipped flag is wanted
            if (target == $"equippedCharm_{Id}")
            {
                return IsEquipped;
            }
            // Return orig if we don't want to make any changes
            return orig;
        }

        private string OnLanguageGetHook(string key, string sheetTitle, string orig)
        {
            // Check if the charm name is wanted
            if (key == $"CHARM_NAME_{Id}")
            {
                return GetName();
            }
            // Check if the charm description is wanted
            else if (key == $"CHARM_DESC_{Id}")
            {
                return GetDescription();
            }
            // Return orig if we don't want to make any changes
            return orig;
        }

        /// <summary>
        /// Give the player this charm
        /// </summary>
        /// <param name="consideredNew">Should this charm be considered new</param>
        public void GiveCharm(bool consideredNew = false)
        {
            PlayerData.instance.SetBool($"gotCharm_{Id}", true);
            PlayerData.instance.SetBool($"newCharm_{Id}", consideredNew);
        }
        /// <summary>
        /// Take the charm away from the player
        /// </summary>
        public void TakeCharm()
        {
            PlayerData.instance.SetBool($"gotCharm_{Id}", false);
            PlayerData.instance.SetBool($"newCharm_{Id}", false);
            PlayerData.instance.SetBool($"equippedCharm_{Id}", false);
        }
        /// <summary>
        /// Get the charm state for serialization
        /// </summary>
        /// <returns>The charm state</returns>
        public EasyCharmState GetCharmState()
        {
            return new EasyCharmState { GotCharm = GotCharm, IsEquipped = IsEquipped, IsNew = IsNew };
        }
        /// <summary>
        /// Restore the charm state from EasyCharmState
        /// </summary>
        /// <param name="state">The state to restore</param>
        public void RestoreCharmState(EasyCharmState state)
        {
            GotCharm = state.GotCharm;
            IsEquipped = state.IsEquipped;
            IsNew = state.IsNew;
        }

    }
}
