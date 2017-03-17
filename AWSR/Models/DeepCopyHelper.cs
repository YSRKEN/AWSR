using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AWSR.Models
{
	// ディープコピーを行うためのジェネリックメソッド
	// 参考→http://d.hatena.ne.jp/tekk/20100131/1264913887
	public static class DeepCopyHelper
	{
		public static T DeepCopy<T>(T target) {
			T result;
			var b = new BinaryFormatter();
			var mem = new MemoryStream();
			try {
				b.Serialize(mem, target);
				mem.Position = 0;
				result = (T)b.Deserialize(mem);
			}
			finally {
				mem.Close();
			}
			return result;
		}
	}
}
