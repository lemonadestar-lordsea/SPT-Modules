using System.Reflection;
using UnityEngine;
using TMPro;
using EFT;
using Aki.Common;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;

namespace Aki.Singleplayer.Patches.Dev
{
    public class CoordinatesPatch : GenericPatch<CoordinatesPatch>
    {
        private static TextMeshProUGUI alphaLabel = null;
        public static PropertyInfo _playerProperty;
        public static PropertyInfo _uiProperty;
        public static bool firstTime = true;

        public CoordinatesPatch() : base(prefix: nameof(PrefixPatch))
        {
        }

        static void PrefixPatch(object __instance)
        {
			if (firstTime)
			{
				alphaLabel = GameObject.Find("AlphaLabel").GetComponent<TextMeshProUGUI>();
				alphaLabel.color = Color.green;
				alphaLabel.fontSize = 22;
				alphaLabel.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/ARIAL SDF");
				firstTime = false;
			}

            var playerOwner = (GamePlayerOwner)_playerProperty.GetValue(__instance);
            var aiming = LookingRaycast(playerOwner.Player);
			bool keyDown = Input.GetKeyDown(KeyCode.LeftControl);
            string PlayerPosition = null;

			if (alphaLabel != null && keyDown)
				alphaLabel.text = $"Looking at: [{aiming.x}, {aiming.y}, {aiming.z}]";
                PlayerPosition = $"Here is the character position: [{playerOwner.transform.position.x},{playerOwner.transform.position.y},{playerOwner.transform.position.z},{playerOwner.transform.rotation}]";
                Log.Info(alphaLabel.text);
                Log.Info(PlayerPosition);
        }

        protected override MethodBase GetTargetMethod()
        {
            var localGameBaseType = Constants.LocalGameType.BaseType;
            _playerProperty = localGameBaseType.GetProperty("PlayerOwner", BindingFlags.Public | BindingFlags.Instance);
            _uiProperty = localGameBaseType.GetProperty("GameUi", BindingFlags.NonPublic | BindingFlags.Instance);
            return localGameBaseType.GetMethod("Update", Constants.PrivateFlags);
        }

        public static Vector3 LookingRaycast(Player player)
        {
            try
            {
                if (player != null && player.Fireport == null)
                    return Vector3.zero;

                RaycastHit raycastHit = new RaycastHit();
                Physics.Linecast(player.Fireport.position, player.Fireport.position - player.Fireport.up * 1000f, out raycastHit, 331776);
                return raycastHit.point;
            }
            catch
            {
                return Vector3.zero;
            }
        }
    }
}
