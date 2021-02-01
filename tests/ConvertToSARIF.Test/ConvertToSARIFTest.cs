using System;
using Xunit;
using ConvertToSARIF;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;
using ConvertToSARIF.Engine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Management.Automation.Language;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.CodeAnalysis.Sarif;
using System.Runtime.InteropServices;

namespace ConvertToSARIF.Test
{
    public class ConvertToSARIFTest
    {
        [Fact]
        [Trait("Category", "Integration")]
        public void ConvertToSARIF_Basic()
        {
            PublishModule();

            string testPath = @".\data\Basic\";
            Collection<PSObject> result;

            Runspace runspace = InitializeRunspace();

            runspace.Open();

            using (PowerShell ps = PowerShell.Create(runspace))
            {
                ps.Commands.AddScript($@"Invoke-ScriptAnalyzer {testPath} | ConvertTo-SARIF -FilePath {testPath}\actual.sarif");
                result = ps.Invoke();
            };

            runspace.Close();
            
            Assert.True(File.Exists(testPath + @"\actual.sarif"));
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void ConvertToSARIF_GithubActions()
        {
            PublishModule();

            string testPath = @".\data\GithubActions";
            string sourcePath = testPath + @"\src";

            CloneProject(@"https://github.com/actions/virtual-environments", sourcePath);

            Runspace runspace = InitializeRunspace();

            runspace.Open();

            using (PowerShell ps = PowerShell.Create(runspace))
            {
                ps.Commands.AddScript($@"Invoke-ScriptAnalyzer {sourcePath} -Recurse | ConvertTo-SARIF -FilePath {testPath}\actual.sarif");
                ps.Invoke();
            };

            runspace.Close();
            
            // Assert that the file has been created thus the run was successfull
            Assert.True(File.Exists(testPath + @"\actual.sarif"));
        }

        private void CloneProject(string url, string outputDirectory)
        {
            Runspace runspace = RunspaceFactory.CreateRunspace();
            
            runspace.Open();

            using (PowerShell ps = PowerShell.Create(runspace))
            {
                ps.Commands.AddScript(@"git clone " + url + " " + outputDirectory);
                var results = ps.Invoke();
            }

            runspace.Close();
        } 

        private Runspace InitializeRunspace(string directory = "ConvertToSARIFModule")
        {
            InitialSessionState initialSessionState = InitialSessionState.CreateDefault();
           
            // The command will fail for linux and macOS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                initialSessionState.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Unrestricted;
            }

            var fullPath = Path.GetFullPath($"{directory}/ConvertToSARIF.dll");

            initialSessionState.ImportPSModule(fullPath);

            Runspace runspace = RunspaceFactory.CreateRunspace(initialSessionState);

            return runspace;
        }

        private void PublishModule(string output = "ConvertToSARIFModule")
        {
            Runspace runspace = RunspaceFactory.CreateRunspace();

            runspace.Open();

            using (PowerShell ps = PowerShell.Create(runspace))
            {
                ps.Commands.AddScript(@"dotnet publish ..\..\..\..\..\src\ConvertToSARIF\ConvertToSARIF.csproj -o " + output);
                ps.Invoke();
            }

            runspace.Close();
        }
    }
}
