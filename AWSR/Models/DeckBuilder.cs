using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace AWSR.Models
{
	static class DeckBuilder
	{
		// 各種定数
		#region 分析用のモデル
		[JsonObject("deck")]
		private class FleetModel
		{
			// プロパティ
			[JsonProperty("version")]
			[DefaultValue(4)]
			private int Version { get; set; }
			[JsonProperty("f1", DefaultValueHandling = DefaultValueHandling.Ignore)]
			private UnitModel Unit1 { get; set; }
			[JsonProperty("f2", DefaultValueHandling = DefaultValueHandling.Ignore)]
			private UnitModel Unit2 { get; set; }
			[JsonProperty("f3", DefaultValueHandling = DefaultValueHandling.Ignore)]
			private UnitModel Unit3 { get; set; }
			[JsonProperty("f4", DefaultValueHandling = DefaultValueHandling.Ignore)]
			private UnitModel Unit4 { get; set; }
			// Fleetクラスを構築する
			public Fleet ToFleet() {
				// 書き込むためのインスタンスを作成する
				var outputFleet = new Fleet();
				// 艦隊
				var unitList = new List<UnitModel>();
				if (Unit1 != null) unitList.Add(Unit1);
				if (Unit2 != null) unitList.Add(Unit2);
				if (Unit3 != null) unitList.Add(Unit3);
				if (Unit4 != null) unitList.Add(Unit4);
				foreach(var fleet in unitList) {
					var tempUnit = new Unit();
					// 艦娘
					var kammusuList = new List<KammusuModel>();
					if (fleet.Kammusu1 != null && fleet.Kammusu1.Id > 0) kammusuList.Add(fleet.Kammusu1);
					if (fleet.Kammusu2 != null && fleet.Kammusu2.Id > 0) kammusuList.Add(fleet.Kammusu2);
					if (fleet.Kammusu3 != null && fleet.Kammusu3.Id > 0) kammusuList.Add(fleet.Kammusu3);
					if (fleet.Kammusu4 != null && fleet.Kammusu4.Id > 0) kammusuList.Add(fleet.Kammusu4);
					if (fleet.Kammusu5 != null && fleet.Kammusu5.Id > 0) kammusuList.Add(fleet.Kammusu5);
					if (fleet.Kammusu6 != null && fleet.Kammusu6.Id > 0) kammusuList.Add(fleet.Kammusu6);
					foreach (var kammusu in kammusuList) {
						var tempKammusu = new Kammusu();
						tempKammusu.Id = kammusu.Id;
						tempKammusu.Level = kammusu.Level;
						tempKammusu.Luck = kammusu.Luck;
						tempKammusu.IsKammusu = true;
						// 装備
						var weaponList = new List<WeaponModel>();
						weaponList.Add(kammusu.Items.Item1);
						weaponList.Add(kammusu.Items.Item2);
						weaponList.Add(kammusu.Items.Item3);
						weaponList.Add(kammusu.Items.Item4);
						weaponList.Add(kammusu.Items.ItemX);
						foreach (var weapon in weaponList) {
							var tempWeapon = new Weapon();
							if(weapon != null) {
								// 装備が存在する場合
								tempWeapon.Id = weapon.Id;
								tempWeapon.Improvement = weapon.Rf;
								tempWeapon.Proficiency = weapon.Mas;
							}
							else {
								// 装備が存在しない場合
								tempWeapon.Id = -1;
								tempWeapon.Improvement = 0;
								tempWeapon.Proficiency = 0;
							}
							tempKammusu.Weapon.Add(tempWeapon);
						}
						tempUnit.Kammusu.Add(tempKammusu);
					}
					outputFleet.Unit.Add(tempUnit);
				}
				return outputFleet;
			}
		}
		[JsonObject("unit")]
		private class UnitModel
		{
			// プロパティ
			[JsonProperty("s1", DefaultValueHandling = DefaultValueHandling.Ignore)]
			public KammusuModel Kammusu1 { get; set; }
			[JsonProperty("s2", DefaultValueHandling = DefaultValueHandling.Ignore)]
			public KammusuModel Kammusu2 { get; set; }
			[JsonProperty("s3", DefaultValueHandling = DefaultValueHandling.Ignore)]
			public KammusuModel Kammusu3 { get; set; }
			[JsonProperty("s4", DefaultValueHandling = DefaultValueHandling.Ignore)]
			public KammusuModel Kammusu4 { get; set; }
			[JsonProperty("s5", DefaultValueHandling = DefaultValueHandling.Ignore)]
			public KammusuModel Kammusu5 { get; set; }
			[JsonProperty("s6", DefaultValueHandling = DefaultValueHandling.Ignore)]
			public KammusuModel Kammusu6 { get; set; }
		}
		[JsonObject("ship")]
		private class KammusuModel
		{
			// プロパティ
			[JsonProperty("id")]
			public int Id { get; set; }
			[JsonProperty("lv")]
			public int Level { get; set; }
			[JsonProperty("luck")]
			public int Luck { get; set; }
			[JsonProperty("items")]
			public WeaponListModel Items { get; set; }
		}
		[JsonObject("items")]
		private class WeaponListModel
		{
			// プロパティ
			[JsonProperty("i1", DefaultValueHandling = DefaultValueHandling.Ignore)]
			public WeaponModel Item1 { get; set; }
			[JsonProperty("i2", DefaultValueHandling = DefaultValueHandling.Ignore)]
			public WeaponModel Item2 { get; set; }
			[JsonProperty("i3", DefaultValueHandling = DefaultValueHandling.Ignore)]
			public WeaponModel Item3 { get; set; }
			[JsonProperty("i4", DefaultValueHandling = DefaultValueHandling.Ignore)]
			public WeaponModel Item4 { get; set; }
			[JsonProperty("ix", DefaultValueHandling = DefaultValueHandling.Ignore)]
			public WeaponModel ItemX { get; set; }
		}
		[JsonObject("item")]
		private class WeaponModel
		{
			// プロパティ
			[JsonProperty("id")]
			public int Id { get; set; }
			[JsonProperty("rf")]
			public int Rf { get; set; }
			[JsonProperty("mas")]
			public int Mas { get; set; }
		}
		#endregion

		/// <summary>
		/// 入力されたJSON文字列を大艦隊クラスに変換する
		/// </summary>
		/// <param name="inputDeckBuilderText">JSON文字列</param>
		/// <returns>大艦隊クラス</returns>
		public static Fleet ToFleet(string inputDeckBuilderText) {
			// JSONをデシリアライズする
			var jsonModel = JsonConvert.DeserializeObject<FleetModel>(inputDeckBuilderText);
			// jsonModelからFleetクラスを構築する
			return jsonModel.ToFleet();
		}
	}
}
