using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
	// AstPrinter implements the visitor pattern to print the AST
	internal class AstPrinter : Expr.IVisitor<string>
	{
		// Print the AST
		internal string Print(Expr expr)
		{
			return expr.Accept(this);
		}

		// Visit Binary expression
		public string VisitBinaryExpr(Expr.Binary expr)
		{
			return Parenthesize(expr.Operator.lexeme, expr.Left, expr.Right);
		}

		// Visit Grouping expression
		public string VisitGroupingExpr(Expr.Grouping expr)
		{
			return Parenthesize("group", expr.Expression);
		}

		// Visit Literal expression
		public string VisitLiteralExpr(Expr.Literal expr)
		{
			if (expr.Value == null) return "nil";
			return expr.Value.ToString();
		}

		// Visit Unary expression
		public string VisitUnaryExpr(Expr.Unary expr)
		{
			return Parenthesize(expr.Operator.lexeme, expr.Right);
		}

		// Visit Variable expression
		public string VisitVariableExpr(Expr.Variable expr)
		{
			return expr.Name.lexeme;
		}

    // Visit Assign expression
    public string VisitAssignExpr(Expr.Assign expr)
    {
      return Parenthesize("assign " + expr.Name.lexeme, expr.Value);
    }

		// Visit Logical expression
		public string VisitLogicalExpr(Expr.Logical expr)
		{
			return Parenthesize(expr.Operator.lexeme, expr.Left, expr.Right);
    }

    // Helper method to parenthesize the expression
    private string Parenthesize(string name, params Expr[] exprs)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("(").Append(name);
			foreach (Expr expr in exprs)
			{
				builder.Append(" ");
				builder.Append(expr.Accept(this));
			}
			builder.Append(")");

			return builder.ToString();
		}

		// Main method to test the AstPrinter
		//public static void Main(string[] args)
		//{
		//	Expr expression = new Expr.Binary(
		//		new Expr.Unary(
		//			new Token(TokenType.MINUS, "-", null, 1),
		//			new Expr.Literal(123)),
		//		new Token(TokenType.STAR, "*", null, 1),
		//		new Expr.Grouping(
		//			new Expr.Literal(45.67)));

		//	Console.WriteLine(new AstPrinter().Print(expression));
		//	Console.ReadLine();
		//}
	}
}
