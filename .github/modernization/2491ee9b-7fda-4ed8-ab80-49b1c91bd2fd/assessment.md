# Phase 1 assessment — 2491ee9b-7fda-4ed8-ab80-49b1c91bd2fd

Checked 2026-10-28 on Windows_NT in `C:\workspace\repo`. Observations below precede source changes. Goal: move this synthetic pricing sample to .NET 10 without changing behavior, subsequently add one regression test, and produce exact build/test results with marker `acceptance-20261028191242-8eaf7f-windows-validated`. Scope is this repository only; no workflow, deployment, secret, push, or PR changes.

## Summary

- **Observed:** One SDK-style C# test project targets .NET 8; full Release build GREEN (zero warnings); full existing tests GREEN, **9 passed / 0 failed / 0 skipped** from TRX. Lint not configured (`Pricing.csproj`, `README.md:6-11`).
- **Recommendation:** In-place retarget of the single project to `net10.0`; characterize an uncovered behavior before retargeting, preserving existing assertions. Overall risk **low**, chiefly because the direct test-tool versions have not yet been tested here against `net10.0`. The installed SDK is 10.0.402, but installed runtime 10.0.13 is behind the vendor-listed latest 10.0.14.

## Inventory / stack inventory

| Unit | Path | Kind | Stack | Current version (evidence) | Candidate target |
|---|---|---|---|---|---|
| Pricing | `Pricing.csproj` | Single test project containing library code and tests | C#, .NET SDK-style, xUnit | `net8.0` (`Pricing.csproj:1-8`), xUnit 2.9.2 (`:13`) | `net10.0`, retain behavior and test discovery |
| Calculation API | `PriceCalculator.cs` | In-project implementation, no separate project | C# | integer-cent arithmetic and invariant receipt (`PriceCalculator.cs:25-69`) | Same public behavior |
| Tests | `PriceCalculatorTests.cs` | In-project tests | xUnit Fact/Theory (`PriceCalculatorTests.cs:7-48`) | 9 discovered cases | Existing 9 plus one later regression |

`git ls-files`: **5 tracked files**: 2 `.cs`, 1 `.csproj`, 1 `.md`, 1 `.gitignore`. No solution, project references, other languages, package-manager lockfiles, `global.json`, `Directory.*.props`, NuGet configuration, build wrappers, CI/workflow definitions, container/deployment files, or application configuration among tracked files. Project graph: one leaf `Pricing.csproj`, with both implementation and test code compiled together; no inter-project edges. `README.md:6-11` documents `dotnet build` and `dotnet test`. `.gitignore:1-3` ignores `bin/`, `obj/`, `TestResults/`. No setting keys in tracked configuration. A local ignored `.futuristic/gates` entry exists but is not tracked build/CI configuration.

Environment: `dotnet --info` reports Windows 10.0.26100, SDK **10.0.402**, installed SDKs 8.0.425/10.0.401/10.0.402, installed Microsoft.NETCore.App runtimes 8.0.31/10.0.12/10.0.13; no `global.json`. No Java/Node/Python project or runtime baseline needed. Docker client/server `docker version` both exit successfully (29.8.2; server Linux/amd64), but this suite has no service dependency.

## Version and support facts / support status

| Fact | Value | Source URL or local evidence | Checked on |
|---|---|---|---|
| Current project framework | `.NET 8` LTS, support ends **2026-11-10** (still supported on assessment date) | `Pricing.csproj:4`; [Microsoft .NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core) | 2026-10-28 |
| Target framework | `.NET 10` LTS, support ends **2028-11-14**; latest listed patch **10.0.14** | [Microsoft .NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core) | 2026-10-28 |
| Patch qualification | Vendor says remain on latest servicing patch; local .NET 10 runtime 10.0.13 and 8.0.31 lag vendor-listed 10.0.14 and 8.0.33 respectively | [Microsoft .NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core); `dotnet --info` | 2026-10-28 |
| Direct NuGet dependencies | No public end-of-support dates verified for the specific package versions; target compatibility of the full combination **unverified until a net10.0 build/test**. Runner page says .NET 6+; Test SDK package page displays computed net10.0 compatibility. | [Test SDK 17.11.1](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/17.11.1), [xUnit 2.9.2](https://www.nuget.org/packages/xunit/2.9.2), [runner 2.8.2](https://www.nuget.org/packages/xunit.runner.visualstudio/2.8.2) | 2026-10-28 |

Do not describe `.NET 8` as out of support yet. The machine's installed patch level is not proof of the patch level obtainable from public sources.

## Build, test and CI

Observed commands, executed unmodified against the project; durations from wall-clock PowerShell stopwatch. No validation defaults were supplied. README commands were adapted to specify the only project and Release configuration; the test run uses machine-readable TRX. `--no-restore` follows the recorded restore, and `--no-build` follows the recorded build.

| Command | Source | Exit code | Duration | Result |
|---|---|---:|---:|---|
| `dotnet restore Pricing.csproj --nologo` | prerequisite to README build | 0 | 7.57 s | Restored project |
| `dotnet build Pricing.csproj -c Release --no-restore --nologo` | README `dotnet build` | 0 | 8.86 s | Build succeeded; **0 warnings, 0 errors** |
| `dotnet test Pricing.csproj -c Release --no-build --logger trx --results-directory .modernization-scratch/baseline --nologo` | README `dotnet test`; baseline skill TRX | 0 | 3.10 s | TRX counters: total 9, executed 9, passed 9, failed 0, notExecuted/skipped 0; one test assembly, 109 ms test execution |

TRX artifact: `.modernization-scratch/baseline/ContainerUser_RUN-2491EE9B7FD_2026-10-28_23_00_24_net8.0.trx` (scratch is locally ignored via `.git/info/exclude`, not committed). **Failing tests: none; first error lines: none; flaky tests: none observed, not established by repeated runs**. No baseline rerun is required because the suite is green. Failure classification: not applicable. No CI systems or pinned CI runtime were found among tracked files; no lint command is configured.

**Proposed authoritative gates after each change (PowerShell on Windows_NT):** `dotnet build Pricing.csproj -c Release --nologo` (full build/implicit restore), then `dotnet test Pricing.csproj -c Release --no-build --logger trx --results-directory .modernization-scratch/validation --nologo` (full suite). Check TRX counters against baseline and require zero failures, no missing tests; after adding the regression expect at least 10 cases. For the final verdict run the same complete build and test gates and record exit codes, durations, warnings, TRX counts, and the requested acceptance marker in the final report. Lint: not configured; do not manufacture a narrower gate.

## Dependencies and vulnerabilities

`Pricing.csproj:11-15` has three direct NuGet references. `dotnet list Pricing.csproj package --include-transitive` exit 0 confirms the resolved versions. `dotnet list Pricing.csproj package --outdated --include-transitive` exit 0 lists current registry latests below. `dotnet list Pricing.csproj package --vulnerable --include-transitive` exit 0, NuGet source `https://api.nuget.org/v3/index.json`: **no vulnerable packages reported (critical 0, high 0, moderate 0, low 0; no advisories/fixed versions)**. This scanner result is not a guarantee of absence of undisclosed vulnerabilities.

| Package | Current | Target compatibility / maintenance evidence | Known vulnerabilities | Action |
|---|---:|---|---|---|
| `Microsoft.NET.Test.Sdk` | 17.11.1 | [NuGet package page](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/17.11.1) lists computed `net10.0` compatibility; latest reported **18.11.0**. No version-specific EOL confirmed. | None reported | Retain unless net10 test discovery requires update; validate |
| `xunit` | 2.9.2 | [NuGet package page](https://www.nuget.org/packages/xunit/2.9.2); latest reported **2.9.3**. Combination on target not yet validated; no version-specific EOL confirmed. | None reported | Retain unless incompatibility found |
| `xunit.runner.visualstudio` | 2.8.2 | [NuGet package page](https://www.nuget.org/packages/xunit.runner.visualstudio/2.8.2) describes support for .NET 6+; latest reported **4.0.1**. No version-specific EOL confirmed. | None reported | Retain unless discovery requires update |

Transitive outdated versions reported by the same command: `Microsoft.CodeCoverage` 17.11.1→18.11.0; `Microsoft.TestPlatform.ObjectModel` and `Microsoft.TestPlatform.TestHost` 17.11.1→18.11.0; `Newtonsoft.Json` 13.0.1→13.0.4; `System.Reflection.Metadata` 1.6.0→10.0.14; `xunit.analyzers` 1.16.0→2.2.0; `xunit.assert`, `xunit.core`, `xunit.extensibility.core`, `xunit.extensibility.execution` 2.9.2→2.9.3. Also resolved `xunit.abstractions` 2.0.3 (not flagged outdated). Specific deprecation/unmaintained status of these resolved versions **unverified**; do not conflate “outdated” with “deprecated”. No private registry/feed configuration found among tracked files; audit reported only the public NuGet source.

## Blockers and hotspots

| Pattern / area | Files affected | Example evidence | Migration approach |
|---|---:|---|---|
| Legacy .NET APIs (`System.Web`, `HttpContext.Current`, WCF, remoting, `BinaryFormatter`, `AppDomain.CreateDomain`, Windows Registry/EventLog, `System.Drawing`, COM, external `HintPath`) | 0 in tracked `.cs`/`.csproj` | `git grep -n -E` for these patterns, exit 1 (no matches) | None identified |
| Overflow and integer rounding | 1 | `PriceCalculator.cs:33-39,50-54` | Preserve unchecked arithmetic and rounding; characterize an edge case before edits |
| Receipt formatting/culture | 1 | `PriceCalculator.cs:57-69` uses `InvariantCulture`; tests assert ordinary example at `PriceCalculatorTests.cs:35-40` | Avoid formatting changes; preserve snapshot-like expectation |
| Test discovery coupled to a single project | 1 | `Pricing.csproj:8,11-15` | Retain test SDK/adapter behavior and assert TRX test count |

## Test services

Tests use in-memory `List<LineItem>` and xUnit only (`PriceCalculatorTests.cs:10-47`); no database, broker, cache, external HTTP, Testcontainers, compose, or service connection is referenced in tracked code. **Needed: none; missing: none.** Docker is available, and `DOCKER_HOST` and `TESTCONTAINERS_HOST_OVERRIDE` variable names are present (values deliberately not read or printed); no containers need to be started. If future tests require remote Docker services, use the override host, not localhost. No connection-string variables identified by name in the environment probe.

## Coverage and characterization candidates

No coverage report or coverage gate exists, so percentage coverage is **unverified**. Existing nine discovered cases cover subtotal, discount-code handling, rounding, receipt, and invalid quantities/prices (`PriceCalculatorTests.cs:9-47`). Candidate for exactly one later regression/characterization test: a boundary case for integer-cent rounding or unknown/whitespace discount behavior, asserted against today's unchanged code first; alternatively receipt output for an empty list (presently uncovered). Test `LineItem` constructor argument exception details and large-number overflow only if affected by the upgrade; avoid introducing a desired new behavior as if it were the baseline. Record the selected case and its pre-retarget passing result.

## Risks

| Risk | Severity | Likelihood | Evidence and mitigation |
|---|---|---|---|
| Test packages might alter discovery or fail for .NET 10 | medium | low | `Pricing.csproj:11-15` uses older packages. Build/test target first, require TRX count at least baseline nine; update narrowly only if necessary. |
| Untested arithmetic edge behavior may change accidentally | low | medium | `PriceCalculator.cs:33-69`; add one pre-change characterization test and preserve current behavior. |
| Local runtimes lag vendor's latest servicing patch | low | high | `dotnet --info` lists 10.0.13; vendor policy lists 10.0.14. Use a supported current patch when available; do not modify system runtime as part of this repository assessment. |
| No existing CI/lint safety net | low | medium | Only five tracked files; enforce full build/test gates in plan, document exact final results. |

## Recommended approach

In-place framework upgrade (one project, no deployment/consumer graph). Separate a passing pre-change characterization test from retargeting, then change only the target framework needed for `net10.0`, updating test packages only if full test discovery/build demonstrates necessity. Keep result logs local, never edit workflows/deployment, and finish with authoritative whole-project build/test and report containing `acceptance-20261028191242-8eaf7f-windows-validated`. Preserve public arithmetic, string output and all existing tests.

## Open questions

None requiring input: target `.NET 10`, one later regression test and repository scope are specified. Exact chosen regression case can be selected by the planner/executor from the observed behavior candidates. Package deprecation/support dates remain unverified; verify before proposing optional upgrades.

## Candidate tasks

1. **Characterize (pre-retarget):** add exactly one regression test for currently untested, observable pricing behavior; skill `test-baseline-and-characterization`; full build/test gates; verify test count increases from 9.
2. **Retarget:** change `Pricing.csproj` from `net8.0` to `net10.0` without modifying pricing behavior; skills `build-fix-loop`, `test-baseline-and-characterization`; full build/test gates and comparison of discovered tests. Only consider narrow package remediation if the gates reveal incompatibility.
3. **Final validate and report:** full Release build and entire suite on `net10.0`, audit and read-only diff review; skill `modernization-report`; record exact exit codes, durations, warning counts, TRX results and required acceptance marker. No workflow/deploy/PR work.
