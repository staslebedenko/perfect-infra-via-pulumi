using System.Threading.Tasks;
using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;

class MyStack : Stack
{
    public MyStack()
    {
        // Create an Azure Resource Group
        var resourceGroup = new ResourceGroup("fancyGroup", new ResourceGroupArgs
        {
            Location = "northeurope"
        });

        var groupWithRealName = new ResourceGroup("fancyGroupReal", new ResourceGroupArgs
        {
            Location = "northeurope",
            ResourceGroupName = "fancyGroupReal"
            //Name = "fancyGroupReal"
        });

        // Create an Azure resource (Storage Account)
        var storageAccount = new StorageAccount("fancyaccount", new StorageAccountArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Location = "northeurope",
            Sku = new SkuArgs
            {
                Name = SkuName.Standard_LRS
            },
            Kind = Kind.StorageV2
        });

        // Export the primary key of the Storage Account
        
        // this.PrimaryStorageKey = Output.Tuple(resourceGroup.Name, storageAccount.Name).Apply(names =>
        //     Output.CreateSecret(GetStorageAccountPrimaryKey(names.Item1, names.Item2)));

        var appServicePlan = new AppServicePlan("festive-function-plan", new AppServicePlanArgs{
            ResourceGroupName = resourceGroup.Name,
            Kind = "Windows",
            Sku = new  SkuDescriptionArgs {
                Tier = "Dynamic",
                Name = "Y1"
            }
        });

        // var app = new WebApp("festive-webapi-app", new WebAppArgs{
        //     Kind = "FunctionApp",
        //     ResourceGroupName = resourceGroup.Name,
        //     ServerFarmId = appServicePlan.Id
        // });
    }

    // [Output]
    // public Output<string> PrimaryStorageKey { get; set; }

    // private static async Task<string> GetStorageAccountPrimaryKey(string resourceGroupName, string accountName)
    // {
    //     var accountKeys = await ListStorageAccountKeys.InvokeAsync(new ListStorageAccountKeysArgs
    //     {
    //         ResourceGroupName = resourceGroupName,
    //         AccountName = accountName
    //     });
    //     return accountKeys.Keys[0].Value;
    // }
}
