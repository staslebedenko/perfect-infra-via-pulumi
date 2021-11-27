# perfect-infra-via-pulumi

We will start with the setup and deployment of the local environment, then proceed to the CI/CD with Github actions

The best way to install Pulumi CLI is to use chocolately package manager
https://docs.chocolatey.org/en-us/choco/setup

And then run the first command via CMD, use the upgrade command 

```
choco install pulumi
```

For this workshop you will need at least 3.17, so if you already have the old installation of Pulumi :), please upgrade
```
pulumi version
choco upgrade pulumi
```

Create a new folder and start CMD or paste in explorer nav bar.
ommand to open VS code with the current folder.
```
code .
```

Then proceed with Terminal windows inside VSCode with 

```
az login
```
After login close browser window

And start with setup of the new pulumi project(You will be prompted to login)
```
pulumi new azure-csharp
```

Then choose the name of the current stack

And set a desired location of your Azure resources.
```
pulumi config set azure-native:location northeurope
```


# Get principal credentials from your Azure subscription via Azure CLI. Password based.
And get your subscription ID

```
  az ad sp create-for-rbac --name ServicePrincipalName --role Contributor
  
  # save the ouput
  
  az account show --query id -o tsv
  
  # save output as well
  

  # alternative command, if you want to limit access to the particular resource group
   az ad sp create-for-rbac --name "myApp" --role contributor \
                            --scopes /subscriptions/{subscription-id}/resourceGroups/{resource-group} \
                            --sdk-auth
                            
  # Replace {subscription-id}, {resource-group} with the subscription, resource group details

  # The command should output a JSON object similar to this:

  {
    "clientId": "<GUID>",
    "clientSecret": "<GUID>",
    "subscriptionId": "<GUID>",
    "tenantId": "<GUID>",
    (...)
  }
```
AppId is the client ID
Password is the client secret
Tenant is the tenant ID

Then prepare the following strings and run them from a local command prompt, so Pulumi can use them later on with pipelines.

```
pulumi config set azure-native:clientId <clientID>

pulumi config set azure-native:clientSecret <clientSecret> --secret

pulumi config set azure-native:tenantId <tenantID>

pulumi config set azure-native:subscriptionId <subscriptionId>
```


Then you should Create a new access token on Pulumi web site
https://app.pulumi.com/yourname/settings/tokens

And add it to the GitHub repository secret settings.
Repository settings, add secret menu.
