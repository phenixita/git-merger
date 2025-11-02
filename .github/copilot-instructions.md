## Purpose

Short, actionable instructions for AI coding agents working on this repo. Focus on the concrete patterns, files, and commands an agent needs to be immediately productive.

## Big picture (what edits are usually needed)
- This is a small .NET 9 CLI tool that copies the latest commit from every branch of a *source* Git repo into a *target* Git repo.
- Core logic lives in `GitMerger.Core` (see `GitMerger.Core/GitMerger.cs`). The console UI is in `GitMerger/Program.cs`.

## Important components & where to look
- Orchestration: `GitMerger.Core/GitMerger.cs` — iterates branches, checks out source/target branches, copies files, stages and commits.
- Copy abstraction: `GitMerger.Core/ICopyService.cs` — implement platform-specific copy logic here.
- Windows implementation: `GitMerger.Core/RobocopyService.cs` — launches `robocopy` and waits for exit. This explains the repo's Windows-only behaviour.
- App entrypoint / inputs: `GitMerger/Program.cs` — reads three Console inputs (source path, target path, subdir) and passes a hard-coded `Signature` to the core.
- Tests & expected behaviour: `GitMerger.Core.Tests/UnitTest1.cs` — shows how the library is exercised, plus `TestAssets/` directories used by tests.

## Concrete developer workflows (commands you'll run)
- Build solution: `dotnet build GitMerger.sln --configuration Release` (or omit `--configuration Release` for debug builds).
- Run CLI interactively: `cd GitMerger` then `dotnet run` (it prompts for source, target and subdir).
- Run compiled binary: after building Release, run `GitMerger/bin/Release/net9.0/GitMerger.dll` via `dotnet` or on Windows use `GitMerger.exe`.
- Run tests (from repo root): `dotnet test GitMerger.Core.Tests` — tests use relative `TestAssets` folders; run from solution root to match paths.

## Project-specific conventions & gotchas
- Platform: The default `ICopyService` uses `robocopy` (Windows). For cross-platform work implement a new `ICopyService` (e.g., using `System.IO` or `rsync`) and swap in `Program.cs`.
- Branch names: code removes `origin/` prefix when creating target branches (search for `Replace("origin/", "")` in `GitMerger.Core/GitMerger.cs`).
- Default branch mismatch: `Program.cs` passes `"main"` as the root branch name, while README and tests refer to `master`. Verify which default you want and keep it consistent.
- Commit behavior: code stages `*` and commits with message `Import <branch-name>`; commit exceptions are caught and written to Console (see `GitMerger.Core/GitMerger.cs`).
- Hardcoded identity: Program uses Signature with name/email `test`/`test@mail.eu` — update before production use.

## Integration points & dependencies
- Uses LibGit2Sharp for all Git operations (`Repository`, `Commands.Checkout`, `Commands.Stage`). See `GitMerger.Core/*` files.
- Robocopy is invoked as an external process in `RobocopyService` (so the tool requires that binary on PATH when used on Windows).
- Tests use MSTest and Moq; test assets live under `GitMerger.Core.Tests/TestAssets`.

## Typical small edits and where to implement them
- Add cross-platform copy: implement `ICopyService` and change `Program.cs` to instantiate the correct service.
- Add CLI args: replace Console.ReadLine usage in `Program.cs` with argument parsing and unit-testable entry points.
- Fix default branch: unify `main` vs `master` in `Program.cs`, README.md and tests.

## Quick pointers for an AI agent
- When modifying behavior, update examples in `README.md` and `GitMerger.Core.Tests` to reflect new defaults or copy semantics.
- Use `CloneBranch` in `GitMerger.Core/GitMerger.cs` as the authoritative sequence for copy/checkout/commit steps.
- Search the repo for `RobocopyService`, `ICopyService`, `Commands.Checkout`, and `Import ` to find the key integration points quickly.

## When submitting PRs
- Keep changes small and focused: change one area (copy strategy, CLI, or tests) per PR.
- If you change the default branch name or signature, update `README.md` and relevant tests in `GitMerger.Core.Tests`.

---
If anything here is unclear or you want more/less detail in any section, tell me which parts to expand (examples, file snippets, or a proposed CLI spec) and I will iterate.
