using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static cslox.TokenType;

namespace cslox
{
	internal class Parser
	{
		private List<Token> tokens;
		private int current = 0;

		/**
		 *
		 */
		internal Parser(List<Token> tokens)
		{
			this.tokens = tokens;
		}

		internal List<Stmt> Parse()
		{
			List<Stmt> Statements = new List<Stmt>();
			while (!IsAtEnd())
			{
				Statements.Add(Statement());
			}

			return Statements;
		}

		/**
		 *
		 */
		private Expr Expression()
		{
			return Equality();
		}

		private Stmt Statement()
		{
			if (Match(TokenType.PRINT)) return PrintStatement();

			return ExpressionStatement();
		}

		private Stmt PrintStatement()
		{
			Expr Value = Expression();
			Consume(SEMICOLON, "Expect ';' after value.");
			return new Stmt.Print(Value);
		}

		private Stmt ExpressionStatement()
		{
			Expr expr = Expression();
			Consume(SEMICOLON, "Expect ';' after expression.");
			return new Stmt.Expression(expr);
		}

		/**
		 *
		 */
		private Expr Equality()
		{
			Expr expr = Comparison();

			while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
			{
				Token op = Previous();
				Expr right = Comparison();
				expr = new Expr.Binary(expr, op, right);
			}

			return expr;
		}

		/**
		 *
		 */
		private Expr Comparison()
		{
			Expr expr = Term();

			while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
			{
				Token op = Previous();
				Expr right = Term();
				expr = new Expr.Binary(expr, op, right);
			}

			return expr;
		}

		/**
		 *
		 */
		private Expr Term()
		{
			Expr expr = Factor();

			while (Match(TokenType.MINUS, TokenType.PLUS))
			{
				Token op = Previous();
				Expr right = Factor();
				expr = new Expr.Binary(expr, op, right);
			}

			return expr;
		}

		/**
		 *
		 */
		private Expr Factor()
		{
			Expr expr = Unary();

			while (Match(TokenType.SLASH, TokenType.STAR))
			{
				Token op = Previous();
				Expr right = Unary();
				expr = new Expr.Binary(expr, op, right);
			}

			return expr;
		}

		/**
		 *
		 */
		private Expr Unary()
		{
			if (Match(TokenType.BANG, TokenType.MINUS))
			{
				Token op = Previous();
				Expr right = Unary();
				return new Expr.Unary(op, right);
			}

			return Primary();
		}

		/**
		 *
		 */
		private Expr Primary()
		{
			if (Match(TokenType.FALSE)) return new Expr.Literal(false);
			if (Match(TokenType.TRUE)) return new Expr.Literal(true);
			if (Match(TokenType.NIL)) return new Expr.Literal(null);

			if (Match(TokenType.NUMBER, TokenType.STRING))
			{
				return new Expr.Literal(Previous().literal);
			}

			if (Match(TokenType.LEFT_PAREN))
			{
				Expr expr = Expression();
				Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
				return new Expr.Grouping(expr);
			}

			throw Error(Peek(), "Expect expression.");
		}

		/**
		 * Returns true if the current token is any of the given types.
		 */
		private bool Match(params TokenType[] types)
		{
			foreach (TokenType type in types)
			{
				if (Check(type))
				{
					Advance();
					return true;
				}
			}

			return false;
		}

		/**
		 * If the current token is of the given type, consumes it and returns it.
		 * Otherwise, throws an error.
		 */
		private Token Consume(TokenType type, string message)
		{
			if (Check(type)) return Advance();

			throw Error(Peek(), message);
		}

		/**
		 * Returns true if the current token is of the given type.
		 */
		private bool Check(TokenType type)
		{
			if (IsAtEnd()) return false;

			return Peek().type == type;
		}

		/**
		 * Advances the current token and returns it.
		 */
		private Token Advance()
		{
			if (!IsAtEnd()) current++;
			return Previous();
		}

		/**
		 * Returns true if the current token is EOF.
		 */
		private bool IsAtEnd()
		{
			return Peek().type == TokenType.EOF;
		}

		/**
		 * Returns the current token.
		 */
		private Token Peek()
		{
			return tokens[current];
		}

		/**
		 * Returns the token before the current one.
		 */
		private Token Previous()
		{
			return tokens[current - 1];
		}

		/**
		 *
		 */
		private ParseError Error(Token token, string message)
		{
			Lox.Error(token, message);
			return new ParseError();
		}

		/**
		 *  
		 */
		private void Synchronize()
		{
			Advance();

			while (!IsAtEnd())
			{
				if (Previous().type == TokenType.SEMICOLON) return;

				switch (Peek().type)
				{
					case TokenType.CLASS:
					case TokenType.FUN:
					case TokenType.VAR:
					case TokenType.FOR:
					case TokenType.IF:
					case TokenType.WHILE:
					case TokenType.PRINT:
					case TokenType.RETURN:
						return;
				}

				Advance();

			}

		}

		/**
		 *
		 */
		private class ParseError : Exception
		{

		}

	}

}
