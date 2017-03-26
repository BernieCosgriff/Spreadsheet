// Skeleton written by Joe Zachary for CS 3500, January 2015
// Revised by Joe Zachary, January 2016
// JLZ Repaired pair of mistakes, January 24, 2016

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public struct Formula
    {
        // List of all of the tokens from the formula.
        private List<string> tokens;

        // Set of all variables contained in the formula.
        private HashSet<string> vars;

        // Contains the normalized formula
        private StringBuilder form;

        const string varCheck = "^[a-z][a-z0-9]*$";


        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// </summary>
        public Formula(String formula) : this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.Additionally, takes in a Normalizer
        /// that converts variables into a cononical form and a Validator that imposes extra 
        /// restrictions on the validity of the variable.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula f is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.  If f is syntactically correct but contains a variable 
        /// x such that N(x) is not a legal variable according to the standard Formula rules, 
        /// throws a FormulaFormatException with an explanatory message. If f contains a variable 
        /// x such that V(N(x)) is false, throws a FormulaFormatException with 
        /// an explanatory message. Otherwise, N(x) will be used in place of x in the formula.
        /// </summary>
        public Formula(string formula, Normalizer normalizer, Validator validator)
        {
            if (normalizer == null || validator == null)
            {
                throw new ArgumentNullException();
            }
            //Check if formula is empty
            if (String.IsNullOrWhiteSpace(formula))
            {
                throw new FormulaFormatException("No formula was given to evaluate.");
            }

            // Tells if the current token is a variable
            bool isVar = false;

            // Contains the normalized version of the current variable
            string normalized = "";

            // String builder that will build the formula using the normalized variables
            form = new StringBuilder(formula);

            // Set of normalized variables
            vars = new HashSet<string>();

            // Holds values from try parse
            double value;

            // Keeps track of how many opening vs closing parenthesis
            int parenthesis = 0;

            // Tells whether the previous token was an opening parenthesis or an operator
            bool afterOpeningParenthOrOp = false;

            // Tells whether the previous token was a number, variable, or closing parenthesis
            bool afterNumberVariableClosingParenth = false;

            // Tokens from the original formula
            tokens = new List<string>(GetTokens(formula));

            Regex regex = new Regex(varCheck, RegexOptions.IgnoreCase);

            // Tokens from the normalized formula
            List<string> normalTokens = new List<string>();

            // First token in tokens
            string firstToken = tokens.First();

            // Last token in tokens
            string lastToken = tokens.Last();

            //The last token of a formula must be a number, a variable, or a closing parenthesis
            if (!double.TryParse(lastToken, out value) && !lastToken.Equals(")") && !regex.IsMatch(lastToken))
            {
                throw new FormulaFormatException("The last token of a formula must be a number, a variable, or a closing parenthesis.");
            }

            //The first token of a formula must be a number, a variable, or an opening parenthesis
            if (!double.TryParse(firstToken, out value) && !firstToken.Equals("(") && !regex.IsMatch(firstToken))
            {
                throw new FormulaFormatException("The first token of a formula must be a number, a variable, or an opening parenthesis.");
            }

            foreach (string token in tokens)
            {
                //If token is a varible normalize it and validate it
                if (regex.IsMatch(token))
                {
                    isVar = true;

                    normalized = normalizer(token);

                    if (!regex.IsMatch(normalized))
                    {
                        throw new FormulaFormatException("The normalized version of " + token + " is not valid.");
                    }

                    if (!validator(normalized))
                    {
                        throw new FormulaFormatException("The normalized version of " + token + " (" + normalized + ")" + " is not valid according to the Validator.");
                    }
                }
                //The only legal tokens are the four operator symbols (+ - * /), left parentheses, right parentheses, non-negative numbers, and variables consisting of one or more letters followed by one or more digits
                if (!isVar && !isOperator(token) && !token.Equals("(") && !token.Equals(")") && !double.TryParse(token, out value) && value < 0)
                {
                    throw new FormulaFormatException("The only legal tokens are the four operator symbols (+ - * /), left parentheses, right parentheses, non-negative numbers, and variables consisting of one or more letters followed by one or more digits.");
                }

                //Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis
                if (afterOpeningParenthOrOp && !double.TryParse(token, out value) && !token.Equals("(") && !isVar)
                {
                    throw new FormulaFormatException("Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis.");
                }

                //Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis
                else if (afterNumberVariableClosingParenth && !isOperator(token) && !token.Equals(")"))
                {
                    throw new FormulaFormatException("Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis.");
                }

                else if (token.Equals("("))
                {
                    afterOpeningParenthOrOp = true;
                    parenthesis++;
                }
                else if (token.Equals(")"))
                {
                    afterNumberVariableClosingParenth = true;

                    //Closing parenthesis cannot exceed the number of opening parenthesis
                    if (--parenthesis < 0)
                    {
                        throw new FormulaFormatException("The total number of opening parentheses must equal the total number of closing parentheses.");
                    }
                }
                else if (isOperator(token))
                {
                    afterNumberVariableClosingParenth = false;
                    afterOpeningParenthOrOp = true;
                }
                else if (double.TryParse(token, out value))
                {
                    afterOpeningParenthOrOp = false;
                    afterNumberVariableClosingParenth = true;
                }
                else if (isVar)
                {
                    if (!vars.Contains(normalized))
                    {
                        vars.Add(normalized);
                    }
                    afterOpeningParenthOrOp = false;
                    afterNumberVariableClosingParenth = true;
                }
                if (isVar)
                {
                    normalTokens.Add(normalized);
                    form.Replace(token, normalized);
                    isVar = false;
                }
                else
                {
                    normalTokens.Add(token);
                }
            }

            //If a "(" is left over, throw an exception
            if (parenthesis > 0)
            {
                throw new FormulaFormatException("One or more parenthesis are unclosed.");
            }
            tokens = normalTokens;

        }

        /// <summary>
        /// Returns an ISet of all of the variables
        /// contained in the formula.
        /// </summary>
        /// <returns></returns>
        public ISet<string> GetVariables()
        {
            if (vars == null)
            {
                vars = new HashSet<string>();
            }
            return new HashSet<string>(vars);
        }

        /// <summary>
        /// Returns a string version of the normalized formula.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (form == null)
            {
                form = new StringBuilder("0");
            }
            return form.ToString();
        }

        /// <summary>
        /// Determines whether the token is any of the following:
        /// "*" "/" "-" "+".
        /// </summary>
        /// <param name="token"></param>
        /// <returns>true if token is an operator and false otherwise.</returns>
        private static bool isOperator(string token)
        {
            return token.Equals("*") || token.Equals("/") || token.Equals("-") || token.Equals("+");
        }

        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// </summary>
        public double Evaluate(Lookup lookup)
        {

        // Stack to store operators and parenthesis.
        Stack<string> ops = new Stack<string>();

        // Stack to store double values. All variables will have their
        // values looked-up and placed in this stack.
        Stack<double> values = new Stack<double>();

        Regex regex = new Regex(varCheck, RegexOptions.IgnoreCase);
            if (form == null)
            {
                tokens = new List<string>() { "0" };
                ops = new Stack<string>();
                values = new Stack<double>();
                return 0;
            }
            //Holds out values and will also have the final value stored in it.
            double value = 0;

            foreach (string token in tokens)
            {
                //If token is a double
                if (double.TryParse(token, out value))
                {
                    //If there are no operators push value onto the value stack
                    if (ops.Count == 0)
                    {
                        values.Push(value);
                    }
                    //See what the next operator is
                    //If its a "*", multiply value and whats on top of the value stack
                    else if (ops.Peek().Equals("*"))
                    {
                        ops.Pop();
                        values.Push(values.Pop() * value);
                    }
                    //If its a "/", divide whats on top of the value stack by value
                    else if (ops.Peek().Equals("/"))
                    {
                        if (value == 0)
                        {
                            throw new FormulaEvaluationException("Division by zero is not allowed.");
                        }
                        ops.Pop();
                        values.Push(values.Pop() / value);
                    }
                    //Otherwise just push it onto the value stack
                    else
                    {
                        values.Push(value);
                    }
                }
                //If token is a variable get its value and treat it like a double
                else if (regex.IsMatch(token))
                {

                    if (ops.Count == 0)
                    {
                        try
                        {
                            values.Push(lookup(token));
                        }
                        catch (UndefinedVariableException)
                        {
                            throw new FormulaEvaluationException("Variable " + token + " is not defined.");
                        }
                    }
                    else if (ops.Peek().Equals("*"))
                    {
                        try
                        {
                            ops.Pop();
                            values.Push(values.Pop() * lookup(token));
                        }
                        catch (UndefinedVariableException)
                        {
                            throw new FormulaEvaluationException("Variable " + token + " is not defined.");
                        }
                    }
                    else if (ops.Peek().Equals("/"))
                    {
                        try
                        {
                            if ((value = lookup(token)) == 0)
                            {
                                throw new FormulaEvaluationException("Division by zero is not allowed.");
                            }
                            ops.Pop();
                            values.Push(values.Pop() / value);
                        }
                        catch (UndefinedVariableException)
                        {
                            throw new FormulaEvaluationException("Variable " + token + " is not defined.");
                        }
                    }
                    else
                    {
                        try
                        {
                            values.Push(lookup(token));
                        }
                        catch (UndefinedVariableException)
                        {
                            throw new FormulaEvaluationException("Variable " + token + " is not defined.");
                        }
                    }
                }
                //If token is a "+" or "-"
                else if (token.Equals("+") || token.Equals("-"))
                {
                    //If there are no operators push it onto ops
                    if (ops.Count == 0)
                    {
                        ops.Push(token);
                    }
                    //If "+" is on top of ops add the next two numbers from values
                    else if (ops.Peek().Equals("+"))
                    {
                        ops.Pop();
                        values.Push(values.Pop() + values.Pop());
                    } //Same for "-"
                    else if (ops.Peek().Equals("-"))
                    {
                        ops.Pop();
                        values.Push((-values.Pop()) + values.Pop());
                    }
                    ops.Push(token);
                }
                //If token is a "*" or "/"
                else if (token.Equals("*") || token.Equals("/"))
                {
                    ops.Push(token);
                }
                //If token is a "("
                else if (token.Equals("("))
                {
                    ops.Push(token);
                }
                //If token is a ")"
                else if (token.Equals(")"))
                {
                    //If "+" is on top of ops add the next two numbers from values
                    if (ops.Peek().Equals("+"))
                    {
                        ops.Pop();
                        values.Push(values.Pop() + values.Pop());
                    }
                    //Same for "-"
                    if (ops.Peek().Equals("-"))
                    {
                        ops.Pop();
                        values.Push(-values.Pop() + values.Pop());
                    }
                    //"(" will be on top of ops so pop it
                    ops.Pop();
                    if (ops.Count != 0)
                    {
                        //If "*" is on top multiply the top two numbers from values
                        if (ops.Peek().Equals("*"))
                        {
                            ops.Pop();
                            values.Push(values.Pop() * values.Pop());
                        }
                    }
                    if (ops.Count != 0)
                    {
                        //Same for "/"
                        if (ops.Peek().Equals("/"))
                        {
                            if (value == 0)
                            {
                                throw new FormulaEvaluationException("Division by zero is not allowed.");
                            }
                            ops.Pop();
                            values.Push(values.Pop() / value);
                        }
                    }
                }
            }
            //If ops is empty return what is left in the value stack
            if (ops.Count == 0)
            {
                value = values.Pop();
            }
            //Otherwise do the last operation
            else
            {
                if (ops.Peek().Equals("+"))
                {
                    value = values.Pop() + values.Pop();
                }
                if (ops.Peek().Equals("-"))
                {
                    value = (-values.Pop() + values.Pop());
                }
            }
            return value;
        }

        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string s);

    /// <summary>
    /// Converts variables into a canonical form.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public delegate string Normalizer(string s);

    /// <summary>
    /// Imposes extra restrictions on the validity of a variable, 
    /// beyond the ones already built into the Formula definition.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public delegate bool Validator(string s);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(String variable)
            : base(variable)
        {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message)
        {
        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(String message) : base(message)
        {
        }
    }
}
