using System;
using System.Collections.Generic;

namespace MonoDevelop.HaxeBinding
{
	public struct HxcppStackInfo
	{
		// 1: stack line num. 2: function name. 3: file name. 4: line number
		public int num;
		public string name;
		public string file;
		public int line;
	}

	public class HxcppCommandResult
	{
		public int depth;
		public int depth_unprocessed;
		public List<HxcppStackInfo> stackElements = new List<HxcppStackInfo>();
		public int threadId;
		public List<string> vars = new List<string>();

		public HxcppCommandResult ()
		{
		}
	}
}

