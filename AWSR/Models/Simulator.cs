using System.Collections.Generic;

namespace AWSR.Models
{
	static class Simulator
	{
		public static string MonteCarlo(Fleet friend, Fleet enemy, int simulationSize) {
			string output =  "【モンテカルロシミュレーション】\n";
			output += $"反復回数：{simulationSize}回\n";
			// 初期状態を記録する
			var firstFriendAirsList = friend.AirsList;
			var firstEnemyAirsList = enemy.AirsList;

			return output;
		}
	}
}
