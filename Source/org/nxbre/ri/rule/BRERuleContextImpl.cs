namespace org.nxbre.ri.rule
{
	using System;
	using System.Collections;
	
	using org.nxbre.rule;
	/// <summary> Strict implementation of BRERuleContext.
	/// <P>
	/// This class is sealed so that no one tries to extend it. 
	/// Implementation Inheritance can be a dangerous thing.
	/// </P>
	/// <P>
	/// If a developer wishes to extend this in some way they should use
	/// AbstractBRERuleContext instead since it is designed to be added on to.
	/// </P>
	/// *
	/// </summary>
	/// <seealso cref="org.nxbre.rule.AbstractBRERuleContext">
	/// </seealso>
	/// <P>
	/// <PRE>
	/// CHANGELOG:
	/// v1.6	- Removed param methods.  Changed Iterator
	/// methods to return Maps
	/// *
	/// </PRE>
	/// </P>
	/// <author>  Sloan Seaman </author>
	/// <author>  David Dossot </author>
	/// <version>  1.8.1 </version>
	public sealed class BRERuleContextImpl:AbstractBRERuleContext
	{
		/// <summary> Creates a new instance of the object
		/// <P> 
		/// This constructor makes a call to a protected constructor
		/// within AbstractBRERuleContext.  This is another reason
		/// why this class is sealed, because this technically breaks
		/// encapsulation.  For more info see:</P><P>
		/// <I>Effective Java</I> (first printing) by Joshua Bloch.
		/// Item 14, pg. 71
		/// </P>
		/// *
		/// </summary>
		/// <param name="aStack">The Stack for the call stack
		/// </param>
		/// <param name="aFactories">The Map for the RuleFactory's
		/// </param>
		/// <param name="aOperators">The Map for the Operators
		/// </param>
		/// <param name="aResults">The Map for the RuleResults
		/// </param>
		public BRERuleContextImpl(Stack aStack, Hashtable aFactories, Hashtable aOperators, Hashtable aResults):base(aStack, aFactories, aOperators, aResults)
		{
		}
		
		/// <summary> Performs a shallow copy of the Rule Context, i.e. returns a new RuleContext
		/// containing shallow copies of its internal hashtables and stack
		/// </summary>
		public override object Clone() {
			return new BRERuleContextImpl((Stack)internalCallStack.Clone(),
			                              (Hashtable)factories.Clone(),
			                              (Hashtable)operators.Clone(),
			                              (Hashtable)results.Clone());
		}
		
		/// <summary> Sets a business object
		/// *
		/// </summary>
		/// <param name="aId">The UID of the business object
		/// </param>
		/// <param name="aObject">The business object
		/// 
		/// </param>
		public override void SetObject(object aId, object aObject)
		{
			SetResult(aId, new BRERuleObject(aObject));
		}

		/// <summary> Returns a business object
		/// *
		/// </summary>
		/// <param name="aId">The UID of the business object
		/// </param>
		/// <returns> The requested business object
		/// 
		/// </returns>
		public override object GetObject(object aId) {
			object aObject = GetResult(aId);
			if (aObject != null) aObject = ((IBRERuleResult)aObject).Result;
			return aObject;			
		}
	}
}
