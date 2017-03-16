using System;
using System.Collections.Generic;
using System.Linq;
using static AWSR.Models.Constant;

namespace AWSR.Models
{
	// AirsList[ユニット番号][艦娘番号][装備番号]
	using AirsList = List<List<List<int>>>;
	// LeaveAirsList[ユニット番号][艦娘番号][装備番号][0～最大搭載数]
	using LeaveAirsList = List<List<List<List<int>>>>;
	static class Simulator
	{
		static Random rand;
		// モンテカルロシミュレーションを行う
		public static string MonteCarlo(Fleet friend, Fleet enemy, int simulationSize) {
			string output =  "【モンテカルロシミュレーション】\n";
			output += $"反復回数：{simulationSize}回\n";
			// 初期状態を記録する
			var firstFriendAirsList = friend.AirsList;
			var firstEnemyAirsList = enemy.AirsList;
			// 保存用バッファを用意する
			var friendLeaveAirsList = MakeLeaveList(firstFriendAirsList);
			var enemyLeaveAirsList = MakeLeaveList(firstEnemyAirsList);
			// 反復計算を行う
			var friendAirsList = DeepCopyHelper.DeepCopy(firstFriendAirsList);
			var enemyAirsList = DeepCopyHelper.DeepCopy(firstEnemyAirsList);
			var AirWarStatusCount = new List<int> { 0,0,0,0,0 };
			for(int i = 0; i < simulationSize; ++i) {
				// 状態を初期化する
				CopyAirsList(firstFriendAirsList, friendAirsList);
				CopyAirsList(firstEnemyAirsList, enemyAirsList);
				#region ステージ1：航空戦
				// 制空値を計算する
				int friendAirValue = NowAirValue(friend, friendAirsList);
				int enemyAirValue = NowAirValue(enemy, enemyAirsList);
				// 制空状態を判断する
				AirWarStatus airWarStatus = CalcAirWarStatus(friendAirValue, enemyAirValue);
				++AirWarStatusCount[(int)airWarStatus];
				// 割合撃墜を行う
				St1FriendBreak(friend, friendAirsList, airWarStatus);
				St1EnemyBreak(enemy, enemyAirsList, airWarStatus);
				#endregion
				#region ステージ2：対空砲火
				#endregion
				// 残数を記録する
				MemoLeaveList(friendAirsList, friendLeaveAirsList);
				MemoLeaveList(enemyAirsList, enemyLeaveAirsList);
			}
			//
			Console.WriteLine("自艦隊");
			for (int i = 0; i < firstFriendAirsList.Count; ++i) {
				for (int j = 0; j < firstFriendAirsList[i].Count; ++j) {
					for (int k = 0; k < firstFriendAirsList[i][j].Count; ++k) {
						if(firstFriendAirsList[i][j][k] > 0) {
							Console.Write($"{i + 1}-{j + 1}-{k + 1} ");
							for (int m = 0; m <= firstFriendAirsList[i][j][k]; ++m) {
								Console.Write($"{friendLeaveAirsList[i][j][k][m]},");
							}
							Console.WriteLine("");
						}
					}
				}
			}
			Console.WriteLine("敵艦隊");
			for (int i = 0; i < firstEnemyAirsList.Count; ++i) {
				for (int j = 0; j < firstEnemyAirsList[i].Count; ++j) {
					for (int k = 0; k < firstEnemyAirsList[i][j].Count; ++k) {
						if(firstEnemyAirsList[i][j][k] > 0) {
							Console.Write($"{i + 1}-{j + 1}-{k + 1} ");
							for (int m = 0; m <= firstEnemyAirsList[i][j][k]; ++m) {
								Console.Write($"{enemyLeaveAirsList[i][j][k][m]},");
							}
							Console.WriteLine("");
						}
					}
				}
			}
			// 結果を書き出す
			output += "制空状態：\n";
			output += "本隊";
			for(int i = 0; i < (int)AirWarStatus.Size; ++i) {
				var i2 = (AirWarStatus)i;
				output += $"　{i2.ToStr()}：{Math.Round(100.0 * AirWarStatusCount[i] / simulationSize, 1)}％";
			}
			output += "\n";
			return output;
		}
		// 乱数を初期化する
		public static void Initialize() {
			rand = new Random();
		}
		// [a,b]の整数一様乱数を作成する
		private static int RandInt(int a, int b) {
			return rand.Next(a, b);
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
		// LeaveAirsListをAirsListから作成する
		private static LeaveAirsList MakeLeaveList(AirsList airsList) {
			var leaveAirsList = new LeaveAirsList();
			for (int i = 0; i < airsList.Count; ++i) {
				var tempList = new List<List<List<int>>>();
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
			return leaveAirsList;
		}
		// AirsListをLeaveAirsListに記録する
		private static void MemoLeaveList(AirsList airsList, LeaveAirsList leaveAirsList) {
			for (int i = 0; i < airsList.Count; ++i) {
				for (int j = 0; j < airsList[i].Count; ++j) {
					for (int k = 0; k < airsList[i][j].Count; ++k) {
						int a = airsList[i][j][k];
						++leaveAirsList[i][j][k][airsList[i][j][k]];
					}
				}
			}
		}
		// 現在の制空値を計算する
		private static int NowAirValue(Fleet fleet, AirsList airslist) {
			int airValue = 0;
			for (int ui = 0; ui < fleet.Unit.Count; ++ui) {
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
		private static void St1FriendBreak(Fleet friend, AirsList friendAirsList, AirWarStatus airWarStatus) {
			for (int i = 0; i < friend.Unit.Count; ++i) {
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
						if(breakCount < 0) {
							int a = 0;
						}
						friendAirsList[i][j][k] -= breakCount;
					}
				}
			}
		}
		// ステージ1撃墜処理(敵軍)
		private static void St1EnemyBreak(Fleet enemy, AirsList enemyAirsList, AirWarStatus airWarStatus) {
			for (int i = 0; i < enemy.Unit.Count; ++i) {
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
	}
}
