using System;
using System.Collections.Generic;
using static AWSR.Models.Constant;

namespace AWSR.Models
{
	using AirsList = List<List<List<int>>>;
	static class Simulator
	{
		public static string MonteCarlo(Fleet friend, Fleet enemy, int simulationSize) {
			string output =  "【モンテカルロシミュレーション】\n";
			output += $"反復回数：{simulationSize}回\n";
			// 初期状態を記録する
			var firstFriendAirsList = friend.AirsList;
			var firstEnemyAirsList = enemy.AirsList;
			// 反復計算を行う
			var friendAirsList = DeepCopyHelper.DeepCopy(firstFriendAirsList);
			var enemyAirsList = DeepCopyHelper.DeepCopy(firstEnemyAirsList);
			var AirWarStatusCount = new List<int> { 0,0,0,0,0 };
			for(int i = 0; i < simulationSize; ++i) {
				// 状態を初期化する
				CopyAirsList(firstFriendAirsList, friendAirsList);
				CopyAirsList(firstEnemyAirsList, enemyAirsList);
				// 制空値を計算する
				int friendAirValue = NowAirValue(friend, friendAirsList);
				int enemyAirValue = NowAirValue(enemy, enemyAirsList);
				// 制空状態を判断する
				AirWarStatus airWarStatus = CalcAirWarStatus(friendAirValue, enemyAirValue);
				++AirWarStatusCount[(int)airWarStatus];
				
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
	}
}
