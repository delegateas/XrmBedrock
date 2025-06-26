# Organizing code in Areas

To organize the code a set of areas are defined in this file and used as folders in several projects in the solution.
These projects are:
`src/Azure/DataverseService`
`src/Azure/*FunctionApp` (naming of AzureFunctionApp-projects reflects areas)
`src/Dataverse/SharedPluginLogic/Plugins`
`src/Dataverse/SharedPluginLogic/Logic`
`test/IntegrationTests`

When the solution is taken from GitHub the definitions below will be the examples that the solution is born with.
You should change this as soon as you start developing funtionality in your solution to reflect the business domain that you are working with.

**Recommendation**: If you are using [Data Model Viewer](https://github.com/delegateas/DataModelViewer) (and you really should!) use the areas you define here as your [Grouping](https://github.com/delegateas/DataModelViewer?tab=readme-ov-file#grouping)

Below is a section for each area containing a list of each of the entities that are included in the area. The entities are presented with thier business name as well as their logical name in parenthesis.
For the sake of everybody working on this project keep them sorted to maintain overview and reduce potential merge conflicts.

## Area: EconomyArea
Invoice (mgs_invoice)
Invoice Collection (mgs_invoicecollection)
Product (mgs_product)
Subscription (mgs_subscription)
Transaction (mgs_transaction)

## Area: ExampleActivityArea
Note (annotation)
Task (task)

## Area: ExampleCustomerArea
Account (account)
Contact (contact)

## Recommandations on how to define Areas
TODO