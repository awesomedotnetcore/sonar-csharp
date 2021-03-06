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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using SonarAnalyzer.Common;
using SonarAnalyzer.Common.VisualBasic;
using SonarAnalyzer.Helpers;

namespace SonarAnalyzer.Rules.VisualBasic
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    [Rule(DiagnosticId)]
    public sealed class FunctionComplexity : FunctionComplexityBase
    {
        private static readonly DiagnosticDescriptor rule =
            DiagnosticDescriptorBuilder.GetDescriptor(DiagnosticId, MessageFormat, RspecStrings.ResourceManager,
                isEnabledByDefault: false);
        protected override DiagnosticDescriptor Rule => rule;

        protected override void Initialize(ParameterLoadingAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionInNonGenerated(
                c => CheckComplexity<MethodBlockBaseSyntax>(c, m => m.BlockStatement.GetLocation(), "procedure"),
                SyntaxKind.SubBlock);

            context.RegisterSyntaxNodeActionInNonGenerated(
                c => CheckComplexity<MethodBlockBaseSyntax>(c, m => m.BlockStatement.GetLocation(), "function"),
                SyntaxKind.FunctionBlock);

            context.RegisterSyntaxNodeActionInNonGenerated(
                c => CheckComplexity<MethodBlockBaseSyntax>(c, m => m.BlockStatement.GetLocation(), "constructor"),
                SyntaxKind.ConstructorBlock);

            context.RegisterSyntaxNodeActionInNonGenerated(
                c => CheckComplexity<OperatorBlockSyntax>(c, m => m.OperatorStatement.GetLocation(), "operator"),
                SyntaxKind.OperatorBlock);

            context.RegisterSyntaxNodeActionInNonGenerated(
                c => CheckComplexity<AccessorBlockSyntax>(c, m => m.AccessorStatement.GetLocation(), "accessor"),
                SyntaxKind.GetAccessorBlock,
                SyntaxKind.SetAccessorBlock,
                SyntaxKind.AddHandlerAccessorBlock,
                SyntaxKind.RemoveHandlerAccessorBlock);
        }

        protected override int GetComplexity(SyntaxNode node) =>
            new Metrics(node.SyntaxTree).GetComplexity(node);

        protected override sealed GeneratedCodeRecognizer GeneratedCodeRecognizer =>
            Helpers.VisualBasic.GeneratedCodeRecognizer.Instance;
    }
}
