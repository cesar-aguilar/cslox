using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jlox {

	internal abstract class Expr {

		internal interface IVisitor<R> {
			R VisitBinaryExpr(Binary expr);
			R VisitGroupingExpr(Grouping expr);
			R VisitLiteralExpr(Literal expr);
			R VisitUnaryExpr(Unary expr);
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

		internal abstract R Accept<R>(IVisitor<R> visitor);
	}

}
