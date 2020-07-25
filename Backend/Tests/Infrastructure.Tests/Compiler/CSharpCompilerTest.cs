using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Compiler.Compilers.CSharp;
using Infrastructure.Compiler.Compilers.Java;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Infrastructure.Tests.Compiler
{
    public class CSharpCompilerTest : CompilerTestBase
    {
        private static readonly IMock<ILogger<CSharpCompiler>> Logger = new Mock<ILogger<CSharpCompiler>>();
        
        [Fact]
        public async Task Compile_SingleFile()
        {
            var files = await LoadSourceFiles("./SourceFiles/CSharp/SingleFile");
            
            var compiler = new CSharpCompiler(Logger.Object);

            var output = await compiler.Compile(files);

            output.Should().NotBeNull();
            output.Success.Should().BeTrue();
            output.Output.Should().BeEmpty();
            output.Files.Should().HaveCount(2);
            output.Files.Should().ContainSingle(file => file.Name == "Solution.dll");
            output.Files.Should().ContainSingle(file => file.Name == "Solution.runtimeconfig.json");
        }

        [Fact]
        public async Task Compile_NoSolution()
        {
            var files = new List<SourceFile>();

            var compiler = new CSharpCompiler(Logger.Object);

            var output = await compiler.Compile(files);

            output.Should().NotBeNull();
            output.Success.Should().BeFalse();
            output.Files.Should().BeEmpty();
            output.Output.Should().Be("The source files do not contain Solution.cs");
        }
        
        [Fact]
        public async Task Compile_MultipleFiles()
        {
            var files = await LoadSourceFiles("./SourceFiles/CSharp/MultipleFiles");
            
            var compiler = new CSharpCompiler(Logger.Object);

            var output = await compiler.Compile(files);

            output.Should().NotBeNull();
            output.Success.Should().BeTrue();
            output.Files.Should().HaveCount(2);
            output.Files.Should().ContainSingle(f => f.Name == "Solution.dll");
            output.Files.Should().ContainSingle(file => file.Name == "Solution.runtimeconfig.json");
        }
        
        [Fact]
        public async Task Compile_SyntaxError()
        {
            var files = await LoadSourceFiles("./SourceFiles/CSharp/SyntaxError");
            
            var compiler = new CSharpCompiler(Logger.Object);

            var output = await compiler.Compile(files);

            output.Should().NotBeNull();
            output.Success.Should().BeFalse();
            output.Output.Should().NotBeNull();
        }
    }
}