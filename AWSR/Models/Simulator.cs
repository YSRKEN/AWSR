using System;
using System.Collections.Generic;

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
			var AirWarStatusCount = new int[] { 0, 0, 0, 0, 0 };
			for(int i = 0; i < simulationSize; ++i) {
				// 状態を初期化する
				CopyAirsList(firstFriendAirsList, friendAirsList);
				CopyAirsList(firstEnemyAirsList, enemyAirsList);
				// 制空値を計算する
				int friendAirValue = NowAirValue(friend, friendAirsList);
				int enemyAirValue = NowAirValue(enemy, enemyAirsList);
				// 制空状態を判断し、結果を配列に加算する
				if(friendAirValue >= enemyAirValue * 3) {
					++AirWarStatusCount[0];
				}else if(friendAirValue * 2 >= enemyAirValue * 3) {
					++AirWarStatusCount[1];
				}else if(friendAirValue * 3 >= enemyAirValue * 2) {
					++AirWarStatusCount[2];
				}else if(friendAirValue * 3 >= enemyAirValue) {
					++AirWarStatusCount[3];
				}
				else {
					++AirWarStatusCount[4];
				}
			}

			// 結果を書き出す
			output += "制空状態：\n";
			output += "本隊";
			output += $"　確保：{Math.Round(100.0 * AirWarStatusCount[0] / simulationSize, 1)}％";
			output += $"　優勢：{Math.Round(100.0 * AirWarStatusCount[1] / simulationSize, 1)}％";
			output += $"　均衡：{Math.Round(100.0 * AirWarStatusCount[2] / simulationSize, 1)}％";
			output += $"　劣勢：{Math.Round(100.0 * AirWarStatusCount[3] / simulationSize, 1)}％";
			output += $"　喪失：{Math.Round(100.0 * AirWarStatusCount[4] / simulationSize, 1)}％";
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
	}
}
