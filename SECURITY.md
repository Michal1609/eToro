# Security Policy

## Credentials

Do not commit real eToro `ApiKey` or `UserKey` values.

Use one of these mechanisms instead:

- .NET user secrets for local development.
- Environment variables such as `EToroOptions__ApiKey` and `EToroOptions__UserKey`.
- CI or production secret stores for deployed applications.

`appsettings.example.json` and `tests/EToro.Tests/appsettings.test.example.json` contain dummy values only.

## Reporting

Report security issues privately to the maintainer before opening a public issue.

## Supported Versions

Until the first stable public release, only the latest code on `main` is supported.
