/*
Copyright 2018 [andriniaina](https://github.com/andriniaina/Farmhash.Sharp.HashObject/)
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Farmhash.Sharp
{
    public class HashObject
    {
        /*
        static void Main()
        {
        }
        */
        static Dictionary<Type, List<Func<object, IEnumerable<byte>>>> functionCache = new Dictionary<Type, List<Func<object, IEnumerable<byte>>>>();
        public static ulong Hash64<T>(T o)
        {
            List<Func<object, IEnumerable<byte>>> functions;
            if (!functionCache.TryGetValue(typeof(T), out functions))
            {
                lock (functionCache)
                    if (!functionCache.TryGetValue(typeof(T), out functions))
                    {
                        functions = BuildHashFunctionsWrapped<T>(typeof(T)).Select(f => f.Compile()).ToList();
                        if (functions.Count == 0) throw new NotSupportedException("no hash function found");
                        functionCache.Add(typeof(T), functions);
                    }
            }

            var bytes = functions.SelectMany(f => f(o)).ToArray();
            return Farmhash.Hash64(bytes, bytes.Length);
        }

        internal static ulong Hash64_NoCache_forBenchmarks<T>(T o)
        {
            IEnumerable<Func<object, IEnumerable<byte>>> functions;
            functions = BuildHashFunctionsWrapped<T>(typeof(T)).Select(f => f.Compile()).ToList();
            var bytes = functions.SelectMany(f => f(o)).ToArray();
            return Farmhash.Hash64(bytes, bytes.Length);
        }

        private static IEnumerable<Expression<Func<object, IEnumerable<byte>>>> BuildHashFunctionsWrapped<T>(Type t)
        {
            var xExpr = Expression.Parameter(typeof(object), "x");
            var xExprCasted = Expression.Convert(xExpr, t);
            var castedVariableExpr = Expression.Variable(t, "castedVariable");
            var assignement = Expression.Assign(castedVariableExpr, xExprCasted);

            foreach (var expr in BuildHashLambdas(t))
            {
                var block = Expression.Block(new ParameterExpression[] { castedVariableExpr }, assignement, Expression.Invoke(expr, castedVariableExpr));
                //var sdf = Expression.Invoke(block, xExpr);
                var xx = Expression.Lambda<Func<object, IEnumerable<byte>>>(block, xExpr);
                yield return xx;
            }
        }
        private static IEnumerable<LambdaExpression> BuildHashLambdas(Type t)
        {
            var xInputParameter = Expression.Parameter(t, "input");

            LambdaExpression extractBytesExpr = null;
            if (t == typeof(bool))
            {
                extractBytesExpr = BoolToBytesExpr;
            }
            else if (t == typeof(byte))
            {
                extractBytesExpr = ByteToBytesExpr;
            }
            else if (t == typeof(sbyte))
            {
                extractBytesExpr = SByteToBytesExpr;
            }
            else if (t == typeof(short))
            {
                extractBytesExpr = ShortToBytesExpr;
            }
            else if (t == typeof(ushort))
            {
                extractBytesExpr = UShortToBytesExpr;
            }
            else if (t == typeof(int))
            {
                extractBytesExpr = Int32ToBytesExpr;
            }
            else if (t == typeof(uint))
            {
                extractBytesExpr = UInt32ToBytesExpr;
            }
            else if (t == typeof(long))
            {
                extractBytesExpr = Int64ToBytesExpr;
            }
            else if (t == typeof(ulong))
            {
                extractBytesExpr = UInt64ToBytesExpr;
            }
            else if (t == typeof(float))
            {
                extractBytesExpr = FloatToBytesExpr;
            }
            else if (t == typeof(double))
            {
                extractBytesExpr = DoubleToBytesExpr;
            }
            else if (t == typeof(decimal))
            {
                extractBytesExpr = DecimalToBytesExpr;
            }
            else if (t == typeof(char))
            {
                extractBytesExpr = CharToBytesExpr;
            }
            else if (t == typeof(string))
            {
                extractBytesExpr = StringToBytesExpr;
            }
            else if (t == typeof(DateTime))
            {
                extractBytesExpr = DateTimeToBytesExpr;
            }
            else if (t.IsEnum)
            {
                var xExprCasted = Expression.Convert(xInputParameter, typeof(IConvertible));
                var castedVariableExpr = Expression.Variable(typeof(IConvertible), "castedVariable");
                var assignement = Expression.Assign(castedVariableExpr, xExprCasted);

                var block = Expression.Block(new ParameterExpression[] { castedVariableExpr }, assignement, Expression.Invoke(IConvertibleToBytesExpr, castedVariableExpr));
                extractBytesExpr = Expression.Lambda(block, xInputParameter);
            }
            else if (typeof(IDictionary).IsAssignableFrom(t))
            {
                var pExprKeys = Expression.Property(xInputParameter, "Keys");
                var pExprValues = Expression.Property(xInputParameter, "Values");

                var subexprKeys = BuildHashLambdas(t.GetProperty("Keys").PropertyType);
                foreach (var e in subexprKeys)
                {
                    yield return Expression.Lambda(Expression.Invoke(e, pExprKeys), xInputParameter);
                }
                var subexprValues = BuildHashLambdas(t.GetProperty("Values").PropertyType);
                foreach (var e in subexprValues)
                {
                    yield return Expression.Lambda(Expression.Invoke(e, pExprValues), xInputParameter);
                }
                yield break;//                throw new NotImplementedException();
            }
            else if (t.IsGenericType && typeof(IEnumerable).IsAssignableFrom(t.GetGenericTypeDefinition()))
            {
                Type underlyingType = t.GetGenericArguments()[0];
                var subexpr = BuildHashLambdas(underlyingType);
                foreach (var e in subexpr)
                {
                    var pBoxedUnderlyingObject = Expression.Parameter(typeof(object), "boxedUnderlyingObject");
                    var xExprCasted = Expression.Convert(pBoxedUnderlyingObject, underlyingType);
                    var castedVariableExpr = Expression.Variable(underlyingType, "castedVariable");
                    var assignement = Expression.Assign(castedVariableExpr, xExprCasted);
                    var block = Expression.Block(new ParameterExpression[] { castedVariableExpr }, assignement, Expression.Invoke(e, castedVariableExpr));
                    var xxxxx = Expression.Lambda<Func<object, IEnumerable<byte>>>(block, pBoxedUnderlyingObject);

                    var converter = xxxxx.Compile();

                    var f = GetIEnumerableToBytes(converter);
                    yield return f;
                }
                yield break;
            }
            else if (typeof(object).IsAssignableFrom(t))
            {
                foreach (var pInfo in t.GetProperties())
                {
                    Expression pExpr = Expression.Property(xInputParameter, pInfo);

                    var subexpr = BuildHashLambdas(pInfo.PropertyType);
                    foreach (var e in subexpr)
                    {
                        var xxInputParameter = Expression.Parameter(t, "xx");
                        var xxxxx = Expression.Lambda(Expression.Invoke(e, pExpr), xInputParameter);
                        yield return xxxxx;
                    }
                }
                yield break;
            }
            else
            {
                throw new NotImplementedException($"type {t} is not supported");
            }
            // var xx = Expression.Lambda(Expression.Invoke(extractBytesExpr, xInputParameter), xInputParameter);
            yield return extractBytesExpr;
        }

        private static Expression<Func<IEnumerable, IEnumerable<byte>>> GetIEnumerableToBytes(Func<object, IEnumerable<byte>> converter)
        {
            Expression<Func<System.Collections.IEnumerable, IEnumerable<byte>>> expr = d => GetBytes(d, converter);
            return expr;
        }

        private static IEnumerable<byte> GetBytes(IEnumerable d, Func<object, IEnumerable<byte>> f)
        {
            foreach (var item in d)
            {
                foreach (var b in f(item))
                {
                    yield return b;
                }
            }
        }

        private static IEnumerable<byte> GetDecimalBytes(decimal d)
        {
            foreach (var i in Decimal.GetBits(d))
                foreach (var b in BitConverter.GetBytes(i))
                    yield return b;
        }

        static readonly Expression<Func<DateTime, IEnumerable<byte>>> DateTimeToBytesExpr = i => BitConverter.GetBytes(i.Ticks);
        static readonly Expression<Func<IConvertible, IEnumerable<byte>>> IConvertibleToBytesExpr = i => BitConverter.GetBytes(Convert.ToInt64(i));
        static readonly Expression<Func<bool, IEnumerable<byte>>> BoolToBytesExpr = i => BitConverter.GetBytes(i);
        static readonly Expression<Func<byte, IEnumerable<byte>>> ByteToBytesExpr = i => BitConverter.GetBytes(i);
        static readonly Expression<Func<sbyte, IEnumerable<byte>>> SByteToBytesExpr = i => BitConverter.GetBytes(i);
        static readonly Expression<Func<short, IEnumerable<byte>>> ShortToBytesExpr = i => BitConverter.GetBytes(i);
        static readonly Expression<Func<ushort, IEnumerable<byte>>> UShortToBytesExpr = i => BitConverter.GetBytes(i);
        static readonly Expression<Func<int, IEnumerable<byte>>> Int32ToBytesExpr = i => BitConverter.GetBytes(i);
        static readonly Expression<Func<uint, IEnumerable<byte>>> UInt32ToBytesExpr = i => BitConverter.GetBytes(i);
        static readonly Expression<Func<long, IEnumerable<byte>>> Int64ToBytesExpr = i => BitConverter.GetBytes(i);
        static readonly Expression<Func<ulong, IEnumerable<byte>>> UInt64ToBytesExpr = i => BitConverter.GetBytes(i);
        static readonly Expression<Func<char, IEnumerable<byte>>> CharToBytesExpr = c => BitConverter.GetBytes(c);
        static readonly Expression<Func<double, IEnumerable<byte>>> DoubleToBytesExpr = s => BitConverter.GetBytes(s);
        static readonly Expression<Func<decimal, IEnumerable<byte>>> DecimalToBytesExpr = d => GetDecimalBytes(d);
        static readonly Expression<Func<float, IEnumerable<byte>>> FloatToBytesExpr = s => BitConverter.GetBytes(s);
        static readonly Expression<Func<string, IEnumerable<byte>>> StringToBytesExpr = s => Encoding.UTF8.GetBytes(s);
    }
}
