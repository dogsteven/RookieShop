To run this project, follow the following instructions.

### 1. Setup Database

Install [Postgresql](https://www.postgresql.org/) and the [vector extension](https://github.com/pgvector/pgvector). Create 2 databases:
1. `rookie_shop` with the vector extension enabled. 
2. `quartz` with this [schema](https://github.com/quartznet/quartznet/blob/main/database/tables/tables_postgres.sql).
3. Update connection strings.
4. Install `dotnet-ef` tool, go to `RookieShop.WebApi` directory and migrate database with the following `DbContext`: `ProductCatalogDbContextImpl`, `ImageGalleryDbContextImpl`, `ShoppingDbContext`, and `OrderingDbContext`.

### 2. Setup Keycloak

#### 2.1 Install [Keycloak](https://www.keycloak.org/) and bootstrap its.

#### 2.2 Start Keycloak, go to its Admin site and create a realm named `rookie-shop`.

#### 2.3 Setup Web API client

1. In `rookie-shop` realm, create an OIDC client with the name `rookie-shop-web-api`.
2. To use Swagger authentication feature, enable the `Client authentication` and the `Standard flow` options for Authorization Code Flow.
3. Add `Audience` mapper type to its dedicated scope with `rookie-shop-web-api` as an included client audience.
4. Copy the `Id` and the `Secret` of this client to `Keycloak:Swagger` setting in `RookieShop.WebApi` project.

#### 2.4 Setup Service accounts

1. In `master` realm, create an OIDC client with the name `rookie-shop-admin`.
2. To use Keycloak's Admin API, enable the `Client authentication` and the `Service accounts roles` options.
3. In `Service accounts roles` tab, add `admin` role.
4. Copy the `Id` and the `Secret` of this client to `Keycloak:ServiceAccount` setting in `RookieShop.WebApi` project.

#### 2.5 Setup Front Store MVC client

1. In `rookie-shop` realm, create an OIDC client named `rookie-shop-front-store-mvc`.
2. Enable the `Client authentication` and the `Standard flow` options for Authorization Code Flow.
3. Add `Audience` mapper type to its dedicated scope with `rookie-shop-web-api` as an included client audience.
4. Add `Real Roles` mapper type to its dedicated scope with `Add to Id token` enabled to make the MVC's authorization middleware works.  
5. Copy the `Id` and the `Secret` of this client to `Keycloak:ClientSettings` setting in `RookieShop.FrontStore` project.

### 3. Setup semantic search

Download an [ONNX model](https://huggingface.co/onnx-models/all-MiniLM-L6-v2-onnx) of `all-MiniLM-L6-v2`, place it in the `RookieShop.WebApi/model` directory with the name `model.onnx`.

### 4. Install RabbitMQ

Install [RabbitMQ](https://www.rabbitmq.com/) and start it.

### 5. Install Azurite

Install [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio%2Cblob-storage) and start it.