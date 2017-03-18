using Newtonsoft.Json;
using System.Collections.Generic;
using static AWSR.Models.Constant;

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
						// 装備(データベースから情報を拾う)
						foreach (int id in DataBase.Kammusu(kammusu).WeaponId) {
							// 初期装備データでは、id == -1だと「それ以降は装備していない」ことを指す
							if (id == -1)
								break;
							var tempWeapon = new Weapon();
							tempWeapon.Id = id;
							tempWeapon.Improvement = 0;
							tempWeapon.Proficiency = 0;
							tempWeapon.Complete();
							tempKammusu.Weapon.Add(tempWeapon);
						}
						tempKammusu.Complete();
						tempUnit.Kammusu.Add(tempKammusu);
					}
					outputFleet.Unit.Add(tempUnit);
				}
				outputFleet.Complete();
				return outputFleet;
			}
		}
		#endregion

		/// <summary>
		/// 入力された文字列を大艦隊クラスに変換する
		/// </summary>
		/// <param name="inputEnemyDataText">文字列</param>
		/// <returns>大艦隊クラス</returns>
		public static Fleet ToFleet(string inputEnemyDataText) {
			// まずはJSONと仮定して読み込む
			try {
				// JSONをデシリアライズする
				var jsonModel = JsonConvert.DeserializeObject<JsonModel>(inputEnemyDataText);
				// jsonModelからFleetクラスを構築する
				return jsonModel.ToFleet();
			}
			// 無理だったらenm形式と考えて読み込む
			// 1行目：陣形(単縦陣・複縦陣・輪形陣・梯形陣・単横陣。連合艦隊だと第一～第四が単横陣・複縦陣・輪形陣・単縦陣に対応)
			// 2行目：艦隊数。通常艦隊なら1、連合艦隊なら2
			// 3行目：第1艦隊。艦の名称が一番艦から順にカンマ区切りで書かれている
			// 4行目：第2艦隊(同上)
			// ※同一名の艦の候補が複数ある場合、デフォルトでは「-1」「-2」等が末尾に付いている
			catch {
				var fleet = new Fleet();
				using(var rs = new System.IO.StringReader(inputEnemyDataText)) {
					// 1行目：陣形
					string formationStr = rs.ReadLine();
					var formationHash = new Dictionary<string, Formation> {
						{ "単縦陣", Formation.Trail },
						{ "複縦陣", Formation.SubTrail },
						{ "輪形陣",Formation.Circle },
						{ "梯形陣", Formation.Echelon },
						{ "単横陣", Formation.Abreast },
					};
					fleet.Formation = formationHash[formationStr];
					// 2行目：艦隊数
					int fleetCount = int.Parse(rs.ReadLine());
					fleetCount = (fleetCount <= 1 ? 1 : 2);
					// 3・4行目：第1・2艦隊
					for(int i = 0; i < fleetCount; ++i) {
						string getLine = rs.ReadLine();
						var column = getLine.Split(',');
						var tempUnit = new Unit();
						foreach (string columnName in column) {
							var tempKammusu = new Kammusu();
							int id = DataBase.KammusuId(columnName);
							if (id >= 0) {
								tempKammusu.Id = id;
								tempKammusu.Level = 0;
								tempKammusu.Luck = -1;
								tempKammusu.IsKammusu = false;
								// 装備(データベースから情報を拾う)
								foreach (int id2 in DataBase.Kammusu(id).WeaponId) {
									// 初期装備データでは、id == -1だと「それ以降は装備していない」ことを指す
									if (id2 == -1)
										break;
									var tempWeapon = new Weapon();
									tempWeapon.Id = id2;
									tempWeapon.Improvement = 0;
									tempWeapon.Proficiency = 0;
									tempWeapon.Complete();
									tempKammusu.Weapon.Add(tempWeapon);
								}
								tempKammusu.Complete();
								tempUnit.Kammusu.Add(tempKammusu);
							}
						}
						fleet.Unit.Add(tempUnit);
					}
				}
				fleet.Complete();
				return fleet;
			}
		}
	}
}
