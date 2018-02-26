using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace DiscordBot
{
    public static class UserCompareExtension
    {
        public static List<Variance> DetailedCompare<T>(this T lhs, T rhs)
        {
            List<Variance> variances = new List<Variance>();

            if (lhs != null && rhs != null)
            {
                FieldInfo[] fi = lhs.GetType().GetFields();
                foreach (FieldInfo f in fi)
                {
                    Variance v = new Variance
                    {
                        property = f.Name,
                        lhs = f.GetValue(lhs),
                        rhs = f.GetValue(rhs)
                    };

                    if (!Equals(v.lhs, v.rhs))
                        variances.Add(v);
                }

                PropertyInfo[] pi = lhs.GetType().GetProperties();
                foreach (var p in pi)
                {
                    Variance v = new Variance
                    {
                        property = p.Name,
                        lhs = p.GetValue(lhs),
                        rhs = p.GetValue(rhs)
                    };

                    if (!Equals(v.lhs, v.rhs))
                        variances.Add(v);
                }
            }

            return variances;
        }
    }

    public class Variance
    {
        public String property { get; set; }
        public Object lhs { get; set; }
        public Object rhs { get; set; }

        public override string ToString() => $"{property}: {lhs} -> {rhs}";
    }
}
