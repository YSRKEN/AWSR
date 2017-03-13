using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSR.Models
{
	static class AirWarSimulator
	{
		#region 定数・ヘルパーメソッド
		public enum Formation { Trail, SubTrail, Circle, Echelon, Abreast };
		// 陣形を文字列に変換する
		public static string ToStr(this Formation formation) {
			string[] name = { "単縦陣", "複縦陣", "輪形陣", "梯形陣", "単横陣" };
			return name[(int)formation];
		}
		// 数値を熟練度文字列に変換する
		public static string ToMasStr(int mas) {
			string[] name = { "", "|", "||", "|||", "/", "//", "///", ">>" };
			return name[(mas < 0 || mas > 7 ? 0 : mas)];
		}
		#endregion
	}
}
