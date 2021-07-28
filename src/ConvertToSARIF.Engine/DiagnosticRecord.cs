using System;
using Microsoft.CodeAnalysis.Sarif;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace ConvertToSARIF.Engine
{
    public class DiagnosticRecord
    {
        public string RuleName { get; set; }

        public string Severity { get; set; }

        public string ScriptPath { get; set; }

        public int Line { get; set; }

        public string Message { get; set; }

        public IList<Suppression> Suppressions
        {
            get
            {
                if (suppressions == null)
                {
                    suppressions = new List<Suppression>();
                }

                return suppressions;
            }
            set
            {
                suppressions = value;
            }
        }
        private IList<Suppression> suppressions;

        public DiagnosticRecord(PSObject inputObject)
        {
            RuleName = inputObject?.Properties?[nameof(RuleName)]?.Value?.ToString();

            Severity = inputObject?.Properties?[nameof(Severity)]?.Value?.ToString();

            ScriptPath = inputObject?.Properties?[nameof(ScriptPath)]?.Value?.ToString();

            if (int.TryParse(inputObject?.Properties?[nameof(Line)]?.Value?.ToString(), out int line))
            {
                Line = line;
            }

            Message = inputObject?.Properties?[nameof(Message)]?.Value?.ToString();

            dynamic SuppressionList = inputObject?.Properties?[nameof(Suppression)]?.Value;

            if (SuppressionList != null)
            {
                foreach (dynamic suppression in SuppressionList)
                {
                    Suppression suppressions = new Suppression();
                    suppressions.Justification = suppression.Justification;
                    suppressions.Kind = SuppressionKind.InSource;

                    if (suppressions != null)
                    {
                        Suppressions.Add(suppressions);
                    }
                }
            }
        }

        public DiagnosticRecord()
        {
        }
    }
}
