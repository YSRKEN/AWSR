using System;
using System.Collections.Generic;
using System.Linq;
using static AWSR.Models.Constant;

namespace AWSR.Models
{
	// KammusuList[ユニット番号][艦娘番号]
	using KammusuList = List<List<int>>;
	// AirsList[ユニット番号][艦娘番号][装備番号]
	using AirsList = List<List<List<int>>>;
	// LeaveAirsList[ユニット番号][艦娘番号][装備番号][0～最大搭載数]
	using LeaveAirsList = List<List<List<List<int>>>>;
	static class Simulator
	{
		private static Random rand = null;
		private static KammusuList friendKammusuList = null;
		private static KammusuList enemyKammusuList = null;
		private static LeaveAirsList friendLeaveAirsList = null;
		private static LeaveAirsList enemyLeaveAirsList = null;
		// モンテカルロシミュレーションを行う
		public static string MonteCarlo(Fleet friend, Fleet enemy, int simulationSize) {
			string output =  "【モンテカルロシミュレーション】\n";
			output += $"反復回数：{simulationSize}回\n";
			// 初期状態を記録する
			var firstFriendAirsList = friend.AirsList;
			var firstEnemyAirsList = enemy.AirsList;
			// 保存用バッファを用意する
			MakeLists(firstFriendAirsList, out friendKammusuList, out friendLeaveAirsList);
			MakeLists(firstEnemyAirsList, out enemyKammusuList, out enemyLeaveAirsList);
			// 反復計算を行う
			var friendAirsList = DeepCopyHelper.DeepCopy(firstFriendAirsList);
			var enemyAirsList = DeepCopyHelper.DeepCopy(firstEnemyAirsList);
			var AirWarStatusCount = new List<int> { 0,0,0,0,0 };
			int unitCount = UnitCount(enemy.Unit.Count, friend.Unit.Count);
			for(int i = 0; i < simulationSize; ++i) {
				// 状態を初期化する
				CopyAirsList(firstFriendAirsList, friendAirsList);
				CopyAirsList(firstEnemyAirsList, enemyAirsList);
				#region ステージ1：航空戦
				// 制空値を計算する
				int friendAirValue = NowAirValue(friend, unitCount, friendAirsList);
				int enemyAirValue = NowAirValue(enemy, unitCount, enemyAirsList);
				// 制空状態を判断する
				AirWarStatus airWarStatus = CalcAirWarStatus(friendAirValue, enemyAirValue);
				++AirWarStatusCount[(int)airWarStatus];
				// 割合撃墜を行う
				St1FriendBreak(friend, unitCount, friendAirsList, airWarStatus);
				St1EnemyBreak(enemy, unitCount, enemyAirsList, airWarStatus);
				#endregion
				#region ステージ2：対空砲火
				St2FriendBreak(friend, enemy, unitCount, friendAirsList);
				St2EnemyBreak(enemy, friend, unitCount,  enemyAirsList);
				#endregion
				// 残数を記録する
				MemoLeaveList(friend, enemy, friendAirsList, friendKammusuList, friendLeaveAirsList);
				MemoLeaveList(enemy, friend, enemyAirsList, enemyKammusuList, enemyLeaveAirsList);
			}
			// 結果を書き出す
			output += "【制空状態】\n";
			output += "本隊";
			for(int i = 0; i < (int)AirWarStatus.Size; ++i) {
				var i2 = (AirWarStatus)i;
				output += $"　{i2.ToStr()}：{Math.Round(100.0 * AirWarStatusCount[i] / simulationSize, 1)}％";
			}
			output += "\n";
			output += "【全滅率】\n";
			output += "自艦隊：\n";
			for (int i = 0; i < friendKammusuList.Count; ++i) {
				for (int j = 0; j < friendKammusuList[i].Count; ++j) {
					int sum = firstFriendAirsList[i][j].Sum();
					if(sum > 0) {
						output += $"{friend.Unit[i].Kammusu[j].Name}→{Math.Round(100.0 * friendKammusuList[i][j] / simulationSize, 1)}％\n";
					}
				}
			}
			output += "敵艦隊：\n";
			for (int i = 0; i < enemyKammusuList.Count; ++i) {
				for (int j = 0; j < enemyKammusuList[i].Count; ++j) {
					int sum = firstEnemyAirsList[i][j].Sum();
					if (sum > 0) {
						output += $"{enemy.Unit[i].Kammusu[j].Name}→{Math.Round(100.0 * enemyKammusuList[i][j] / simulationSize, 1)}％\n";
					}
				}
			}
			return output;
		}
		// 結果を出力する
		public static void ResultData(Fleet friendFleet, Fleet enemyFleet, int simulationSize, out List<string> nameList, out List<List<List<double>>> perList) {
			nameList = new List<string>();
			perList = new List<List<List<double>>>();
			for (int i = 0; i < friendLeaveAirsList.Count; ++i) {
				for (int j = 0; j < friendLeaveAirsList[i].Count; ++j) {
					nameList.Add($"{i + 1}-{j + 1} {friendFleet.Unit[i].Kammusu[j].Name}");
					var tempList1 = new List<List<double>>();
					for (int k = 0; k < friendLeaveAirsList[i][j].Count; ++k) {
						if (friendLeaveAirsList[i][j][k].Count == 1)
							continue;
						var tempList2 = new List<double>();
						for (int m = 0; m < friendLeaveAirsList[i][j][k].Count; ++m) {
							tempList2.Add(100.0 * friendLeaveAirsList[i][j][k][m] / simulationSize);
						}
						tempList1.Add(tempList2);
					}
					perList.Add(tempList1);
				}
			}
			for (int i = 0; i < enemyLeaveAirsList.Count; ++i) {
				for (int j = 0; j < enemyLeaveAirsList[i].Count; ++j) {
					nameList.Add($"{i + 1}-{j + 1} {enemyFleet.Unit[i].Kammusu[j].Name}");
					var tempList1 = new List<List<double>>();
					for (int k = 0; k < enemyLeaveAirsList[i][j].Count; ++k) {
						if (enemyLeaveAirsList[i][j][k].Count == 1)
							continue;
						var tempList2 = new List<double>();
						for (int m = 0; m < enemyLeaveAirsList[i][j][k].Count; ++m) {
							tempList2.Add(100.0 * enemyLeaveAirsList[i][j][k][m] / simulationSize);
						}
						tempList1.Add(tempList2);
					}
					perList.Add(tempList1);
				}
			}
		}
		// 乱数を初期化する
		public static void Initialize() {
			rand = new Random();
		}
		// [a,b]の整数一様乱数を作成する
		private static int RandInt(int a, int b) {
			return rand.Next(a, b + 1);
		}
		// AirsListをコピーする
		private static void CopyAirsList(AirsList src, AirsList dst) {
			for(int i = 0; i < src.Count; ++i) {
				for (int j = 0; j < src[i].Count; ++j) {
					for (int k = 0; k < src[i][j].Count; ++k) {
						dst[i][j][k] = src[i][j][k];
					}
				}
			}
		}
		// KammusuListとLeaveAirsListをAirsListから作成する
		private static void MakeLists(AirsList airsList, out KammusuList kammusuList, out LeaveAirsList leaveAirsList) {
			kammusuList = new KammusuList();
			leaveAirsList = new LeaveAirsList();
			for (int i = 0; i < airsList.Count; ++i) {
				var tempList = new List<List<List<int>>>();
				kammusuList.Add(Enumerable.Repeat(0, airsList[i].Count).ToList());
				for (int j = 0; j < airsList[i].Count; ++j) {
					var tempList2 = new List<List<int>>();
					for (int k = 0; k < airsList[i][j].Count; ++k) {
						var tempList3 = Enumerable.Repeat(0, airsList[i][j][k] + 1).ToList();
						tempList2.Add(tempList3);
					}
					tempList.Add(tempList2);
				}
				leaveAirsList.Add(tempList);
			}
		}
		// AirsListをLeaveAirsListに記録する
		private static void MemoLeaveList(Fleet friendFleet, Fleet enemyFleet, AirsList airsList, KammusuList kammusuList, LeaveAirsList leaveAirsList) {
			for (int i = 0; i < airsList.Count; ++i) {
				for (int j = 0; j < airsList[i].Count; ++j) {
					bool allBrokenFlg = true;
					bool hasPAPBWB = false;
					for (int k = 0; k < friendFleet.Unit[i].Kammusu[j].SlotCount; ++k) {
						string type = friendFleet.Unit[i].Kammusu[j].Weapon[k].Type;
						if (type == "艦上攻撃機"
						|| type == "艦上爆撃機"
						|| type == "水上爆撃機"
						|| type == "噴式戦闘爆撃機") {
							hasPAPBWB = true;
							if (airsList[i][j][k] != 0) {
								allBrokenFlg = false;
							}
						}
						++leaveAirsList[i][j][k][airsList[i][j][k]];
					}
					if (allBrokenFlg && hasPAPBWB) {
						++kammusuList[i][j];
					}
				}
			}
		}
		// 現在の制空値を計算する
		private static int NowAirValue(Fleet fleet, int unitCount, AirsList airslist) {
			int airValue = 0;
			for (int ui = 0; ui < unitCount; ++ui) {
				var kammusuList = fleet.Unit[ui].Kammusu;
				for (int ki = 0; ki < kammusuList.Count; ++ki) {
					var weaponList = kammusuList[ki].Weapon;
					for (int wi = 0; wi < weaponList.Count; ++wi) {
						airValue += weaponList[wi].AirValue(airslist[ui][ki][wi]);
					}
				}
			}
			return airValue;
		}
		// 制空状態を判断する
		private static AirWarStatus CalcAirWarStatus(int friendAirValue, int enemyAirValue) {
			if (friendAirValue >= enemyAirValue * 3) {
				return AirWarStatus.Best;
			}
			else if (friendAirValue * 2 >= enemyAirValue * 3) {
				return AirWarStatus.Good;
			}
			else if (friendAirValue * 3 >= enemyAirValue * 2) {
				return AirWarStatus.Balance;
			}
			else if (friendAirValue * 3 >= enemyAirValue) {
				return AirWarStatus.Bad;
			}
			else {
				return AirWarStatus.Worst;
			}
		}
		// ステージ1撃墜処理(自軍)
		private static void St1FriendBreak(Fleet friend, int unitCount, AirsList friendAirsList, AirWarStatus airWarStatus) {
			for (int i = 0; i < unitCount; ++i) {
				for (int j = 0; j < friend.Unit[i].Kammusu.Count; ++j) {
					for (int k = 0; k < friend.Unit[i].Kammusu[j].Weapon.Count; ++k) {
						string type = friend.Unit[i].Kammusu[j].Weapon[k].Type;
						if(type != "艦上戦闘機"
						&& type != "艦上攻撃機"
						&& type != "艦上爆撃機"
						&& type != "水上爆撃機"
						&& type != "水上戦闘機"
						&& type != "噴式戦闘爆撃機") {
							continue;
						}
						int breakCount = friendAirsList[i][j][k] * RandInt(St1FriendBreakMin[(int)airWarStatus], St1FriendBreakMax[(int)airWarStatus]) / 256;
						friendAirsList[i][j][k] -= breakCount;
					}
				}
			}
		}
		// ステージ1撃墜処理(敵軍)
		private static void St1EnemyBreak(Fleet enemy, int unitCount, AirsList enemyAirsList, AirWarStatus airWarStatus) {
			for (int i = 0; i < unitCount; ++i) {
				for (int j = 0; j < enemy.Unit[i].Kammusu.Count; ++j) {
					for (int k = 0; k < enemy.Unit[i].Kammusu[j].Weapon.Count; ++k) {
						string type = enemy.Unit[i].Kammusu[j].Weapon[k].Type;
						if (type != "艦上戦闘機"
						&& type != "艦上攻撃機"
						&& type != "艦上爆撃機"
						&& type != "水上爆撃機"
						&& type != "水上戦闘機"
						&& type != "噴式戦闘爆撃機") {
							continue;
						}
						// St1自軍と違い、未検証なので適当に整数一様乱数で処理しています
						int breakCount = enemyAirsList[i][j][k] * RandInt(St1EnemyBreakMin[(int)airWarStatus], St1EnemyBreakMax[(int)airWarStatus]) / 100;
						enemyAirsList[i][j][k] -= breakCount;
					}
				}
			}
		}
		// ステージ2撃墜処理(自軍)
		private static void St2FriendBreak(Fleet friend, Fleet enemy, int unitCount, AirsList friendAirsList) {
			// 迎撃艦を一覧を算出し、それぞれの撃墜量を出す
			var breakPer = enemy.BreakPer;
			var breakFixed = enemy.BreakFixed;
			// 撃墜処理
			for (int i = 0; i < unitCount; ++i) {
				for (int j = 0; j < friend.Unit[i].Kammusu.Count; ++j) {
					for (int k = 0; k < friend.Unit[i].Kammusu[j].Weapon.Count; ++k) {
						string type = friend.Unit[i].Kammusu[j].Weapon[k].Type;
						if (type != "艦上攻撃機"
						&& type != "艦上爆撃機"
						&& type != "水上爆撃機"
						&& type != "噴式戦闘爆撃機") {
							continue;
						}
						// 迎撃艦を選択する
						int selectKammusuIndex = RandInt(0, breakPer.Count - 1);
						// 割合撃墜
						if(RandInt(0,1) == 1) {
							int breakCount = (int)(breakPer[selectKammusuIndex] * friendAirsList[i][j][k]);
							friendAirsList[i][j][k] -= breakCount;
						}
						// 固定撃墜
						if (RandInt(0, 1) == 1) {
							int breakCount = breakFixed[selectKammusuIndex];
							friendAirsList[i][j][k] = Math.Max(friendAirsList[i][j][k] - breakCount, 0);
						}
					}
				}
			}
		}
		// ステージ2撃墜処理(敵軍)
		private static void St2EnemyBreak(Fleet enemy, Fleet friend, int unitCount, AirsList enemyAirsList) {
			// 迎撃艦を一覧を算出し、それぞれの撃墜量を出す
			var breakPer = friend.BreakPer;
			var breakFixed = friend.BreakFixed;
			// 撃墜処理
			for (int i = 0; i < unitCount; ++i) {
				for (int j = 0; j < enemy.Unit[i].Kammusu.Count; ++j) {
					for (int k = 0; k < enemy.Unit[i].Kammusu[j].Weapon.Count; ++k) {
						string type = enemy.Unit[i].Kammusu[j].Weapon[k].Type;
						if (type != "艦上攻撃機"
						&& type != "艦上爆撃機"
						&& type != "水上爆撃機"
						&& type != "噴式戦闘爆撃機") {
							continue;
						}
						// 迎撃艦を選択する
						int selectKammusuIndex = RandInt(0, breakPer.Count - 1);
						// 割合撃墜
						if (RandInt(0, 1) == 1) {
							int breakCount = (int)(breakPer[selectKammusuIndex] * enemyAirsList[i][j][k]);
							enemyAirsList[i][j][k] -= breakCount;
						}
						// 固定撃墜
						if (RandInt(0, 1) == 1) {
							int breakCount = breakFixed[selectKammusuIndex] + 1;
							enemyAirsList[i][j][k] = Math.Max(enemyAirsList[i][j][k] - breakCount, 0);
						}
					}
				}
			}
		}
	}
}
