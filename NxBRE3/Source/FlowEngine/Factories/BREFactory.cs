namespace NxBRE.FlowEngine.Factories
{
	using System;
	
	using net.ideaity.util.events;
	
	using NxBRE.FlowEngine;
	using NxBRE.FlowEngine.IO;
	
	/// <summary>This factory allows an easy creation of a BRE object</summary>
	/// <author>David Dossot</author>
	public class BREFactory {
		private DispatchException exceptionHandler;
		private DispatchLog logHandler;
		private DispatchRuleResult resultHandler;
		                         
		public BREFactory():this(null, null, null) {}
		
		public BREFactory(DispatchException exceptionHandler):this(exceptionHandler, null, null) {}
		
		public BREFactory(DispatchException exceptionHandler,
		                  DispatchLog logHandler):	this(exceptionHandler, logHandler, null) {}
		
		public BREFactory(DispatchException exceptionHandler,
	                    DispatchLog logHandler,
	                    DispatchRuleResult resultHandler) {
			this.exceptionHandler = exceptionHandler;
			this.logHandler = logHandler;
		  this.resultHandler = resultHandler;
		}
		
		public virtual IFlowEngine NewBRE(IRulesDriver rulesDriver) {
			BREImpl bre = new BREImpl();
			
			// Lets register handlers
			if (logHandler != null) bre.LogHandlers += logHandler;
			if (exceptionHandler != null) bre.ExceptionHandlers += exceptionHandler;
			if (resultHandler != null) bre.ResultHandlers += resultHandler;

			if (!bre.Init(rulesDriver)) return null;
			else return bre;
		}
	}
	
}
