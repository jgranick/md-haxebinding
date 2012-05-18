using System;
using System.Xml;
using MonoDevelop.Ide.CodeCompletion;
using ICSharpCode.NRefactory.Completion;


namespace MonoDevelop.HaxeBinding.Languages.Gui
{

	public class HaxeParameterDataProvider : IParameterDataProvider
	{
		
		/*public int OverloadCount { get { return 1; } }
		
		private string[] parameters;
		private bool showCompletion = false;
		private string signature;*/
		
		private int offset;
		private string[] parameters;
		private string signature;

		
		public HaxeParameterDataProvider ()
		{
			
		}
		
		
		public void Update (CodeCompletionContext completionContext, XmlDocument data)
		{
			//showCompletion = true;
			offset = completionContext.TriggerOffset;
			
			signature = data.FirstChild.InnerText.Trim ();
			signature = signature.Replace (" ", "");
			signature = signature.Replace ("<", "&lt;");
			signature = signature.Replace (">", "&gt;");
			
			parameters = signature.Split (new string[] { "-&gt;" }, StringSplitOptions.None);
		}
		
		
		/*public void Clear ()
		{
			showCompletion = false;
			parameters = new string[] {};
		}

		
		public int GetCurrentParameterIndex (ICompletionWidget widget, CodeCompletionContext ctx)
		{
			if (showCompletion)
			{
				return 0;	
			}
			
			return -1;
		}

		
		public string GetMethodMarkup (int overload, string[] parameterMarkup, int currentParameter)
		{
			if (showCompletion)
			{
				string display = "(";
				
				for (int i = 0; i < parameters.Length - 1; i++)
				{
					string param = parameters [i];
					int index = param.IndexOf (":");
					
					if (index > -1)
					{	
						if (i == currentParameter)
						{
							display += "<span foreground=\"#a0a0f7\"><b>" + param.Substring (0, index) + "</b>" + param.Substring (index) + "</span>";
						}
						else
						{
							display += "<b>" + param.Substring (0, index) + "</b>" + param.Substring (index);
						}
						
						if (i < parameters.Length - 2)
						{
							display += ", ";
						}
					}
				}
				
				display += "):" + parameters [parameters.Length - 1];
				
				return display;
			}
			
			return null;
		}

		
		public int GetParameterCount (int overload)
		{
			if (showCompletion)
			{
				return parameters.Length - 1;
			}
			
			return 0;
		}

		
		public string GetParameterMarkup (int overload, int paramIndex)
		{
			if (showCompletion)
			{
				return parameters [paramIndex];
			}
			
			return null;
		}

		
		public void Update (CodeCompletionContext completionContext, XmlDocument data)
		{
			showCompletion = true;
			
			signature = data.FirstChild.InnerText.Trim ();
			signature = signature.Replace (" ", "");
			signature = signature.Replace ("<", "&lt;");
			signature = signature.Replace (">", "&gt;");
			
			parameters = signature.Split (new string[] { "-&gt;" }, StringSplitOptions.None);
		}*/
		
		
		public int StartOffset {
			get {
				return offset;
			}
		}
		
		
		public string GetHeading (int overload, string[] parameterDescription, int currentParameter)
		{
			/*StringBuilder result = new StringBuilder ();
//			int curLen = 0;
			result.Append (ambience.GetString (indexers [overload].ReturnType, OutputFlags.ClassBrowserEntries));
			result.Append (' ');
			result.Append ("<b>");
			result.Append (resolvedExpression);
			result.Append ("</b>");
			result.Append ('[');
			int parameterCount = 0;
			foreach (string parameter in parameterMarkup) {
				if (parameterCount > 0)
					result.Append (", ");
				result.Append (parameter);
				parameterCount++;
			}
			result.Append (']');
			return result.ToString ();*/
			//return parameterDescription[currentParameter];
			//return parameterDescription[0] + ", " + parameterDescription[1];
			string display = "(";
				
				for (int i = 0; i < parameters.Length - 1; i++)
				{
					string param = parameters [i];
					int index = param.IndexOf (":");
					
					if (index > -1)
					{	
						if (i == currentParameter)
						{
							display += "<span foreground=\"#a0a0f7\"><b>" + param.Substring (0, index) + "</b>" + param.Substring (index) + "</span>";
						}
						else
						{
							display += "<b>" + param.Substring (0, index) + "</b>" + param.Substring (index);
						}
						
						if (i < parameters.Length - 2)
						{
							display += ", ";
						}
					}
				}
				
				display += "):" + parameters [parameters.Length - 1];
				
				return display;
		}
		
		public string GetDescription (int overload, int currentParameter)
		{
			/*StringBuilder result = new StringBuilder ();
			var curParameter = currentParameter >= 0 && currentParameter < indexers [overload].Parameters.Count ? indexers [overload].Parameters [currentParameter] : null;
			if (curParameter != null) {
				string docText = AmbienceService.GetDocumentation (indexers [overload]);
				if (!string.IsNullOrEmpty (docText)) {
					var paramRegex = new Regex ("(\\<param\\s+name\\s*=\\s*\"" + curParameter.Name + "\"\\s*\\>.*?\\</param\\>)", RegexOptions.Compiled);
					var match = paramRegex.Match (docText);
					if (match.Success) {
						result.AppendLine ();
						string text = match.Groups [1].Value;
						text = "<summary>" + AmbienceService.GetDocumentationSummary (indexers [overload]) + "</summary>" + text;
						result.Append (AmbienceService.GetDocumentationMarkup (text, new AmbienceService.DocumentationFormatOptions {
							HighlightParameter = curParameter.Name,
							MaxLineLength = 60
						}));
					}
				}
			}
			
			return result.ToString ();*/
			//return "desc";
			return "";
		}
		
		public string GetParameterDescription (int overload, int paramIndex)
		{
			/*var indexer = indexers[overload];
			
			if (paramIndex < 0 || paramIndex >= indexer.Parameters.Count)
				return "";
			
			return ambience.GetString (indexer, indexer.Parameters [paramIndex], OutputFlags.AssemblyBrowserDescription | OutputFlags.HideExtensionsParameter | OutputFlags.IncludeGenerics | OutputFlags.IncludeModifiers | OutputFlags.IncludeParameterName | OutputFlags.HighlightName);*/
			//return "";
			return parameters [paramIndex];
		}

		public int GetParameterCount (int overload)
		{
			/*if (overload >= Count)
				return -1;
			var indexer = indexers[overload];
			return indexer != null && indexer.Parameters != null ? indexer.Parameters.Count : 0;*/
			//return 0;
			return parameters.Length - 1;
		}

		public bool AllowParameterList (int overload)
		{
			/*if (overload >= Count)
				return false;
			var lastParam = indexers[overload].Parameters.LastOrDefault ();
			return lastParam != null && lastParam.IsParams;
			*/
			return true;
		}
		
		public int Count {
			get {
				//return indexers != null ? indexers.Count : 0;
				//return 0;
				return 1;
			}
		}

	}
	
}