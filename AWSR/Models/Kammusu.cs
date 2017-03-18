using System;
using System.Linq;
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
		private int id;
		public int Id {
			get { return id; }
			set { id = value; }
		}
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
		// 所持装備
		private List<Weapon> weapon;
		public List<Weapon> Weapon {
			get { return weapon; }
			set { weapon = value; }
		}
		// 艦娘か否か
		public bool IsKammusu { get; set; }
		// 艦名
		public string Name {
			get {
				return DataBase.Kammusu(Id).Name;
			}
		}
		// 搭載数
		public int[] Airs {
			get {
				return DataBase.Kammusu(Id).Airs;
			}
		}
		// スロット数
		private int slotCount;
		public int SlotCount {
			get {
				return slotCount;
			}
		}
		// 対空値
		public int AntiAir {
			get {
				return DataBase.Kammusu(Id).AntiAir;
			}
		}
		// 艦種
		public FleetType Type {
			get {
				return DataBase.Kammusu(Id).Type;
			}
		}
		// 加重対空値
		public int WeightAntiAir {
			get {
				// 素対空値(艦娘か深海棲艦かで変化する)
				double weightAntiAir;
				if (IsKammusu) {
					weightAntiAir = AntiAir;
				}
				else {
					weightAntiAir = (int)(2.0 * Math.Sqrt(AntiAir));
				}
				bool hasWeaponFlg = false;
				// 装備毎の加重対空値を加算する
				foreach (var weapon in Weapon) {
					// 装備データを取得する
					if (weapon.Name != "―")
						hasWeaponFlg = true;
					// 装備の素対空値
					double tempAntiAir = weapon.AntiAir;
					// 装備の種類から、装備倍率と改修倍率を算出する
					double ri = 0.0, gi = 0.0;
					if (weapon.Name.Contains("高角砲")) {
						// 高角砲の場合
						ri = 4.0; gi = 3.0;
					}
					else if (weapon.Name.Contains("高射装置")) {
						// 高射装置の場合
						ri = 4.0; gi = 0.0;
					}
					else if (weapon.Type == "対空機銃") {
						// 機銃の場合
						ri = 6.0; gi = 4.0;
					}
					else if (weapon.Type.Contains("電探")) {
						ri = 3.0; gi = 0.0;
					}
					// 装備倍率と改修倍率を加算する
					weightAntiAir += ri * tempAntiAir + gi * Math.Sqrt(weapon.Improvement);
				}
				return (hasWeaponFlg ? (int)(weightAntiAir / 2) * 2 : (int)(weightAntiAir));
			}
		}
		// カットインの種類
		private CutInType cutInType = CutInType.None;
		public CutInType CutInType {
			get {
				return cutInType;
			}
		}
		// 事前計算した値を確定させる
		public void Complete() {
			cutInType = CalcCutInType();
			slotCount = DataBase.Kammusu(Id).SlotCount;
		}
		private CutInType CalcCutInType() {
			// 「効果が高い順」に判定する
			// 優先度1：専用カットイン(同一グループ内では強い方を優先)
			if (Name.Contains("秋月") || Name.Contains("照月") || Name.Contains("初月")) {
				int hagCount = Weapon.Count(w => w.Name.Contains("高角砲") || w.Name == "5inch連装砲 Mk.28 mod.2");
				bool radarAny = Weapon.Any(w => w.Type.Contains("電探"));
				if (hagCount >= 2 && radarAny)
					return CutInType.Akiduki1;
				if (hagCount >= 1 && radarAny)
					return CutInType.Akiduki2;
				if (hagCount >= 2)
					return CutInType.Akiduki3;
			}
			if (Name == "摩耶改二") {
				bool hagAny = Weapon.Any(w => w.Name.Contains("高角砲") || w.Name == "5inch連装砲 Mk.28 mod.2");
				bool aagSpecialAny = Weapon.Any(w =>
					w.Name == "25mm三連装機銃 集中配備"
					|| w.Name == "Bofors 40mm四連装機関砲"
					|| w.Name == "QF 2ポンド8連装ポンポン砲"
				);
				bool radarAntiAirAny = Weapon.Any(w =>
					w.Type.Contains("電探")
					&& (w.Name.Contains("対空")
					|| w.Name == "FuMO25 レーダー"
					|| w.Name == "15m二重測距儀+21号電探改二")
				);
				if (hagAny && aagSpecialAny && radarAntiAirAny)
					return CutInType.Maya1;
				if (hagAny && aagSpecialAny)
					return CutInType.Maya2;
			}
			// 五十鈴改二
			if (Name == "五十鈴改二") {
				bool hagAny = Weapon.Any(w => w.Name.Contains("高角砲") || w.Name == "5inch連装砲 Mk.28 mod.2");
				bool aagAny = Weapon.Any(w => w.Type == "対空機銃");
				bool radarAntiAirAny = Weapon.Any(w =>
					w.Type.Contains("電探")
					&& (w.Name.Contains("対空")
					|| w.Name == "FuMO25 レーダー"
					|| w.Name == "15m二重測距儀+21号電探改二")
				);
				if (hagAny && aagAny && radarAntiAirAny)
					return CutInType.Isuzu1;
				if (hagAny && aagAny)
					return CutInType.Isuzu2;
			}
			if (Name == "霞改二乙") {
				bool hagAny = Weapon.Any(w => w.Name.Contains("高角砲") || w.Name == "5inch連装砲 Mk.28 mod.2");
				bool aagAny = Weapon.Any(w => w.Type == "対空機銃");
				bool radarAntiAirAny = Weapon.Any(w =>
					w.Type.Contains("電探")
					&& (w.Name.Contains("対空")
					|| w.Name == "FuMO25 レーダー"
					|| w.Name == "15m二重測距儀+21号電探改二")
				);
				if (hagAny && aagAny && radarAntiAirAny)
					return CutInType.Kasumi1;
				if (hagAny && aagAny)
					return CutInType.Kasumi2;
			}
			if (Name == "皐月改二") {
				bool aagSpecialAny = Weapon.Any(w =>
					w.Name == "25mm三連装機銃 集中配備"
					|| w.Name == "Bofors 40mm四連装機関砲"
					|| w.Name == "QF 2ポンド8連装ポンポン砲"
				);
				if (aagSpecialAny)
					return CutInType.Satsuki;
			}
			if (Name == "鬼怒改二") {
				bool hagNormalAny = Weapon.Any(w =>
					w.Name.Contains("高角砲")
					&& w.Name != "10cm連装高角砲+高射装置"
					&& w.Name != "12.7cm高角砲+高射装置"
					&& w.Name != "90mm単装高角砲"
					&& w.Name != "5inch連装砲 Mk.28 mod.2"
				);
				bool aagSpecialAny = Weapon.Any(w =>
					w.Name == "25mm三連装機銃 集中配備"
					|| w.Name == "Bofors 40mm四連装機関砲"
					|| w.Name == "QF 2ポンド8連装ポンポン砲"
				);
				if (hagNormalAny && aagSpecialAny)
					return CutInType.Kinu1;
				if (aagSpecialAny)
					return CutInType.Kinu2;
			}
			// 優先度2：通常カットイン
			// 4→5→6→8→7→12→9の順で判定する
			{
				bool isBB = (
					Type == FleetType.CC
					|| Type == FleetType.BB
					|| Type == FleetType.BBV);
				bool gumLargeAny = Weapon.Any(w => w.Type == "大口径主砲");
				bool sansikiAny = Weapon.Any(w => w.Type == "対空強化弾");
				bool aadAny = Weapon.Any(w =>
					w.Name.Contains("高射装置")
					|| w.Name == "10cm連装高角砲+高射装置"
					|| w.Name == "12.7cm高角砲+高射装置"
					|| w.Name == "90mm単装高角砲"
					|| w.Name == "5inch連装砲 Mk.28 mod.2"
				);
				bool radarAntiAirAny = Weapon.Any(w =>
					w.Type.Contains("電探")
					&& (w.Name.Contains("対空")
					|| w.Name == "FuMO25 レーダー"
					|| w.Name == "15m二重測距儀+21号電探改二")
				);
				// 4
				if (isBB && gumLargeAny && sansikiAny && aadAny && radarAntiAirAny)
					return CutInType.BattleShip1;
				// 5
				int hagSpecialCount = Weapon.Count(w =>
					w.Name == "10cm連装高角砲+高射装置"
					|| w.Name == "12.7cm高角砲+高射装置"
					|| w.Name == "90mm単装高角砲"
					|| w.Name == "5inch連装砲 Mk.28 mod.2"
				);
				if (hagSpecialCount >= 2 && radarAntiAirAny)
					return CutInType.Normal1;
				// 6
				if (isBB && gumLargeAny && sansikiAny && aadAny)
					return CutInType.BattleShip2;
				// 8
				if (hagSpecialCount >= 1 && radarAntiAirAny)
					return CutInType.Normal3;
				// 7
				bool hagAny = Weapon.Any(w => w.Name.Contains("高角砲") || w.Name == "5inch連装砲 Mk.28 mod.2");
				if (hagAny && aadAny && radarAntiAirAny)
					return CutInType.Normal2;
				// 12
				int aagSpecialCount = Weapon.Count(w =>
					w.Name == "25mm三連装機銃 集中配備"
					|| w.Name == "Bofors 40mm四連装機関砲"
					|| w.Name == "QF 2ポンド8連装ポンポン砲"
				);
				int aagCount = Weapon.Count(w => w.Type == "対空機銃");
				if (aagSpecialCount >= 1 && aagCount >= 2 && radarAntiAirAny)
					return CutInType.Normal5;
				// 9
				if (hagAny && aadAny)
					return CutInType.Normal4;
			}
			// カットインなし
			return CutInType.None;
		}
		// コンストラクタ
		public Kammusu() {
			Weapon = new List<Weapon>();
		}
	}
}
