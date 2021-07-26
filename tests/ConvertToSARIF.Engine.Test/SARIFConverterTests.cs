using Microsoft.CodeAnalysis.Sarif;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace ConvertToSARIF.Engine.Test
{
    public class SARIFConverterTests
    {
        private readonly  Mock<SARIFConverter> SARIFConverterMock;

        private SARIFConverter SARIFConverter => SARIFConverterMock.Object;


        public SARIFConverterTests()
        {
            SARIFConverterMock = new Mock<SARIFConverter>()
            {
                CallBase = true
            };
        }

        #region ConvertToSarifLog
        [Fact]
        [Trait("Category", "Unit")]
        public void ConvertToSarifLog_Nulls()
        {
            Assert.Throws<ArgumentNullException>(() => SARIFConverter.ConvertToSarifLog(null, null));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ConvertToSarifLog_Empty_DiagnosticRecords_Filter()
        {
            string ignorePattern = "vendor";

            SarifLog actualSarifLog = SARIFConverter.ConvertToSarifLog(new List<DiagnosticRecord>(), new Regex(ignorePattern));

            Assert.NotNull(actualSarifLog);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ConvertToSarifLog_Validation()
        {

            // This will also validate ordering of the diagnostic records
            var diagnosticRecords = new List<DiagnosticRecord>()
            {
                new DiagnosticRecord()
                {
                    Line = 1,
                    RuleName = "Write-Host",
                    Message = "Don't use Write-Host fake.ps1",
                    ScriptPath = "C:\\users\\johnsmith\\fake.ps1",
                    Severity = "Warning"
                },
                new DiagnosticRecord()
                {
                    Line = 1,
                    RuleName = "Write-Host",
                    Message = "Don't use Write-Host on file fake1.ps1",
                    ScriptPath = "C:\\users\\johnsmith\\fake1.ps1",
                    Severity = "Warning"
                }
            };

            SARIFConverterMock.Setup(x => x.HydrateArtifacts(
                It.IsAny<HashSet<Artifact>>(),
                It.IsAny<DiagnosticRecord>()))
                .CallBase();

            SARIFConverterMock.Setup(x => x.HydrateResults(
                It.IsAny<IList<Result>>(),
                It.IsAny<DiagnosticRecord>()))
                .CallBase();
            
            SARIFConverterMock.Setup(x => x.HydrateRules(
                It.IsAny<HashSet<ReportingDescriptor>>(),
                It.IsAny<DiagnosticRecord>()))
                .CallBase();
           
            SARIFConverterMock.Setup(x => x.ConstructSarifLog(
                It.IsAny<IList<Result>>(),
                It.IsAny<HashSet<Artifact>>(),
                It.IsAny<HashSet<ReportingDescriptor>>()))
                .CallBase();

            SarifLog actualSARIFLog = SARIFConverter.ConvertToSarifLog(diagnosticRecords, null);

            SARIFConverterMock.Verify(
                x => x.HydrateArtifacts(
                    It.IsAny<HashSet<Artifact>>(),
                    It.IsAny<DiagnosticRecord>()),
                Times.Exactly(diagnosticRecords.Count));

            SARIFConverterMock.Verify(
                x => x.HydrateResults(
                    It.IsAny<IList<Result>>(),
                    It.IsAny<DiagnosticRecord>()),
                Times.Exactly(diagnosticRecords.Count));

            SARIFConverterMock.Verify(
                x => x.HydrateRules(
                    It.IsAny<HashSet<ReportingDescriptor>>(),
                    It.IsAny<DiagnosticRecord>()),
                Times.Exactly(diagnosticRecords.Count));

            SARIFConverterMock.Verify(
                x => x.ConstructSarifLog(
                    It.IsAny<IList<Result>>(),
                    It.IsAny<HashSet<Artifact>>(),
                    It.IsAny<HashSet<ReportingDescriptor>>()),
                Times.Once);


            Assert.NotNull(actualSARIFLog);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ConvertToSarifLog_Validation_Filter()
        {

            // This will also validate ordering of the diagnostic records
            var diagnosticRecords = new List<DiagnosticRecord>()
            {
                new DiagnosticRecord()
                {
                    Line = 1,
                    RuleName = "Write-Host",
                    Message = "Don't use Write-Host fake.ps1",
                    ScriptPath = "C:\\users\\johnsmith\\fake.ps1",
                    Severity = "Warning"
                },
                 new DiagnosticRecord()
                {
                    Line = 1,
                    RuleName = "Write-Host",
                    Message = "Don't use Write-Host fake.ps1",
                    ScriptPath = "C:\\users\\johnsmith\\fake2.ps1",
                    Severity = "Warning"
                },
                new DiagnosticRecord()
                {
                    Line = 1,
                    RuleName = "Write-Host",
                    Message = "Don't use Write-Host on file fake1.ps1",
                    ScriptPath = "C:\\users\\johnsmith\\vendor\\fake1.ps1",
                    Severity = "Warning"
                }
            };

            int validDiagnosticRecords = 2;

            string ignorePattern = "vendor";

            SarifLog actualSARIFLog = SARIFConverter.ConvertToSarifLog(diagnosticRecords, new Regex(ignorePattern));

            SARIFConverterMock.Verify(
                x => x.HydrateArtifacts(
                    It.IsAny<HashSet<Artifact>>(),
                    It.IsAny<DiagnosticRecord>()),
                Times.Exactly(validDiagnosticRecords));

            SARIFConverterMock.Verify(
                x => x.HydrateResults(
                    It.IsAny<IList<Result>>(),
                    It.IsAny<DiagnosticRecord>()),
                Times.Exactly(validDiagnosticRecords));

            SARIFConverterMock.Verify(
                x => x.HydrateRules(
                    It.IsAny<HashSet<ReportingDescriptor>>(),
                    It.IsAny<DiagnosticRecord>()),
                Times.Exactly(validDiagnosticRecords));

            SARIFConverterMock.Verify(
                x => x.ConstructSarifLog(
                    It.IsAny<IList<Result>>(),
                    It.IsAny<HashSet<Artifact>>(),
                    It.IsAny<HashSet<ReportingDescriptor>>()),
                Times.Once);


            Assert.NotNull(actualSARIFLog);
        }

        #endregion

        #region ResolveSeverity
        [Theory]
        [Trait("Category", "Unit")]
        [InlineData("")]
        [InlineData(null)]
        public void ResolveSeverity_NullOrEmpty(string severity)
        {
            FailureLevel actualFailureLevel = SARIFConverter.ResolveSeverity(severity);

            Assert.Equal(FailureLevel.None, actualFailureLevel);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ResolveSeverity_Warning()
        {
            FailureLevel actualFailureLevel = SARIFConverter.ResolveSeverity("Warning");

            Assert.Equal(FailureLevel.Warning, actualFailureLevel);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public void ResolveSeverity_Error()
        {
            FailureLevel actualFailureLevel = SARIFConverter.ResolveSeverity("Error");

            Assert.Equal(FailureLevel.Error, actualFailureLevel);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ResolveSeverity_Information()
        {
            FailureLevel actualFailureLevel = SARIFConverter.ResolveSeverity("Information");

            Assert.Equal(FailureLevel.Note, actualFailureLevel);
        }


        [Fact]
        [Trait("Category", "Unit")]
        public void ResolveSeverity_ParseError()
        {
            FailureLevel actualFailureLevel = SARIFConverter.ResolveSeverity("ParseError");

            Assert.Equal(FailureLevel.Error, actualFailureLevel);
        }
        #endregion

        #region HydrateArtifacts

        [Fact]
        [Trait("Category", "Unit")]
        public void HydrateArtifacts_Null_DiagnosticRecord()
        {
            var artifacts = new HashSet<Artifact>();

            Assert.Throws<ArgumentNullException>(() => SARIFConverter.HydrateArtifacts(artifacts, null));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void HydrateArtifacts_Null_Artifacts()
        {
            var diagnosticError = new DiagnosticRecord();

            Assert.Throws<ArgumentNullException>(() => SARIFConverter.HydrateArtifacts(null, diagnosticError));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void HydrateArtifacts_Both_Null()
        {
            Assert.Throws<ArgumentNullException>(() => SARIFConverter.HydrateArtifacts(null, null));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void HydrateArtifacts_Added()
        {
            var artifacts = new HashSet<Artifact>();
            var diagnosticRecord = new DiagnosticRecord()
            {
                ScriptPath = Directory.GetCurrentDirectory()
            };

            SARIFConverter.HydrateArtifacts(artifacts, diagnosticRecord);

            var expectedArtifact = new Artifact()
            {
                Location = new ArtifactLocation()
                {
                    Uri = new Uri(diagnosticRecord.ScriptPath)
                }
            };

            Assert.Contains(expectedArtifact, artifacts, Artifact.ValueComparer);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void HydrateArtifacts_Null_ScriptPath()
        {
            var artifacts = new HashSet<Artifact>();
            var diagnosticRecord = new DiagnosticRecord()
            {
                ScriptPath = null
            };

            Assert.Throws<ArgumentNullException>(() => SARIFConverter.HydrateArtifacts(artifacts, diagnosticRecord));
        }


        [Fact]
        [Trait("Category", "Unit")]
        public void HydrateArtifacts_Invalid_ScriptPath()
        {
            var artifacts = new HashSet<Artifact>();
            var diagnosticRecord = new DiagnosticRecord()
            {
                ScriptPath = "This is invalid format"
            };

            Assert.Throws<UriFormatException>(() => SARIFConverter.HydrateArtifacts(artifacts, diagnosticRecord));
        }
        #endregion

        #region HydrateRules

        [Fact]
        [Trait("Category", "Unit")]
        public void HydrateRules_Null_ReportingDescriptor()
        {
            var rules = new HashSet<ReportingDescriptor>();

            Assert.Throws<ArgumentNullException>(() => SARIFConverter.HydrateRules(rules, null));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void HydrateRules_Null_Rules()
        {
            var diagnosticRecord = new DiagnosticRecord();

            Assert.Throws<ArgumentNullException>(() => SARIFConverter.HydrateRules(null, diagnosticRecord));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void HydrateRules_Both_Null()
        {
            Assert.Throws<ArgumentNullException>(() => SARIFConverter.HydrateRules(null, null));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void HydrateRules_Added()
        {
            var rules = new HashSet<ReportingDescriptor>();

            var diagnosticRecord = new DiagnosticRecord()
            {
                RuleName = "Write-Host"
            };

            SARIFConverter.HydrateRules(rules, diagnosticRecord);

            var expectedRule = new ReportingDescriptor()
            {
                Id = diagnosticRecord?.RuleName,
                Name = diagnosticRecord?.RuleName,

                HelpUri = new Uri("https://github.com/PowerShell/Psscriptanalyzer")
            };

            Assert.Contains(expectedRule, rules, ReportingDescriptor.ValueComparer);
        }

        [Theory]
        [Trait("Category", "Unit")]
        [InlineData("")]
        [InlineData(null)]
        public void HydrateRules_NullOrEmptyRuleName(string ruleName)
        {
            var rules = new HashSet<ReportingDescriptor>();

            var diagnosticRecord = new DiagnosticRecord()
            {
                RuleName = ruleName
            };

            SARIFConverter.HydrateRules(rules, diagnosticRecord);

            // It should procude nullable or empty id object
            var expectedRule = new ReportingDescriptor()
            {
                Id = diagnosticRecord?.RuleName,
                Name = diagnosticRecord?.RuleName,
                HelpUri = new Uri("https://github.com/PowerShell/Psscriptanalyzer")
            };

            Assert.Contains(expectedRule, rules, ReportingDescriptor.ValueComparer);
        }
        #endregion

        #region HydrateResults
        [Fact]
        [Trait("Category", "Unit")]
        public void HydrateResults_Nulls()
        {
            Assert.Throws<ArgumentNullException>(() => SARIFConverter.HydrateResults(null, null));
            Assert.Throws<ArgumentNullException>(() => SARIFConverter.HydrateResults(new List<Result>(), null));
            Assert.Throws<ArgumentNullException>(() => SARIFConverter.HydrateResults(null, new DiagnosticRecord()));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void HydrateResults_Added()
        {
            var diagnosticRecord = new DiagnosticRecord()
            {
                Line = 1,
                Message = "Dont use Write-Host",
                RuleName = "Write-Host",
                ScriptPath = "C:\\users\\johnsmith",
                Severity = "Warning"
            };

            var expectedResult = new Result()
            {
                RuleId = diagnosticRecord.RuleName,
                Level = FailureLevel.Warning,
                Message = new Message()
                {
                    Text = diagnosticRecord.Message
                },
                Locations = new List<Location>()
                {
                    new Location()
                    {
                        PhysicalLocation = new PhysicalLocation()
                        {
                            ArtifactLocation = new ArtifactLocation()
                            {
                                Uri = new Uri(diagnosticRecord.ScriptPath)
                            },
                            Region = new Region()
                            {
                                StartLine = diagnosticRecord.Line,
                                EndColumn = 0
                            }
                        }
                    }
                },
                Suppressions = new List<Suppression>()
            };

            var results = new List<Result>();

            SARIFConverterMock.Setup(x => x.ResolveSeverity(diagnosticRecord.Severity)).CallBase();

            SARIFConverter.HydrateResults(results, diagnosticRecord);

            SARIFConverterMock.Verify(x => x.ResolveSeverity(diagnosticRecord.Severity), Times.Once);

            Assert.Contains(expectedResult, results, Result.ValueComparer);
        }
        #endregion

        #region ConstructSarifLog
        [Fact]
        [Trait("Category", "Unit")]
        public void ConstructSarifLog_Nulls()
        {
            Assert.Throws<ArgumentNullException>(() => SARIFConverter.ConstructSarifLog(null, null, null));
            Assert.Throws<ArgumentNullException>(() => SARIFConverter.ConstructSarifLog(new List<Result>(), null, null));
            Assert.Throws<ArgumentNullException>(() => SARIFConverter.ConstructSarifLog(null, new HashSet<Artifact>(), null));
            Assert.Throws<ArgumentNullException>(() => SARIFConverter.ConstructSarifLog(null, null, new HashSet<ReportingDescriptor>()));
            Assert.Throws<ArgumentNullException>(() => SARIFConverter.ConstructSarifLog(new List<Result>(), new HashSet<Artifact>(), null));
            Assert.Throws<ArgumentNullException>(() => SARIFConverter.ConstructSarifLog(new List<Result>(), null, new HashSet<ReportingDescriptor>()));
            Assert.Throws<ArgumentNullException>(() => SARIFConverter.ConstructSarifLog(null, new HashSet<Artifact>(), new HashSet<ReportingDescriptor>()));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ConstructSarifLog_Empty()
        {
            var expectedSARIFLog = new SarifLog()
            {
                Version = SarifVersion.Current,
                SchemaUri = new Uri(ToolInformation.SARIFSchemaUri),
                Runs = new List<Run>()
                {
                    new Run()
                    {
                        Tool = new Tool()
                        {
                            Driver = new ToolComponent()
                            {
                                Name = ToolInformation.ToolName,
                                InformationUri = new Uri(ToolInformation.ToolUri),
                                Rules = new List<ReportingDescriptor>()
                            }
                        },
                        Artifacts = new List<Artifact>(),
                        Results = new List<Result>()
                    }
                }

            };

            SarifLog actualSARIFLog = SARIFConverter.ConstructSarifLog(new List<Result>(), new HashSet<Artifact>(), new HashSet<ReportingDescriptor>());

            Assert.Equal(expectedSARIFLog, actualSARIFLog, SarifLog.ValueComparer);
        }
        #endregion
    }
}
