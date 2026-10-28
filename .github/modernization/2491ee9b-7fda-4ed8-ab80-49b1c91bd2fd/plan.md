# Goal and target

Modernize the single synthetic `Pricing.csproj` project from `net8.0` to `net10.0`, preserve pricing behavior, run all existing tests, add exactly one regression test, and report exact final build/test results with marker `acceptance-20261028191242-8eaf7f-windows-validated`.

# Approach

The assessment records a green Release baseline: build with zero warnings and **9 passed, 0 failed, 0 skipped** in TRX. Add one test against unchanged .NET 8 code first; then retarget the one project; then repeat authoritative full-project validation and write the report. Each task uses the same full build and test gates, with TRX confirming 10 discovered and passing tests after T001. Do not weaken existing tests or validation.

# Decisions

- **Target:** `net10.0`, replacing `net8.0` in `Pricing.csproj:4`. The assessment's [Microsoft .NET support-policy evidence](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core), checked 2026-10-28, identifies .NET 8 as supported through 2026-11-10 and .NET 10 as LTS through 2028-11-14. The assessment records installed SDK 10.0.402.
- **Characterization:** One `[Fact]` asserting that an empty list produces `0 item(s), discount 0.00, total 0.00`. `PriceCalculator.cs:50-69` supports this current result; `PriceCalculatorTests.cs:35-40` covers a nonempty receipt but not an empty one. Confirm it passes before retargeting.
- **Dependencies:** Initially retain `Microsoft.NET.Test.Sdk` 17.11.1, `xunit` 2.9.2, and `xunit.runner.visualstudio` 2.8.2 (`Pricing.csproj:11-15`). Their combined .NET 10 build and discovery behavior remains unverified until T002's gates. Do not make an optional package-upgrade task.

# Assumptions

- The assessment found one project, no CI or lint configuration, no test services, and no baseline failure requiring stabilization.
- `.modernization-scratch/` remains local and ignored; TRX files and raw logs are not committed.
- Gates run non-interactively from the repository root on Windows_NT.

# Out of scope

Other projects or samples, workflow edits, deployment, secrets, system-runtime installation, optional dependency upgrades, pushing, and opening a PR. If reaching the goal requires materially broader work, stop and seek approval rather than expanding scope.

# Tasks

### T001: Add one pre-retarget regression test
- Depends on: -
- Skills: test-baseline-and-characterization, build-fix-loop
- Validation:
  - build: `dotnet build Pricing.csproj -c Release --nologo`
  - test: `dotnet test Pricing.csproj -c Release --no-build --logger trx --results-directory .modernization-scratch/validation --nologo`
- Characterization: Add exactly one empty-receipt `[Fact]` to `PriceCalculatorTests.cs`, asserting `0 item(s), discount 0.00, total 0.00` against unchanged `net8.0` code.
- Risk: low (an asserted edge case could misstate current behavior); mitigate by running it before any retarget and checking TRX.
- Done when: The original nine cases and the one new case pass—10 passed, 0 failed, 0 skipped—with no production-code or existing-test changes.

Keep the test as a permanent regression check. Record its pre-retarget result and compare discovery with the assessment's nine-case baseline.

### T002: Retarget Pricing to .NET 10
- Depends on: T001
- Skills: build-fix-loop, test-baseline-and-characterization
- Validation:
  - build: `dotnet build Pricing.csproj -c Release --nologo`
  - test: `dotnet test Pricing.csproj -c Release --no-build --logger trx --results-directory .modernization-scratch/validation --nologo`
- Characterization: Preserve and rerun T001's passing empty-receipt test and all nine existing cases.
- Risk: medium (existing test tools may fail or alter discovery on .NET 10); mitigate with the full gates and TRX count comparison. Review the focused project diff and any necessary compatibility fix.
- Done when: `Pricing.csproj` targets `net10.0`, the full build succeeds, and TRX reports 10 passed, 0 failed, 0 skipped without changed pricing behavior or weakened tests.

Change the target framework only initially. If a package incompatibility is demonstrated, diagnose the root cause; keep any necessary remediation narrowly scoped and separately reviewable. Stop and seek approval if the required change materially expands the agreed scope.

### T003: Final validation and report
- Depends on: T001, T002
- Skills: modernization-report, test-baseline-and-characterization
- Validation:
  - build: `dotnet build Pricing.csproj -c Release --nologo`
  - test: `dotnet test Pricing.csproj -c Release --no-build --logger trx --results-directory .modernization-scratch/validation --nologo`
- Characterization: Confirm the single added case remains present and that TRX discovers all 10 cases.
- Risk: low (a stale or miscounted result could misrepresent completion); mitigate by collecting final gate results after the last code change and reading TRX counters.
- Done when: The final full build and test gates pass, a read-only diff confirms scope, and `.github/modernization/2491ee9b-7fda-4ed8-ab80-49b1c91bd2fd/final-report.md` records actual exit codes, durations, warning count, TRX totals, baseline comparison, and the exact marker `acceptance-20261028191242-8eaf7f-windows-validated`.

Use the assessment, task progress notes, final gate evidence, and repository diff for the report. State any unverified result as unverified; do not substitute the baseline run for final validation.

# Risks

Test-adapter compatibility and test discovery are the principal migration risks; passing 10/10 TRX cases is required, not merely a successful test-command exit. The assessment also notes that installed .NET 10 runtime 10.0.13 trails the vendor-listed 10.0.14 patch; document that limitation without changing the machine or claiming it affects the verified repository gates.
