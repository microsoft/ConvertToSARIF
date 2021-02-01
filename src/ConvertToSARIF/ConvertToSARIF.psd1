@{

RootModule = '.\ConvertToSARIF.dll'

ModuleVersion = '1.0.0'

GUID = '3bb597cc-9d33-4110-a2b1-920994274285'

Author = 'Antonios Katopodis'

CompanyName = 'Microsoft'

Copyright = '© Microsoft Corporation. All rights reserved.'

Description = 'A CMDLet for converting PSScriptAnalyzer output to the SARIF format.'

FunctionsToExport = '*'

CmdletsToExport = '*'

VariablesToExport = '*'

AliasesToExport = '*'

PrivateData = @{

    PSData = @{

        Tags = 'PSScriptAnalyzer', 'Powershell', 'SARIF'

        LicenseUri = 'https://github.com/microsoft/ConvertTo-SARIF/blob/main/LICENSE'

        ProjectUri = 'https://github.com/microsoft/ConvertTo-SARIF'
    }

} 
}

