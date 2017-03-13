﻿using static AWSR.Models.AirWarSimulator;

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
		private int improvement;
		public int Improvement{
			get {
				return improvement;
			}
			set {
				improvement = (value < 0 ? 0 : value > MaxImprovement ? MaxImprovement : value);
			}
		}
		// 艦載機熟練度
		private int proficiency;
		public int Proficiency {
			get {
				return proficiency;
			}
			set {
				proficiency = (value < 0 ? 0 : value > MaxProficiency ? MaxProficiency : value);
			}
		}
	}
}