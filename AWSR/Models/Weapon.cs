using System;
using System.Collections.Generic;
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
				return DataBase.Weapon(Id).Name;
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
		// 迎撃値
		public int Intercept {
			get {
				return DataBase.Weapon(Id).Intercept;
			}
		}
		// 種類
		private string type;
		public string Type {
			get {
				return type;
			}
		}
		// St1で撃墜されうるか
		private bool isStage1;
		private bool CalcIsStage1() {
			if (Type == "艦上戦闘機"
			|| Type == "艦上攻撃機"
			|| Type == "艦上爆撃機"
			|| Type == "水上爆撃機"
			|| Type == "水上戦闘機"
			|| Type == "噴式戦闘爆撃機") {
				return true;
			}
			else {
				return false;
			}
		}
		public bool IsStage1 {
			get {
				return isStage1;
			}
		}
		// St1で撃墜されうるか(基地航空隊編)
		private bool isStage1X;
		private bool CalcIsStage1X() {
			if (Type == "艦上戦闘機"
			|| Type == "艦上攻撃機"
			|| Type == "艦上爆撃機"
			|| Type == "水上爆撃機"
			|| Type == "水上戦闘機"
			|| Type == "噴式戦闘爆撃機"
			|| Type == "陸上攻撃機"
			|| Type == "局地戦闘機"
			|| Type == "艦上偵察機"
			|| Type == "水上偵察機"
			|| Type == "大型飛行艇") {
				return true;
			}
			else {
				return false;
			}
		}
		public bool IsStage1X {
			get {
				return isStage1X;
			}
		}
		// St2で撃墜されうるか
		private bool isStage2;
		private bool CalcIsStage2() {
			if (Type == "艦上攻撃機"
			|| Type == "艦上爆撃機"
			|| Type == "水上爆撃機"
			|| Type == "噴式戦闘爆撃機") {
				return true;
			}
			else {
				return false;
			}
		}
		public bool IsStage2 {
			get {
				return isStage2;
			}
		}
		// St2で撃墜されうるか(基地航空隊編)
		private bool isStage2X;
		private bool CalcIsStage2X() {
			if (Type == "艦上攻撃機"
			|| Type == "艦上爆撃機"
			|| Type == "水上爆撃機"
			|| Type == "噴式戦闘爆撃機"
			|| Type == "陸上攻撃機") {
				return true;
			}
			else {
				return false;
			}
		}
		public bool IsStage2X {
			get {
				return isStage2X;
			}
		}
		// 対空値
		public int AntiAir {
			get {
				return DataBase.Weapon(Id).AntiAir;
			}
		}
		// 制空値
		public List<int> airValue;
		private List<int> CalcAirValue() {
			var airValue = new List<int>();
			for(int i = 0; i < 300; ++i) {
				airValue.Add(CalcAirValueImpl(i));
			}
			return airValue;
		}
		public int AirValue(int airSize) {
			return airValue[airSize];
		}
		/// <summary>
		/// その艦載機における制空値を計算する
		/// </summary>
		/// <param name="airSize">スロットの搭載数数</param>
		/// <returns><制空値/returns>
		private int CalcAirValueImpl(int airSize) {
			// データベースに存在しない装備における制空値は0とする
			if (!DataBase.ContainsWeapon(Id))
				return 0;
			// 艦戦・艦攻・艦爆・水爆・水戦・噴式以外は制空値0とする
			if (!IsStage1)
				return 0;
			// 素対空値と改修度
			double airValue = AntiAir;
			switch (Type) {
			case "艦上戦闘機":
				airValue += 0.2 * Improvement;
				break;
			case "艦上爆撃機":
				// wikiaには「爆戦」とあったので、厳密に爆戦を指定する
				if (Name.Contains("爆戦")) {
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
			switch (Type) {
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
			case "艦上攻撃機":
				airValue += Math.Sqrt((double)nativeProficiency[Proficiency] / 10);
				break;
			case "艦上爆撃機":
				airValue += Math.Sqrt((double)nativeProficiency[Proficiency] / 10);
				break;
			case "噴式戦闘爆撃機":
				airValue += Math.Sqrt((double)nativeProficiency[Proficiency] / 10);
				break;
			}
			return (int)(airValue);
		}
		// 制空値(基地航空隊編)
		public List<int> airValueX;
		private List<int> CalcAirValueX() {
			var airValueX = new List<int>();
			for (int i = 0; i < 300; ++i) {
				airValueX.Add(CalcAirValueXImpl(i));
			}
			return airValueX;
		}
		public int AirValueX(int airSize) {
			return airValueX[airSize];
		}
		/// <summary>
		/// その艦載機における制空値を計算する
		/// </summary>
		/// <param name="airSize">スロットの搭載数数</param>
		/// <returns><制空値/returns>
		private int CalcAirValueXImpl(int airSize) {
			// データベースに存在しない装備における制空値は0とする
			if (!DataBase.ContainsWeapon(Id))
				return 0;
			// ステージ1に参加しなければ制空値0とする
			if (!isStage1X)
				return 0;
			// 素対空値と改修度
			double airValue = AntiAir;
			switch (Type) {
				case "艦上戦闘機":
				airValue += 0.2 * Improvement;
				break;
				case "艦上爆撃機":
				// wikiaには「爆戦」とあったので、厳密に爆戦を指定する
				if (Name.Contains("爆戦")) {
					airValue += 0.25 * Improvement;
				}
				break;
			}
			// 迎撃値
			airValue += 1.5 * Intercept;
			// √スロット数を乗算
			airValue *= Math.Sqrt(airSize);
			// 艦載機熟練度
			var nativeProficiency = new int[] { 0, 10, 25, 40, 55, 70, 85, 100 };
			var typeBonusAF = new int[] { 0, 0, 2, 5, 9, 14, 14, 22 };
			var typeBonusWB = new int[] { 0, 0, 1, 1, 1, 3, 3, 6 };
			switch (Type) {
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
			case "局地戦闘機":
				airValue += Math.Sqrt((double)nativeProficiency[Proficiency] / 10);
				airValue += typeBonusAF[Proficiency];
				break;
			case "陸上攻撃機":
				airValue += Math.Sqrt((double)nativeProficiency[Proficiency] / 10);
				break;
			case "艦上攻撃機":
				airValue += Math.Sqrt((double)nativeProficiency[Proficiency] / 10);
				break;
			case "艦上爆撃機":
				airValue += Math.Sqrt((double)nativeProficiency[Proficiency] / 10);
				break;
			case "噴式戦闘爆撃機":
				airValue += Math.Sqrt((double)nativeProficiency[Proficiency] / 10);
				break;
			case "大型飛行艇":
				airValue += Math.Sqrt((double)nativeProficiency[Proficiency] / 10);
				break;
			case "水上偵察機":
				airValue += Math.Sqrt((double)nativeProficiency[Proficiency] / 10);
				break;
			case "艦上偵察機":
				airValue += Math.Sqrt((double)nativeProficiency[Proficiency] / 10);
				break;
			}
			return (int)(airValue);
		}
		// 事前計算した値を確定させる
		public void Complete() {
			type = DataBase.Weapon(Id).Type;
			isStage1 = CalcIsStage1();
			isStage2 = CalcIsStage2();
			airValue = CalcAirValue();
			isStage1X = CalcIsStage1X();
			isStage2X = CalcIsStage2X();
			airValueX = CalcAirValueX();
		}
	}
}
