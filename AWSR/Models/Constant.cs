namespace AWSR.Models
{
	static class Constant
	{
		// 読み込み用定数(最大のレベル・運・改修度・熟練度)
		public static readonly int MaxLevel = 155;
		public static readonly int MaxLuck = 100;
		public static readonly int MaxImprovement = 10;
		public static readonly int MaxProficiency = 7;
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
		// 数値を熟練度文字列に変換する
		public static string ToMasStr(int mas) {
			string[] name = { "", "|", "||", "|||", "/", "//", "///", ">>" };
			return name[(mas < 0 || mas > 7 ? 0 : mas)];
		}
	}
}
