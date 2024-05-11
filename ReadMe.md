# Invoice Demo Project using MartinDB
This project illustrates a simple invoice workflow using [MartenDB](https://martendb.io/)

## Getting Started
Running from Visual Studio will start the docker containers and API project.

Next we need to create the database.

#### Creating the database using psql

```powershell
# Find Postgres container
docker ps
# Copy the container id for postgres container
# Shell into postgres container
docker exec -it container_id bash
```

You can also create the database using any Postgres client.
I found the VS Code Plugin [VS Code - Postgres](https://marketplace.visualstudio.com/items?itemName=ckolkman.vscode-postgres) very easy to use.

After the databse is created and API is running, you can use the [Request.http](./src/InvoiceMartenDbDemo/Requests.http) helper file to test the API.

## Postgres Cheat sheet
```bash
# Log into postgres
psql -U postgres

# Create database
create database invoicemartendbdemo;
# Connect to database
\c invoicemartendbdemo;

# Show Tables
\d 
# Enable/Disable Expanded Display
\x

# Example of querying json data
select * from mt_doc_billedinvoice where data ->> 'ClientId' = '3fa85f64-5717-4562-b3fc-2c963f66afa6';

drop database if exists invoicemartendbdemo;
```

