using System;
using System.Collections.Generic;
using static AWSR.Models.Constant;

namespace AWSR.Models
{
	/// <summary>
	/// 艦娘クラス
	/// </summary>
	class Kammusu
	{
		// 艦船ID
		public int Id { get; set; }
		// レベル
		private int level;
		public int Level {
			get {
				return level;
			}
			set {
				level = (value < 1 ? 1 : value > MaxLevel ? MaxLevel : value);
			}
		}
		// 運
		private int luck;
		public int Luck {
			get {
				return luck;
			}
			set {
				luck = (value < -1 ? -1 : value > MaxLuck ? MaxLuck : value);
			}
		}
		// 艦名
		public string Name {
			get {
				return DataBase.Kammusu(Id).Name;
			}
		}
		// 所持装備
		public List<Weapon> Weapon { get; set; }
		// 艦娘か否か
		public bool IsKammusu { get; set; }
		// 加重対空値
		public int WeightAntiAir {
			get {
				// 素対空値
				double weightAntiAir = DataBase.Kammusu(Id).AntiAir;
				bool hasWeaponFlg = false;
				// 装備毎の加重対空値を加算する
				foreach(var weapon in Weapon) {
					// 装備データを取得する
					var weaponData = DataBase.Weapon(weapon.Id);
					if (weaponData.Name != "―")
						hasWeaponFlg = true;
					// 装備の素対空値
					double tempAntiAir = weaponData.AntiAir;
					// 装備の種類から、装備倍率と改修倍率を算出する
					double ri = 0.0, gi = 0.0;
					if (weaponData.Name.Contains("高角砲")) {
						// 高角砲の場合
						ri = 4.0; gi = 3.0;
					}else if (weaponData.Name.Contains("高射装置")) {
						// 高射装置の場合
						ri = 4.0; gi = 0.0;
					}else if(weaponData.Type == "対空機銃") {
						// 機銃の場合
						ri = 6.0; gi = 4.0;
					}else if (weaponData.Type.Contains("電探")) {
						ri = 3.0; gi = 0.0;
					}
					// 装備倍率と改修倍率を加算する
					weightAntiAir += ri * tempAntiAir + gi * Math.Sqrt(weapon.Improvement);
				}
				return (hasWeaponFlg ? (int)(weightAntiAir / 2) * 2 : (int)(weightAntiAir));
			}
		}
		// コンストラクタ
		public Kammusu() {
			Weapon = new List<Weapon>();
		}
	}
}
