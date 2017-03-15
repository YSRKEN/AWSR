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
			var AirWarStatusCount = new int[] { 0, 0, 0, 0, 0 };
			for(int i = 0; i < simulationSize; ++i) {
				var friendAirsList = DeepCopyHelper.DeepCopy(firstFriendAirsList);
				var enemyAirsList = DeepCopyHelper.DeepCopy(firstEnemyAirsList);
			}
			return output;
		}
	}
}
