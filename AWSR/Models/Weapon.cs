namespace AWSR.Models
{
	/// <summary>
	/// 装備クラス
	/// </summary>
	class Weapon
	{
		// 装備ID
		public int Id { get; set; }
		// 装備改修度
		public int Improvement{ get; set; }
		// 艦載機熟練度
		public int Proficiency { get; set; }
	}
}
