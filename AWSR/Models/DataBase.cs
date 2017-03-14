using System.Collections.Generic;
using System.Text.RegularExpressions;
using static AWSR.Models.Constant;

namespace AWSR.Models
{
	static class DataBase
	{
		private static Dictionary<int, KammusuData> kammusuDictionary;
		// データベースを初期化
		public static void Initialize() {
			// 艦娘側のデータベースを読み込む
			kammusuDictionary = new Dictionary<int, KammusuData>();
			using (var sr = new System.IO.StreamReader(@"ships.csv")) {
				while (!sr.EndOfStream) {
					// 1行を読み込む
					string line = sr.ReadLine();
					// マッチさせてから各数値を取り出す
					string pattern = @"(?<Number>\d+),(?<Name>[^,]+),(?<Type>\d+),(?<AntiAir>\d+),(?<Airs1>\d+)/(?<Airs2>\d+)/(?<Airs3>\d+)/(?<Airs4>\d+)/(?<Airs5>\d+),(?<WeaponId1>(|-)\d+)/(?<WeaponId2>(|-)\d+)/(?<WeaponId3>(|-)\d+)/(?<WeaponId4>(|-)\d+)/(?<WeaponId5>(|-)\d+),(?<IsKammusu>\d)";
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
							name, type, antiAir,
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
			return;
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
			public KammusuData(string name, FleetType type, int antiAir, int[] airs, int[] weaponId, bool isKammusu) {
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
