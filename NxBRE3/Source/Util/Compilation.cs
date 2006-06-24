namespace NxBRE.Util
{
	using Microsoft.CSharp;
	using Microsoft.VisualBasic;

	using System;
	using System.Collections;
	using System.IO;
	using System.CodeDom.Compiler;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Text.RegularExpressions;
	
	/// <summary>
	/// An evaluator that receives its arguments as key/value pairs.
	/// </summary>
	public interface IDictionaryEvaluator {
		object Run(IDictionary arguments);
	}

	/// <summary>
	/// An evaluator that receives its arguments as a list of values, whose position is important.
	/// </summary>
	public interface IListEvaluator {
		object Run(IList values);
	}

	/// <summary>
	/// An evaluator that takes no arguments.
	/// </summary>
	public interface IEvaluator {
		object Run();
	}
	
	/// <summary>NxBRE utilities for on-the-fly compiling C# code</summary>
	/// <author>David Dossot</author>
	/// <version>2.4</version>
	public abstract class Compilation {
		private static CodeDomProvider csCodeProvider = new CSharpCodeProvider();
		private static CodeDomProvider vbCodeProvider = new VBCodeProvider();

		private static string nxbreAssemblyLocation = String.Empty;
		private const string NXBRE_DLL = "NxBRE.dll";
		
		private static ReferenceLinkModes referenceLinkMode = ReferenceLinkModes.Full;
		
		private Compilation() {}
		
		/// <summary>
		/// Defines the different strategies for adding references when on-the-fly compiling classes.
		/// </summary>
		public enum ReferenceLinkModes {
			/// <summary>
			/// No references will be added.
			/// </summary>
			None,
			/// <summary>
			/// Only a reference to NxBRE.dll will be added.
			/// </summary>
			NxBRE,
			/// <summary>
			/// References to all assemblies of the current domain will be added.
			/// </summary>
			CurrentDomain,
			/// <summary>
			/// References to NxBRE.dll and all assemblies of the current domain will be added.
			/// </summary>
			Full};

		/// <summary>
		/// Gets or sets the active reference link mode. Default is Full.
		/// </summary>
		public static ReferenceLinkModes ReferenceLinkMode {
			get {
				return referenceLinkMode;
			}
			set {
				referenceLinkMode = value;
			}
		}

		/// <summary>
		/// Performs an immediate evaluation of an expression that takes no argument.
		/// </summary>
		/// <remarks>
		/// The compiled expression is not cached.
		/// </remarks>
		/// <param name="expression">The C# expression to evaluate.</param>
		/// <returns>An object produced by the expression.</returns>
		public static object Evaluate(string expression) {
			string code = "class Evaluator:org.nxbre.util.IEvaluator { public object Run() {return ("
										+ PrepareExpression(expression)
										+ ");}}";
			
			return ((IEvaluator)LoadCSClass("Evaluator", code, true)).Run();
			
		}

		/// <summary>
		/// Performs an immediate evaluation of an expression that takes lists for parameter names and values.
		/// </summary>
		/// <remarks>
		/// The compiled expression is not cached.
		/// </remarks>
		/// <param name="expression">The C# expression to evaluate.</param>
		/// <param name="placeHolderRegexpPattern">The regexp used to recoginize the argument placeholders.</param>
		/// <param name="variableNames">The list of argument names.</param>
		/// <param name="values">The list of values.</param>
		/// <returns>An object produced by the expression.</returns>
		public static object Evaluate(string expression, string placeHolderRegexpPattern, IList variableNames, IList values) {
			return NewEvaluator(expression, placeHolderRegexpPattern, variableNames, values).Run(values);
		}
		
		/// <summary>
		/// Instantiates a new evaluator that receives its arguments as a list of values.
		/// </summary>
		/// <param name="expression">The C# expression to compile.</param>
		/// <param name="placeHolderRegexpPattern">The regexp used to recoginize the argument placeholders.</param>
		/// <param name="variableNames">The list of argument names.</param>
		/// <param name="values">The list of values.</param>
		/// <returns>The compiled evaluator.</returns>
		public static IListEvaluator NewEvaluator(string expression, string placeHolderRegexpPattern, IList variableNames, IList values) {
			string code = "class Evaluator:org.nxbre.util.IListEvaluator { public object Run(System.Collections.IList values) {return ("
										+ Regex.Replace(PrepareExpression(expression), placeHolderRegexpPattern, new MatchEvaluator(new ListVariableReplacer(variableNames, values).ReplaceListVariable))
										+ ");}}";
			
			return (IListEvaluator)LoadCSClass("Evaluator", code, true);
		}
	
		/// <summary>
		/// Performs an immediate evaluation of an expression that takes key/value pairs as arguments.
		/// </summary>
		/// <remarks>
		/// The compiled expression is not cached.
		/// </remarks>
		/// <param name="expression">The C# expression to evaluate.</param>
		/// <param name="placeHolderRegexpPattern">The regexp used to recoginize the argument placeholders.</param>
		/// <param name="arguments">The key/value pairs of arguments.</param>
		/// <returns>An object produced by the expression.</returns>
		public static object Evaluate(string expression, string placeHolderRegexpPattern, IDictionary arguments) {
			return Evaluate(expression, placeHolderRegexpPattern, null, arguments);
		}
	
		/// <summary>
		/// Performs an immediate evaluation of an expression that takes key/value pairs as arguments.
		/// </summary>
		/// <remarks>
		/// The compiled expression is not cached.
		/// </remarks>
		/// <param name="expression">The C# expression to evaluate.</param>
		/// <param name="placeHolderRegexpPattern">The regexp used to recoginize the argument placeholders.</param>
		/// <param name="numericArgumentPattern">A regexp pattern used to recognize placeholders for arguments whose names are integers and not strings. Use null if not needed.</param>
		/// <param name="arguments">The key/value pairs of arguments.</param>
		/// <returns>An object produced by the expression.</returns>
		public static object Evaluate(string expression, string placeHolderRegexpPattern, string numericArgumentPattern, IDictionary arguments) {
			return NewEvaluator(expression, placeHolderRegexpPattern, numericArgumentPattern, arguments).Run(arguments);
		}
		
		/// <summary>
		/// Instantiates a new evaluator that receives its arguments as key/value pairs.
		/// </summary>
		/// <param name="expression">The C# expression to evaluate.</param>
		/// <param name="placeHolderRegexpPattern">The regexp used to recoginize the argument placeholders.</param>
		/// <param name="arguments">The key/value pairs of arguments.</param>
		/// <returns>The compiled evaluator.</returns>
		public static IDictionaryEvaluator NewEvaluator(string expression, string placeHolderRegexpPattern,  IDictionary arguments) {
			return NewEvaluator(expression, placeHolderRegexpPattern, null, arguments);
		}

		/// <summary>
		/// Instantiates a new evaluator that receives its arguments as key/value pairs.
		/// </summary>
		/// <param name="expression">The C# expression to evaluate.</param>
		/// <param name="placeHolderRegexpPattern">The regexp used to recoginize the argument placeholders.</param>
		/// <param name="numericArgumentPattern">A regexp pattern used to recognize placeholders for arguments whose names are integers and not strings. Use null if not needed.</param>
		/// <param name="arguments">The key/value pairs of arguments.</param>
		/// <returns>The compiled evaluator.</returns>
		public static IDictionaryEvaluator NewEvaluator(string expression, string placeHolderRegexpPattern, string numericArgumentPattern, IDictionary arguments) {
			string code = "class Evaluator:org.nxbre.util.IDictionaryEvaluator { public object Run(System.Collections.IDictionary values) {return ("
				+ Regex.Replace(PrepareExpression(expression), placeHolderRegexpPattern, new MatchEvaluator(new DictionaryVariableReplacer(arguments, numericArgumentPattern).ReplaceDictionaryVariable))
										+ ");}}";
			
			return (IDictionaryEvaluator)LoadCSClass("Evaluator", code, true);
		}
	
		// ----- INTERNAL MEMBERS -----------------------------------------------
		
		internal static object LoadCSClass(string targetClassName, string source, bool sourceIsString) {
			return LoadClass(csCodeProvider, targetClassName, source, sourceIsString);
		}

		internal static object LoadVBClass(string targetClassName, string source, bool sourceIsString) {
			return LoadClass(vbCodeProvider, targetClassName, source, sourceIsString);
		}
	
		// ----- PRIVATE MEMBERS -----------------------------------------------
		
		private class ListVariableReplacer {
			private readonly IList variableNames;
			private readonly IList values;
			
			public ListVariableReplacer(IList variableNames, IList values) {
				this.variableNames = variableNames;
				this.values = values;
			}
			
			public string ReplaceListVariable(Match m) {
				int variableIndex = variableNames.IndexOf(m.Groups[1].Value);
				
				if (variableIndex >= values.Count) throw new BREException("Not enough values to resolve expression: missing index " + variableIndex);
				
				return "((" +
							 values[variableIndex].GetType().FullName +
							 ")values[" + variableIndex + "])";
			}		
		}

		private class DictionaryVariableReplacer {
			private readonly IDictionary arguments;
			private readonly Regex numericArgumentRegex;
			
			public DictionaryVariableReplacer(IDictionary arguments, string numericArgumentPattern) {
				this.arguments = arguments;
				if (numericArgumentPattern != null)	numericArgumentRegex = new Regex(numericArgumentPattern);
				else numericArgumentRegex = null;
			}
			
			public string ReplaceDictionaryVariable(Match m) {
				if ((numericArgumentRegex != null) && (numericArgumentRegex.IsMatch(m.Groups[0].Value))) {
					int variableName = Convert.ToInt32(m.Groups[1].Value);
					
					if (!arguments.Contains(variableName)) throw new BREException("Not enough arguments to resolve expression: missing " + variableName);
					
					return "((" +
								 arguments[variableName].GetType().FullName +
								 ")values[" + variableName + "])";
				} else {
					string variableName = m.Groups[1].Value;
					
					if (!arguments.Contains(variableName)) throw new BREException("Not enough arguments to resolve expression: missing '" + variableName + "'");
					
					return "((" +
								 arguments[variableName].GetType().FullName +
								 ")values[\"" + variableName + "\"])";
				}
			}
		}
		
		private static string PrepareExpression(string expression) {
			if (expression.StartsWith("expr:")) return expression.Substring(5);
			else return expression;
		}

		///<remarks>Brendan Ingram has greatly improved this method.</remarks>		
		private static object LoadClass(CodeDomProvider codeProvider, string targetClassName, string source, bool sourceIsString) {
			CompilerParameters compilerParameters = new CompilerParameters();
			compilerParameters.GenerateExecutable = false;
			compilerParameters.GenerateInMemory = true;
			compilerParameters.IncludeDebugInformation = true;
			compilerParameters.TreatWarningsAsErrors = false;
			
			// Add the reference to the NxBRE.dll assembly.
			if ((ReferenceLinkMode == ReferenceLinkModes.NxBRE) || (ReferenceLinkMode == ReferenceLinkModes.Full))
				AddReferencedAssembly(compilerParameters, NxbreAssemblyLocation);
		
			// Add all implicitly referenced assemblies
			if ((ReferenceLinkMode == ReferenceLinkModes.CurrentDomain) || (ReferenceLinkMode == ReferenceLinkModes.Full))
				foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
					// do not add AssemblyBuilders (bug 1482753), thanks to Bob Brumfield
					if (!(assembly is AssemblyBuilder))
						AddReferencedAssembly(compilerParameters, assembly.Location);
			
			ICodeCompiler compiler = codeProvider.CreateCompiler();
			CompilerResults cr;
			
			if (sourceIsString)
				cr = compiler.CompileAssemblyFromSource(compilerParameters, source);
			else
				cr = compiler.CompileAssemblyFromFile(compilerParameters, source);
			
			if (cr.Errors.Count == 0)
				return cr.CompiledAssembly.CreateInstance(targetClassName);
			else
				throw new BREException(GetCompilerErrors(cr));
		}
		
		//--------- Private Methods ---------------
		
		private static void AddReferencedAssembly(CompilerParameters compilerParameters, string assemblyLocation) {
			try	{
				if (compilerParameters.ReferencedAssemblies.IndexOf(assemblyLocation) < 0)
					compilerParameters.ReferencedAssemblies.Add(assemblyLocation);
			}
			catch {
				// Ignore any error
			}
		}
		
		private static string NxbreAssemblyLocation {
			get {
				lock(nxbreAssemblyLocation) {
					
					if (nxbreAssemblyLocation != String.Empty) return nxbreAssemblyLocation;
					
					// ------------------------------------------------------------------------------
					// Find the NxBRE.dll assembly and add reference - the assembly could be found
					// in one of many different locations, so search in all possible locations.
					// ------------------------------------------------------------------------------
					
					// Look in the current directory.
					nxbreAssemblyLocation = NXBRE_DLL;
					if (File.Exists(nxbreAssemblyLocation)) return nxbreAssemblyLocation;
					
					// Look in the app domains base directory.
					nxbreAssemblyLocation = AppDomain.CurrentDomain.BaseDirectory + NXBRE_DLL;
					if (File.Exists(nxbreAssemblyLocation)) return nxbreAssemblyLocation;
					
					// Look in the bin subdirectory of the app domain base directory (ASP.NET)
					nxbreAssemblyLocation = AppDomain.CurrentDomain.BaseDirectory + @"bin/" + NXBRE_DLL;
					if (File.Exists(nxbreAssemblyLocation)) return nxbreAssemblyLocation;
					
					throw new BREException(NXBRE_DLL + " is impossible to find");
				}
			}
		}
		
		private static string GetCompilerErrors(CompilerResults cr) {
			string errors = "Compiler returned with result code: " +
											cr.NativeCompilerReturnValue.ToString() +
											";\n";
			
	    // If errors occurred during compilation, output the compiler output and errors.
	    foreach(CompilerError ce in cr.Errors)
	    	if (!(ce.IsWarning))
	    		errors += ("CompilerError::" + ce.ToString() + ";\n");

			return errors;
		}

	}
}

