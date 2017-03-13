﻿using Newtonsoft.Json;
using System.Collections.Generic;
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
				// 書き込むためのインスタンスを作成する
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
						var tempKammusu = new Kammusu();
						tempKammusu.Id = kammusu;
						tempKammusu.Level = 0;
						tempKammusu.Luck = -1;
						tempKammusu.IsKammusu = false;
						tempUnit.Kammusu.Add(tempKammusu);
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
		public static Fleet ToFleet(string inputEnemyDataText) {
			// JSONをデシリアライズする
			var jsonModel = JsonConvert.DeserializeObject<JsonModel>(inputEnemyDataText);
			// jsonModelからFleetクラスを構築する
			return jsonModel.ToFleet();
		}
	}
}
