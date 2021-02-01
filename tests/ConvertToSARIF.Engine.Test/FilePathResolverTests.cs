using ConvertToSARIF.Engine;
using Moq;
using System;
using System.IO;
using Xunit;

namespace ConvertToSARIF.Engine.Test
{
    public class FilePathResolverTests
    {
        private readonly IMock<FilePathResolver> FilePathResolverMock;
        private FilePathResolver FilePathResolver => FilePathResolverMock.Object;

        public FilePathResolverTests()
        {
            FilePathResolverMock = new Mock<FilePathResolver>
            {
                CallBase = true
            };
        }

        #region FilePath
        [Fact]
        [Trait("Category", "Unit")]
        public void FilePath_Resolver_BothNullable()
        {
            string actualMessage = Assert.Throws<FileNotFoundException>( () => FilePathResolver.Resolver(null, null)).Message;

            Assert.Equal(Properties.Resources.FileNotProvided, actualMessage);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void FilePath_Resolver_CurrentPathNullable()
        {
            string filePath = "fake.sarif";

            string actual = FilePathResolver.Resolver(filePath, null);

            Assert.Equal(filePath, actual);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public void FilePath_Resolver_FilePathNullable()
        {
            string currentPath = @"C:\Users\johnsmith";

            string actualMessage = Assert.Throws<FileNotFoundException>(() => FilePathResolver.Resolver(null, currentPath)).Message;

            Assert.Equal(Properties.Resources.FileNotProvided, actualMessage);
        }
        #endregion
    }
}
