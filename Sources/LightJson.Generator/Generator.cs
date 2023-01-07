using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LightJson.Generator;

[Generator]
public sealed class JsonDeserializerGenerator : IIncrementalGenerator
{
    private static string TypeCheckFunctionCall(SupportedTypes type) 
    {
        return type switch 
        {
            SupportedTypes.Int => ".ConvertToArrayInt()",
            SupportedTypes.String => ".ConvertToArrayString()",
            SupportedTypes.Boolean => ".ConvertToArrayBoolean()",
            SupportedTypes.Float => ".ConvertToArrayFloat()",
            SupportedTypes.Double => ".ConvertToArrayDouble()",
            SupportedTypes.Char => ".ConvertToArrayChar()",
            // ListArrays
            SupportedTypes.ListInt => ".ConvertToIntList()",
            SupportedTypes.ListString => ".ConvertToStringList()",
            SupportedTypes.ListBoolean => ".ConvertToBooleanList()",
            SupportedTypes.ListFloat => ".ConvertToFloatList()",
            SupportedTypes.ListDouble => ".ConvertToDoubleList()",
            SupportedTypes.ListOther => ".ConvertToList()",
            //2D Arrays
            SupportedTypes.Int2D => ".ConvertToArrayInt2D()",
            SupportedTypes.String2D => ".ConvertToArrayString2D()",
            SupportedTypes.Boolean2D => ".ConvertToArrayBoolean2D()",
            SupportedTypes.Float2D => ".ConvertToArrayFloat2D()",
            SupportedTypes.Double2D => ".ConvertToArrayDouble2D()",
            SupportedTypes.Char2D => ".ConvertToArrayChar2D()",
            _ => throw new System.NotImplementedException()
        };
    }

    private static bool JDictionary(AttributeData attr) 
    {
        if (!attr.NamedArguments.IsEmpty) 
        {
            var args = attr.NamedArguments;
            foreach (var arg in args)
            {
                var typedConstant = arg.Value;
                if (arg.Key == "Dynamic")
                    return (bool)typedConstant.Value;
            }
        }
        return false;
    }

    private static SupportedTypes JArray(AttributeData attr) 
    {
        SupportedTypes type = SupportedTypes.Int;
        if (!attr.ConstructorArguments.IsEmpty) 
        {
            var args = attr.ConstructorArguments;
            if (args.Length == 1)
                type = (SupportedTypes)args[0].Value;
        }

        if (!attr.NamedArguments.IsEmpty) 
        {
            var args = attr.NamedArguments;
            foreach (var arg in args)
            {
                var typedConstant = arg.Value;
                if (arg.Key == "Type")
                    type = (SupportedTypes)typedConstant.Value;
            }
        }
        return type;
    }

    private static string JName(string name, AttributeData attr)
    {
        if (!attr.ConstructorArguments.IsEmpty)
        {
            var args = attr.ConstructorArguments;
            if (args.Length == 1)
                name = (string)args[0].Value;
        }

        if (!attr.NamedArguments.IsEmpty)
        {
            var args = attr.NamedArguments;
            foreach (var arg in args)
            {
                var typedConstant = arg.Value;
                if (arg.Key == "JsonName")
                    name = (string)typedConstant.Value;
            }
        }

        return name;
    }

    private static bool CheckIfDeserializable(ISymbol symbol, INamedTypeSymbol json) 
    {
        if (symbol is IPropertySymbol prop) 
        {
            if (prop.Type.Interfaces.Any(x => x.Name == "IJsonDeserializable") || 
            prop.Type.GetAttributes().Any(x => x.AttributeClass.Name == "JsonSerializableAttribute"))
            {
                return true;
            }
        }
        else if (symbol is IFieldSymbol field) 
        {
            if (field.Type.Interfaces.Any(x => x.Name == "IJsonDeserializable") || 
            field.Type.GetAttributes().Any(x => x.AttributeClass.Name == "JsonSerializableAttribute"))
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<INamedTypeSymbol> GetSymbols(Compilation compilation, ImmutableArray<TypeDeclarationSyntax> syn) 
    {
        var partialClasses = syn;
        // var partialClasses = ((SyntaxReceiver)context.SyntaxReceiver).PartialClasses;

        foreach (var part in partialClasses) 
        {
            var model = compilation.GetSemanticModel(part.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(part);

            if (HasAttribute(symbol, "JsonSerializable")) 
            {
                yield return symbol;
            }
        }
    }

    private static bool HasAttribute(ISymbol symbol, string attributeName) 
        => symbol.GetAttributes().Any(attr => attr.AttributeClass.Name.StartsWith(attributeName));


    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // context.RegisterPostInitializationOutput(
        //     ctx => ctx.AddSource("JsonSerializableAttribute.g.cs", JsonSerializableAttribute));
        var typeProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
            (node, _) => node is TypeDeclarationSyntax syntax 
                && syntax.AttributeLists.Count > 0
                && syntax.Modifiers.Any(SyntaxKind.PartialKeyword),

            (ctx, _) => 
            {
                var jsonSerializableSyntax = (TypeDeclarationSyntax)ctx.Node;
                foreach (var attribListSyntax in jsonSerializableSyntax.AttributeLists) 
                {
                    foreach (var attribSyntax in attribListSyntax.Attributes) 
                    {
                        if (ctx.SemanticModel.GetSymbolInfo(attribSyntax).Symbol is not IMethodSymbol attribSymbol) 
                        {
                            continue;
                        }


                        var namedTypedSymbol = attribSymbol.ContainingType;
                        var fullName = attribSymbol.ToDisplayString();
                        if (fullName == "LightJson.Serialization.JsonSerializableAttribute.JsonSerializableAttribute()")
                            return jsonSerializableSyntax;
                    }
                }

                return null;
            })
            .Where(m => m is not null);
        
        var compilation = context.CompilationProvider.Combine(typeProvider.Collect());
        
        context.RegisterSourceOutput(compilation, (ctx, source) => Generate(ctx, source.Left, source.Right));
        // context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        // throw new NotImplementedException();
    }

    private static void Generate(SourceProductionContext ctx, Compilation comp, ImmutableArray<TypeDeclarationSyntax> syn) 
    {
        if (syn.IsDefaultOrEmpty)
            return;

        var jsonAttribute = comp.GetTypeByMetadataName("LightJson.Serialization.JsonSerializable");

        foreach (var symbol in GetSymbols(comp, syn)) 
        {
            var members = symbol.GetMembers().OfType<ISymbol>().ToList();

            var sb = new ValueStringBuilder(64);
            sb.AppendLine("//Source Generated Code");
            sb.AppendLine("using System;");
            sb.AppendLine("using LightJson;");
            sb.AppendLine("");

            sb.AppendLine($"namespace {symbol.GetSymbolNamespace()};");

            sb.AppendLine($"partial {symbol.ClassOrStruct()} {symbol.Name} : IJsonDeserializable");
            sb.AppendLine("{");
            
            sb.AppendLine("public void Deserialize(JsonObject obj)");
            sb.AppendLine("{");
            foreach (var sym in members) 
            {
                if (sym is not IPropertySymbol && sym is not IFieldSymbol)
                    continue;

                if (sym is IFieldSymbol && !sym.HasAttributeName("JObject"))
                    continue;

                var name = sym.Name;
                SupportedTypes arrayType = SupportedTypes.Int; 
                var additionalFunctionCall = "";

                // Get the Attribute
                foreach (var attr in sym.GetAttributes()) 
                {
                    if (attr.AttributeClass.Name == "JIgnoreAttribute")
                        goto Ignore;

                    else if (attr.AttributeClass.Name == "JNameAttribute")
                    {
                        name = JName(name, attr);
                    } 

                    else if (attr.AttributeClass.Name == "JDictionaryAttribute") 
                    {
                        if (JDictionary(attr)) 
                        {
                            additionalFunctionCall = ".ToDictionary()";
                        } else 
                        {
                            var result = new ValueStringBuilder();
                            if (sym is IFieldSymbol field) 
                            {
                                var fieldType = field.Type.ToDisplayString().AsSpan();
                                var pr = field.Type.ToDisplayParts(SymbolDisplayFormat.FullyQualifiedFormat);
                                int i = 0;
                                while (pr[i].ToString() != "string") 
                                {
                                    result.Append(pr[i].ToString());
                                    i++;
                                }
                                var disp = fieldType.Slice(result.Length).TrimEnd('>');
                                
                                additionalFunctionCall = $".ToDictionary<{new string(disp)}>()";
                            } else if (sym is IPropertySymbol prop) 
                            {
                                var propType = prop.Type.ToDisplayString().AsSpan();
                                var pr = prop.Type.ToDisplayParts(SymbolDisplayFormat.FullyQualifiedFormat);
                                int i = 0;
                                while (pr[i].ToString() != "string") 
                                {
                                    result.Append(pr[i].ToString());
                                    i++;
                                }
                                var disp = propType.Slice(result.Length).TrimEnd('>');
                                
                                additionalFunctionCall = $".ToDictionary<{new string(disp)}>()";
                            }

                        }
                    }

                    else if (attr.AttributeClass.Name == "JArrayAttribute") 
                    {
                        arrayType = JArray(attr);
                        if (arrayType == SupportedTypes.Other2D) 
                        {
                            if (sym is IPropertySymbol prop) 
                            {
                                var propType = prop.Type.ToDisplayString();
                                var pr = propType.Substring(0, propType.Length - 3);
                                additionalFunctionCall = $".ConvertToArray<{pr}>()";
                            } else if (sym is IFieldSymbol field) 
                            {
                                var propType = field.Type.ToDisplayString();
                                var pr = propType.Substring(0, propType.Length - 3);
                                additionalFunctionCall = $".ConvertToArray<{pr}>()";
                            }

                        }
                        else if (arrayType == SupportedTypes.Other) 
                        {
                            if (sym is IFieldSymbol field) 
                            {
                                var propType = field.Type.ToDisplayString();
                                var pr = propType.Substring(0, propType.Length - 2);
                                additionalFunctionCall = $".ConvertToArray<{pr}>()";
                            } else if (sym is IPropertySymbol prop) 
                            {
                                var propType = prop.Type.ToDisplayString();
                                var pr = propType.Substring(0, propType.Length - 2);
                                additionalFunctionCall = $".ConvertToArray<{pr}>()";
                            }
                        }
                        else if (arrayType == SupportedTypes.ListOther) 
                        {
                            var result = new ValueStringBuilder();
                            if (sym is IFieldSymbol field) 
                            {
                                var propType = field.Type.ToDisplayString().AsSpan();
                                var pr = field.Type.ToDisplayParts(SymbolDisplayFormat.FullyQualifiedFormat);
                                int i = 0;
                                while (pr[i].ToString() != "<") 
                                {
                                    result.Append(pr[i].ToString());
                                    i++;
                                }
                                var disp = propType.Slice(result.Length).TrimEnd('>');
                                
                                additionalFunctionCall = $".ConvertToList<{new string(disp)}>()";
                            } else if (sym is IPropertySymbol prop) 
                            {
                                var propType = prop.Type.ToDisplayString().AsSpan();
                                var pr = prop.Type.ToDisplayParts(SymbolDisplayFormat.FullyQualifiedFormat);
                                int i = 0;
                                while (pr[i].ToString() != "<") 
                                {
                                    result.Append(pr[i].ToString());
                                    i++;
                                }
                                var disp = propType.Slice(result.Length).TrimEnd('>');
                                
                                additionalFunctionCall = $".ConvertToList<{new string(disp)}>()";
                            }
                        }
                        else 
                        {
                            additionalFunctionCall = TypeCheckFunctionCall(arrayType);
                        }

                    }

                }
                // Check if its deserializable
                if (CheckIfDeserializable(sym, jsonAttribute)) 
                {
                    if (sym is IFieldSymbol field) 
                    {
                        additionalFunctionCall = $".Convert<{field.Type}>()";
                    }
                    else if (sym is IPropertySymbol prop) 
                    {
                        additionalFunctionCall = $".Convert<{prop.Type}>()";
                    }

                }
                sb.AppendLine($"{sym.Name} = obj[\"{name}\"]{additionalFunctionCall};");
                Ignore:
                sb.Append("");
            }

            sb.AppendLine("}");

            sb.AppendLine("}");

            ctx.AddSource($"{symbol.Name}.g.cs", sb.ToString());
        }
    }

    public class SyntaxReceiver : ISyntaxReceiver
    {
        public List<TypeDeclarationSyntax> PartialClasses = new();
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is TypeDeclarationSyntax syntax 
                && syntax.AttributeLists.Count > 0
                && syntax.Modifiers.Any(SyntaxKind.PartialKeyword)) 
            {
                PartialClasses.Add(syntax);
            }
        }
    }
}

public enum SupportedTypes 
{
    Int, Boolean, Float, Double, Char, String, Other,
    Int2D, Boolean2D, Float2D, Double2D, Char2D, String2D, Other2D,
    ListInt, ListBoolean, ListFloat, ListDouble, ListString, ListOther
}