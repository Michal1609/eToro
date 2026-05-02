# CI/CD pipeliny

Repozitář používá GitHub Actions pro validaci pull requestů, bezpečnostní kontroly, ověření balíčku a draft releasy.

Výchozí tok přispívání je:

```text
feature branch -> pull request -> povinné kontroly -> merge do main
```

Přímé pushe do `main` mají být blokované přes GitHub branch protection. Workflow soubory jsou v `.github/workflows/`, ale tato dokumentace je v `docs/`, aby byl pipeline kontrakt dostupný spolu se zbytkem dokumentace pro přispěvatele.

## CI

Workflow soubor: `.github/workflows/ci.yml`

Spouští se při:

- pull requestech mířících do `main`
- pushi do `main`
- ručním spuštění přes `workflow_dispatch`

Účel:

- obnovit NuGet balíčky
- sestavit `EToro.slnx` v konfiguraci `Release`
- považovat warningy za chyby
- spustit test suite
- nahrát `.trx` výsledky testů
- ověřit NativeAOT kompatibilitu na Linuxu
- vytvořit NuGet package artefakty pro kontrolu
- spustit Podman smoke test v oficiálním .NET SDK kontejneru

Hlavní build a test job běží na `ubuntu-latest` i `windows-latest`. Linux navíc provádí NativeAOT publish a vytvoření balíčku, protože výstup balíčku stačí ověřit jednou za běh.

Podman job je oddělený od OS matrixu. Ověřuje, že projekt jde otestovat v čistém kontejnerizovaném .NET SDK prostředí. U public repozitáře je to užitečné, protože přispěvatelé mohou používat různé lokální stroje, zatímco kontejner dává opakovatelný Linux baseline.

CI pouze vytváří package artefakty. Nepublikuje balíčky na NuGet.org.

## Secret Scan

Workflow soubor: `.github/workflows/secret-scan.yml`

Spouští se při:

- pull requestech mířících do `main`
- pushi do `main`
- týdenním plánovaném scanu
- ručním spuštění přes `workflow_dispatch`

Účel:

- proskenovat historii repozitáře přes Gitleaks
- zachytit omylem commitnuté API klíče, tokeny, lokální konfigurace a další secrets
- udržet public repozitář bezpečný pro externí přispěvatele

Checkout používá celou historii (`fetch-depth: 0`), aby Gitleaks nekontroloval jen poslední commit.

## CodeQL

Workflow soubor: `.github/workflows/codeql.yml`

Spouští se při:

- pull requestech mířících do `main`
- pushi do `main`
- týdenním plánovaném scanu
- ručním spuštění přes `workflow_dispatch`

Účel:

- spustit GitHub CodeQL analýzu pro C#
- zapsat bezpečnostní a code quality nálezy do GitHub code scanning
- zachytit běžné zranitelnostní vzory před mergem

Workflow používá `build-mode: none`. Pro tuto C# knihovnu je to vhodné, protože normální build už pokrývá CI a CodeQL umí analyzovat zdrojové kódy bez opakování celého buildu.

## Dependency Review

Workflow soubor: `.github/workflows/dependency-review.yml`

Spouští se při:

- pull requestech mířících do `main`

Účel:

- zkontrolovat změny NuGet a GitHub Actions závislostí přidané pull requestem
- shodit pull request, pokud přidává novou high-severity zranitelnou závislost
- zviditelnit riziko závislostí před mergem

Tento workflow je záměrně jen pro PR, protože dependency review porovnává pull request proti cílové větvi.

## Release

Workflow soubor: `.github/workflows/release.yml`

Spouští se při:

- tagách odpovídajících `v*.*.*`
- ručním spuštění přes `workflow_dispatch` se vstupním tagem

Účel:

- obnovit balíčky
- sestavit projekt v `Release`
- spustit testy
- vytvořit `.nupkg` a `.snupkg` artefakty
- nahrát package artefakty k workflow runu
- vytvořit draft GitHub Release s přiloženými package soubory

Release workflow aktuálně vytváří pouze draft GitHub Release. Nepublikuje na NuGet.org a nepotřebuje NuGet API key.

## Branch Protection

Nastavení repozitáře by mělo vyžadovat pull request před mergem do `main`.

Doporučené required checks:

- `Build and test (ubuntu-latest)`
- `Build and test (windows-latest)`
- `Podman smoke`
- `Gitleaks`
- `Analyze C#`
- `Review dependency changes`

Doporučené branch protection nastavení:

- vyžadovat pull request před mergem
- vyžadovat úspěšné status checks před mergem
- vyžadovat aktuální branch před mergem
- blokovat force push
- blokovat smazání větve
- zneplatnit starší approvals po pushi nových commitů
- vyžadovat review od code owners, až bude `CODEOWNERS` rozšířen za aktuální catch-all owner záznam

## NuGet publishing

Publish na NuGet.org záměrně není součástí aktuální pipeline.

Před jeho zapnutím přidejte samostatné release rozhodnutí a chráněný publish krok pomocí:

- environmentu s ručním schválením
- repository secretu typu `NUGET_API_KEY`
- triggeru pouze na tag
- package signing nebo provenance kontrol, pokud je bude release politika vyžadovat
