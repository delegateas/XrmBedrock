# External API

ASP.NET Core Minimal API with Azure AD authentication for accessing membership data in Dataverse.

## Overview

This API provides external access to membership (subscription) data stored in Dataverse. It supports both interactive user authentication and service-to-service (client credentials) authentication.

## Azure AD App Registration Setup

You need TWO app registrations in Azure AD:

### 1. API App Registration (Backend)

This is the main API application that protects the endpoints.

1. Go to Azure Portal > Azure Active Directory > App registrations
2. Click "New registration"
3. Configure:
   - Name: `ExternalApi`
   - Supported account types: Accounts in this organizational directory only
   - Redirect URI: Leave empty
4. After creation, go to "Expose an API":
   - Set Application ID URI: `api://<client-id>`
   - Add a scope:
     - Scope name: `access_as_user`
     - Who can consent: Admins and users
     - Admin consent display name: Access External API as user
     - Admin consent description: Allows the app to access the External API on behalf of the signed-in user
     - User consent display name: Access External API
     - User consent description: Allows the app to access membership data on your behalf
     - State: Enabled
5. Go to "App roles" and create a role for service-to-service access:
   - Display name: `API.Access`
   - Allowed member types: Applications
   - Value: `API.Access`
   - Description: Allows client applications to access the API
   - State: Enabled

### 2. Swagger App Registration (SPA)

This allows Swagger UI to authenticate users interactively.

1. Go to Azure Portal > Azure Active Directory > App registrations
2. Click "New registration"
3. Configure:
   - Name: `ExternalApi-Swagger`
   - Supported account types: Accounts in this organizational directory only
   - Redirect URI: Single-page application (SPA)
     - Add: `https://localhost:5001/swagger/oauth2-redirect.html`
     - Add: `https://<your-production-url>/swagger/oauth2-redirect.html`
4. Go to "API permissions":
   - Click "Add a permission"
   - Select "My APIs" > `ExternalApi`
   - Select "Delegated permissions"
   - Check `access_as_user`
   - Click "Add permissions"
   - Click "Grant admin consent" (if you have admin rights)

## Configuration Reference

Update `appsettings.json` with your Azure AD values:

| Setting | Description |
|---------|-------------|
| `AzureAd:TenantId` | Your Azure AD tenant ID (GUID) |
| `AzureAd:ClientId` | Client ID of the API App Registration |
| `AzureAd:Audience` | Should be `api://<api-client-id>` |
| `AzureAd:Scopes` | The scope exposed by the API (`access_as_user`) |
| `SwaggerAd:ClientId` | Client ID of the Swagger App Registration |
| `DataverseUrl` | Your Dataverse environment URL |

## Authorization Policies

| Policy | Description | Required Claims |
|--------|-------------|-----------------|
| `ApiAccess` | General authenticated access for both app and user tokens | `appid` OR `scp` claim |
| `InteractiveUsers` | Only for interactive user tokens (from Swagger or user-facing apps) | `scp` claim |
| `ClientCredentialsOnly` | Only for service-to-service (app-only) tokens | `appid` + `idtyp=app` |

## Local Development

1. Ensure you are logged in to Azure CLI:
   ```bash
   az login
   ```

2. Set your Dataverse environment URL in `appsettings.json`

3. Run the API:
   ```bash
   dotnet run
   ```

4. The API will be available at:
   - HTTP: `http://localhost:5000`
   - HTTPS: `https://localhost:5001`
   - Swagger UI: `https://localhost:5001/swagger`

Note: In development mode, authentication is not required by default (FallbackPolicy is null).

## Endpoints

| Method | Route | Policy | Description |
|--------|-------|--------|-------------|
| GET | `/` | Anonymous | Health check / status |
| GET | `/health` | Anonymous | Health check endpoint |
| GET | `/api/memberships/{customerId}` | ApiAccess | Get memberships for a customer |
| POST | `/api/memberships` | InteractiveUsers | Create a new membership |
| GET | `/api/memberships/admin/{customerId}` | ClientCredentialsOnly | Admin endpoint for memberships |

## Troubleshooting

### 401 Unauthorized

- Verify your Azure AD configuration in `appsettings.json`
- Ensure the token is valid and not expired
- Check that the audience in the token matches `api://<client-id>`

### 403 Forbidden

- The user/app is authenticated but lacks the required permissions
- For `InteractiveUsers`: Ensure the token has the `scp` claim with `access_as_user`
- For `ClientCredentialsOnly`: Ensure the token has `idtyp=app` claim
- Verify app roles and API permissions are correctly configured

### Swagger OAuth Issues

- Ensure redirect URIs are correctly configured in the Swagger App Registration
- Verify the Swagger App Registration has permission to the API scope
- Check that admin consent has been granted for the API permission
- If using HTTPS locally, ensure your certificate is trusted

### Dataverse Connection Issues

- Verify you are logged in to Azure CLI (`az login`)
- Ensure your Azure AD account has access to the Dataverse environment
- Check the `DataverseUrl` in configuration is correct
