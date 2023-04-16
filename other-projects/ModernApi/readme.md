## Local development

Create a certificate that azurite can use to enable HTTPS.  
`dotnet dev-certs https --trust -ep cert.pfx -p docker`  

Place the generated cert in the `./certs` folder.

## Useful links
- https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio
- https://github.com/Azure/Azurite#customized-storage-accounts--keys
- https://learn.microsoft.com/en-us/dotnet/azure/sdk/authentication-local-development-dev-accounts?tabs=azure-cli%2Csign-in-visual-studio%2Ccommand-line