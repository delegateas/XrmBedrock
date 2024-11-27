# Project Purpose
This project facilitates the logic that interacts with Dataverse and is used from both Azure (Function Apps and Web Apps) and Dataverse Plugins

# Project Organisation
The elements of this project are organised as follows.
There is a folder for each area of the solution, where the areas are inspired of those defined in D101 Data model diagrams (Visio).
In each folder is a set of Services all relating to the area.
For a simple area there will probably just be one but for complex areas the will probably be plenty.
The general service can be split into more specialized services. First step of splitting is probably to take methods related to a specific table/entity to it's own service.

Splits of the Services are made for several reasons where the most severe are
1. If it grows too big: Split it up
2. If Several other services depend upon some methods: Split so that these methods are placed in a more specialised service that can be depended upon af other services

NOTE! When depending on other services make sure not to introduce cyclic dependencies! Have the strategy of specialization top of mind!

NOTE! Services from one area may depend on (more specialised) services of other areas!

Currently there are these Areas that contain logic related to the listed tables (not a complete mapping, please help improving)

- Activity
    - All Dataverse Activities like Task, Email etc
- Commitee
    - Commitee (? Udvalg)
    - Commitee member
- Customer
    - Account
    - Contact
- Economy
    - Arreascase
    - Invoice
    - Payment agreement
    - Transaction
- Employment
    - Employment
    - Placementrule
    - Position
    - WorkplaceType
- Product
    - Discount
    - MembershipCancellationReason
    - Product
    - Subscription
- Prosocity
    - Prosociety
    - Prosociety member
- Utility
    - A bucket collection stuff not contained in the rest
    - City
    - Country
    - Municipality
    - Postalcode
    - Systemuser
    - Team
