using Entities;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;

namespace Tests;

public class ArchitectureTests
{
    [Fact]
    public async Task EntityPropertyAssignedInUseCasesProject()
    {
        var parts = Assembly.GetExecutingAssembly().Location.Split('\\').SkipLast(5).ToArray();
        var slnFolderPath = Path.Join(parts);
        var slnPath = Directory.GetFiles(slnFolderPath, "*.sln", SearchOption.TopDirectoryOnly).FirstOrDefault();
        Assert.NotNull(slnPath);

        if (!MSBuildLocator.IsRegistered)
        {
            var instances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            MSBuildLocator.RegisterInstance(instances.OrderByDescending(x => x.Version).First());
        }

        var workspace = MSBuildWorkspace.Create();

        var solution = await workspace.OpenSolutionAsync(slnPath!);

        var entitiesAssemblyName = typeof(Product).Assembly.GetName().Name;

        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync();

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(syntaxTree);

                var assignments = syntaxTree.GetRoot().DescendantNodes()
                    .OfType<AssignmentExpressionSyntax>()
                    .ToArray();

                foreach (var assignment in assignments)
                {
                    if (assignment.Left is MemberAccessExpressionSyntax memberAccess)
                    {
                        var moduleInfo = semanticModel.GetSymbolInfo(memberAccess);
                        var containingModule = moduleInfo.Symbol.ContainingModule.Name.Replace(".dll", "");

                        Assert.False(project.Name != entitiesAssemblyName && containingModule == entitiesAssemblyName, $"Property of domain entity assigned in {project.Name} assembly: {assignment}");
                    }
                }
            }

        }
    }
}