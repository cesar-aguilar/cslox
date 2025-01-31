using System.Collections.Generic;
using System;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace jlox
{
	// Interpreter class implements the visitor pattern
	internal class Interpreter : Expr.IVisitor<Object>, Stmt.IVisitor
	{

		public void Interpret(List<Stmt> Statements)
		{
			try
			{
				foreach (Stmt statement in Statements)
				{
					execute(statement);
				}
			}
			catch (RunTimeError error)
			{
				Lox.RunTimeError(error);
			}
		
		}

		public Object VisitLiteralExpr(Expr.Literal expr)
		{
			return expr.Value;
		}

		public Object VisitGroupingExpr(Expr.Grouping expr)
		{
			return evaluate(expr.Expression);
		}

		public Object VisitUnaryExpr(Expr.Unary expr)
		{
			Object right = evaluate(expr.Right);

			switch (expr.Operator.type)
			{
				case TokenType.BANG:
					return !isTruthy(right);

				case TokenType.MINUS:
					checkNumberOperand(expr.Operator, right);
					return -(double)right;
			}


			return null;

		}

		public Object VisitBinaryExpr(Expr.Binary expr)
		{
			Object left = evaluate(expr.Left);
			Object right = evaluate(expr.Right);

			switch (expr.Operator.type)
			{

				// Comparison
				case TokenType.GREATER:
					checkNumberOperands(expr.Operator, left, right);
					return (double)left > (double)right;

				case TokenType.GREATER_EQUAL:
					checkNumberOperands(expr.Operator, left, right);
					return (double)left >= (double)right;

				case TokenType.LESS:
					checkNumberOperands(expr.Operator, left, right);
					return (double)left < (double)right;

				case TokenType.LESS_EQUAL:
					checkNumberOperands(expr.Operator, left, right);
					return (double)left <= (double)right;

				// Arithmetic
				case TokenType.MINUS:
					checkNumberOperands(expr.Operator, left, right);
					return (double)left - (double)right;

				case TokenType.SLASH:
					checkNumberOperands(expr.Operator, left, right);
					return (double)left / (double)right;

				case TokenType.STAR:
					checkNumberOperands(expr.Operator, left, right);
					return (double)left * (double)right;

				case TokenType.PLUS:
					if (left is double && right is double)
					{
						return (double)left + (double)right;
					}
					if (left is string && right is string)
					{
						return (string)left + (string)right;
					}
					throw new RunTimeError(expr.Operator, "Operands must be of the same type.");

				// Equality
				case TokenType.BANG_EQUAL:
					return !isEqual(left, right);
				case TokenType.EQUAL_EQUAL:
					return isEqual(left, right);
			}

			return null;

		}

		public void VisitExpressionStmt(Stmt.Expression stmt)
		{
			evaluate(stmt.expression);
		}

		public void VisitPrintStmt(Stmt.Print stmt)
		{
			Object value = evaluate(stmt.expression);
			Console.WriteLine(Stringify(value));
		}

		private Object evaluate(Expr expr)
		{
			return expr.Accept(this);
		}

		private void execute(Stmt stmt)
		{
			stmt.Accept(this);
		}

		private bool isTruthy(Object obj)
		{
			if (obj == null) return false;
			if (obj is bool) return (bool)obj;
			return true;
		}

		private bool isEqual(Object a, Object b)
		{
			if (a == null && b == null) return true;
			if (a == null) return false;

			return a.Equals(b);
		}

		private void checkNumberOperand(Token op, Object operand)
		{
			if (operand is double) return;
			throw new RunTimeError(op, "Operand must be a number.");
		}

		private void checkNumberOperands(Token op, Object left, Object right)
		{
			if (left is double && right is double) return;
			throw new RunTimeError(op, "Operands must be numbers.");
		}

		private string Stringify(Object obj)
		{
			if (obj == null) return "nil";

			if (obj is double)
			{
				string text = obj.ToString();
				if (text.EndsWith(".0"))
				{
					text = text.Substring(0, text.Length - 2);
				}
				return text;
			}

			return obj.ToString();
		}

	}
}
