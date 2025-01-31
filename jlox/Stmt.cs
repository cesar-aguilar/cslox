using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jlox {

	internal abstract class Stmt {

		internal interface IVisitor {
			void VisitExpressionStmt(Expression stmt);
			void VisitPrintStmt(Print stmt);
		}
		// Expression
		internal class Expression : Stmt {

			public readonly Expr expression;

			internal Expression(Expr expression) {
				this.expression = expression;
			}

			internal override void Accept(IVisitor visitor) {
				visitor.VisitExpressionStmt(this);
			}

		}

		// Print
		internal class Print : Stmt {

			public readonly Expr expression;

			internal Print(Expr expression) {
				this.expression = expression;
			}

			internal override void Accept(IVisitor visitor) {
				visitor.VisitPrintStmt(this);
			}

		}

		internal abstract void Accept(IVisitor visitor);
		
	}

}
