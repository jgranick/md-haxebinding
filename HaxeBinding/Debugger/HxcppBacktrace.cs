using System;
using Mono.Debugging.Backend;
using Mono.Debugging.Client;

namespace MonoDevelop.HaxeBinding
{
	public class HxcppBacktrace : IBacktrace, IObjectValueSource
	{
		int fcount;
		//StackFrame firstFrame;
		HxcppDbgSession session;
		//DissassemblyBuffer[] disBuffers;
		//int currentFrame = -1;
		//long threadId;

		public HxcppBacktrace ()
		{
		}

		#region IObjectValueSource implementation

		public ObjectValue[] GetChildren (ObjectPath path, int index, int count, EvaluationOptions options)
		{
			throw new NotImplementedException ();
		}

		public EvaluationResult SetValue (ObjectPath path, string value, EvaluationOptions options)
		{
			throw new NotImplementedException ();
		}

		public ObjectValue GetValue (ObjectPath path, EvaluationOptions options)
		{
			throw new NotImplementedException ();
		}

		public object GetRawValue (ObjectPath path, EvaluationOptions options)
		{
			throw new NotImplementedException ();
		}

		public void SetRawValue (ObjectPath path, object value, EvaluationOptions options)
		{
			throw new NotImplementedException ();
		}

		#endregion

		#region IBacktrace implementation

		public StackFrame[] GetStackFrames (int firstIndex, int lastIndex)
		{
			throw new NotImplementedException ();
		}

		public ObjectValue[] GetLocalVariables (int frameIndex, EvaluationOptions options)
		{
			throw new NotImplementedException ();
		}

		public ObjectValue[] GetParameters (int frameIndex, EvaluationOptions options)
		{
			throw new NotImplementedException ();
		}

		public ObjectValue GetThisReference (int frameIndex, EvaluationOptions options)
		{
			throw new NotImplementedException ();
		}

		public ExceptionInfo GetException (int frameIndex, EvaluationOptions options)
		{
			throw new NotImplementedException ();
		}

		public ObjectValue[] GetAllLocals (int frameIndex, EvaluationOptions options)
		{
			throw new NotImplementedException ();
		}

		public ObjectValue[] GetExpressionValues (int frameIndex, string[] expressions, EvaluationOptions options)
		{
			throw new NotImplementedException ();
		}

		public CompletionData GetExpressionCompletionData (int frameIndex, string exp)
		{
			throw new NotImplementedException ();
		}

		public AssemblyLine[] Disassemble (int frameIndex, int firstLine, int count)
		{
			throw new NotImplementedException ();
		}

		public ValidationResult ValidateExpression (int frameIndex, string expression, EvaluationOptions options)
		{
			throw new NotImplementedException ();
		}

		public int FrameCount {
			get {
				throw new NotImplementedException ();
			}
		}

		#endregion
	}
}

