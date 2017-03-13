using System.Collections.Generic;
using System.Linq;
using static AWSR.Models.AirWarSimulator;

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
		public string InfoText {
			get {
				string output = "";
				output += $"陣形 : {Formation.ToStr()}\n";
				// Selectメソッドでインデックスを付加する手法は次のURLを参考にした
				// http://blog.okazuki.jp/entry/20100728/1280300415
				foreach (var unit in Unit.Select((v, i) => new { v, i })) {
					foreach (var kammusu in unit.v.Kammusu.Select((v, i) => new { v, i })) {
						output += (unit.i != 0 ? $"({unit.i + 1}-{kammusu.i + 1})" : $"({kammusu.i + 1})");
						output += $"{kammusu.v.Id}";
						output += (kammusu.v.Level != 0 ? $" Lv.{kammusu.v.Level}" : "");
						output += (kammusu.v.Luck != -1 ? $" 運{kammusu.v.Luck}" : "");
						foreach (var weapon in kammusu.v.Weapon.Select((v, i) => new { v, i })) {
							output += (weapon.i == 0 ? "　" : ",");
							output += $"{weapon.v.Id}";
							output += ToMasStr(weapon.v.Proficiency);
							output += (weapon.v.Improvement > 0 ? $"★{weapon.v.Improvement}" : "");
						}
						output += "\n";
					}
				}
				return output;
			}
		}
		// デッキビルダーのJSONに変換
		public string ToDeckBuilderText() {
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
						output += (weapon.i != 0 ? "," : "");
						output += $"\"i{weapon.i + 1}\":{{";
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
		// コンストラクタ
		public Fleet() {
			Unit = new List<Unit>();
		}
	}
}
