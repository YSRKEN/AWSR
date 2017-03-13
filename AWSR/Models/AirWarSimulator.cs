using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSR.Models
{
	static class AirWarSimulator
	{
		public enum Formation { Trail, SubTrail, Circle, Echelon, Abreast };
		public static string ToStr(this Formation formation) {
			string[] name = { "単縦陣", "複縦陣", "輪形陣", "梯形陣", "単横陣" };
			return name[(int)formation];
		}
	}
}
