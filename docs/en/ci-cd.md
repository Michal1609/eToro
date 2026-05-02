# CI/CD Pipelines

This repository uses GitHub Actions for pull request validation, security checks, package verification, and draft releases.

The default contribution flow is:

```text
feature branch -> pull request -> required checks -> merge to main
```

Direct pushes to `main` should be blocked by GitHub branch protection. The workflow files live in `.github/workflows/`, but this page is kept in `docs/` so contributors can find the pipeline contract together with the rest of the project documentation.

## CI

Workflow file: `.github/workflows/ci.yml`

Runs on:

- pull requests targeting `main`
- pushes to `main`
- manual `workflow_dispatch`

Purpose:

- restore NuGet packages
- build `EToro.slnx` in `Release`
- treat warnings as errors
- run the test suite
- upload `.trx` test results
- validate NativeAOT compatibility on Linux
- create NuGet package artifacts for inspection
- run a Podman smoke test inside the official .NET SDK container

The CI workflow runs on both `ubuntu-latest` and `windows-latest` for the main build and test job. Linux additionally performs the NativeAOT publish and package creation because those checks are enough to validate package output once per run.

The Podman job is separate from the OS matrix. It verifies that the project can be tested inside a clean containerized .NET SDK environment. This is useful for a public repository because contributors may use different local machines, while the container gives a repeatable Linux baseline.

CI only packs artifacts. It does not publish packages to NuGet.org.

## Secret Scan

Workflow file: `.github/workflows/secret-scan.yml`

Runs on:

- pull requests targeting `main`
- pushes to `main`
- a weekly scheduled scan
- manual `workflow_dispatch`

Purpose:

- scan the repository history with Gitleaks
- catch accidentally committed API keys, tokens, local config values, and other secrets
- keep the public repository safe for external contributors

The checkout uses full history (`fetch-depth: 0`) so Gitleaks can inspect more than just the latest commit.

## CodeQL

Workflow file: `.github/workflows/codeql.yml`

Runs on:

- pull requests targeting `main`
- pushes to `main`
- a weekly scheduled scan
- manual `workflow_dispatch`

Purpose:

- run GitHub CodeQL analysis for C#
- report security and code quality findings to GitHub code scanning
- catch common vulnerability patterns before changes are merged

The workflow uses `build-mode: none`, which is suitable for this C# library because the normal build is already covered by CI and CodeQL can analyze the source without repeating the full build.

## Dependency Review

Workflow file: `.github/workflows/dependency-review.yml`

Runs on:

- pull requests targeting `main`

Purpose:

- review NuGet and GitHub Actions dependency changes introduced by a pull request
- fail the pull request when a new high-severity vulnerable dependency is introduced
- make dependency risk visible before merge

This workflow is intentionally PR-only because dependency review compares the pull request against the target branch.

## Release

Workflow file: `.github/workflows/release.yml`

Runs on:

- tags matching `v*.*.*`
- manual `workflow_dispatch` with a tag input

Purpose:

- restore packages
- build in `Release`
- run tests
- create `.nupkg` and `.snupkg` artifacts
- upload package artifacts to the workflow run
- create a draft GitHub Release with the package files attached

The release workflow currently creates a draft GitHub Release only. It does not publish to NuGet.org and it does not require a NuGet API key.

## Branch Protection

The repository settings should require pull requests before merging into `main`.

Recommended required checks:

- `Build and test (ubuntu-latest)`
- `Build and test (windows-latest)`
- `Podman smoke`
- `Gitleaks`
- `Analyze C#`
- `Review dependency changes`

Recommended branch protection settings:

- require a pull request before merging
- require status checks to pass before merging
- require branches to be up to date before merging
- block force pushes
- block branch deletion
- dismiss stale approvals when new commits are pushed
- require review from code owners when CODEOWNERS is expanded beyond the current catch-all owner

## NuGet Publishing

Publishing to NuGet.org is intentionally not part of the current pipeline.

Before enabling it, add a dedicated release decision and protect the publish step with:

- an environment requiring manual approval
- a repository secret such as `NUGET_API_KEY`
- a tag-only trigger
- package signing or provenance checks if required by the release policy
