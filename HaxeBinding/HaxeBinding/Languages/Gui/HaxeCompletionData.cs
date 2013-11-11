using System;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Core;

namespace MonoDevelop.HaxeBinding
{
	public class HaxeCompletionData : CompletionData
	{
		public HaxeCompletionData (string text, IconId icon, string description) : base(text, icon, description)
		{
		}

		public override TooltipInformation CreateTooltipInformation (bool smartWrap)
		{
			TooltipInformation tmpTooltip = new TooltipInformation ();
			tmpTooltip.SummaryMarkup = Description;
			tmpTooltip.SignatureMarkup = "<b>" + DisplayText + "</b>";
			return tmpTooltip;
		}
	}
}

