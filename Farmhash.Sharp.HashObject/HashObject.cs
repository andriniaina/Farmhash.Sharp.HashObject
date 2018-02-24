/*
Copyright 2018 [andriniaina](https://github.com/andriniaina)
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
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
        static IDictionary<Type, List<Func<object, IEnumerable<byte>>>> functionCache = new Dictionary<Type, List<Func<object, IEnumerable<byte>>>>();
        public static ulong Hash64<T>(T o)
        {
            List<Func<object, IEnumerable<byte>>> functions;
            if (!functionCache.TryGetValue(typeof(T), out functions))
            {
                lock (functionCache)
                    if (!functionCache.TryGetValue(typeof(T), out functions))
                    {
                        functions = BuildHashFunctions(typeof(T)).Select(f => f.Compile()).ToList();
                        if (functions.Count == 0) throw new NotSupportedException("no hash function found");
                        functionCache.Add(typeof(T), functions);
                    }
            }

            var bytes = functions.SelectMany(f => f(o)).ToArray();
            return Farmhash.Hash64(bytes, bytes.Length);
        }

        internal static ulong GetHashNoCache<T>(T o)
        {
            IEnumerable<Func<object, IEnumerable<byte>>> functions;
            functions = BuildHashFunctions(typeof(T)).Select(f => f.Compile()).ToList();
            var bytes = functions.SelectMany(f => f(o)).ToArray();
            return Farmhash.Hash64(bytes, bytes.Length);
        }

        private static IEnumerable<Expression<Func<object, IEnumerable<byte>>>> BuildHashFunctions(Type t)
        {
            var xExpr = Expression.Parameter(typeof(object), "x");
            var xExprCasted = Expression.Convert(xExpr, t);
            var castedVariableExpr = Expression.Variable(t, "castedVariable");
            var assignement = Expression.Assign(castedVariableExpr, xExprCasted);
            foreach (var pInfo in t.GetProperties())
            {
                Expression pExpr = Expression.Property(castedVariableExpr, pInfo);
                LambdaExpression extractBytesExpr = null;
                if (pInfo.PropertyType == typeof(bool))
                {
                    extractBytesExpr = BoolToBytesExpr;
                }
                else if (pInfo.PropertyType == typeof(byte))
                {
                    extractBytesExpr = ByteToBytesExpr;
                }
                else if (pInfo.PropertyType == typeof(sbyte))
                {
                    extractBytesExpr = SByteToBytesExpr;
                }
                else if (pInfo.PropertyType == typeof(short))
                {
                    extractBytesExpr = ShortToBytesExpr;
                }
                else if (pInfo.PropertyType == typeof(ushort))
                {
                    extractBytesExpr = UShortToBytesExpr;
                }
                else if (pInfo.PropertyType == typeof(int))
                {
                    extractBytesExpr = Int32ToBytesExpr;
                }
                else if (pInfo.PropertyType == typeof(uint))
                {
                    extractBytesExpr = UInt32ToBytesExpr;
                }
                else if (pInfo.PropertyType == typeof(long))
                {
                    extractBytesExpr = Int64ToBytesExpr;
                }
                else if (pInfo.PropertyType == typeof(ulong))
                {
                    extractBytesExpr = UInt64ToBytesExpr;
                }
                else if (pInfo.PropertyType == typeof(float))
                {
                    extractBytesExpr = FloatToBytesExpr;
                }
                else if (pInfo.PropertyType == typeof(double))
                {
                    extractBytesExpr = DoubleToBytesExpr;
                }
                else if (pInfo.PropertyType == typeof(decimal))
                {
                    extractBytesExpr = DecimalToBytesExpr;
                }
                else if (pInfo.PropertyType == typeof(char))
                {
                    extractBytesExpr = CharToBytesExpr;
                }
                else if (pInfo.PropertyType == typeof(string))
                {
                    extractBytesExpr = StringToBytesExpr;
                }
                else if (pInfo.PropertyType == typeof(DateTime))
                {
                    extractBytesExpr = DateTimeToBytesExpr;
                }
                else if (pInfo.PropertyType.IsEnum)
                {
                    pExpr = Expression.Convert(pExpr, typeof(IConvertible));
                    extractBytesExpr = IConvertibleToBytesExpr;
                }
                else if (typeof(object).IsAssignableFrom(pInfo.PropertyType))
                {
                    var subexpr = BuildHashFunctions(pInfo.PropertyType);
                    foreach (var e in subexpr)
                    {
                        var xxExpr = Expression.Parameter(typeof(object), "thisGonBeGud");
                        var xxExprCasted = Expression.Convert(xxExpr, t);
                        var trololol = Expression.Property(xxExprCasted, pInfo);
                        var itwoooooooorkkkkss = Expression.Invoke(e, trololol);
                        var xxxxx = Expression.Lambda<Func<object, IEnumerable<byte>>>(itwoooooooorkkkkss, xxExpr);
                        yield return xxxxx;
                    }
                    continue;
                }
                else
                {
                    throw new NotImplementedException($"type {pInfo.PropertyType} is not supported");
                }
                var block = Expression.Block(new ParameterExpression[] { castedVariableExpr }, assignement, Expression.Invoke(extractBytesExpr, pExpr));
                var xx = Expression.Lambda<Func<object, IEnumerable<byte>>>(block, xExpr);
                yield return xx;
            }
            // throw new NotImplementedException();
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
