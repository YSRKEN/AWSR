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
		private class JsonModel
		{
			[JsonProperty("formation")]
			private string formation { get; set; }
			[JsonProperty("fleet")]
			private List<List<int>> fleet { get; set; }
			// Fleetクラスを構築する
			public Fleet ToFleet() {
				// 艦隊数
				var outputFleet = new Fleet();
				// 陣形
				var formationHash = new Dictionary<string, Formation> {
					{ "trail", Formation.Trail },
					{ "subTrail", Formation.SubTrail },
					{ "circle",Formation.Circle },
					{ "echelon", Formation.Echelon },
					{ "abreast", Formation.Abreast },
				};
				outputFleet.Formation = formationHash[formation];
				// 艦船
				foreach(var unit in fleet) {
					var tempUnit = new Unit();
					foreach (int kammusu in unit) {
						var tempKammsu = new Kammusu();
						tempKammsu.Id = kammusu;
						tempKammsu.Level = 0;
						tempKammsu.Luck = -1;
						tempKammsu.IsKammusu = false;
						tempUnit.Kammusu.Add(tempKammsu);
					}
					outputFleet.Unit.Add(tempUnit);
				}
				return outputFleet;
			}
		}
		#endregion

		/// <summary>
		/// 入力されたJSON文字列を大艦隊クラスに変換する
		/// </summary>
		/// <param name="inputJsonText">JSON文字列</param>
		/// <returns>大艦隊クラス</returns>
		private static Fleet ToFleet(string inputJsonText) {
			// JSONをデシリアライズする
			var jsonModel = JsonConvert.DeserializeObject<JsonModel>(inputJsonText);
			// jsonModelからFleetクラスを構築する
			return jsonModel.ToFleet();
		}

		/// <summary>
		/// 入力されたJSON文字列を解析し、敵艦隊文字列として出力する
		/// </summary>
		/// <param name="inputEnemyDataText">敵艦隊用のJSON文字列</param>
		/// <returns>敵艦隊文字列</returns>
		public static string InfoText(string inputEnemyDataText) {
			// JSON文字列を大艦隊クラスに変換する
			var fleet = ToFleet(inputEnemyDataText);
			// 艦隊毎に読み込み処理を行う
			return fleet.InfoText;
		}
	}
}
