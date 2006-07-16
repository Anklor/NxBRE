// For useful output, start with: Login login.xml

namespace NxBRE.Examples
{
	using System;
	using System.Diagnostics;

	using net.ideaity.util;

	using NxBRE.FlowEngine;
	using NxBRE.FlowEngine.Factories;
	using NxBRE.FlowEngine.IO;
	using NxBRE.FlowEngine.Rules;
	
	public class Login
	{
		private bool lockLogin = false;
		
		// These var's are for setup
		private static bool SHOWSTACK = false;
		
		public Login(string aXMLFile, SourceLevels engineTraceLevel, SourceLevels ruleBaseTraceLevel)
		{
			try
			{
				IFlowEngine bre = new BREFactoryConsole(engineTraceLevel, ruleBaseTraceLevel).NewBRE(new XBusinessRulesFileDriver(aXMLFile));
				if (bre != null) {
					bre.ResultHandlers += new DispatchRuleResult(HandleBRERuleResult);
					Increment inc = new Increment();
					inc.Init(1);
					
					bre.RuleContext.SetFactory("Incrementor", inc);
					
					Console.Out.WriteLine("Attempt #1: Login locked: {0}", CheckLogin(bre));
					Console.Out.WriteLine("Attempt #2: Login locked: {0}", CheckLogin(bre));
					Console.Out.WriteLine("Attempt #3: Login locked: {0}", CheckLogin(bre));
					if (SHOWSTACK) Console.Out.WriteLine(bre.RuleContext.ToString());
				}
				else Console.Error.WriteLine("BRE init failed");
			}
			catch (System.Exception e)
			{
				Console.Error.WriteLine(e.ToString());
			}
		}
		
		/// <summary>This is what your code would call to check the login
		/// </summary>
		public virtual bool CheckLogin(IFlowEngine aBRE)
		{
			aBRE.Process();
			return lockLogin;
		}
		
		/// <summary>This is the method that is required by the listener interface.  It 
		/// will get called whenever the LoginResult rule is executed
		/// </summary>
		public virtual void  HandleBRERuleResult(object sender, IBRERuleResult aBRR)
		{
			if (aBRR.MetaData.Id.Equals("LoginResult")) lockLogin = true;
		}
		
		/// <summary>EVERYTHING FROM HERE DOWN IS JUST TO SET THINGS UP.....
		/// </summary>
		public static void Main(string[] args)
		{
			Arguments argOpt = new Arguments();
			
			SourceLevels engineTraceLevel = SourceLevels.Warning;
			SourceLevels ruleBaseTraceLevel = SourceLevels.Warning;

			argOpt.Usage = new string[]{"Usage",
																	"\tLogin (options) [xmlfile]",
																	"",
																	"\toptions:",
																	"\t  -s | -S  Turn ON/OFF RuleContext dump",
																	"\t  -e [System.Diagnostics.SourceLevels]  Set the Engine Trace Source Level",
																	"\t  -l [System.Diagnostics.SourceLevels]  Set the RuleBase Trace Source Level",
																	"\t  -h       This message"};
			
			
			if (args.Length == 0)	{
				argOpt.printUsage();
				System.Environment.Exit(1);
			}
			
			argOpt.parseArgumentTokens(args, new char[]{'e', 'l'});
			
			int c;
			while ((c = argOpt.getArguments()) != - 1)
			{
				switch (c)
				{
					case 's': 
						SHOWSTACK = true;
						break;
					
					case 'S': 
						SHOWSTACK = false;
						break;
					
					case 'e': 
						engineTraceLevel = (SourceLevels)Enum.Parse(typeof(SourceLevels), argOpt.StringParameter);
						break;
					
					case 'l': 
						ruleBaseTraceLevel = (SourceLevels)Enum.Parse(typeof(SourceLevels), argOpt.StringParameter);
						break;
					
					case 'h': 
						argOpt.printUsage();
						System.Environment.Exit(0);
						break;
					
					default: 
						break;
				}
			}
			
			Login bret = new Login(argOpt.getListFiles(), engineTraceLevel, ruleBaseTraceLevel);
		}
	}
}
