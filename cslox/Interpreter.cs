using System.Collections.Generic;
using System;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
	// Interpreter class implements the visitor pattern
	internal class Interpreter : Expr.IVisitor<Object>, Stmt.IVisitor
	{

		private LoxEnvironment globals = new LoxEnvironment();
		private LoxEnvironment environment;

		
		public Interpreter()
    {
      
      // Define native functions here, e.g., clock
      globals.Define("clock", new NativeLoxCallables.Clock());

      environment = globals;

    }

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

    // ==============================================
    // Visitor methods for expressions
    // ==============================================

    public Object VisitLiteralExpr(Expr.Literal expr)
		{
			return expr.Value;
		}

		public Object VisitLogicalExpr(Expr.Logical expr)
		{
			Object left = evaluate(expr.Left);
			
			if (expr.Operator.type == TokenType.OR)
			{
				return isTruthy(left);
      } else
			{
				if (!isTruthy(left)) return false;
      }

			return isTruthy(evaluate(expr.Right));
		
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

    public Object VisitVariableExpr(Expr.Variable expr)
    {
      return environment.Get(expr.Name);
    }

    public Object VisitAssignExpr(Expr.Assign expr)
		{
			Object value = evaluate(expr.Value);
      environment.Assign(expr.Name, value);
			return value;

    }

		public Object VisitCallExpr(Expr.Call expr)
		{

			Object callee = evaluate(expr.Callee);

      List<Object> arguments = new List<Object>();

			foreach (Expr argument in expr.Arguments)
			{
				arguments.Add(evaluate(argument));
			}

      if (!(callee is ILoxCallable))
      {
        throw new RunTimeError(expr.Paren, "Can only call functions and classes.");
      }

      ILoxCallable function = (ILoxCallable)callee;

			if (arguments.Count() != function.Arity())
      {
        throw new RunTimeError(expr.Paren, "Expected " + function.Arity() + " arguments but got " + arguments.Count() + ".");
      }

      return function.Call(this, arguments);

    }

    // ==============================================
    // Visitor methods for statements
    // ==============================================

    public void VisitExpressionStmt(Stmt.Expression stmt)
		{
			evaluate(stmt.expression);
		}

		public void VisitPrintStmt(Stmt.Print stmt)
		{
			Object value = evaluate(stmt.expression);
			Console.WriteLine(Stringify(value));
		}

		public void VisitVarStmt(Stmt.Var stmt)
		{
			Object value = null;
			if (stmt.initializer != null)
			{
				value = evaluate(stmt.initializer);
			}

			environment.Define(stmt.name.lexeme, value);
		}
	
		public void VisitBlockStmt(Stmt.Block stmt)
		{
			ExecuteBlock(stmt.statements, new LoxEnvironment(environment));
			return;
    }

		public void VisitIfStmt(Stmt.If stmt)
    {
      if (isTruthy(evaluate(stmt.condition)))
      {
        execute(stmt.thenBranch);
      }
      else if (stmt.elseBranch != null)
      {
        execute(stmt.elseBranch);
      }

			return;
    }

		public void VisitWhileStmt(Stmt.While stmt)
		{
			while (isTruthy(evaluate(stmt.condition)))
			{
				execute(stmt.body);
			}

			return;
		}

    // ==============================================
    // Helper methods
    // ==============================================

    private Object evaluate(Expr expr)
		{
			return expr.Accept(this);
		}

		private void execute(Stmt stmt)
		{
			stmt.Accept(this);
		}

		private void ExecuteBlock(List<Stmt> statements, LoxEnvironment env)
    {
      LoxEnvironment previous = this.environment;

      try
      {
        this.environment = env;

        foreach (Stmt statement in statements)
        {
          execute(statement);
        }
      }
      finally
      {
        this.environment = previous;
      }
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
