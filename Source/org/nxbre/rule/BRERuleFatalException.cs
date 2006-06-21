namespace org.nxbre.rule
{
	using System;
	/// <summary> This error is thrown when a severe error is generated from
	/// the Busines Rule (BRF), usually from the executeRule().
	/// If this error occurs within executeRule, any errors in a stack should
	/// be immediatly processed and an attempt to gracefully degradate the application
	/// should occur.
	/// *
	/// </summary>
	/// <author>  Sloan Seaman
	/// </author>
	/// <version>  1.0
	/// </version>
	public class BRERuleFatalException:BRERuleException
	{
		/// <summary> Defines a new BRERuleFatalException with a specific msg
		/// *
		/// </summary>
		/// <param name="aMsg">The error message
		/// 
		/// </param>
		public BRERuleFatalException(string aMsg):this(aMsg, null)
		{
		}
		
		/// <summary> Defines a new BRERuleFatalException with a specific msg,
		/// and the resultSet that may have been created (nullable)
		/// *
		/// </summary>
		/// <param name="aMsg">The error message
		/// </param>
		/// <param name="aBRR">The BusinessRuleResult that may have been created
		/// 
		/// </param>
		public BRERuleFatalException(string aMsg, IBRERuleResult aBRR):base(aMsg, aBRR)
		{
		}
	}
}
