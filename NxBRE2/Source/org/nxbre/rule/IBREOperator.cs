namespace org.nxbre.rule
{
	using System;
	using System.Collections;
	/// <summary> This interface defines the comparison methods used by operators from
	/// within the Compare tag
	/// </summary>
	/// <P>
	/// <PRE>
	/// CHANGELOG:
	/// v1.6.1	- Now can throw exceptions
	/// v1.5	- Created
	/// </PRE>
	/// </P>
	/// <author>  Sloan Seaman
	/// </author>
	/// <version>  1.6.1
	/// </version>
	public interface IBREOperator
		{
			/// <summary> Method to compare one object against another
			/// *
			/// </summary>
			/// <param name="aBRC">The BRERuleContext object containing all the state
			/// information for use by this method.
			/// </param>
			/// <param name="aMap">A Map object containing the parameters from the XML
			/// </param>
			/// <param name="aObj">The object (value) to compare against.
			/// </param>
			/// <param name="aCompareTo">The object to compare it to.
			/// </param>
			/// <returns>s True if they meet the condition, False otherwise.
			/// 
			/// </returns>
			bool ExecuteComparison(IBRERuleContext aBRC, Hashtable aMap, object aObj, object aCompareTo);
			
			/// <summary>Defines whether an operator is ablt to deal with empty operands</summary>
			/// <returns>True or False depending of the operator.</returns>
			bool AcceptsNulls {
				get ;
			}
		}
}

