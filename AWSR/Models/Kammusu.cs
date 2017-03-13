using System.Collections.Generic;

namespace AWSR.Models
{
	/// <summary>
	/// 艦娘クラス
	/// </summary>
	class Kammusu
	{
		// 艦船ID
		public int Id { get; set; }
		// レベル
		public int Level { get; set; }
		// 運
		public int Luck { get; set; }
		// 所持装備
		public List<Weapon> Weapon { get; set; }
	}
}
