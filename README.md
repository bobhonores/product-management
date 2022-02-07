# Product Management

## Introduction

This project exposes an API that allows managing the information of products and their reviews.

## Configuration

### Database

This API uses a SQLite database which it could be obtain using one of the following options.

- Using the source code, you could execute the migrations to generate a brand new database. [Microsoft: Migrations - Create your database and schema](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli#create-your-database-and-schema)

- A sample database was uploaded in the folder `sampleDb` which could be copied in the root of the project or in the desired path used in the configuration file `appSettings.json` in the entry `DbConnectionString`.

### External Service

This API communicates with an external service sending product's id to the service.

The endpoint could be configure in the configuration file `appSettings.json` in the entry `ExternalServiceSettings:Uri` and it expects the following sample response.

```json
{
  "event_id": "24lkjG0C6hQwTrQeKPuocIWYLFw",
  "deployment_id": "d_Pas55b8",
  "timestamp": "2022-02-07T05:44:53.265Z",
  "value": "45.24"
}
```

## Technology Stack

- .NET 5
- EF Core
- SQLite
- MediatR
- FluentValidation
- LazyCache
- SqlKata

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[MIT](https://choosealicense.com/licenses/mit/)
