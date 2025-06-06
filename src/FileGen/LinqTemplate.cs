﻿using System;
using ZLinq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FileGen;

public partial class Commands
{
    public void LinqTemplate()
    {
        var enumerableMethods = typeof(Enumerable).GetMethods()
            .Where(x => IsExtensionMethod(x))
            .ToArray();

        var returnEnumerables = enumerableMethods
            .Where(x => x.ReturnType.Name == "IEnumerable`1")
            .GroupBy(x => x.Name)
            .OrderBy(x => x.Key)
            .ToArray();

        // OrderBy/ThenBy is here but ok(fix manually...)
        var others = enumerableMethods
            .Where(x => x.ReturnType.Name != "IEnumerable`1")
            .GroupBy(x => x.Name)
            .OrderBy(x => x.Key)
            .ToArray();

        foreach (var item in returnEnumerables)
        {
            EmiTEnumeratorTemplate(item);
        }

        foreach (var item in others)
        {
            EmitOtherTemplate(item);
        }
    }

    // KeyValuePair type format is failed but ok(fix manually...)

    static void EmiTEnumeratorTemplate(IGrouping<string, MethodInfo> methods)
    {
        Directory.CreateDirectory("linq1");

        var className = methods.Key;
        var fileName = $"{className}.cs";

        var sb1 = new StringBuilder(); // ZLinq
        sb1.AppendLine("namespace ZLinq");
        sb1.AppendLine("{");
        sb1.AppendLine("    partial class ValueEnumerableExtensions");
        sb1.AppendLine("    {");

        var sb2 = new StringBuilder(); // ZLinq.Linq
        sb2.AppendLine("namespace ZLinq.Linq");
        sb2.AppendLine("{");

        var i = 0;
        foreach (var methodInfo in methods)
        {
            var suffix = (++i == 1) ? "" : i.ToString();

            if (!methodInfo.GetParameters()[0].ParameterType.IsGenericType) continue;

            var t = methodInfo.GetParameters()[0].ParameterType.GetGenericArguments()[0]; // this IEnumerable<T> source's T
            var enumerableType = $"{className}ValueEnumerable{suffix}";
            var baseGenericArguments = string.Join(", ", methodInfo.GetGenericArguments().Select(x => x.Name).ToArray());
            var genericArguments = "TEnumerator" + (baseGenericArguments == "" ? "" : $", {baseGenericArguments}");
            var baseParameters = string.Join(", ", methodInfo.GetParameters().Skip(1).Select(x => $"{FormatType(x.ParameterType)} {x.Name}").ToArray()); // skip TSource source
            var parameters = "TEnumerator source" + (baseParameters == "" ? "" : $", {baseParameters}");
            var baseParameterNames = string.Join(", ", methodInfo.GetParameters().Skip(1).Select(x => $"{x.Name}").ToArray());
            var parameterNames = "source" + (baseParameterNames == "" ? "" : $", {baseParameterNames}");
            var enumerableElementType = methodInfo.ReturnType.GetGenericArguments()[0].Name;

            // sb1
            sb1.AppendLine($"""
        public static {enumerableType}<{genericArguments}> {className}<{genericArguments}>(this {parameters})
            where TEnumerator : struct, IValueEnumerable<{t}>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new({parameterNames});

""");

            // sb2
            sb2.AppendLine($$"""
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct {{enumerableType}}<{{genericArguments}}>({{parameters}})
        : IValueEnumerable<{{enumerableElementType}}>
        where TEnumerator : struct, IValueEnumerable<{{t}}>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        public ValueEnumerator<{{enumerableType}}<{{genericArguments}}>, {{enumerableElementType}}> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            throw new NotImplementedException();
            // return source.TryGetNonEnumeratedCount(count);
            // count = 0;
            // return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<{{enumerableElementType}}> span)
        {
            throw new NotImplementedException();
            // span = default;
            // return false;
        }

        public bool TryGetNext(out {{enumerableElementType}} current)
        {
            throw new NotImplementedException();
            // Unsafe.SkipInit(out current);
            // return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }

""");
        }

        sb1.AppendLine("    }");
        sb1.AppendLine("}");
        sb2.AppendLine("}");

        var file = sb1.ToString() + Environment.NewLine + sb2.ToString();

        Console.WriteLine(file);
        File.WriteAllText(Path.Combine("linq1", fileName), file);
    }

    static void EmitOtherTemplate(IGrouping<string, MethodInfo> methods)
    {
        Directory.CreateDirectory("linq2");

        var className = methods.Key;
        var fileName = $"{className}.cs";

        var sb1 = new StringBuilder(); // ZLinq
        sb1.AppendLine("namespace ZLinq");
        sb1.AppendLine("{");
        sb1.AppendLine("    partial class ValueEnumerableExtensions");
        sb1.AppendLine("    {");

        var i = 0;
        foreach (var methodInfo in methods)
        {
            var suffix = (++i == 1) ? "" : i.ToString();

            if (!methodInfo.GetParameters()[0].ParameterType.IsGenericType) continue;

            var t = methodInfo.GetParameters()[0].ParameterType.GetGenericArguments()[0]; // this IEnumerable<T> source's T
                                                                                          // var enumerableType = $"{className}ValueEnumerable{suffix}";
            var baseGenericArguments = string.Join(", ", methodInfo.GetGenericArguments().Select(x => x.Name).ToArray());
            var genericArguments = "TEnumerator" + (baseGenericArguments == "" ? "" : $", {baseGenericArguments}");
            var baseParameters = string.Join(", ", methodInfo.GetParameters().Skip(1).Select(x => $"{FormatType(x.ParameterType)} {x.Name}").ToArray()); // skip TSource source
            var parameters = "TEnumerator source" + (baseParameters == "" ? "" : $", {baseParameters}");
            //var baseParameterNames = string.Join(", ", methodInfo.GetParameters().Skip(1).Select(x => $"{x.Name}"));
            //var parameterNames = "source" + (baseParameterNames == "" ? "" : $", {baseParameterNames}");
            //var enumerableElementType = methodInfo.ReturnType.GetGenericArguments()[0].Name;
            var returnType = FormatType(methodInfo.ReturnType);

            // sb1
            sb1.AppendLine($$"""
        public static {{returnType}} {{className}}<{{genericArguments}}>(this {{parameters}})
            where TEnumerator : struct, IValueEnumerable<{{t}}>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

""");
        }

        sb1.AppendLine("    }");
        sb1.AppendLine("}");

        var file = sb1.ToString();

        Console.WriteLine(file);
        File.WriteAllText(Path.Combine("linq2", fileName), file);
    }

    static bool IsExtensionMethod(MethodInfo methodInfo)
    {
        if (methodInfo == null) return false;
        return methodInfo.IsStatic && methodInfo.IsDefined(typeof(ExtensionAttribute), false);
    }

    static string FormatType(Type type)
    {
        if (!type.IsGenericType) return type.Name;

        var genericName = type.GetGenericTypeDefinition().Name.Split('`')[0];
        var typeArguments = type.GetGenericArguments();
        var arguments = string.Join(", ", typeArguments.Select(t => FormatType(t)).ToArray());

        return $"{genericName}<{arguments}>";
    }
}
