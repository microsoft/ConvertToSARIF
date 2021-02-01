using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Sarif;
using System.Text.RegularExpressions;

namespace ConvertToSARIF.Engine
{
    public class SARIFConverter
    {
        public virtual SarifLog ConvertToSarifLog(IEnumerable<DiagnosticRecord> diagnosticRecords, Regex filterRegex)
        {
            if (diagnosticRecords is null)
            {
                throw new ArgumentNullException(nameof(diagnosticRecords));
            }

            if(filterRegex != null)
            {
                diagnosticRecords = diagnosticRecords.Where(x => !filterRegex.IsMatch(x.ScriptPath));
            }

            // PSScriptAnalyzer will give differently ordered results for the same codebase so 
            // we have to sort the results for equality reasons.
            diagnosticRecords = diagnosticRecords
                .OrderBy(x => x.ScriptPath)
                .ThenBy(x => x.Line)
                .ThenBy(x => x.Severity)
                .ThenBy(x => x.RuleName)
                .ThenBy(x => x.Message);

            var artifacts = new HashSet<Artifact>(Artifact.ValueComparer);
            var rules = new HashSet<ReportingDescriptor>(ReportingDescriptor.ValueComparer);
            var results = new List<Result>();

            foreach (var diagnosticRecord in diagnosticRecords)
            {
                HydrateArtifacts(artifacts, diagnosticRecord);

                HydrateRules(rules, diagnosticRecord);

                HydrateResults(results, diagnosticRecord);
            }

            SarifLog log = ConstructSarifLog(results, artifacts, rules);
            
            return log;
        }

        public virtual FailureLevel ResolveSeverity(string severity)
        {
            switch (severity)
            {
                case "Information":
                    return FailureLevel.Note;
                case "Warning":
                    return FailureLevel.Warning;
                case "Error":
                    return FailureLevel.Error;
                case "ParseError":
                    return FailureLevel.Error;
                default:
                    return FailureLevel.None;
            }
        }

        public virtual void HydrateRules(HashSet<ReportingDescriptor> rules, DiagnosticRecord diagnosticRecord)
        {
            if (rules is null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            if (diagnosticRecord is null)
            {
                throw new ArgumentNullException(nameof(diagnosticRecord));
            }

            var rule = new ReportingDescriptor()
            {
                Id = diagnosticRecord?.RuleName,
                Name = diagnosticRecord?.RuleName,
                HelpUri = new Uri("https://github.com/PowerShell/Psscriptanalyzer")
            };

            rules?.Add(rule);
        }

        public virtual void HydrateArtifacts(HashSet<Artifact> artifacts, DiagnosticRecord diagnosticRecord)
        {
            if (artifacts is null)
            {
                throw new ArgumentNullException(nameof(artifacts));
            }

            if (diagnosticRecord is null)
            {
                throw new ArgumentNullException(nameof(diagnosticRecord));
            }

            var artifact = new Artifact()
            {
                Location = new ArtifactLocation()
                {
                    Uri = new Uri(diagnosticRecord?.ScriptPath)
                }
            };

            artifacts?.Add(artifact);
        }

        public virtual void HydrateResults(IList<Result> results, DiagnosticRecord diagnosticRecord)
        {
            if (results is null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            if (diagnosticRecord is null)
            {
                throw new ArgumentNullException(nameof(diagnosticRecord));
            }

            var failureLevel = ResolveSeverity(diagnosticRecord?.Severity);

            var result = new Result()
            {
                Level = failureLevel,
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
                RuleId = diagnosticRecord.RuleName
            };

            results.Add(result);
        }

        public virtual SarifLog ConstructSarifLog(
            IList<Result> results,
            HashSet<Artifact> artifacts,
            HashSet<ReportingDescriptor> rules)
        {
            if (results is null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            if (artifacts is null)
            {
                throw new ArgumentNullException(nameof(artifacts));
            }

            if (rules is null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            return new SarifLog()
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
                                Rules = rules?.ToList()
                            }
                        },
                        Artifacts = artifacts?.ToList(),
                        Results = results?.ToList()
                    }
                }

            };
        }
    }
}
