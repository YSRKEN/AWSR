using System.Collections.Generic;
using static AWSR.Models.Constant;

namespace AWSR.Models
{
	static class DataBase
	{
		private static Dictionary<int, KammusuData> kammusuDictionary;
		// データベースを初期化
		public static void Initialize()
		{

		}
		// 艦娘データの内部表現
		class KammusuData
		{
			public string Name { get; private set; }
			public FleetType Type { get; private set; }
			public int AntiAir { get; private set; }
			public int[] Airs { get; private set; }
			public int[] WeaponId { get; private set; }
			public bool IsKammusu { get; private set; }
			// コンストラクタ
			public KammusuData(string name, FleetType type, int antiAir, int[] airs, int[] weaponId, bool isKammusu)
			{
				Name = name;
				Type = type;
				AntiAir = antiAir;
				Airs = airs;
				WeaponId = weaponId;
				IsKammusu = isKammusu;
			}
		}
	}
}
