# Deploying Facets to Azure

Facets runs as two Azure resources:

- **App Service** running `Facets.Api`. It also serves the Angular site (`FacetsUI`) from `wwwroot`.
- **Function App** running `Facets.FunctionApp.CP` (.NET 8 isolated). It sends emails and SMS from storage queues, receives OnePay payment notifications, and archives audit logs.

## Why it works locally but not on Azure

Locally, all settings (database, JWT key, OnePay keys and so on) come from **User Secrets**. .NET only loads User Secrets in the `Development` environment, so they never reach Azure. `appsettings.json` contains no settings, which means every value below has to be added in the Azure Portal.

If a required API setting is missing, the API now stops at startup with a message naming the missing settings. That message appears in **Log stream** and in `%HOME%\LogFiles\Facets`.

To see your local values, run `dotnet user-secrets list` in the `Facets.Api` folder. In Visual Studio, right-click Facets.Api → *Manage User Secrets*.

## 1. App Service (Facets.Api) settings

In the Azure Portal, go to **App Service → Settings → Environment variables**.

Setting names use `__` (two underscores) where the JSON would have nesting.

**Connection strings tab**

| Name | Type | Value |
|---|---|---|
| `MSSQLDbConnection` | SQLAzure | SQL connection string |
| `AzureStorage` | Custom | Storage account connection string. This must be the **same storage account** the Function App uses for `AzureWebJobsStorage`, because the API puts emails and SMS on queues there and the Function App reads them. |

**App settings tab: required**

| Name | Notes |
|---|---|
| `JwtConfig__Issuer` | |
| `JwtConfig__Audience` | |
| `JwtConfig__SigningKey` | Long random secret. Use a different value than in development. |
| `JwtConfig__TokenLifetime` | TimeSpan format, e.g. `08:00:00` |
| `OnePaySettings__BaseURL` | e.g. `https://api.onepay.lk/` |
| `OnePaySettings__AppID` | |
| `OnePaySettings__AppToken` | |
| `OnePaySettings__HashSalt` | |
| `OnePaySettings__PaymentRequestEndPoint` | e.g. `v3/checkout/link/` |
| `OnePaySettings__TransactionRedirectUrl` | Public-site payment result page, HTTPS |

**App settings tab: optional**

| Name | Notes |
|---|---|
| `OnePaySettings__TransactionStatusEndPoint` | Defaults to `v3/transaction/status/` |
| `Assocify__BaseURL`, `Assocify__TenantId`, `Assocify__FuncAppKeys__GetMemberBySearchValue`, `Assocify__FuncAppKeys__IsMemberAvailable` | Needed for Assocify member lookup |
| `Cors__AllowedOrigins` | Comma-separated list. Defaults to `https://exhibition.facetssrilanka.com,https://facets-uat.azurewebsites.net`. Add your App Service or custom domain if it is different. |
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | Enables Application Insights |
| `Swagger__Enabled` | `true` to expose Swagger UI outside Development (off by default; it lists every endpoint). |
| `FileStorage__PrivateContainers` | Default `true`: uploaded files (NIC scans, photos, logos) are stored in private containers and the API returns short-lived signed links. Set `false` only to roll back to public containers. |
| `FileStorage__SignedUrlLifetimeHours` | Default `12`. |
| `FileStorage__PublicContainers` | Containers that stay public, default `common-images` (the email logo). |

**Networking**

On the Azure SQL server, go to **Networking** and allow Azure services, or add the App Service's outbound IP addresses.

## 2. Function App (Facets.FunctionApp.CP) settings

| Name | Notes |
|---|---|
| `FUNCTIONS_WORKER_RUNTIME` | `dotnet-isolated` |
| `AzureWebJobsStorage` | Same storage account as the API's `AzureStorage` |
| `ArchiveAppAuditLogCron` | CRON for the audit-log archive, e.g. `0 0 2 * * *`. **If this is missing, the whole Function App fails to start.** |
| `ConnectionStrings__MSSQLDbConnection` | Same database as the API |
| `ConnectionStrings__AzureStorage` | Same as above |
| `OnePaySettings__*` | Same values as the API. The payment webhook uses them to verify each payment with OnePay. |
| `Smtp_From`, `Smtp_Password` | Sender account and app password for outgoing email. These used to be hard-coded in the source. |
| `Smtp_Host`, `Smtp_Port` | Optional. Default to `smtp.gmail.com` and `587`. |
| `TextIt_Id`, `TextIt_Password`, `TextIt_From` | SMS for Sri Lankan numbers |
| `TextIt_BaseUrl` | Optional. Defaults to `https://textit.biz/sendmsg/index.php`. The old code used plain `http://`, sending the password unencrypted. If TextIt rejects HTTPS, set this to the `http://` URL knowingly. |
| `TWILIO_ACCOUNT_SID`, `TWILIO_AUTH_TOKEN`, `TWILIO_FromPhoneNumber` | SMS for other numbers |

In OnePay's dashboard, set the payment notification URL to the function URL including its key:

```
https://<function-app>.azurewebsites.net/api/onepay/notify-payment-update?code=<function key>
```

## 3. Database migrations

Apply migrations to the Azure database before you deploy code that depends on them. This release adds `20260928000000_PaymentAndOtpHardening`.

```
dotnet ef database update --project Facets.Persistence --startup-project Facets.Api --connection "<azure sql connection string>"
```

If creating the payment index fails, the table already contains duplicate online payments. Find them with the query in that migration's comment.

## 4. Publishing

Publishing `Facets.Api` (Visual Studio *Publish*, or `dotnet publish -c Release`) now builds the Angular site in production mode and puts it in `wwwroot`. This needs Node.js on the machine that publishes.

If you have already built the site yourself, add `/p:SkipSpaBuild=true`.

The production frontend calls the API on its own host, so the same build works on any App Service or custom domain without editing `environment.prod.ts`.

`FacetsUI/package-lock.json` is now committed. Previously a clean `npm install` pulled newer, incompatible versions of `ngx-scanner-qrcode` and `@types/node`, and the production build failed.

## 5. Data protection notes

- **Private file storage.** On first use after this release the API switches its five containers (`visitor-documents`, `team-member-documents`, `team-member-profile-image`, `user-profile-images`, `event-logos`) to private and signs every file link it returns. The `AzureStorage` connection string must include the account key (the default "Connection string" from the portal does). If images stop loading, check the API log for "Storage client cannot sign URLs".
- **Visitor tokens are confined to the public site.** Admin endpoints reject visitor (OTP) tokens unless marked `[AllowPublicSiteUser]`. If a public page starts getting 403s after this release, that endpoint needs the attribute.
- **Visitor list report needs the "Generate visitor list report" claim.** The check had been commented out; staff roles that should see the report must have that claim.
- **Rotate the leaked secrets.** The certificate private key and the Gmail app password were in the public repository and remain in its history. Reissue the certificate and change the mail password.

## 6. Checking it is working

- Browse to `https://<app>.azurewebsites.net/`. The site should load.
- If the API fails to start, open **Log stream** and look for "Facets.Api cannot start". The message lists the missing settings.
- For more detail, check the log files in `%HOME%\LogFiles\Facets` (App Service → Advanced Tools → Debug console).
