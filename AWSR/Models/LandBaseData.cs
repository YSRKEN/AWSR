using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSR.Models
{
	class LandBaseData
	{
		/// <summary>
		/// 入力された文字列を大艦隊クラスに変換する
		/// </summary>
		/// <param name="inputEnemyDataText">文字列</param>
		/// <returns>大艦隊クラス</returns>
		public static LandBase ToLandBase(string inputEnemyDataText) {
			var landbase = new LandBase();
			// bas形式と考えて読み込む
			using (var rs = new System.IO.StringReader(inputEnemyDataText)) {
				while (rs.Peek() > -1) {
					try {
						string getLine = rs.ReadLine();
						var column = getLine.Split(',');
						if (column.Count() < 13)
							continue;
						
					}
					catch {
						continue;
					}
				}
			}
			return landbase;
		}
	}
}
