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
        Statements.Add(Declaration());
      }

      return Statements;
    }

    /**
		 *
		 */
    private Expr Expression()
    {
      return Assignment();
    }

    private Stmt Declaration()
    {
      try
      {
        if (Match(VAR)) return VarDeclaration();
        return Statement();
      }
      catch (ParseError error)
      {
        Synchronize();
        return null;
      }

    }

    private Stmt Statement()
    {

      if (Match(TokenType.FOR)) return ForStatement();

      if (Match(TokenType.IF)) return IfStatement();

      if (Match(TokenType.PRINT)) return PrintStatement();

      if (Match(TokenType.WHILE)) return WhileStatement();

      if (Match(TokenType.LEFT_BRACE)) return new Stmt.Block(Block());

      return ExpressionStatement();
    }

    private Stmt ForStatement()
    {
      Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");

      Stmt initializer;
      if (Match(SEMICOLON))
      {
        initializer = null;
      }
      else if (Match(VAR))
      {
        initializer = VarDeclaration();
      }
      else
      {
        initializer = ExpressionStatement();
      }

      Expr condition = null;
      if (!Check(SEMICOLON))
      {
        condition = Expression();
      }
      Consume(SEMICOLON, "Expect ';' after loop condition.");

      Expr increment = null;
      if (!Check(TokenType.RIGHT_PAREN))
      {
        increment = Expression();
      }
      Consume(RIGHT_PAREN, "Expect ')' after for clauses.");

      Stmt body = Statement();

      if (increment != null)
      {
        body = new Stmt.Block(new List<Stmt> {
          body,
          new Stmt.Expression(increment)
        });

      }

      if (condition == null) condition = new Expr.Literal(true);

      body = new Stmt.While(condition, body);

      if (initializer != null)
      {
        body = new Stmt.Block(new List<Stmt> { initializer, body });
      }

      return body;

    }

    private Stmt IfStatement()
    {
      Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");

      Expr condition = Expression();

      Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

      Stmt thenBranch = Statement();
      Stmt elseBranch = null;

      if (Match(TokenType.ELSE))
      {
        elseBranch = Statement();
      }

      return new Stmt.If(condition, thenBranch, elseBranch);
    }

    private Stmt PrintStatement()
    {
      Expr Value = Expression();
      Consume(SEMICOLON, "Expect ';' after value.");
      return new Stmt.Print(Value);
    }

    private Stmt VarDeclaration()
    {
      Token name = Consume(IDENTIFIER, "Expect variable name.");

      Expr initializer = null;

      if (Match(EQUAL))
      {
        initializer = Expression();
      }
      Consume(SEMICOLON, "Expect ';' after variable declaration.");
      return new Stmt.Var(name, initializer);

    }

    private Stmt WhileStatement()
    {
      Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
      Expr condition = Expression();
      Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
      Stmt body = Statement();

      return new Stmt.While(condition, body);

    }

    private Stmt ExpressionStatement()
    {
      Expr expr = Expression();
      Consume(SEMICOLON, "Expect ';' after expression.");
      return new Stmt.Expression(expr);
    }

    private List<Stmt> Block()
    {
      List<Stmt> Statements = new List<Stmt>();

      while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
      {
        Statements.Add(Declaration());
      }

      Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");

      return Statements;
    }

    private Expr Assignment()
    {
      Expr expr = Or();

      if (Match(EQUAL))
      {
        Token equals = Previous();
        Expr value = Assignment();

        if (expr is Expr.Variable variableExpr)
        {
          Token name = variableExpr.Name;
          return new Expr.Assign(name, value);
        }

        Error(equals, "Invalid assignment target.");

      }

      return expr;

    }

    private Expr Or()
    {
      Expr expr = And();

      while (Match(TokenType.OR))
      {
        Token op = Previous();
        Expr right = And();
        expr = new Expr.Logical(expr, op, right);
      }

      return expr;

    }

    private Expr And()
    {
      Expr expr = Equality();

      while (Match(TokenType.AND))
      {
        Token op = Previous();
        Expr right = Equality();
        expr = new Expr.Logical(expr, op, right);
      }

      return expr;

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

      return Call();
    }

    private Expr Call()
    {
      Expr expr = Primary();

      while (true)
      {
        if (Match(LEFT_PAREN))
        {
          expr = FinishCall(expr);
        }
        else
        {
          break;
        }
      }

      return expr;

    }

    private Expr FinishCall(Expr callee)
    {
      List<Expr> Args = new List<Expr>();

      if (!Check(RIGHT_PAREN))
      {
        do
        {
          if (Args.Count >= 255)
          {
            Error(Peek(), "Can't have more than 255 arguments.");
          }
          Args.Add(Expression());
        } while (Match(COMMA));
      }

      Token paren = Consume(RIGHT_PAREN, "Expect ')' after arguments.");

      return new Expr.Call(callee, paren, Args);

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

      if (Match(TokenType.IDENTIFIER))
      {
        return new Expr.Variable(Previous());
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
