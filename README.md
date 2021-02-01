# ConvertToSARIF

![Unit Tests](https://github.com/microsoft/ConvertTo-SARIF/workflows/Unit%20Tests/badge.svg?event=push)  ![CI](https://github.com/microsoft/ConvertTo-SARIF/workflows/CI/badge.svg?event=push)

This is a simple utility PSCmdlet used to convert the output of the [PSScriptAnalyzer](https://github.com/PowerShell/Psscriptanalyzer) to the SARIF format using pipelines.

## Download
The tool is available for download from the PowerShell Gallery [here](https://www.powershellgallery.com/packages/ConvertToSARIF/1.0.0).

## Getting Started
To run the program you have to call the `Invoke-ScriptAnalyzer` and then pipeline it to the `ConvertTo-SARIF`:

`Invoke-ScriptAnalyzer .\scripts\ -Recurse | ConvertTo-SARIF -FilePath results.sarif`

ConvertToSARIF has a single required parameter `FilePath`, which is relative to the current directory of Powershell.


## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft 
trademarks or logos is subject to and must follow 
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
