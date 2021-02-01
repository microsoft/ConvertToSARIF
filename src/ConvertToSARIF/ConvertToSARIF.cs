using Microsoft.CodeAnalysis.Sarif;
using ConvertToSARIF.Engine;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace ConvertToSARIF
{
    [Cmdlet(VerbsData.ConvertTo, "SARIF")]
    public class ConvertToSARIF : PSCmdlet
    {
        private readonly IList<DiagnosticRecord> diagnosticsRecords = new List<DiagnosticRecord>();

        [Parameter(ValueFromPipeline = true), ValidateNotNullOrEmpty]
        public PSObject InputObject { get; set; }

        [Parameter(Mandatory = true, Position = 0), ValidateNotNullOrEmpty]
        public string FilePath { get; set; }

        [Parameter(Mandatory = false)]
        public string IgnorePattern { get; set; }

        public SARIFConverter SARIFConverter { get; set; }
        public FilePathResolver FilePathResolver { get; set; }


        protected override void BeginProcessing()
        {
            SARIFConverter = new SARIFConverter();
            FilePathResolver = new FilePathResolver();

            base.BeginProcessing();
        }


        protected override void EndProcessing()
        {
            string sarifFilePath = FilePathResolver.Resolver(FilePath, SessionState.Path.CurrentFileSystemLocation.Path);

            SarifLog log = SARIFConverter.ConvertToSarifLog(diagnosticsRecords, string.IsNullOrWhiteSpace(IgnorePattern) ? null : new Regex(IgnorePattern));
            
            log?.Save(sarifFilePath);
            
            base.EndProcessing();
        }

        protected override void ProcessRecord()
        {
            diagnosticsRecords.Add(new DiagnosticRecord(InputObject));

            base.ProcessRecord();
        }
    }
}