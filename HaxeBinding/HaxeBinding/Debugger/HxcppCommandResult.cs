using System;
using System.Collections.Generic;

namespace MonoDevelop.HaxeBinding
{
	struct HxcppStackInfo
	{
		// 1: stack line num. 2: function name. 3: file name. 4: line number
		public int num;
		public string name;
		public string file;
		public long line;
	}

	public class HxcppCommandResult
	{
		public HxcppBacktrace backtrace;
		public int depth;
		public int depth_unprocessed;
		public List<HxcppStackInfo> stack = new List<HxcppStackInfo>();
		public int threadId;

		public HxcppCommandResult ()
		{
		}
	}
}

