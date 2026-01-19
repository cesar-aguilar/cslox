using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox {

	internal abstract class Stmt {

		internal interface IVisitor {
			void VisitExpressionStmt(Expression stmt);
			void VisitPrintStmt(Print stmt);
			void VisitVarStmt(Var stmt);
			void VisitBlockStmt(Block stmt);
      void VisitIfStmt(If stmt);
      void VisitWhileStmt(While stmt);
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

		// Var
		internal class Var : Stmt {

			public readonly Token name;
			public readonly Expr initializer;

			internal Var(Token name, Expr initializer) {
				this.name = name;
				this.initializer = initializer;
			}

			internal override void Accept(IVisitor visitor) {
				 visitor.VisitVarStmt(this);
			}

		}

		// Block
		internal class Block : Stmt
		{

			public readonly List<Stmt> statements;

			public Block(List<Stmt> statements)
			{
				this.statements = statements;
			}

      internal override void Accept(IVisitor visitor)
      {
        visitor.VisitBlockStmt(this);
      }

    }

		// If
		internal class If : Stmt
		{

			public readonly Expr condition;
			public readonly Stmt thenBranch;
			public readonly Stmt elseBranch;

			public If(Expr condition, Stmt thenBranch, Stmt elseBranch)
			{
				this.condition = condition;
				this.thenBranch = thenBranch;
				this.elseBranch = elseBranch;
			}

			internal override void Accept(IVisitor visitor)
			{
				visitor.VisitIfStmt(this);
			}

		}

		// While
		internal class While : Stmt
		{
			public readonly Expr condition;
			public readonly Stmt body;

			public While(Expr condition, Stmt body)
			{
				this.condition = condition;
				this.body = body;
			}

      internal override void Accept(IVisitor visitor)
      {
				visitor.VisitWhileStmt(this);
      }

    }

    internal abstract void Accept(IVisitor visitor);
	}

}
