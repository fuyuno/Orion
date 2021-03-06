﻿using System.Linq.Expressions;

namespace Orion.Scripting.Ast
{
    internal class AstMultiplyOperator : AstOperator
    {
        public AstMultiplyOperator(string value) : base(value) { }

        protected override Expression CreateExpression<T>(ParameterExpression parameter, Expression left, Expression right)
        {
            return Expression.Multiply(left, right);
        }
    }
}