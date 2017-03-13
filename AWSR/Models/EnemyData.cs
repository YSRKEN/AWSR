using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using static AWSR.Models.AirWarSimulator;

namespace AWSR.Models
{
	class EnemyData
	{
		// 各種定数

		#region 分析用のモデル
		[JsonObject("enemy")]
		private class EnemyDataModel
		{
			// プロパティ
			[JsonProperty("formation")]
			private string formation { get; set; }
			public Formation Formation {
				get {
					switch (formation) {
					case "trail":
						return Formation.Trail;
					case "subTrail":
						return Formation.SubTrail;
					case "circle":
						return Formation.Circle;
					case "echelon":
						return Formation.Echelon;
					case "abreast":
						return Formation.Abreast;
					default:
						return Formation.Trail;
					}
				}
			}
			[JsonProperty("fleet")]
			public List<List<int>> Fleet { get; set; }
		}
		#endregion

		/// <summary>
		/// 入力されたJSON文字列を解析する
		/// </summary>
		/// <param name="inputEnemyDataText">敵艦隊用のJSON文字列</param>
		/// <returns>デシリアライズした結果</returns>
		private static EnemyDataModel ToEnemyDataModel(string inputEnemyDataText)
			=> JsonConvert.DeserializeObject<EnemyDataModel>(inputEnemyDataText);

		/// <summary>
		/// 入力されたJSON文字列を解析し、敵艦隊文字列として出力する
		/// </summary>
		/// <param name="inputEnemyDataText">敵艦隊用のJSON文字列</param>
		/// <returns>敵艦隊文字列</returns>
		public static string InfoText(string inputEnemyDataText) {
			string output = "";
			// JSONをデシリアライズする
			var enemyData = ToEnemyDataModel(inputEnemyDataText);
			// 艦隊毎に読み込み処理を行う
			output += $"陣形 : {enemyData.Formation.ToStr()}\n";
			foreach (var fleet in enemyData.Fleet.Select((v, i) => new { v, i })) {
				foreach (var ship in fleet.v.Select((v, i) => new { v, i })) {
					output += $"({(fleet.i != 0 ? $"{fleet.i + 1}-" : "")}{ship.i + 1}){ship.v}\n";
				}
			}
			return output;
		}
	}
}
