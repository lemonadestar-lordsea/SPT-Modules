using EFT;
using Aki.SinglePlayer.Utils.Healing;

namespace Aki.SinglePlayer.Models
{
    public class SaveProfileRequest
	{
		public string exit = "left";
		public Profile profile;
		public bool isPlayerScav;
		public PlayerHealth health;
	}
}
