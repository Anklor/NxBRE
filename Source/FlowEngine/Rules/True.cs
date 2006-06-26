namespace NxBRE.FlowEngine.Rules
{
	using System;
	using System.Collections;
	
	using NxBRE.FlowEngine;
	
	/// <summary> This rule will always return true.
	/// *
	/// </summary>
	/// <P>
	/// <PRE>
	/// CHANGELOG:
	/// v1.0	- Created
	/// v1.01	- Removed setRuleParams and instead added a Map param to
	/// executeRule as required in the new BRERuleFactory
	/// Removed throws from executeRule
	/// v1.5	- Renamed and moved to different package
	/// v1.6	- Made final
	/// </PRE>
	/// </P>
	/// <author>  Sloan Seaman
	/// </author>
	public sealed class True : IBRERuleFactory
	{
		
		/// <summary> Always returns true
		/// *
		/// </summary>
		/// <param name="aBrc">- The BRERuleContext object
		/// </param>
		/// <param name="aMap">- The IDictionary of parameters from the XML
		/// </param>
		/// <param name="aStep">- The step that it is on
		/// </param>
		/// <returns> Boolean.TRUE
		/// 
		/// </returns>
		public object ExecuteRule(IBRERuleContext aBrc, IDictionary aMap, object aStep)
		{
			return true;
		}
	}
}
