using Aki.Common;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT;
using System;
using System.Reflection;
using UnityEngine;

namespace Aki.Singleplayer.Patches.Dev
{
    public class CoordinatesPatch : Patch
    {
        // private static TextMeshProUGUI _alphaLabel;
        private static PropertyInfo _playerProperty;

        public CoordinatesPatch() : base(T: typeof(CoordinatesPatch), prefix: nameof(PrefixPatch))
        {
        }

        private static void PrefixPatch(object __instance)
        {
            //if (Input.GetKeyDown(KeyCode.LeftControl))
            //{
            //    if (_alphaLabel == null)
            //    {
            //        _alphaLabel = GameObject.Find("AlphaLabel").GetComponent<TextMeshProUGUI>();
            //        _alphaLabel.color = Color.green;
            //        _alphaLabel.fontSize = 22;
            //        _alphaLabel.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/ARIAL SDF");
            //    }

            //    var playerOwner = (GamePlayerOwner)_playerProperty.GetValue(__instance);
            //    var aiming = LookingRaycast(playerOwner.Player);

            //    if (_alphaLabel != null)
            //    {
            //        _alphaLabel.text = $"Looking at: [{aiming.x}, {aiming.y}, {aiming.z}]";
            //        Log.Info(_alphaLabel.text);
            //    }

            //    var position = playerOwner.transform.position;
            //    var rotation = playerOwner.transform.rotation.eulerAngles;
            //    Log.Info($"Character position: [{position.x},{position.y},{position.z}] | Rotation: [{rotation.x},{rotation.y},{rotation.z}]");
            //}
        }

        protected override MethodBase GetTargetMethod()
        {
            var localGameBaseType = Constants.LocalGameType.BaseType;
            _playerProperty = localGameBaseType.GetProperty("PlayerOwner", BindingFlags.Public | BindingFlags.Instance);
            return localGameBaseType.GetMethod("Update", Constants.PrivateFlags);
        }

        public static Vector3 LookingRaycast(Player player)
        {
            try
            {
                if (player == null || player.Fireport == null)
                {
                    return Vector3.zero;
                }

                Physics.Linecast(player.Fireport.position, player.Fireport.position - player.Fireport.up * 1000f, out var raycastHit, 331776);
                return raycastHit.point;
            }
            catch (Exception e)
            {
                Log.Error($"Coordinate Debug raycast failed: {e.Message}");
                return Vector3.zero;
            }
        }
    }
}
