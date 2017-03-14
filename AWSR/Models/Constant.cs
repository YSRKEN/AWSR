namespace AWSR.Models
{
	static class Constant
	{
		// 艦種
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
	}
}
