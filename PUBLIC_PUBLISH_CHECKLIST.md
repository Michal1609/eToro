# Public Publish Checklist

This repository is prepared for a clean public import under the `EToro` project name.

## Done in this pass

- Rename solution, source project, test project, namespaces, usings, and docs to `EToro`.
- Remove local task/refactor notes, generated `bin`/`obj` outputs, and committed test credentials.
- Keep only dummy appsettings templates with redacted error handling enabled.
- Update `.gitignore` for build outputs, IDE state, local secrets, API dumps, coverage, and scan reports.
- Add NuGet-ready package metadata to the library project.

## Before creating the public repository

- Create a new repository with no imported history.
- Copy only tracked project files, not `.git`, `.vs`, `.claude`, local `appsettings*.json`, or any `.env` files.
- Run a secret scan on the final folder before the first public push.
- Decide whether the public package id remains `grznarai.etoro`.
- Add repository URL metadata once the public URL exists.

## Next phase

- Add CI for restore, build, unit tests, and test result publishing.
- Add dependency and vulnerability scanning.
- Add package validation and automated NuGet package creation.
- Add release tagging and NuGet.org publishing rules.
