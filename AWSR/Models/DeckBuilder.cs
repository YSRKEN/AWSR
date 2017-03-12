using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AWSR.Models
{
	static class DeckBuilder
	{
		// 各種定数
		const int MaxFleetCount = 4;
		const int MaxShipCount = 6;
		const int MaxItemCount = 5;
		#region 分析用のモデル
		[JsonObject("deck")]
		private class DeckBuilderModel
		{
			// フィールド
			public fleet Fleet;
			// プロパティ
			[JsonProperty("version")]
			public int Version { get; set; }
			[JsonProperty("f1")]
			private DeckBuilderFleetModel Fleet1 { get; set; }
			[JsonProperty("f2")]
			private DeckBuilderFleetModel Fleet2 { get; set; }
			[JsonProperty("f3")]
			private DeckBuilderFleetModel Fleet3 { get; set; }
			[JsonProperty("f4")]
			private DeckBuilderFleetModel Fleet4 { get; set; }
			// コンストラクタ
			public DeckBuilderModel() {
				Fleet = new fleet(this);
			}
			// 内部クラス
			public class fleet : IEnumerable
			{
				// フィールド
				private DeckBuilderModel dbm;
				// インデクサ
				public DeckBuilderFleetModel this[int n] {
					get {
						switch (n) {
							case 0:
								return dbm.Fleet1;
							case 1:
								return dbm.Fleet2;
							case 2:
								return dbm.Fleet3;
							case 3:
								return dbm.Fleet4;
							default:
								throw new IndexOutOfRangeException();
						}
					}
				}
				// メソッド
				public IEnumerator<DeckBuilderFleetModel> GetEnumerator() {
					for (int i = 0; i < MaxFleetCount; i++) {
						yield return this[i];
					}
				}
				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
				// コンストラクタ
				public fleet(DeckBuilderModel dbm) {
					this.dbm = dbm;
				}
			}
		}
		[JsonObject("fleet")]
		private class DeckBuilderFleetModel
		{
			// フィールド
			public ship Ship;
			// プロパティ
			[JsonProperty("s1")]
			private DeckBuilderShipModel Ship1 { get; set; }
			[JsonProperty("s2")]
			private DeckBuilderShipModel Ship2 { get; set; }
			[JsonProperty("s3")]
			private DeckBuilderShipModel Ship3 { get; set; }
			[JsonProperty("s4")]
			private DeckBuilderShipModel Ship4 { get; set; }
			[JsonProperty("s5")]
			private DeckBuilderShipModel Ship5 { get; set; }
			[JsonProperty("s6")]
			private DeckBuilderShipModel Ship6 { get; set; }
			// コンストラクタ
			public DeckBuilderFleetModel() {
				Ship = new ship(this);
			}
			// 内部クラス
			public class ship : IEnumerable
			{
				// フィールド
				private DeckBuilderFleetModel dbfm;
				// インデクサ
				public DeckBuilderShipModel this[int n] {
					get {
						switch (n) {
							case 0:
								return dbfm.Ship1;
							case 1:
								return dbfm.Ship2;
							case 2:
								return dbfm.Ship3;
							case 3:
								return dbfm.Ship4;
							case 4:
								return dbfm.Ship5;
							case 5:
								return dbfm.Ship6;
							default:
								throw new IndexOutOfRangeException();
						}
					}
				}
				// メソッド
				public IEnumerator<DeckBuilderShipModel> GetEnumerator() {
					for (int i = 0; i < MaxShipCount; i++) {
						yield return this[i];
					}
				}
				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
				// コンストラクタ
				public ship(DeckBuilderFleetModel dbfm) {
					this.dbfm = dbfm;
				}
			}
		}
		[JsonObject("ship")]
		private class DeckBuilderShipModel
		{
			// プロパティ
			[JsonProperty("id")]
			public int Id { get; set; }
			[JsonProperty("lv")]
			public int Level { get; set; }
			[JsonProperty("luck")]
			public int Luck { get; set; }
			[JsonProperty("items")]
			public DeckBuilderItemsModel Items { get; set; }
		}
		[JsonObject("items")]
		private class DeckBuilderItemsModel
		{
			// フィールド
			public item Item;
			// プロパティ
			[JsonProperty("i1")]
			private DeckBuilderItemModel Item1 { get; set; }
			[JsonProperty("i2")]
			private DeckBuilderItemModel Item2 { get; set; }
			[JsonProperty("i3")]
			private DeckBuilderItemModel Item3 { get; set; }
			[JsonProperty("i4")]
			private DeckBuilderItemModel Item4 { get; set; }
			[JsonProperty("ix")]
			private DeckBuilderItemModel ItemX { get; set; }
			// コンストラクタ
			public DeckBuilderItemsModel() {
				Item = new item(this);
			}
			// 内部クラス
			public class item : IEnumerable
			{
				// フィールド
				private DeckBuilderItemsModel dbism;
				// インデクサ
				public DeckBuilderItemModel this[int n] {
					get {
						switch (n) {
							case 0:
								return dbism.Item1;
							case 1:
								return dbism.Item2;
							case 2:
								return dbism.Item3;
							case 3:
								return dbism.Item4;
							case 4:
								return dbism.ItemX;
							default:
								throw new IndexOutOfRangeException();
						}
					}
				}
				// メソッド
				public IEnumerator<DeckBuilderItemModel> GetEnumerator() {
					for (int i = 0; i < MaxItemCount; i++) {
						yield return this[i];
					}
				}
				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
				// コンストラクタ
				public item(DeckBuilderItemsModel dbism) {
					this.dbism = dbism;
				}
			}
		}
		[JsonObject("item")]
		private class DeckBuilderItemModel
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
		/// 入力されたJSON文字列を解析する
		/// </summary>
		/// <param name="inputDeckBuilderText">デッキビルダー用のJSON文字列</param>
		/// <returns>デシリアライズした結果</returns>
		private static DeckBuilderModel ToDeckBuilderModel(string inputDeckBuilderText)
			=> JsonConvert.DeserializeObject<DeckBuilderModel>(inputDeckBuilderText);

		/// <summary>
		/// 入力されたJSON文字列が文法上正しいかを判定する
		/// </summary>
		/// <param name="inputDeckBuilderText"></param>
		/// <returns></returns>
		public static bool IsValidDeckBuilderText(string inputDeckBuilderText) {
			try {
				ToDeckBuilderModel(inputDeckBuilderText);
				return true;
			}
			catch {
				return false;
			}
		}

		/// <summary>
		/// 入力されたJSON文字列を圧縮する
		/// </summary>
		/// <param name="inputDeckBuilderText"></param>
		/// <returns></returns>
		public static string ComplessText(string inputDeckBuilderText) {
			var deckObject = ToDeckBuilderModel(inputDeckBuilderText);
			try {
				return SerializeDeckBuilderModel(deckObject);
			}
			catch {
				return "";
			}
		}

		/// <summary>
		/// 入力されたJSON文字列を解析し、艦隊文字列として出力する
		/// </summary>
		/// <param name="inputDeckBuilderText">デッキビルダー用のJSON文字列</param>
		/// <returns>艦隊文字列</returns>
		public static string InfoText(string inputDeckBuilderText) {
			string output = "";
			// JSONをデシリアライズする
			var deckObject = ToDeckBuilderModel(inputDeckBuilderText);
			// 艦隊毎に読み込み処理を行う
			for (int fi = 0; fi < MaxFleetCount; ++fi)
			{
				var fleetObject = deckObject.Fleet[fi];
				if (fleetObject == null)
					continue;
				output += $"第{fi + 1}艦隊\n";
				for (int si = 0; si < MaxShipCount; ++si)
				{
					var shipObject = fleetObject.Ship[si];
					if (shipObject == null)
						continue;
					output += $"　{si + 1}番艦 : {shipObject.Id}";
					output += (shipObject.Level != 0 ? $" Lv.{shipObject.Level}" : "");
					output += (shipObject.Luck != -1 ? $" 運{shipObject.Luck}" : "");
					output += "\n";
					for (int ii = 0; ii < MaxItemCount; ++ii)
					{
						var itemObject = shipObject.Items.Item[ii];
						if (itemObject == null)
							continue;
						output += $"　　{ii + 1}スロット : {itemObject.Id}";
						output += (itemObject.Rf != 0 ? $" 改修度{itemObject.Rf}" : "");
						output += (itemObject.Mas != 0 ? $" 熟練度{itemObject.Mas}" : "");
						output += "\n";
					}
				}
			}
			return output;
		}

		/// <summary>
		/// DeckBuilderModelをJSONに書き戻す
		/// </summary>
		/// <param name="model">DeckBuilderModel</param>
		/// <returns>JSON文字列</returns>
		private static string SerializeDeckBuilderModel(DeckBuilderModel model) {
			var output = "";
			// テンポラリ変数に艦隊情報を詰め込む
			output += $"{{\"version\":{model.Version},";
			bool first_flg1 = true;
			for (int fi = 0; fi < MaxFleetCount; ++fi) {
				if (model.Fleet[fi] == null)
					continue;
				if (!first_flg1)
					output += ",";
				first_flg1 = false;
				output += $"\"f{fi + 1}\":{{";
				bool first_flg2 = true;
				for (int si = 0; si < MaxShipCount; ++si) {
					if (model.Fleet[fi].Ship[si] == null)
						continue;
					if (!first_flg2)
						output += ",";
					first_flg2 = false;
					output += $"\"s{si + 1}\":{{";
					output += $"\"id\":\"{model.Fleet[fi].Ship[si].Id}\",";
					output += $"\"lv\":{model.Fleet[fi].Ship[si].Level},";
					output += $"\"luck\":{model.Fleet[fi].Ship[si].Luck},";
					output += "\"items\":{";
					bool first_flg3 = true;
					for (int ii = 0; ii < MaxItemCount; ++ii) {
						if (model.Fleet[fi].Ship[si].Items.Item[ii] == null)
							continue;
						if (!first_flg3)
							output += ",";
						first_flg3 = false;
						output += $"\"i{ii + 1}\":{{";
						output += $"\"id\":{model.Fleet[fi].Ship[si].Items.Item[ii].Id},";
						output += $"\"rf\":\"{model.Fleet[fi].Ship[si].Items.Item[ii].Rf}\",";
						output += $"\"mas\":{model.Fleet[fi].Ship[si].Items.Item[ii].Mas}";
						output += "}";
					}
					output += "}}";
				}
				output += "}";
			}
			output += "}";
			return output;
		}
	}
}
