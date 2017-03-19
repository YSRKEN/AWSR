using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public string InfoText() {
			string output = "";

			return output;
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
