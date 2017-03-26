using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSR.Models
{
	class FriendData
	{
		/// <summary>
		/// 入力された文字列を大艦隊クラスに変換する
		/// </summary>
		/// <param name="inputFriendDataText">文字列</param>
		/// <returns>大艦隊クラス</returns>
		public static Fleet ToFleet(string inputFriendDataText) {
			var fleet = new Fleet();
			// fre形式と考えて読み込む
			var kammusuList = new List<KeyValuePair<int, int>>();
			var kammusuDataList = new List<Kammusu>();
			using (var rs = new System.IO.StringReader(inputFriendDataText)) {
				while (rs.Peek() > -1) {
					try {
						string getLine = rs.ReadLine();
						var column = getLine.Split(',');
						if (column.Count() < 19)
							continue;
						// 艦隊番号と艦番を読み込む
						int ui = int.Parse(column[0]);
						ui = (ui <= 1 ? 1 : 2);
						int ki = int.Parse(column[1]);
						ki = (ki <= 1 ? 1 : ki >= 6 ? 6 : ki);
						// 艦娘部分のデータを読み込む
						var tempKammusu = new Kammusu();
						int id = DataBase.KammusuId(column[2]);
						if (id >= 0) {
							tempKammusu.Id = id;
							tempKammusu.Level = int.Parse(column[3]);
							tempKammusu.Luck = -1;
							tempKammusu.IsKammusu = true;
							// 装備(データベースから情報を拾う)
							for(int i = 0; i < 5; ++i) {
								var tempWeapon = new Weapon();
								int id2 = DataBase.WeaponId(column[4 + i * 3]);
								if (id2 >= 0) {
									tempWeapon.Id = id2;
									tempWeapon.Improvement = int.Parse(column[5 + i * 3]);
									tempWeapon.Proficiency = int.Parse(column[6 + i * 3]);
								}
								tempWeapon.Complete();
								tempKammusu.Weapon.Add(tempWeapon);
							}
							tempKammusu.Complete();
							kammusuList.Add(new KeyValuePair<int, int>(ui, ki));
							kammusuDataList.Add(tempKammusu);
						}
					}
					catch {
						continue;
					}
				}
			}
			// 読み込んだものをfleetにセットしていく
			int unitCount = kammusuList.Select(k => k.Key).Max();
			for(int ui = 0; ui < unitCount; ++ui) {
				fleet.Unit.Add(new Unit());
				int maxKammusuIndex = kammusuList.Where(k => k.Key == ui + 1).Select(k => k.Value).Max();
				for(int ki = 0; ki < maxKammusuIndex; ++ki) {
					fleet.Unit[ui].Kammusu.Add(new Kammusu());
				}
			}
			for(int i = 0; i < kammusuList.Count; ++i) {
				fleet.Unit[kammusuList[i].Key - 1].Kammusu[kammusuList[i].Value - 1] = kammusuDataList[i];
			}
			fleet.Complete();
			return fleet;
		}
	}
}
