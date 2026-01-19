using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
	public class Lox
	{
		static bool hadError = false;
		static bool hadRunTimeError = false;
		static readonly Interpreter interpreter = new Interpreter();

		public static void Main(string[] args)
		{
			if (args.Length > 1)
			{
				Console.WriteLine("Usage: jlox [script]");
				Environment.Exit(64);
			}
			else if (args.Length == 1)
			{
				RunFile(args[0]);
			}
			else
			{
				RunPrompt();
			}
		}

		private static void RunFile(string path)
		{

			if (!File.Exists(path))
			{
				Console.WriteLine("File not found: " + path);
				Environment.Exit(66);
			}

			byte[] bytes = File.ReadAllBytes(path);

			Run(Encoding.UTF8.GetString(bytes));

			if (hadError) Environment.Exit(65);
			if (hadRunTimeError) Environment.Exit(70);

		}

		private static void RunPrompt()
		{
			Console.WriteLine("Welcome to jlox!");
			Console.WriteLine("To exit, type Ctrl-X and ENTER.\n");

			for (; ; )
			{				
				Console.Write("> ");
				string line = Console.ReadLine();

				// If Ctrl-X then break
				if (line == "\x18") break;

				Run(line);

				hadError = false;

			}

		}

		private static void Run(string source)
		{

			Scanner scanner = new Scanner(source);
			List<Token> tokens = scanner.ScanTokens();

			//foreach (Token token in tokens)
			//{
			//	Console.WriteLine(token);
			//}

			Parser parser = new Parser(tokens);

			List<Stmt> Statements = parser.Parse();

			if (hadError) return;

			interpreter.Interpret(Statements);
			
		}

		public static void Error(int line, string message)
		{
			Report(line, "", message);
		}

		private static void Report(int line, string where, string message)
		{
			Console.WriteLine("[line " + line + "] Error" + where + ": " + message);
			hadError = true;
		}

		public static void Error(Token token, string message)
		{
			if (token.type == TokenType.EOF)
			{
				Report(token.line, " at end", message);
			}
			else
			{
				Report(token.line, " at '" + token.lexeme + "'", message);
			}
		}

		internal static void RunTimeError(RunTimeError error)
		{
			Console.WriteLine(error.Message + "\n[line " + error.token.line + "]");
			hadRunTimeError = true;
		}
	}
}
