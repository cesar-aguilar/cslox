using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox {

	internal abstract class Expr {

		internal interface IVisitor<R> {
			R VisitBinaryExpr(Binary expr);
			R VisitGroupingExpr(Grouping expr);
			R VisitLiteralExpr(Literal expr);
			R VisitUnaryExpr(Unary expr);
			R VisitVariableExpr(Variable expr);
      R VisitAssignExpr(Assign expr);
			R VisitLogicalExpr(Logical expr);
      R VisitCallExpr(Call expr);
    }

		// Assignment
		internal class Assign : Expr
    {

      public readonly Token Name;
      public readonly Expr Value;

      internal Assign(Token Name, Expr Value)
      {
        this.Name = Name;
        this.Value = Value;
      }

      internal override R Accept<R>(IVisitor<R> visitor)
      {
        return visitor.VisitAssignExpr(this);
      }

    }

    // Binary
    internal class Binary : Expr {

			public readonly Expr Left;
			public readonly Token Operator;
			public readonly Expr Right;

			internal Binary(Expr Left, Token Operator, Expr Right) {
				this.Left = Left;
				this.Operator = Operator;
				this.Right = Right;
			}

			internal override R Accept<R>(IVisitor<R> visitor) {
				return visitor.VisitBinaryExpr(this);
			}

		}

		// Logical
		internal class Logical : Expr
		{
			public readonly Expr Left;
			public readonly Token Operator;
			public readonly Expr Right;

			internal Logical(Expr Left, Token Operator, Expr Right)
			{
				this.Left = Left;
				this.Operator = Operator;
				this.Right = Right;
			}

      internal override R Accept<R>(IVisitor<R> visitor)
      {
        return visitor.VisitLogicalExpr(this);
      }

    }

    // Grouping
    internal class Grouping : Expr {

			public readonly Expr Expression;

			internal Grouping(Expr Expression) {
				this.Expression = Expression;
			}

			internal override R Accept<R>(IVisitor<R> visitor) {
				return visitor.VisitGroupingExpr(this);
			}

		}

		// Literal
		internal class Literal : Expr {

			public readonly object Value;

			internal Literal(object Value) {
				this.Value = Value;
			}

			internal override R Accept<R>(IVisitor<R> visitor) {
				return visitor.VisitLiteralExpr(this);
			}

		}

		// Unary
		internal class Unary : Expr {

			public readonly Token Operator;
			public readonly Expr Right;

			internal Unary(Token Operator, Expr Right) {
				this.Operator = Operator;
				this.Right = Right;
			}

			internal override R Accept<R>(IVisitor<R> visitor) {
				return visitor.VisitUnaryExpr(this);
			}

		}

		// Variable
		internal class Variable : Expr {

			public readonly Token Name;

			internal Variable(Token Name) {
				this.Name = Name;
			}

			internal override R Accept<R>(IVisitor<R> visitor) {
				return visitor.VisitVariableExpr(this);
			}

		}

		// Function Call
		internal class Call : Expr
    {
      public readonly Expr Callee;
      public readonly Token Paren;
      public readonly List<Expr> Arguments;

      internal Call(Expr Callee, Token Paren, List<Expr> Arguments)
      {
        this.Callee = Callee;
        this.Paren = Paren;
        this.Arguments = Arguments;
      }

      internal override R Accept<R>(IVisitor<R> visitor)
      {
				return visitor.VisitCallExpr(this);
      }

    }

    internal abstract R Accept<R>(IVisitor<R> visitor);
	}

}
