using System;
using System.Xml;
using MonoDevelop.Ide.CodeCompletion;


namespace MonoDevelop.HaxeBinding.Languages.Gui
{
	public class HaxeParameterDataProvider : IParameterDataProvider
	{
		private bool showCompletion = false;
		private string[] parameters;
		private string signature;
		
		public HaxeParameterDataProvider ()
		{
		}
		
		public int GetCurrentParameterIndex (ICompletionWidget widget, CodeCompletionContext ctx)
		{
			//MonoDevelop.Ide.MessageService.ShowMessage (ctx.TriggerOffset.ToString ());
			
			if (showCompletion) {
				
				return 0;
				
			}
			
			return -1;
		}
		
		public string GetMethodMarkup (int overload, string[] parameterMarkup, int currentParameter)
		{
			
			if (showCompletion) {
				
				string display = "(";
				
				for (int i = 0; i < parameters.Length - 1; i++) {
					
					string param = parameters[i];
					int index = param.IndexOf (":");
					
					if (index > -1) {
						
						if (i == currentParameter) {
							
							display += "<span foreground=\"#a0a0f7\"><b>" + param.Substring (0, index) + "</b>" + param.Substring (index) + "</span>";
							
						} else {
							
							display += "<b>" + param.Substring (0, index) + "</b>" + param.Substring (index);
							
						}
						
						if (i < parameters.Length - 2) {
							
							display += ", ";
							
						}
						
					}
					
				}
				
				display += "):" + parameters[parameters.Length - 1];
				
				return display;
				
			}
			
			return null;
		}
		
		public int GetParameterCount (int overload)
		{
			if (showCompletion) {
				
				return parameters.Length - 1;
				
			}
			
			return 0;
		}
		
		public string GetParameterMarkup (int overload, int paramIndex)
		{
			if (showCompletion) {
				
				return parameters[paramIndex];
				
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
			
			//MonoDevelop.Ide.MessageService.ShowMessage (parameters.Length.ToString ());
			//MonoDevelop.Ide.MessageService.ShowMessage (parameters[0]);
			
		}
		
		public void Clear ()
		{
			showCompletion = false;
			parameters = new string[] {};
		}
		
		public int OverloadCount {
			get {
				return 1;
			}
		}
	}
}

