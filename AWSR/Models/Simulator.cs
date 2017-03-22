using Rei.Random;
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
	// LandBaseList[中隊番号]
	using LandBaseList = List<int>;
	// LandBaseAirsList[中隊番号][装備番号]
	using LandBaseAirsList = List<List<int>>;
	// LeaveLandBaseAirsList[中隊番号][装備番号][0～最大搭載数]
	using LeaveLandBaseAirsList = List<List<List<int>>>;
	static class Simulator
	{
		private static CompatilizedRandom rand = null;
		private static KammusuList friendKammusuList = null;
		private static KammusuList enemyKammusuList = null;
		private static LeaveAirsList friendLeaveAirsList = null;
		private static LeaveAirsList enemyLeaveAirsList = null;
		private static LandBaseList landBaseList = null;
		private static LeaveLandBaseAirsList landBaseLeaveAirsList = null;
		// モンテカルロシミュレーションを行う
		public static string MonteCarlo(Fleet friend, Fleet enemy, LandBase landBase, int simulationSize) {
			if(landBase == null) {
				return MonteCarloImpl(friend, enemy, simulationSize);
			}
			else {
				return MonteCarloImpl(friend, enemy, landBase, simulationSize);
			}
		}
		// モンテカルロシミュレーション(基地航空隊がない場合)
		public static string MonteCarloImpl(Fleet friend, Fleet enemy, int simulationSize) {
			string output = "【モンテカルロシミュレーション】\n";
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
			var AirWarStatusCount = new List<int> { 0, 0, 0, 0, 0 };
			int unitCount = UnitCount(enemy.Unit.Count, friend.Unit.Count);
			for (int i = 0; i < simulationSize; ++i) {
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
				var friendCutInType = GetCutInType(friend);
				var enemyCutInType = GetCutInType(enemy);
				St2FriendBreak(friend, enemy, unitCount, friendAirsList, enemyCutInType);
				St2EnemyBreak(enemy, friend, unitCount, enemyAirsList, friendCutInType);
				#endregion
				// 残数を記録する
				MemoLeaveList(friend, friendAirsList, friendKammusuList, friendLeaveAirsList);
				MemoLeaveList(enemy, enemyAirsList, enemyKammusuList, enemyLeaveAirsList);
			}
			// 結果を書き出す
			output += "【制空状態】\n";
			output += "本隊";
			for (int i = 0; i < (int)AirWarStatus.Size; ++i) {
				var i2 = (AirWarStatus)i;
				output += $"　{i2.ToStr()}：{Math.Round(100.0 * AirWarStatusCount[i] / simulationSize, 1)}％";
			}
			output += "\n";
			output += "【棒立ち率　[スロット毎の全滅率]】\n";
			output += "自艦隊：\n";
			for (int i = 0; i < friendKammusuList.Count; ++i) {
				for (int j = 0; j < friendKammusuList[i].Count; ++j) {
					int sum = firstFriendAirsList[i][j].Sum();
					if (sum > 0) {
						output += $"{friend.Unit[i].Kammusu[j].Name}→{Math.Round(100.0 * friendKammusuList[i][j] / simulationSize, 1)}％";
						for (int k = 0; k < friend.Unit[i].Kammusu[j].SlotCount; ++k) {
							if (firstFriendAirsList[i][j][k] > 0) {
								output += $"　{k + 1}：[{Math.Round(100.0 * friendLeaveAirsList[i][j][k][0] / simulationSize, 1)}％]";
							}
						}
						output += "\n";
					}
				}
			}
			output += "敵艦隊：\n";
			for (int i = 0; i < enemyKammusuList.Count; ++i) {
				for (int j = 0; j < enemyKammusuList[i].Count; ++j) {
					int sum = firstEnemyAirsList[i][j].Sum();
					if (sum > 0) {
						output += $"{enemy.Unit[i].Kammusu[j].Name}→{Math.Round(100.0 * enemyKammusuList[i][j] / simulationSize, 1)}％";
						for (int k = 0; k < enemy.Unit[i].Kammusu[j].SlotCount; ++k) {
							if (firstEnemyAirsList[i][j][k] > 0) {
								output += $"　{k + 1}：[{Math.Round(100.0 * enemyLeaveAirsList[i][j][k][0] / simulationSize, 1)}％]";
							}
						}
						output += "\n";
					}
				}
			}
			return output;
		}
		// モンテカルロシミュレーション(基地航空隊がある場合)
		public static string MonteCarloImpl(Fleet friend, Fleet enemy, LandBase landBase, int simulationSize) {
			string output =  "【モンテカルロシミュレーション】\n";
			output += $"反復回数：{simulationSize}回\n";
			// 初期状態を記録する
			var firstFriendAirsList = friend.AirsList;
			var firstLandBaseAirsList = landBase.AirsList;
			var firstEnemyAirsList = enemy.AirsList;
			// 保存用バッファを用意する
			MakeLists(firstFriendAirsList, out friendKammusuList, out friendLeaveAirsList);
			MakeLists(firstLandBaseAirsList, out landBaseList, out landBaseLeaveAirsList);
			MakeLists(firstEnemyAirsList, out enemyKammusuList, out enemyLeaveAirsList);
			// 反復計算を行う
			var friendAirsList = DeepCopyHelper.DeepCopy(firstFriendAirsList);
			var landBaseAirsList = DeepCopyHelper.DeepCopy(firstLandBaseAirsList);
			var enemyAirsList = DeepCopyHelper.DeepCopy(firstEnemyAirsList);
			var AirWarStatusCount = new List<List<int>>();
			for(int ti = 0; ti <= landBase.TeamCount; ++ti) {
				AirWarStatusCount.Add(new List<int> { 0, 0, 0, 0, 0 });
			}
			int unitCount = UnitCount(enemy.Unit.Count, friend.Unit.Count);
			for (int i = 0; i < simulationSize; ++i) {
				// 状態を初期化する
				CopyAirsList(firstFriendAirsList, friendAirsList);
				CopyAirsList(firstLandBaseAirsList, landBaseAirsList);
				CopyAirsList(firstEnemyAirsList, enemyAirsList);
				// 基地航空隊
				for (int ti = 0; ti < landBase.TeamCount; ++ti) {
					for(int ai = 0; ai < landBase.AttackCount[ti]; ++ai) {
						#region ステージ1：航空戦
						// 制空値を計算する
						int landBaseAirValue = NowAirValueX(landBase, landBaseAirsList, ti);
						int enemyAirValue_ = NowAirValueX(enemy, enemy.Unit.Count, enemyAirsList);
						// 制空状態を判断する
						AirWarStatus airWarStatus_ = CalcAirWarStatus(landBaseAirValue, enemyAirValue_);
						// 割合撃墜を行う
						if (landBase.TeamCount == 1 || ai == 1) {
							++AirWarStatusCount[ti][(int)airWarStatus_];
							St1LandBaseBreak(landBase, landBaseAirsList, ti, airWarStatus_);
						}
						St1EnemyBreak(enemy, enemy.Unit.Count, enemyAirsList, airWarStatus_);
						#endregion
						if (landBase.TeamCount == 1 || ai == 1) {
							#region ステージ2：対空砲火
							var enemyCutInType_ = GetCutInType(enemy);
							St2LandBaseBreak(landBase, enemy, landBaseAirsList, ti, enemyCutInType_);
							#endregion
						}
					}
				}
				// 艦隊戦
				#region ステージ1：航空戦
				// 制空値を計算する
				int friendAirValue = NowAirValue(friend, unitCount, friendAirsList);
				int enemyAirValue = NowAirValue(enemy, unitCount, enemyAirsList);
				// 制空状態を判断する
				AirWarStatus airWarStatus = CalcAirWarStatus(friendAirValue, enemyAirValue);
				++AirWarStatusCount[landBase.TeamCount][(int)airWarStatus];
				// 割合撃墜を行う
				St1FriendBreak(friend, unitCount, friendAirsList, airWarStatus);
				St1EnemyBreak(enemy, unitCount, enemyAirsList, airWarStatus);
				#endregion
				#region ステージ2：対空砲火
				var friendCutInType = GetCutInType(friend);
				var enemyCutInType = GetCutInType(enemy);
				St2FriendBreak(friend, enemy, unitCount, friendAirsList, enemyCutInType);
				St2EnemyBreak(enemy, friend, unitCount,  enemyAirsList, friendCutInType);
				#endregion
				// 残数を記録する
				MemoLeaveList(friend, friendAirsList, friendKammusuList, friendLeaveAirsList);
				MemoLeaveList(landBase, landBaseAirsList, landBaseList, landBaseLeaveAirsList);
				MemoLeaveList(enemy, enemyAirsList, enemyKammusuList, enemyLeaveAirsList);
			}
			// 結果を書き出す
			output += "【制空状態】\n";
			for (int ti = 0; ti <= landBase.TeamCount; ++ti) {
				output += (ti == landBase.TeamCount ? "本隊" : $"基地-{ti + 1}");
				for (int i = 0; i < (int)AirWarStatus.Size; ++i) {
					var i2 = (AirWarStatus)i;
					output += $"　{i2.ToStr()}：{Math.Round(100.0 * AirWarStatusCount[ti][i] / simulationSize, 1)}％";
				}
				output += "\n";
			}
			output += "【棒立ち率　[スロット毎の全滅率]】\n";
			output += "自艦隊：\n";
			for (int i = 0; i < friendKammusuList.Count; ++i) {
				for (int j = 0; j < friendKammusuList[i].Count; ++j) {
					int sum = firstFriendAirsList[i][j].Sum();
					if(sum > 0) {
						output += $"{friend.Unit[i].Kammusu[j].Name}→{Math.Round(100.0 * friendKammusuList[i][j] / simulationSize, 1)}％";
						for (int k = 0; k < friend.Unit[i].Kammusu[j].SlotCount; ++k) {
							if(firstFriendAirsList[i][j][k] > 0) {
								output += $"　{k+1}：[{Math.Round(100.0 * friendLeaveAirsList[i][j][k][0]/ simulationSize, 1)}％]";
							}
						}
						output += "\n";
					}
				}
			}
			output += "基地航空隊：\n";
			for (int ti = 0; ti < landBase.TeamCount; ++ti) {
				output += $"基地-{ti + 1}→{Math.Round(100.0 * landBaseList[ti] / simulationSize, 1)}％";
				for (int wi = 0; wi < landBase.Team[ti].Weapon.Count; ++wi) {
					if (firstLandBaseAirsList[ti][wi] > 0) {
						output += $"　{wi + 1}：[{Math.Round(100.0 * landBaseLeaveAirsList[ti][wi][0] / simulationSize, 1)}％]";
					}
				}
				output += "\n";
			}
			output += "敵艦隊：\n";
			for (int i = 0; i < enemyKammusuList.Count; ++i) {
				for (int j = 0; j < enemyKammusuList[i].Count; ++j) {
					int sum = firstEnemyAirsList[i][j].Sum();
					if (sum > 0) {
						output += $"{enemy.Unit[i].Kammusu[j].Name}→{Math.Round(100.0 * enemyKammusuList[i][j] / simulationSize, 1)}％";
						for (int k = 0; k < enemy.Unit[i].Kammusu[j].SlotCount; ++k) {
							if (firstEnemyAirsList[i][j][k] > 0) {
								output += $"　{k + 1}：[{Math.Round(100.0 * enemyLeaveAirsList[i][j][k][0] / simulationSize, 1)}％]";
							}
						}
						output += "\n";
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
					for (int k = 0; k < friendFleet.Unit[i].Kammusu[j].SlotCount; ++k) {
						if (friendLeaveAirsList[i][j][k].Count == 1)
							continue;
						if (!friendFleet.Unit[i].Kammusu[j].Weapon[k].IsStage1)
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
					for (int k = 0; k < enemyFleet.Unit[i].Kammusu[j].SlotCount; ++k) {
						if (enemyLeaveAirsList[i][j][k].Count == 1)
							continue;
						if (!enemyFleet.Unit[i].Kammusu[j].Weapon[k].IsStage1)
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
		// 結果を出力する
		public static void ResultData(Fleet friendFleet, Fleet enemyFleet, LandBase landBase, int simulationSize, out List<string> nameList, out List<List<List<double>>> perList) {
			nameList = new List<string>();
			perList = new List<List<List<double>>>();
			for (int i = 0; i < friendLeaveAirsList.Count; ++i) {
				for (int j = 0; j < friendLeaveAirsList[i].Count; ++j) {
					nameList.Add($"{i + 1}-{j + 1} {friendFleet.Unit[i].Kammusu[j].Name}");
					var tempList1 = new List<List<double>>();
					for (int k = 0; k < friendFleet.Unit[i].Kammusu[j].SlotCount; ++k) {
						if (friendLeaveAirsList[i][j][k].Count == 1)
							continue;
						if (!friendFleet.Unit[i].Kammusu[j].Weapon[k].IsStage1)
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
			if(landBase != null) {
				for (int ti = 0; ti < landBase.TeamCount; ++ti) {
					nameList.Add($"基地-{ti + 1}");
					var tempList1 = new List<List<double>>();
					for (int wi = 0; wi < landBase.Team[ti].Weapon.Count; ++wi) {
						if (landBaseLeaveAirsList[ti][wi].Count == 1)
							continue;
						if (!landBase.Team[ti].Weapon[wi].IsStage1X)
							continue;
						var tempList2 = new List<double>();
						for (int m = 0; m < landBaseLeaveAirsList[ti][wi].Count; ++m) {
							tempList2.Add(100.0 * landBaseLeaveAirsList[ti][wi][m] / simulationSize);
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
					for (int k = 0; k < enemyFleet.Unit[i].Kammusu[j].SlotCount; ++k) {
						if (enemyLeaveAirsList[i][j][k].Count == 1)
							continue;
						if (!enemyFleet.Unit[i].Kammusu[j].Weapon[k].IsStage1)
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
			rand = new CompatilizedRandom(new SFMT());
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
		// LandBaseAirsList
		private static void CopyAirsList(LandBaseAirsList src, LandBaseAirsList dst) {
			for (int i = 0; i < src.Count; ++i) {
				for (int j = 0; j < src[i].Count; ++j) {
					dst[i][j] = src[i][j];
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
		// LandBaseListとLeaveLandBaseAirsListをLandBaseAirsListから作成する
		private static void MakeLists(LandBaseAirsList airsList, out LandBaseList landBaseList, out LeaveLandBaseAirsList landBaseLeaveAirsList) {
			landBaseList = new LandBaseList();
			landBaseLeaveAirsList = new LeaveLandBaseAirsList();
			for (int i = 0; i < airsList.Count; ++i) {
				var tempList = new List<List<int>>();
				landBaseList.Add(0);
				for (int j = 0; j < airsList[i].Count; ++j) {
					var tempList2 = Enumerable.Repeat(0, airsList[i][j] + 1).ToList();
					tempList.Add(tempList2);
				}
				landBaseLeaveAirsList.Add(tempList);
			}
		}
		// AirsListをLeaveAirsListに記録する
		private static void MemoLeaveList(Fleet friendFleet, AirsList airsList, KammusuList kammusuList, LeaveAirsList leaveAirsList) {
			for (int i = 0; i < airsList.Count; ++i) {
				for (int j = 0; j < airsList[i].Count; ++j) {
					bool allBrokenFlg = true;
					bool hasPAPBWB = false;
					for (int k = 0; k < friendFleet.Unit[i].Kammusu[j].SlotCount; ++k) {
						if (friendFleet.Unit[i].Kammusu[j].Weapon[k].IsStage2) {
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

		// 記録する
		private static void MemoLeaveList(LandBase landBase, LandBaseAirsList landBaseAirsList, LandBaseList landBaseList, LeaveLandBaseAirsList landBaseLeaveAirsList) {
			for (int ti = 0; ti < landBaseAirsList.Count; ++ti) {
				bool allBrokenFlg = true;
				bool hasPAPBWB = false;
				for (int wi = 0; wi < landBase.Team[ti].Weapon.Count; ++wi) {
					if (landBase.Team[ti].Weapon[wi].IsStage2X) {
						hasPAPBWB = true;
						if (landBaseAirsList[ti][wi] != 0) {
							allBrokenFlg = false;
						}
					}
					++landBaseLeaveAirsList[ti][wi][landBaseAirsList[ti][wi]];
				}
				if (allBrokenFlg && hasPAPBWB) {
					++landBaseList[ti];
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
					for (int wi = 0; wi < kammusuList[ki].SlotCount; ++wi) {
						airValue += weaponList[wi].AirValue(airslist[ui][ki][wi]);
					}
				}
			}
			return airValue;
		}
		private static int NowAirValueX(Fleet fleet, int unitCount, AirsList airslist) {
			int airValue = 0;
			for (int ui = 0; ui < unitCount; ++ui) {
				var kammusuList = fleet.Unit[ui].Kammusu;
				for (int ki = 0; ki < kammusuList.Count; ++ki) {
					var weaponList = kammusuList[ki].Weapon;
					for (int wi = 0; wi < kammusuList[ki].SlotCount; ++wi) {
						airValue += weaponList[wi].AirValueX(airslist[ui][ki][wi]);
					}
				}
			}
			return airValue;
		}
		private static int NowAirValueX(LandBase landBase, LandBaseAirsList landBaseAirsList, int ti) { 
			int airValue = 0;
				var weaponList = landBase.Team[ti].Weapon;
				for (int wi = 0; wi < landBase.Team[ti].Airs.Count; ++wi) {
					airValue += weaponList[wi].AirValueX(landBaseAirsList[ti][wi]);
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
						if (!friend.Unit[i].Kammusu[j].Weapon[k].IsStage1)
							continue;
						int breakCount = friendAirsList[i][j][k] * RandInt(St1FriendBreakMin[(int)airWarStatus], St1FriendBreakMax[(int)airWarStatus]) / 256;
						friendAirsList[i][j][k] -= breakCount;
					}
				}
			}
		}
		// ステージ1撃墜処理(基地航空隊)
		private static void St1LandBaseBreak(LandBase landBase, LandBaseAirsList landBaseAirsList, int ti, AirWarStatus airWarStatus) {
			for (int wi = 0; wi < landBase.Team[ti].Weapon.Count; ++wi) {
				if (!landBase.Team[ti].Weapon[wi].IsStage1X)
					continue;
				int breakCount = landBaseAirsList[ti][wi] * RandInt(St1FriendBreakMin[(int)airWarStatus], St1FriendBreakMax[(int)airWarStatus]) / 256;
				landBaseAirsList[ti][wi] -= breakCount;
			}
		}
		// ステージ1撃墜処理(敵軍)
		private static void St1EnemyBreak(Fleet enemy, int unitCount, AirsList enemyAirsList, AirWarStatus airWarStatus) {
			int St1EnemyBreakConstantNow = St1EnemyBreakConstant[(int)airWarStatus];
			for (int i = 0; i < unitCount; ++i) {
				for (int j = 0; j < enemy.Unit[i].Kammusu.Count; ++j) {
					for (int k = 0; k < enemy.Unit[i].Kammusu[j].Weapon.Count; ++k) {
						if (!enemy.Unit[i].Kammusu[j].Weapon[k].IsStage1)
							continue;
						// 例の式で計算
						// https://docs.google.com/spreadsheets/d/1RTOztxst5pFGCi-Qr8dw6DZwYw0jd8fkymGg44dapAk/edit
						int breakCount = (int)(enemyAirsList[i][j][k] * (0.35 * RandInt(0, St1EnemyBreakConstantNow) + 0.65 * RandInt(0, St1EnemyBreakConstantNow)) / 10);
						enemyAirsList[i][j][k] -= breakCount;
					}
				}
			}
		}
		// 対空カットインの種別を決定する
		private static CutInType GetCutInType(Fleet fleet) {
			// リストを取得する
			var cutInList = new List<CutInType>();
			foreach (var unit in fleet.Unit) {
				foreach (var kammusu in unit.Kammusu) {
					var cutInType = kammusu.CutInType;
					if (cutInType != CutInType.None)
						cutInList.Add(cutInType);
				}
			}
			// リストをソートする
			cutInList.Sort((a, b) => (int)b - (int)a);
			// 上から順に判定する
			foreach(var cutIn in cutInList) {
				if(CutInPer[(int)cutIn] > rand.NextDouble()) {
					return cutIn;
				}
			}
			return CutInType.None;
		}
		// ステージ2撃墜処理(自軍)
		private static void St2FriendBreak(Fleet friend, Fleet enemy, int unitCount, AirsList friendAirsList, CutInType cutInType) {
			// 迎撃艦を一覧を算出し、それぞれの撃墜量を出す
			var breakPer = enemy.BreakPer;
			var breakFixed = enemy.BreakFixed(cutInType);
			// 撃墜処理
			for (int i = 0; i < unitCount; ++i) {
				for (int j = 0; j < friend.Unit[i].Kammusu.Count; ++j) {
					for (int k = 0; k < friend.Unit[i].Kammusu[j].Weapon.Count; ++k) {
						if (!friend.Unit[i].Kammusu[j].Weapon[k].IsStage2)
							continue;
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
							breakCount += CutInAddBonus[(int)cutInType];
							friendAirsList[i][j][k] = Math.Max(friendAirsList[i][j][k] - breakCount, 0);
						}
					}
				}
			}
		}
		// ステージ2撃墜処理(基地航空隊)
		private static void St2LandBaseBreak(LandBase landBase, Fleet enemy, LandBaseAirsList landBaseAirsList, int ti, CutInType cutInType) {
			// 迎撃艦を一覧を算出し、それぞれの撃墜量を出す
			var breakPer = enemy.BreakPer;
			var breakFixed = enemy.BreakFixed(cutInType);
			// 撃墜処理
			for (int wi = 0; wi < landBase.Team[ti].Weapon.Count; ++wi) {
				if (!landBase.Team[ti].Weapon[wi].IsStage2X)
					continue;
				// 迎撃艦を選択する
				int selectKammusuIndex = RandInt(0, breakPer.Count - 1);
				// 割合撃墜
				if (RandInt(0, 1) == 1) {
					int breakCount = (int)(breakPer[selectKammusuIndex] * landBaseAirsList[ti][wi]);
					landBaseAirsList[ti][wi] -= breakCount;
				}
				// 固定撃墜
				if (RandInt(0, 1) == 1) {
					int breakCount = breakFixed[selectKammusuIndex];
					breakCount += CutInAddBonus[(int)cutInType];
					landBaseAirsList[ti][wi] = Math.Max(landBaseAirsList[ti][wi] - breakCount, 0);
				}
			}
		}
		// ステージ2撃墜処理(敵軍)
		private static void St2EnemyBreak(Fleet enemy, Fleet friend, int unitCount, AirsList enemyAirsList, CutInType cutInType) {
			// 迎撃艦を一覧を算出し、それぞれの撃墜量を出す
			var breakPer = friend.BreakPer;
			var breakFixed = friend.BreakFixed(cutInType);
			// 撃墜処理
			for (int i = 0; i < unitCount; ++i) {
				for (int j = 0; j < enemy.Unit[i].Kammusu.Count; ++j) {
					for (int k = 0; k < enemy.Unit[i].Kammusu[j].Weapon.Count; ++k) {
						if (!enemy.Unit[i].Kammusu[j].Weapon[k].IsStage2)
							continue;
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
							breakCount += CutInAddBonus[(int)cutInType];
							enemyAirsList[i][j][k] = Math.Max(enemyAirsList[i][j][k] - breakCount, 0);
						}
					}
				}
			}
		}
	}
}
