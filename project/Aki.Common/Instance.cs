/* Instance.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using UnityEngine;

namespace Aki.Common
{
	public class Instance : MonoBehaviour
	{
		private void Start()
		{
			Debug.LogError("Aki.Common: Loaded");
		}
	}
}
