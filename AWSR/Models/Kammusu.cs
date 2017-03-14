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
				return (DataBase.ContainsKammusu(Id) ? DataBase.Kammusu(Id).Name : "？");
			}
		}
		// 所持装備
		public List<Weapon> Weapon { get; set; }
		// 艦娘か否か
		public bool IsKammusu { get; set; }
		// コンストラクタ
		public Kammusu() {
			Weapon = new List<Weapon>();
		}
	}
}
