# Quiz User Simulator

## Instructions

### Working with 'quizsim'

#### How to create image locally

```batch
# Build container image
docker build -t quizsim:latest .

# Run container using command
docker run --env "Endpoint=https://yourserver/QuizHub" quizsim:latest
``` 

#### How to deploy to Azure Container Instances (ACI)

Deploy published image to [Azure Container Instances (ACI)](https://docs.microsoft.com/en-us/azure/container-instances/) the Azure CLI way:

```batch
# Variables
aciName="quizsim"
resourceGroup="quizsim-dev-rg"
location="westeurope"
image="jannemattila/quizsim:latest"
server="https://yourserver/QuizHub"

# Login to Azure
az login

# *Explicitly* select your working context
az account set --subscription <YourSubscriptionName>

# Create new resource group
az group create --name $resourceGroup --location $location

# Create ACI
az container create --name $aciName --image $image --resource-group $resourceGroup -e Endpoint=$server

# Show the properties
az container show --name $aciName --resource-group $resourceGroup

# Show the logs
az container logs --name $aciName --resource-group $resourceGroup

# Wipe out the resources
az group delete --name $resourceGroup -y
``` 

Deploy published image to [Azure Container Instances (ACI)](https://docs.microsoft.com/en-us/azure/container-instances/) the Azure PowerShell way 
(below is the "AzureRm" module example but if you use "Az" module then just
replace that in the command names):

```powershell
# Variables
$aciName="quizsim"
$resourceGroup="quizsim-dev-rg"
$location="westeurope"
$image="jannemattila/quizsim:latest"
$server="https://yourserver/QuizHub"

# Login to Azure
Login-AzureRmAccount

# *Explicitly* select your working context
Select-AzureRmSubscription -SubscriptionName <YourSubscriptionName>

# Create new resource group
New-AzureRmResourceGroup -Name $resourceGroup -Location $location

# Create ACI
New-AzureRmContainerGroup -Name $aciName -Image $image -ResourceGroupName $resourceGroup -EnvironmentVariable @{"Endpoint"=$server}

# Show the properties
Get-AzureRmContainerGroup -Name $aciName -ResourceGroupName $resourceGroup

# Show the logs
Get-AzureRmContainerInstanceLog -ContainerGroupName $aciName -ResourceGroupName $resourceGroup

# Wipe out the resources
Remove-AzureRmResourceGroup -Name $resourceGroup -Force
```

#### How to deploy to Azure Container Services (AKS)

Deploy published image to [Azure Container Services (AKS)](https://docs.microsoft.com/en-us/azure/aks/):

Create `quizsim.yaml`:

```yaml
apiVersion: extensions/v1beta1
kind: Deployment
metadata:
  name: quizsim
  namespace: quiz
spec:
  replicas: 1
  template:
    metadata:
      labels:
        app: quizsim
    spec:
      containers:
      - image: jannemattila/quizsim:latest
        name: quizsim
        env:
          - name: Endpoint
            value: "https://yourserver/QuizHub"
```

```batch
kubectl apply -f quizsim.yaml
```
