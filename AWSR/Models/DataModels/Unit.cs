using System.Collections.Generic;

namespace AWSR.Models
{
	/// <summary>
	/// 艦隊クラス
	/// </summary>
	class Unit
	{
		// 艦隊に属する艦娘
		public List<Kammusu> Kammusu { get; set; }
		// コンストラクタ
		public Unit() {
			Kammusu = new List<Kammusu>();
		}
	}
}
