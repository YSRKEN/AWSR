namespace AWSR.Models
{
	static class Constant
	{
		// 読み込み用定数(最大のレベル・運・改修度・熟練度)
		public static readonly int MaxLevel = 155;
		public static readonly int MaxLuck = 100;
		public static readonly int MaxImprovement = 10;
		public static readonly int MaxProficiency = 7;
		// ステージ1の自軍撃墜割合
		// (味方に比べて敵の撃墜割合はまだ検証中らしい)
		public static int[] St1FriendBreakMin = { 7, 20, 30, 45, 65 };
		public static int[] St1FriendBreakMax = { 15, 45, 75, 105, 150 };
		public static int[] St1EnemyBreakMin = { 0, 0, 0, 0, 0 };
		public static int[] St1EnemyBreakMax = { 100, 80, 60, 40, 10 };
		#region 艦種
		// ただし、浮遊要塞・護衛要塞・泊地棲鬼/姫・南方棲鬼は「重巡洋艦」、
		// 南方棲戦鬼は「航空巡洋艦」、装甲空母鬼/姫・戦艦レ級は「航空戦艦」、
		// 秋津洲は「水上機母艦」カテゴリに入れている
		public enum FleetType {
			None, PT, DD, CL, CLT, CA,
			CAV, CVL, CC, BB, BBV, CV,
			AF, SS, SSV, LST, AV, LHA,
			ACV, AR, AS, CP, AO};
		// 艦種を文字列に変換する
		public static string ToStr(this FleetType fleetType)
		{
			string[] name = {
				"無し", "魚雷艇", "駆逐艦", "軽巡洋艦", "重雷装巡洋艦", "重巡洋艦",
				"航空巡洋艦", "軽空母", "巡洋戦艦", "戦艦", "航空戦艦", "正規空母",
				"陸上型", "潜水艦", "潜水空母", "輸送艦", "水上機母艦", "揚陸艦",
				"装甲空母", "工作艦", "潜水母艦", "練習巡洋艦", "給油艦" };
			return name[(int)fleetType];
		}
		#endregion
		#region 陣形
		public enum Formation { Trail, SubTrail, Circle, Echelon, Abreast };
		// 陣形を文字列に変換する
		public static string ToStr(this Formation formation) {
			string[] name = { "単縦陣", "複縦陣", "輪形陣", "梯形陣", "単横陣" };
			return name[(int)formation];
		}
		#endregion
		#region 対空カットイン
		// 名称
		public enum CutInType {
			None, Akiduki1, Akiduki2, Akiduki3, BattleShip1, Normal1,
			BattleShip2, Normal2, Normal3, Normal4, Maya1, Maya2,
			Normal5, Unknown, Isuzu1, Isuzu2, Kasumi1, Kasumi2,
			Satsuki, Kinu1, Kinu2};
		// 固定ボーナス
		public static int[] CutInAddBonus = new int[] {
			0,7,6,4,6,4,
			4,3,4,2,8,6,
			3,0,4,3,4,2,
			2,5,3,
		};
		// 変動ボーナス
		public static double[] CutInMulBonus = new double[] {
			1.0,1.7,1.7,1.6,1.5,1.5,
			1.45,1.35,1.4,1.3,1.65,1.5,
			1.25,1.0,1.45,1.3,1.4,1.25,
			1.2,1.45,1.25,
		};
		#endregion
		#region 制空状態
		public enum AirWarStatus { Best, Good, Balance, Bad, Worst, Size };
		// 制空状態を文字列に変換する
		public static string ToStr(this AirWarStatus airWarStatus) {
			string[] name = { "確保", "優勢", "均衡", "劣勢", "喪失" };
			return name[(int)airWarStatus];
		}
		#endregion
		// 数値を熟練度文字列に変換する
		public static string ToMasStr(int mas) {
			string[] name = { "", "|", "||", "|||", "/", "//", "///", ">>" };
			return name[(mas < 0 || mas > 7 ? 0 : mas)];
		}
		// 航空戦参加艦隊数
		public static int UnitCount(int x, int y) {
			return ((x - 1) & (y - 1)) + 1;
		}
	}
}
