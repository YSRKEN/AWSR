using System.Collections.Generic;
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
		public List<Unit> Unit = new List<Unit>();
		// 陣形
		public Formation Formation { get; set; }
		// コンストラクタ
		public Fleet(int n) {
			Unit = new List<Unit>(n);
		}
	}
}
