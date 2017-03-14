using System.Collections.Generic;
using System.Text.RegularExpressions;
using static AWSR.Models.Constant;

namespace AWSR.Models
{
	static class DataBase
	{
		// データベースの内部表現(Dictionary)
		private static Dictionary<int, KammusuData> kammusuDictionary;
		private static Dictionary<int, WeaponData> weaponDictionary;
		// データベースを初期化
		public static void Initialize() {
			// 艦娘側のデータベースを読み込む
			kammusuDictionary = new Dictionary<int, KammusuData>();
			using (var sr = new System.IO.StreamReader(@"ships.csv")) {
				while (!sr.EndOfStream) {
					// 1行を読み込む
					string line = sr.ReadLine();
					// マッチさせてから各数値を取り出す
					string pattern = @"(?<Number>\d+),(?<Name>[^,]+),(?<Type>\d+),(?<AntiAir>\d+),(?<SlotCount>\d+),(?<Airs1>\d+)/(?<Airs2>\d+)/(?<Airs3>\d+)/(?<Airs4>\d+)/(?<Airs5>\d+),(?<WeaponId1>(|-)\d+)/(?<WeaponId2>(|-)\d+)/(?<WeaponId3>(|-)\d+)/(?<WeaponId4>(|-)\d+)/(?<WeaponId5>(|-)\d+),(?<IsKammusu>\d)";
					var match = Regex.Match(line, pattern);
					if (!match.Success) {
						continue;
					}
					// 取り出した数値を元に、kammusuDictionaryに代入する
					try {
						// マッチした文字列をそれぞれの型にパースする
						int number = int.Parse(match.Groups["Number"].Value);
						string name = match.Groups["Name"].Value;
						FleetType type = (FleetType)int.Parse(match.Groups["Type"].Value);
						int antiAir = int.Parse(match.Groups["AntiAir"].Value);
						int slotCount = int.Parse(match.Groups["SlotCount"].Value);
						int airs1 = int.Parse(match.Groups["Airs1"].Value);
						int airs2 = int.Parse(match.Groups["Airs2"].Value);
						int airs3 = int.Parse(match.Groups["Airs3"].Value);
						int airs4 = int.Parse(match.Groups["Airs4"].Value);
						int airs5 = int.Parse(match.Groups["Airs5"].Value);
						int weaponId1 = int.Parse(match.Groups["WeaponId1"].Value);
						int weaponId2 = int.Parse(match.Groups["WeaponId2"].Value);
						int weaponId3 = int.Parse(match.Groups["WeaponId3"].Value);
						int weaponId4 = int.Parse(match.Groups["WeaponId4"].Value);
						int weaponId5 = int.Parse(match.Groups["WeaponId5"].Value);
						int isKammusu_ = int.Parse(match.Groups["IsKammusu"].Value);
						bool isKammusu = (isKammusu_ != 0);
						// KammusuData型を生成し、Dictionaryに登録する
						var kammusuData = new KammusuData(
							name, type, antiAir, slotCount,
							new int[] { airs1, airs2, airs3, airs4, airs5 },
							new int[] { weaponId1, weaponId2, weaponId3, weaponId4, weaponId5 },
							isKammusu);
						kammusuDictionary[number] = kammusuData;
					}
					catch {
						continue;
					}
				}
			}
			// 装備側のデータベースを読み込む
			weaponDictionary = new Dictionary<int, WeaponData>();
			using (var sr = new System.IO.StreamReader(@"slotitems.csv")) {
				while (!sr.EndOfStream) {
					// 1行を読み込む
					string line = sr.ReadLine();
					// マッチさせてから各数値を取り出す
					string pattern = @"(?<Number>\d+),(?<Name>[^,]+),(?<Type>[^,]+),(?<AntiAir>\d+),(?<AntiBomb>\d+),(?<Intercept>\d+)";
					var match = Regex.Match(line, pattern);
					if (!match.Success) {
						continue;
					}
					// 取り出した数値を元に、weaponDictionaryに代入する
					try {
						// マッチした文字列をそれぞれの型にパースする
						int number = int.Parse(match.Groups["Number"].Value);
						string name = match.Groups["Name"].Value;
						string type = match.Groups["Type"].Value;
						int antiAir = int.Parse(match.Groups["AntiAir"].Value);
						int antiBomb = int.Parse(match.Groups["AntiBomb"].Value);
						int intercept = int.Parse(match.Groups["Intercept"].Value);
						// WeaponData型を生成し、Dictionaryに登録する
						var weaponData = new WeaponData(name, type, antiAir, antiBomb, intercept);
						weaponDictionary[number] = weaponData;
					}
					catch {
						continue;
					}
				}
			}
		}
		// そのID()の艦娘・装備が存在するかを判定する
		public static bool ContainsKammusu(int id) {
			return kammusuDictionary.ContainsKey(id);
		}
		public static bool ContainsWeapon(int id) {
			return weaponDictionary.ContainsKey(id);
		}
		// データを読み込む
		public static KammusuData Kammusu(int id) {
			KammusuData temp;
			if(kammusuDictionary.TryGetValue(id, out temp)){
				return temp;
			}
			else {
				return null;
			}
		}
		public static WeaponData Weapon(int id) {
			WeaponData temp;
			if (weaponDictionary.TryGetValue(id, out temp)) {
				return temp;
			}
			else {
				return null;
			}
		}
	}
	// 艦娘データの内部表現
	class KammusuData
	{
		// フィールド
		// 艦名,艦種,対空,搭載数,初期装備,艦娘フラグ
		public string Name { get; private set; }
		public FleetType Type { get; private set; }
		public int AntiAir { get; private set; }
		public int SlotCount { get; private set; }
		public int[] Airs { get; private set; }
		public int[] WeaponId { get; private set; }
		public bool IsKammusu { get; private set; }
		// コンストラクタ
		public KammusuData(string name, FleetType type, int antiAir, int slotCount, int[] airs, int[] weaponId, bool isKammusu) {
			Name = name;
			Type = type;
			AntiAir = antiAir;
			SlotCount = slotCount;
			Airs = airs;
			WeaponId = weaponId;
			IsKammusu = isKammusu;
		}
	}
	// 装備データの内部表現
	class WeaponData
	{
		// フィールド
		// 装備名,種別,対空,対爆,迎撃
		public string Name { get; private set; }
		public string Type { get; private set; }
		public int AntiAir { get; private set; }
		public int AntiBomb { get; private set; }
		public int Intercept { get; private set; }
		// コンストラクタ
		public WeaponData(string name, string type, int antiAir, int antiBomb, int intercept) {
			Name = name;
			Type = type;
			AntiAir = antiAir;
			AntiBomb = antiBomb;
			Intercept = intercept;
		}
	}
}
