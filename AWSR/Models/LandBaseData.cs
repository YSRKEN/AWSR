using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSR.Models
{
	internal static class LandBaseData
	{
		/// <summary>
		/// 入力された文字列を大艦隊クラスに変換する
		/// </summary>
		/// <param name="inputAirBaseText">文字列</param>
		/// <returns>大艦隊クラス</returns>
		/// <exception cref="Exception"></exception>
		public static LandBase ToLandBase(string inputAirBaseText) {
			var landbase = new LandBase();
			// bas形式と考えて読み込む
			using (var rs = new System.IO.StringReader(inputAirBaseText)) {
				while (rs.Peek() > -1) {
					try {
						string getLine = rs.ReadLine();
						var column = getLine.Split(',');
						if (column.Length < 13)
							continue;
						var landBaseTeam = new LandBaseTeam();
						int attackCount = int.Parse(column[0]);
						attackCount = attackCount >= 2 ? 2 : 1;
						// 装備(データベースから情報を拾う)
						for (int i = 0; i < 4; ++i) {
							int id2 = DataBase.WeaponId(column[1 + (i * 3)]);
							if (id2 >= 0) {
								var tempWeapon = new Weapon{
									Id = id2,
									Improvement = int.Parse(column[2 + (i * 3)]),
									Proficiency = int.Parse(column[3 + (i * 3)]),
								};
								tempWeapon.Complete();
								landBaseTeam.Weapon[i] = tempWeapon;
								if (tempWeapon.Type.Contains("偵察機"))
									landBaseTeam.Airs[i] = 4;
							}
						}
						landbase.Team.Add(landBaseTeam);
						landbase.AttackCount.Add(attackCount);
					}
					catch {
						continue;
					}
				}
			}
			if (landbase.TeamCount <= 0)
				throw new Exception();
			return landbase;
		}
	}
}
