using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using MonoDevelop.Core.Serialization;


namespace MonoDevelop.HaxeBinding
{

	public class PathHelper
	{
		public static string ToRelativePath (string absolutePath, string relativeTo)
		{
			List<string> fileTokens = new List<string> (absolutePath.Split (Path.DirectorySeparatorChar)), anchorTokens = new List<string> (relativeTo.Split (Path.DirectorySeparatorChar));
			StringBuilder builder = new StringBuilder ();
			int length = 0;

			if (!Path.IsPathRooted (absolutePath))
			{
				return absolutePath;
			}
			if (absolutePath == relativeTo)
			{
				return Path.GetFileName (absolutePath);
			}
	
			if (absolutePath.StartsWith (relativeTo) && Directory.Exists (relativeTo))
			{
				builder.AppendFormat (".{0}", Path.DirectorySeparatorChar);
			}// if absolutePath is inside relativeTo
	
			for (; 0 != fileTokens.Count && 0 != anchorTokens.Count;)
			{
				if (fileTokens [0] == anchorTokens [0])
				{
					fileTokens.RemoveAt (0);
					anchorTokens.RemoveAt (0);
				} else
				{
					break;
				}
			}// strip identical leading path
	
			for (int i=0; i < anchorTokens.Count-1; ++i)
			{
				builder.AppendFormat ("..{0}", Path.DirectorySeparatorChar);
			}// navigate out of anchor subdir
	
			foreach (string token in fileTokens)
			{
				builder.AppendFormat ("{0}{1}", token, Path.DirectorySeparatorChar);
			}// append filepath
	
			length = builder.Length;
			if (0 < builder.Length && Path.DirectorySeparatorChar == builder [builder.Length - 1])
			{
				--length;
			}// check for trailing separator

			return builder.ToString (0, length);
		}// ToRelativePath
	}
	
}