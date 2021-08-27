# Developer Documentation

This doc serves as a guideline to get started with the project.

## Clone the repository
Run `git clone https://github.com/microsoft/ConvertTo-SARIF`

## Publish the project using dotnet
In order to test the module and have it running you have to publish it using `dotnet publish`. Go to the folder for `ConvertToSARIF.csproj` and publish it.

## Import the module
After you have published the module you can import it from the directory it should be under `bin\Debug\netstandard2.0\publish`. The command to import it is:

`Import-Module bin\Debug\netstandard2.0\publish\ConvertToSARIF`

## Testing with PSScriptAnalyzer
Use `Invoke-ScriptAnalyzer` on the directory where you are testing and simply pipe the output to the cmdlet.

## Publishing your changes
If you try to repeat the above steps after you have imported the module you wont be able to use `dotnet publish` because the directory of the publish target is being used by Powershell. Using `Remove-Module` does not resolve this. The best course of action is closing your terminal thus closing the Powershell session. That way you will be able to publish again.
