namespace org.nxbre
{
	using System;
	using System.Xml.XPath;
	
	using net.ideaity.util;

	using org.nxbre.rule;
	
	/// <summary>
	/// This interface defines the Flow Engine (FE) of NxBRE.
	/// </summary>
	/// <author>David Dossot</author>
	/// <version>2.5</version>
	public interface IFlowEngine : IInitializable, IBREDispatcher, ICloneable
		{
			/// <summary> Returns or Sets the RuleContext in it's current state.
			/// If the developer wishes to have a private copy, make sure
			/// to use Clone().
			/// This method allows developers to provide an already populated BRERuleContext.
			/// This is provided to allow for RuleFactories that have already been created, thus 
			/// allowing a more stateful RuleFactory
			/// </summary>
			/// <returns> The RuleContext in its current state</returns>
			IBRERuleContext RuleContext
			{
				get;
				
				set;
			}
			
			/// <summary> Returns the loaded XML Rules in the native NxBRE syntax
			/// </summary>
			/// <returns> The loaded XmlDocumentRules</returns>
			XPathDocument XmlDocumentRules
			{
				get;
			}
			
			/// <summary> Running state of the engine, i.e. when processing.
			/// </summary>
			/// <returns> True if the engine is processing. </returns>
			bool Running
			{
				get;
			}
			
			/// <summary> Execute the BRE.
			/// </summary>
			/// <returns> True if successful, False otherwise
			/// </returns>
			bool Process();
			
			/// <summary> Execute the BRE but only do all the globals and a certain set.
			/// </summary>
			/// <returns> True if successful, False otherwise
			/// </returns>
			bool Process(object aId);
			
			/// <summary> Violently stop the BRE 
			/// </summary>
			void Stop();
			
			/// <summary>Reset the context's call stack and results
			/// </summary>
			void Reset();
		}
}
