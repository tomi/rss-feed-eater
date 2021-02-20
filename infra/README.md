# RSS Feed Eater Infrastructure

Infrastructure is declared using [bicep](https://github.com/Azure/bicep).

Bicep code needs to be built before it can be deployed

```console
bicep build main.bicep
```

## First-time setup

Need to create

1. Resource group

```console
az group create -l westeurope -n {ResourceGroupName}
```

2. Service principal

```console
az ad sp create-for-rbac --name {myApp} --role contributor --scopes /subscriptions/{SubscriptionId}/resourceGroups/{ResourceGroupName} --sdk-auth
```

and store the credentials from the output
