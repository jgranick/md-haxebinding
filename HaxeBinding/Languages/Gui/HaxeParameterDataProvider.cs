using System;
using System.Xml;
using MonoDevelop.Ide.CodeCompletion;


namespace MonoDevelop.HaxeBinding.Languages.Gui
{

	public class HaxeParameterDataProvider : ParameterDataProvider
	{
		
		/*public int OverloadCount { get { return 1; } }
		
		private string[] parameters;
		private bool showCompletion = false;
		private string signature;*/
		
		private int offset;
		private string parameterName;
		private string[] parameters;
		private string signature;

		
		public HaxeParameterDataProvider (int startOffset) : base (startOffset)
		{
			offset = startOffset;
		}
		
		
		public void Update (CodeCompletionContext completionContext, XmlDocument data)
		{
			//showCompletion = true;
			offset = completionContext.TriggerOffset;
			
			signature = data.FirstChild.InnerText.Trim ();
			
			//MonoDevelop.Ide.MessageService.ShowError (signature);
			
			signature = signature.Replace (" ", "");
			signature = signature.Replace ("<", "&lt;");
			signature = signature.Replace (">", "&gt;");

			string[] splitByOpenParen = signature.Split ('(');
			signature = "";

			for (int i = 0; i < splitByOpenParen.Length; i++) {

				string[] splitByCloseParen = splitByOpenParen[i].Split (')');

				for (int j = 0; j < splitByCloseParen.Length; j++) {

					if (i > 0 && j == 0) {

						signature += splitByCloseParen[0].Replace ("-&gt;", "->");

					} else {

						signature += splitByCloseParen[i];

					}

				}

			}
			
			//string[] splitByColon = signature.Split (':');
			
			//if (splitByColon.Length > 1) {
				
				//parameterName = splitByColon[0];
				//signature = splitByColon[1];
				
			//}
			
			//MonoDevelop.Ide.MessageService.ShowError (parameterName);
			//MonoDevelop.Ide.MessageService.ShowError (signature);
			
			parameters = signature.Split (new string[] { "-&gt;" }, StringSplitOptions.None);

			for (int i = 0; i < parameters.Length; i++) {

				parameters[i] = parameters[i].Replace ("->", " -&gt; ");
				//MonoDevelop.Ide.MessageService.ShowError (parameters[i]);

			}
			
			//MonoDevelop.Ide.MessageService.ShowError (parameters.Length.ToString ());
			//MonoDevelop.Ide.MessageService.ShowError (parameters.ToString ());
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
		public override TooltipInformation CreateTooltipInformation (int overload, int currentParameter, bool smartWrap)
		{
			//MonoDevelop.Ide.MessageService.ShowError ("CreateTooltipInformation");
			var tooltipInfo = new TooltipInformation ();
			tooltipInfo.SignatureMarkup = GetHeading (overload, null, currentParameter);
			//tooltipInfo.SignatureMarkup = "lskdfjlsdfj";
			//tooltipInfo.AddCategory ("Parameter", "sdfskdf");
			return tooltipInfo;
		}
		
		
		public int StartOffset {
			get {
				return offset;
			}
		}


		private string FormatShortType (string type)
		{
			string[] keywords = type.Split (' ');
			type = "";

			for (int i = 0; i < keywords.Length; i++) {

				if (keywords[i].IndexOf (".") > -1) {

					string[] segments = keywords[i].Split ('.');

					if (keywords[0].IndexOf ("<") > -1) {

						type += keywords[0].Substring (0, keywords[0].IndexOf ("<") + 1);

					} else if (keywords[0].StartsWith (":")) {

						type += ":";

					}

					type += segments[segments.Length - 1];
					
				} else {

					type += keywords[i];

				}

				if (i < keywords.Length - 1) {

					type += " ";

				}

			}

			return type;
		}
		
		
		public string GetHeading (int overload, string[] parameterDescription, int currentParameter)
		{
			//MonoDevelop.Ide.MessageService.ShowError ("GetHeading");
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
							display += "<span foreground=\"#a0a0f7\"><b>" + param.Substring (0, index) + "</b>" + FormatShortType (param.Substring (index)) + "</span>";
						}
						else
						{
							display += "<b>" + param.Substring (0, index) + "</b>" + FormatShortType (param.Substring (index));
						}
						
						if (i < parameters.Length - 2)
						{
							display += ", ";
						}
					}
				}
				
				display += ") : " + FormatShortType (parameters [parameters.Length - 1]) + " ...";
				
				return display;
		}
		
		public string GetDescription (int overload, int currentParameter)
		{
			//MonoDevelop.Ide.MessageService.ShowError ("GetDescription");
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

		public override string GetParameterName (int overload, int paramIndex)
		{
			//MonoDevelop.Ide.MessageService.ShowError ("GetParameterName");
			//return parameterName;
			return "";
		}
		
		public string GetParameterDescription (int overload, int paramIndex)
		{
			//MonoDevelop.Ide.MessageService.ShowError ("GetParameterDescription");
			/*var indexer = indexers[overload];
			
			if (paramIndex < 0 || paramIndex >= indexer.Parameters.Count)
				return "";
			
			return ambience.GetString (indexer, indexer.Parameters [paramIndex], OutputFlags.AssemblyBrowserDescription | OutputFlags.HideExtensionsParameter | OutputFlags.IncludeGenerics | OutputFlags.IncludeModifiers | OutputFlags.IncludeParameterName | OutputFlags.HighlightName);*/
			//return "";
			return parameters [paramIndex];
		}

		public override int GetParameterCount (int overload)
		{
			//MonoDevelop.Ide.MessageService.ShowError ((parameters.Length - 1).ToString());
			/*if (overload >= Count)
				return -1;
			var indexer = indexers[overload];
			return indexer != null && indexer.Parameters != null ? indexer.Parameters.Count : 0;*/
			//return 0;
			return parameters.Length - 1;
		}

		public override bool AllowParameterList (int overload)
		{
			//MonoDevelop.Ide.MessageService.ShowError ("AllowParameterList");
			/*if (overload >= Count)
				return false;
			var lastParam = indexers[overload].Parameters.LastOrDefault ();
			return lastParam != null && lastParam.IsParams;
			*/
			return true;
		}
		
		public override int Count {
			get {
				//return indexers != null ? indexers.Count : 0;
				//return 0;
				return 1;
			}
		}

	}
	
}