using System.Collections.Generic;
using System.Linq;
using static AWSR.Models.Constant;

namespace AWSR.Models
{
	/// <summary>
	/// 大艦隊クラス
	/// (連合艦隊＝艦隊×2と考える)
	/// </summary>
	class Fleet
	{
		// 大艦隊に属する艦隊
		public List<Unit> Unit { get; set; }
		// 陣形
		public Formation Formation { get; set; }
		// 文字情報
		public string ToInfoText() {
			string output = "";
			output += $"陣形 : {Formation.ToStr()}\n";
			// Selectメソッドでインデックスを付加する手法は次のURLを参考にした
			// http://blog.okazuki.jp/entry/20100728/1280300415
			foreach (var unit in Unit.Select((v, i) => new { v, i })) {
				foreach (var kammusu in unit.v.Kammusu.Select((v, i) => new { v, i })) {
					output += (unit.i != 0 ? $"({unit.i + 1}-{kammusu.i + 1})" : $"({kammusu.i + 1})");
					output += $"{kammusu.v.Name}";
					output += (kammusu.v.Level != 0 ? $" Lv.{kammusu.v.Level}" : "");
					output += (kammusu.v.Luck != -1 ? $" 運{kammusu.v.Luck}" : "");
					foreach (var weapon in kammusu.v.Weapon.Select((v, i) => new { v, i })) {
						// 艦娘がデータベースに載っている際はスロット数を調べ、
						// そこをあふれる位置＝拡張スロットに装備が載ってなければ
						// その部分の表示を省くようにした
						if (DataBase.ContainsKammusu(kammusu.v.Id)
						&& weapon.i >= DataBase.Kammusu(kammusu.v.Id).SlotCount
						&& weapon.v.Id == -1)
							break;
						output += (weapon.i == 0 ? "　" : ",");
						output += $"{weapon.v.Name}";
						output += ToMasStr(weapon.v.Proficiency);
						output += (weapon.v.Improvement > 0 ? $"★{weapon.v.Improvement}" : "");
					}
					output += "\n";
				}
			}
			return output;
		}
		// デッキビルダーのJSONに変換
		public string ToDeckBuilderText() {
			string[] weaponIndexName = { "i1", "i2", "i3", "i4", "ix" };
			string output = "";
			// 艦隊情報を先頭から書き込んでいく
			output += $"{{\"version\":4,";
			foreach (var unit in Unit.Select((v, i) => new { v, i })) {
				output += (unit.i != 0 ? "," : "");
				output += $"\"f{unit.i + 1}\":{{";
				foreach (var kammusu in unit.v.Kammusu.Select((v, i) => new { v, i })) {
					output += (kammusu.i != 0 ? "," : "");
					output += $"\"s{kammusu.i + 1}\":{{";
					output += $"\"id\":\"{kammusu.v.Id}\",";
					output += $"\"lv\":{kammusu.v.Level},";
					output += $"\"luck\":{kammusu.v.Luck},";
					output += "\"items\":{";
					foreach (var weapon in kammusu.v.Weapon.Select((v, i) => new { v, i })) {
						if (weapon.v.Id == -1)
							continue;
						output += (weapon.i != 0 ? "," : "");
						output += $"\"{weaponIndexName[weapon.i]}\":{{";
						output += $"\"id\":{weapon.v.Id},";
						output += $"\"rf\":\"{weapon.v.Improvement}\",";
						output += $"\"mas\":{weapon.v.Proficiency}";
						output += "}";
					}
					output += "}}";
				}
				output += "}";
			}
			output += "}";
			// 出力
			return output;
		}
		// 制空値を計算
		public int AirValue() {
			int airValue = 0;
			foreach(var unit in Unit){
				foreach(var kammusu in unit.Kammusu) {
					// データベースに存在しない艦娘における制空値は0とする
					if (!DataBase.ContainsKammusu(kammusu.Id))
						continue;
					var kammusuData = DataBase.Kammusu(kammusu.Id);
					foreach (var weapon in kammusu.Weapon.Select((v, i) => new { v, i })) {
						airValue += weapon.v.AirValue(kammusuData.Airs[weapon.i]);
					}
				}
			}
			return airValue;
		}
		// 撃墜計算情報
		public string AntiAirText() {
			string output = "";
			foreach (var unit in Unit) {
				foreach (var kammusu in unit.Kammusu) {
					// データベースに存在しない艦娘は飛ばす
					if (!DataBase.ContainsKammusu(kammusu.Id))
						continue;
					output += $"{kammusu.Name}→";
					output += "\n";
				}
			}
			return output;
		}
		// コンストラクタ
		public Fleet() {
			Unit = new List<Unit>();
		}
	}
}
