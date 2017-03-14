using System;
using static AWSR.Models.Constant;

namespace AWSR.Models
{
	/// <summary>
	/// 装備クラス
	/// </summary>
	class Weapon
	{
		// 装備ID
		public int Id { get; set; }
		// 装備名
		public string Name {
			get {
				return (DataBase.ContainsWeapon(Id) ? DataBase.Weapon(Id).Name : "？");
			}
		}
		// 装備改修度
		private int improvement;
		public int Improvement{
			get {
				return improvement;
			}
			set {
				improvement = (value < 0 ? 0 : value > MaxImprovement ? MaxImprovement : value);
			}
		}
		// 艦載機熟練度
		private int proficiency;
		public int Proficiency {
			get {
				return proficiency;
			}
			set {
				proficiency = (value < 0 ? 0 : value > MaxProficiency ? MaxProficiency : value);
			}
		}
		/// <summary>
		/// その艦載機における制空値を計算する
		/// </summary>
		/// <param name="airSize">スロットの搭載数数</param>
		/// <returns><制空値/returns>
		public int AirValue(int airSize) {
			// データベースに存在しない装備における制空値は0とする
			if (!DataBase.ContainsWeapon(Id))
				return 0;
			// 艦戦・艦攻・艦爆・水爆・水戦・噴式以外は制空値0とする
			var weaponData = DataBase.Weapon(Id);
			if (weaponData.Type != "艦上戦闘機"
			&& weaponData.Type != "艦上攻撃機"
			&& weaponData.Type != "艦上爆撃機"
			&& weaponData.Type != "水上爆撃機"
			&& weaponData.Type != "水上戦闘機"
			&& weaponData.Type != "噴式戦闘爆撃機") {
				return 0;
			}
			// 素対空値と改修度
			double airValue = weaponData.AntiAir;
			switch (weaponData.Type) {
			case "艦上戦闘機":
				airValue += 0.2 * Improvement;
				break;
			case "艦上爆撃機":
				// wikiaには「爆戦」とあったので、厳密に爆戦を指定する
				if (weaponData.Name.Contains("爆戦")) {
					airValue += 0.25 * Improvement;
				}
				break;
			}
			// √スロット数を乗算
			airValue *= Math.Sqrt(airSize);
			// 艦載機熟練度
			var nativeProficiency = new int[] { 0, 10, 25, 40, 55, 70, 85, 100 };
			var typeBonusAF = new int[] { 0, 0, 2, 5, 9, 14, 14, 22 };
			var typeBonusWB = new int[] { 0, 0, 1, 1, 1, 3, 3, 6 };
			switch (weaponData.Type) {
			case "艦上戦闘機":
				airValue += Math.Sqrt((double)nativeProficiency[Proficiency] / 10);
				airValue += typeBonusAF[Proficiency];
				break;
			case "水上戦闘機":
				airValue += Math.Sqrt((double)nativeProficiency[Proficiency] / 10);
				airValue += typeBonusAF[Proficiency];
				break;
			case "水上爆撃機":
				airValue += Math.Sqrt((double)nativeProficiency[Proficiency] / 10);
				airValue += typeBonusWB[Proficiency];
				break;
			}
			return (int)(airValue);
		}
	}
}
