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
				foreach (var unit in Unit.Select((v, i) => new { v, i })) {
					foreach (var kammusu in unit.v.Kammusu.Select((v, i) => new { v, i })) {
						output += $"({(unit.i != 0 ? $"{unit.i + 1}-" : "")}{kammusu.i + 1})";
						output += $"{kammusu.v.Id}";
						foreach (var weapon in kammusu.v.Weapon.Select((v, i) => new { v, i })) {
							output += $"{(weapon.i == 0 ? "　" : ",")}";
							output += $"{weapon.v.Id}";
							output += ToMasStr(weapon.v.Proficiency);
							output += $"{(weapon.v.Improvement > 0 ? $"★{weapon.v.Improvement}" : "")}";
						}
						output += "\n";
					}
				}
				return output;
			}
		}
		// コンストラクタ
		public Fleet() {
			Unit = new List<Unit>();
		}
	}
}
