using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Compiler.Compilers.Java;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Infrastructure.Tests.Compiler
{
    public class JavaCompilerTest : CompilerTestBase
    {
        private static readonly IMock<ILogger<JavaCompiler>> Logger = new Mock<ILogger<JavaCompiler>>();

        [Fact]
        public async Task Compile_SingleFile()
        {
            var files = await LoadSourceFiles("./SourceFiles/Java/SingleFile");

            var compiler = new JavaCompiler(Logger.Object);

            var output = await compiler.Compile(files);

            output.Should().NotBeNull();
            output.Output.Should().BeEmpty();
            output.Files.Should().HaveCount(1);
            output.Files.Should().ContainSingle(file => file.Name == "Solution.class");
        }

        [Fact]
        public async Task Compile_NoSolution()
        {
            var files = new List<SourceFile>();

            var compiler = new JavaCompiler(Logger.Object);

            var output = await compiler.Compile(files);

            output.Should().NotBeNull();
            output.Success.Should().BeFalse();
            output.Files.Should().BeEmpty();
            output.Output.Should().Be("The source files do not contain Solution.java");
        }

        [Fact]
        public async Task Compile_MultipleFiles()
        {
            var files = await LoadSourceFiles("./SourceFiles/Java/MultipleFiles");
            
            var compiler = new JavaCompiler(Logger.Object);

            var output = await compiler.Compile(files);

            output.Should().NotBeNull();
            output.Success.Should().BeTrue();
            output.Files.Should().HaveCount(2);
            output.Files.Should().ContainSingle(f => f.Name == "Solution.class");
            output.Files.Should().ContainSingle(f => f.Name == "Helper.class");
        }

        [Fact]
        public async Task Compile_SyntaxError()
        {
            var files = await LoadSourceFiles("./SourceFiles/Java/SyntaxError");
            
            var compiler = new JavaCompiler(Logger.Object);

            var output = await compiler.Compile(files);

            output.Should().NotBeNull();
            output.Success.Should().BeFalse();
            output.Output.Should().NotBeNull();
        }


    }
}