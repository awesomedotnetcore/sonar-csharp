﻿/*
 * SonarAnalyzer for .NET
 * Copyright (C) 2015-2018 SonarSource SA
 * mailto: contact AT sonarsource DOT com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SonarAnalyzer.Common;
using SonarAnalyzer.Helpers;

namespace SonarAnalyzer.Rules.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [Rule(DiagnosticId)]
    public sealed class SpecifyStringComparison : SonarDiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S4058";
        private const string MessageFormat = "Change this call to '{0}' to an overload that accepts a " +
            "'StringComparison' as a parameter.";

        private static readonly DiagnosticDescriptor rule =
            DiagnosticDescriptorBuilder.GetDescriptor(DiagnosticId, MessageFormat, RspecStrings.ResourceManager);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(rule);

        protected override void Initialize(SonarAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionInNonGenerated(
                c =>
                {
                    var invocation = (InvocationExpressionSyntax)c.Node;

                    if (invocation.Expression != null &&
                        IsInvalidCall(invocation.Expression, c.SemanticModel) &&
                        HasOverloadWithStringComparison(invocation.Expression, c.SemanticModel))
                    {
                        c.ReportDiagnosticWhenActive(Diagnostic.Create(rule, invocation.GetLocation(), invocation.Expression));
                    }
                }, SyntaxKind.InvocationExpression);
        }

        private static bool HasOverloadWithStringComparison(ExpressionSyntax expression, SemanticModel semanticModel)
        {
            return semanticModel.GetMemberGroup(expression)
                .OfType<IMethodSymbol>()
                .Where(m => !m.IsObsolete())
                .Any(HasAnyStringComparisonParameter);
        }

        private static bool IsInvalidCall(ExpressionSyntax expression, SemanticModel semanticModel)
        {

            return semanticModel.GetSymbolInfo(expression).Symbol is IMethodSymbol methodSymbol &&
                !HasAnyStringComparisonParameter(methodSymbol) &&
                methodSymbol.GetParameters().Any(parameter => parameter.Type.Is(KnownType.System_String)) &&
                !SpecifyIFormatProviderOrCultureInfo.HasAnyFormatOrCultureParameter(methodSymbol);
        }

        public static bool HasAnyStringComparisonParameter(IMethodSymbol method)
        {
            return method.GetParameters()
                .Any(parameter => parameter.Type.Is(KnownType.System_StringComparison));
        }
    }
}
