﻿using System;
using System.Collections.Generic;
using System.Linq;
using static AWSR.Models.Constant;

namespace AWSR.Models
{
	/// <summary>
	/// 大艦隊クラス
	/// (連合艦隊＝艦隊×2と考える)
	/// </summary>
	class Fleet
	{
		// 大艦隊に属する艦隊
		public List<Unit> Unit { get; set; }
		// 陣形
		public Formation Formation { get; set; }
		// 文字情報
		public string ToInfoText() {
			string output = "";
			output += $"陣形 : {Formation.ToStr()}\n";
			// Selectメソッドでインデックスを付加する手法は次のURLを参考にした
			// http://blog.okazuki.jp/entry/20100728/1280300415
			foreach (var unit in Unit.Select((v, i) => new { v, i })) {
				foreach (var kammusu in unit.v.Kammusu.Select((v, i) => new { v, i })) {
					output += (unit.i != 0 ? $"({unit.i + 1}-{kammusu.i + 1})" : $"({kammusu.i + 1})");
					output += $"{kammusu.v.Name}";
					output += (kammusu.v.Level != 0 ? $" Lv.{kammusu.v.Level}" : "");
					output += (kammusu.v.Luck != -1 ? $" 運{kammusu.v.Luck}" : "");
					foreach (var weapon in kammusu.v.Weapon.Select((v, i) => new { v, i })) {
						// 艦娘がデータベースに載っている際はスロット数を調べ、
						// そこをあふれる位置＝拡張スロットに装備が載ってなければ
						// その部分の表示を省くようにした
						if (DataBase.ContainsKammusu(kammusu.v.Id)
						&& weapon.i >= DataBase.Kammusu(kammusu.v.Id).SlotCount
						&& weapon.v.Id == -1)
							break;
						output += (weapon.i == 0 ? "　" : ",");
						output += $"{weapon.v.Name}";
						output += ToMasStr(weapon.v.Proficiency);
						output += (weapon.v.Improvement > 0 ? $"★{weapon.v.Improvement}" : "");
					}
					output += "\n";
				}
			}
			return output;
		}
		// デッキビルダーのJSONに変換
		public string ToDeckBuilderText() {
			string[] weaponIndexName = { "i1", "i2", "i3", "i4", "ix" };
			string output = "";
			// 艦隊情報を先頭から書き込んでいく
			output += $"{{\"version\":4,";
			foreach (var unit in Unit.Select((v, i) => new { v, i })) {
				output += (unit.i != 0 ? "," : "");
				output += $"\"f{unit.i + 1}\":{{";
				foreach (var kammusu in unit.v.Kammusu.Select((v, i) => new { v, i })) {
					output += (kammusu.i != 0 ? "," : "");
					output += $"\"s{kammusu.i + 1}\":{{";
					output += $"\"id\":\"{kammusu.v.Id}\",";
					output += $"\"lv\":{kammusu.v.Level},";
					output += $"\"luck\":{kammusu.v.Luck},";
					output += "\"items\":{";
					foreach (var weapon in kammusu.v.Weapon.Select((v, i) => new { v, i })) {
						if (weapon.v.Id == -1)
							continue;
						output += (weapon.i != 0 ? "," : "");
						output += $"\"{weaponIndexName[weapon.i]}\":{{";
						output += $"\"id\":{weapon.v.Id},";
						output += $"\"rf\":\"{weapon.v.Improvement}\",";
						output += $"\"mas\":{weapon.v.Proficiency}";
						output += "}";
					}
					output += "}}";
				}
				output += "}";
			}
			output += "}";
			// 出力
			return output;
		}
		// 制空値を計算
		public int AirValue() {
			int airValue = 0;
			foreach(var unit in Unit){
				foreach(var kammusu in unit.Kammusu) {
					// データベースに存在しない艦娘における制空値は0とする
					var kammusuData = DataBase.Kammusu(kammusu.Id);
					foreach (var weapon in kammusu.Weapon.Select((v, i) => new { v, i })) {
						airValue += weapon.v.AirValue(kammusuData.Airs[weapon.i]);
					}
				}
			}
			return airValue;
		}
		// 艦隊防空値を計算
		private double FleetAntiAir() {
			// 各艦娘毎に艦隊防空への影響を算出し、総和を取る
			double fleetAntiAir = 0.0;
			foreach (var unit in Unit) {
				foreach (var kammusu in unit.Kammusu) {
					double tempSum = 0.0;
					foreach (var weapon in kammusu.Weapon) {
						// 装備データを取得する
						var weaponData = DataBase.Weapon(weapon.Id);
						// 装備の素対空値
						double tempAntiAir = weaponData.AntiAir;
						// 装備の種類から、装備倍率と改修倍率を算出する
						double rf = 0.0, gf = 0.0;
						if (weaponData.Name.Contains("高角砲")) {
							// 高角砲の場合
							rf = 0.35; gf = 3.0;
						}
						else if (weaponData.Name.Contains("高射装置")) {
							// 高射装置の場合
							rf = 0.35; gf = 0.0;
						}
						else if (weaponData.Type.Contains("電探")) {
							rf = 0.4; gf = 1.5;
						}
						else if (weaponData.Type == "対空強化弾") {
							rf = 0.6; gf = 0.0;
						}
						else if (weaponData.Type == "対空機銃"
						|| weaponData.Type.Contains("主砲")
						|| weaponData.Type == "副砲"
						|| weaponData.Type == "艦上戦闘機"
						|| weaponData.Type == "艦上爆撃機"
						|| weaponData.Type == "噴式戦闘爆撃機"
						|| weaponData.Type == "水上偵察機") {
							// 機銃等の場合
							rf = 0.2; gf = 0.0;
						}
						// 装備倍率と改修倍率を加算する
						tempSum += rf * tempAntiAir + gf * Math.Sqrt(weapon.Improvement);
					}
					// ここで切り捨てが入ることに注意
					fleetAntiAir += (int)tempSum;
				}
			}
			// 陣形による艦隊防空補正
			double faa = 1.0;
			if(Unit.Count < 2) {
				// 通常艦隊の場合
				if(Formation == Formation.SubTrail) {
					// 複縦陣の場合
					faa = 1.2;
				}else if(Formation == Formation.Circle) {
					// 輪形陣の場合
					faa = 1.6;
				}
			}
			else {
				// 連合艦隊の場合
				if(Formation == Formation.Circle) {
					// 第三警戒航行序列(≒輪形陣)の場合
					faa = 1.5;
				}else if(Formation == Formation.Abreast) {
					// 第一警戒航行序列(対潜陣形)の場合
					faa = 1.1;
				}
			}
			fleetAntiAir = (int)(faa * fleetAntiAir);
			// 最後の補正
			fleetAntiAir = 1.54 * fleetAntiAir;
			return fleetAntiAir;
		}
		// 撃墜計算情報
		public string AntiAirText() {
			// 艦隊防空値
			double fleetAntiAir = FleetAntiAir();
			// 計算しつつ出力する
			string output = "";
			foreach (var unit in Unit.Select((v, i) => new { v, i })) {
				foreach (var kammusu in unit.v.Kammusu) {
					// 艦名
					output += $"{kammusu.Name}→";
					// 加重対空値
					int weightAntiAir = kammusu.WeightAntiAir();
					output += $"加重対空値：{weightAntiAir}";
					// 割合撃墜
					double cf = (Unit.Count < 2 ? 1.0 : unit.i == 0 ? 0.72 : 0.48);
					output += $"　割合撃墜：{Math.Round(cf * weightAntiAir / 4, 1)}％";
					// 固定撃墜
					output += $"　固定撃墜：{(int)((weightAntiAir + fleetAntiAir) * cf / 10)}機";
					output += "\n";
				}
			}
			output += $"艦隊防空：{fleetAntiAir}\n";
			return output;
		}
		// コンストラクタ
		public Fleet() {
			Unit = new List<Unit>();
		}
	}
}
