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
            ParameterExpression castedVariableExpr = Expression.Variable(t, "castedVariable");
            var assignement = Expression.Assign(castedVariableExpr, xExprCasted);
            foreach (var pInfo in t.GetProperties())
            {
                var pExpr = Expression.Property(castedVariableExpr, pInfo);
                LambdaExpression extractBytesExpr = null;
                if (pInfo.PropertyType == typeof(bool))
                {
                    extractBytesExpr = GetExtractBytesExprForBool(pExpr);
                }
                else if (pInfo.PropertyType == typeof(byte))
                {
                    extractBytesExpr = GetExtractBytesExprForByte(pExpr);
                }
                else if (pInfo.PropertyType == typeof(sbyte))
                {
                    extractBytesExpr = GetExtractBytesExprForSByte(pExpr);
                }
                else if (pInfo.PropertyType == typeof(short))
                {
                    extractBytesExpr = GetExtractBytesExprForInt16(pExpr);
                }
                else if (pInfo.PropertyType == typeof(ushort))
                {
                    extractBytesExpr = GetExtractBytesExprForUInt16(pExpr);
                }
                else if (pInfo.PropertyType == typeof(int))
                {
                    extractBytesExpr = GetExtractBytesExprForInt32(pExpr);
                }
                else if (pInfo.PropertyType == typeof(uint))
                {
                    extractBytesExpr = GetExtractBytesExprForUInt32(pExpr);
                }
                else if (pInfo.PropertyType == typeof(long))
                {
                    extractBytesExpr = GetExtractBytesExprForInt64(pExpr);
                }
                else if (pInfo.PropertyType == typeof(ulong))
                {
                    extractBytesExpr = GetExtractBytesExprForUInt64(pExpr);
                }
                else if (pInfo.PropertyType == typeof(float))
                {
                    extractBytesExpr = GetExtractBytesExprForSingle(pExpr);
                }
                else if (pInfo.PropertyType == typeof(double))
                {
                    extractBytesExpr = GetExtractBytesExprForDouble(pExpr);
                }
                else if (pInfo.PropertyType == typeof(char))
                {
                    extractBytesExpr = GetExtractBytesExprForChar(pExpr);
                }
                else if (pInfo.PropertyType == typeof(string))
                {
                    extractBytesExpr = GetExtractBytesExprForStringUtf8(pExpr);
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
                    throw new NotImplementedException();
                }
                var block = Expression.Block(new ParameterExpression[] { castedVariableExpr }, assignement, Expression.Invoke(extractBytesExpr, pExpr));
                var xx = Expression.Lambda<Func<object, IEnumerable<byte>>>(block, xExpr);
                yield return xx;
            }
            // throw new NotImplementedException();
        }

        private static LambdaExpression GetExtractBytesExprForBool(MemberExpression pExpr)
        {
            Expression<Func<bool, IEnumerable<byte>>> byteExtractor = i => BitConverter.GetBytes(i);
            return byteExtractor;
        }
        private static LambdaExpression GetExtractBytesExprForByte(MemberExpression pExpr)
        {
            Expression<Func<byte, IEnumerable<byte>>> byteExtractor = i => BitConverter.GetBytes(i);
            return byteExtractor;
        }
        private static LambdaExpression GetExtractBytesExprForSByte(MemberExpression pExpr)
        {
            Expression<Func<sbyte, IEnumerable<byte>>> byteExtractor = i => BitConverter.GetBytes(i);
            return byteExtractor;
        }
        private static LambdaExpression GetExtractBytesExprForInt16(MemberExpression pExpr)
        {
            Expression<Func<short, IEnumerable<byte>>> byteExtractor = i => BitConverter.GetBytes(i);
            return byteExtractor;
        }
        private static LambdaExpression GetExtractBytesExprForUInt16(MemberExpression pExpr)
        {
            Expression<Func<ushort, IEnumerable<byte>>> byteExtractor = i => BitConverter.GetBytes(i);
            return byteExtractor;
        }
        private static LambdaExpression GetExtractBytesExprForInt32(MemberExpression pExpr)
        {
            Expression<Func<int, IEnumerable<byte>>> byteExtractor = i => BitConverter.GetBytes(i);
            return byteExtractor;
        }
        private static LambdaExpression GetExtractBytesExprForUInt32(MemberExpression pExpr)
        {
            Expression<Func<uint, IEnumerable<byte>>> byteExtractor = i => BitConverter.GetBytes(i);
            return byteExtractor;
        }
        private static LambdaExpression GetExtractBytesExprForInt64(MemberExpression pExpr)
        {
            Expression<Func<long, IEnumerable<byte>>> byteExtractor = i => BitConverter.GetBytes(i);
            return byteExtractor;
        }
        private static LambdaExpression GetExtractBytesExprForUInt64(MemberExpression pExpr)
        {
            Expression<Func<ulong, IEnumerable<byte>>> byteExtractor = i => BitConverter.GetBytes(i);
            return byteExtractor;
        }
        private static LambdaExpression GetExtractBytesExprForChar(MemberExpression pExpr)
        {
            Expression<Func<char, IEnumerable<byte>>> byteExtractor = c => BitConverter.GetBytes(c);
            return byteExtractor;
        }
        private static LambdaExpression GetExtractBytesExprForDouble(MemberExpression pExpr)
        {
            Expression<Func<double, IEnumerable<byte>>> byteExtractor = s => BitConverter.GetBytes(s);
            return byteExtractor;
        }
        private static LambdaExpression GetExtractBytesExprForSingle(MemberExpression pExpr)
        {
            Expression<Func<float, IEnumerable<byte>>> byteExtractor = s => BitConverter.GetBytes(s);
            return byteExtractor;
        }
        private static LambdaExpression GetExtractBytesExprForStringUtf8(MemberExpression pExpr)
        {
            Expression<Func<string, IEnumerable<byte>>> byteExtractor = s => Encoding.UTF8.GetBytes(s);
            return byteExtractor;
        }
    }
}
