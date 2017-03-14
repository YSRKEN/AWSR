using System.Collections.Generic;
using static AWSR.Models.Constant;

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
		private int level;
		public int Level {
			get {
				return level;
			}
			set {
				level = (value < 1 ? 1 : value > MaxLevel ? MaxLevel : value);
			}
		}
		// 運
		private int luck;
		public int Luck {
			get {
				return luck;
			}
			set {
				luck = (value < -1 ? -1 : value > MaxLuck ? MaxLuck : value);
			}
		}
		// 艦名
		public string Name {
			get {
				return DataBase.Kammusu(Id).Name;
			}
		}
		// 所持装備
		public List<Weapon> Weapon { get; set; }
		// 艦娘か否か
		public bool IsKammusu { get; set; }
		// 加重対空値
		public double WeightAntiAir {
			get {
				// スタブ
				double weightAntiAir = 0.0;
				return weightAntiAir;
			}
		}
		// コンストラクタ
		public Kammusu() {
			Weapon = new List<Weapon>();
		}
	}
}
