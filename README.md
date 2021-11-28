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

Then proceed with Terminal windows inside VSCode with AZ login
This is for the demo purposes, for production you need to use environment variables later on with limited access principal,
to resources that is needed for your particular services.

```
az login
```
After login close browser window

And start with setup of the new pulumi project(You will be prompted to login)
```
pulumi new azure-csharp --force
```

Then choose the name of the current stack

And set a desired location of your Azure resources.
```
pulumi config set azure-native:location northeurope
```

Open the result in Visual studio code, change the group name

And run the following command to preview changes on Azure.

```
pulumi preview
```

Then we will deploy changes via
```
pulumi up
```

## The GitHub step

I assume that you know how to create a new GitHub repository and clone it to the local environment.

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

Then prepare the following strings and run them from a local command prompt(or terminal in VSCode), so Pulumi can use them later on with pipelines.

The first option is to set variables and secrect via Pulumi command line.

```
pulumi config set azure-native:clientId <clientID>

pulumi config set azure-native:clientSecret <clientSecret> --secret

pulumi config set azure-native:tenantId <tenantID>

pulumi config set azure-native:subscriptionId <subscriptionId>
```

There is a need to add this values to the secrects in GitHub as well

```
          ARM_CLIENT_ID
          ARM_CLIENT_SECRET
          ARM_TENANT_ID
          ARM_SUBSCRIPTION_ID
```

and to the local environment variables

```
setx ARM_CLIENT_ID "<clientID>"
setx ARM_CLIENT_SECRET "<clientSecret>"
setx ARM_TENANT_ID "<tenantID>"
setx ARM_SUBSCRIPTION_ID "<subscriptionId>"
```

Then you should Create a new access token on Pulumi web site
https://app.pulumi.com/yourname/settings/tokens

And add it to the GitHub repository secret settings.
Repository settings, add secret menu with the following name

```
PULUMI_ACCESS_TOKEN
```

Now we can proceed to the GitHub UI and click Action menu on a github.

Enter a new name pull_request.yml

And replace existing code with following sample

```
name: Pulumi
on:
  push:
    branches: [ main ]
jobs:
  up:
    name: Preview
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - uses: pulumi/actions@v3
        with:
          command: preview
          stack-name: dev
        env:
          PULUMI_ACCESS_TOKEN: ${{ secrets.PULUMI_ACCESS_TOKEN }}
          ARM_CLIENT_ID: ${{ secrets.ARM_CLIENT_ID }}
          ARM_CLIENT_SECRET: ${{ secrets.ARM_CLIENT_SECRET }}
          ARM_TENANT_ID: ${{ secrets.ARM_TENANT_ID }}
          ARM_SUBSCRIPTION_ID: ${{ secrets.ARM_SUBSCRIPTION_ID }}
          ARM_ENVIRONMENT: public
          ARM_LOCATION: northeurope
```

And create a new pull request.

Then we need to add

```
name: Pulumi
on:
  pull_request:
    branches: [ main ]
jobs:
  up:
    name: Update
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup DotNet
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: 3.1
      - uses: pulumi/actions@v3
        with:
          command: up
          stack-name: dev
        env:
          PULUMI_ACCESS_TOKEN: ${{ secrets.PULUMI_ACCESS_TOKEN }}
          ARM_CLIENT_ID: ${{ secrets.ARM_CLIENT_ID }}
          ARM_CLIENT_SECRET: ${{ secrets.ARM_CLIENT_SECRET }}
          ARM_TENANT_ID: ${{ secrets.ARM_TENANT_ID }}
          ARM_SUBSCRIPTION_ID: ${{ secrets.ARM_SUBSCRIPTION_ID }}
          ARM_ENVIRONMENT: public
          ARM_LOCATION: northeurope
```

How cool is that :)
