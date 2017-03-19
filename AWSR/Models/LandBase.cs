﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AWSR.Models.Constant;

namespace AWSR.Models
{
	/// <summary>
	/// 基地航空隊クラス
	/// </summary>
	class LandBase
	{
		// 所属する中隊
		public List<LandBaseTeam> Team { get; set; }
		// 攻撃回数
		public List<int> AttackCount { get; set; }
		// 情報テキスト
		public string InfoText() {
			string output = "";
			int teamCount = Team.Count;
			for(int ti = 0; ti < teamCount; ++ti) {
				output += $"({ti + 1})";
				output += (AttackCount[ti] == 2 ? "集中" : "分散");
				foreach (var weapon in Team[ti].Weapon.Select((v, i) => new { v, i })) {
					output += (weapon.i == 0 ? "　" : ",");
					output += weapon.v.Name;
					output += ToMasStr(weapon.v.Proficiency);
					output += (weapon.v.Improvement > 0 ? $"★{weapon.v.Improvement}" : "");
				}
				output += "\n";
			}
			return output;
		}
		// コンストラクタ
		public LandBase() {
			Team = new List<LandBaseTeam>();
			AttackCount = new List<int>();
		}
	}
	class LandBaseTeam
	{
		// 所持装備
		public List<Weapon> Weapon { get; set; }
		// 発艦数
		public List<int> Airs { get; set; }
		// 計算可能な値を事前に計算しておく
		public void Complete() {

		}
		public LandBaseTeam() {
			Weapon = Enumerable.Repeat(new Weapon(), 4).ToList();
			Airs = Enumerable.Repeat(18, 4).ToList();
		}
	}
}
