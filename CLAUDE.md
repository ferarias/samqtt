# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Restore dependencies
dotnet restore Samqtt.sln

# Build
dotnet build Samqtt.sln --configuration Debug
dotnet build Samqtt.sln --configuration Release

# Run tests
dotnet test Samqtt.sln

# Run (Windows)
dotnet run --project src/Samqtt/Samqtt.csproj --framework net8.0-windows8.0

# Run (Linux/cross-platform)
dotnet run --project src/Samqtt/Samqtt.csproj --framework net8.0

# Publish (Windows self-contained single file)
dotnet publish src/Samqtt/Samqtt.csproj --configuration Release --framework net8.0-windows8.0 --runtime win-x64 --self-contained -p:PublishSingleFile=true

# Publish (Linux self-contained)
dotnet publish src/Samqtt/Samqtt.csproj --configuration Release --framework net8.0 --runtime linux-x64 --self-contained
```

`Directory.build.props` defines two custom MSBuild configurations: `Release-Windows` and `Release-Cross`.

Solution filters: `Samqtt.slnf` (cross-platform only), `Samqtt.Windows.slnf` (includes Windows projects).

## Architecture

SAMQTT is a .NET 8.0 Worker Service that bridges system sensors to an MQTT broker, with Home Assistant MQTT Discovery integration.

**Data flow:**
1. `SamqttBackgroundService` (in `Samqtt.Application`) starts and connects to the MQTT broker
2. Publishes Home Assistant discovery messages for all enabled sensors and actions
3. Subscribes to action topics via `MqttSubscriber`
4. Periodically polls enabled sensors (configurable interval, default 5s) and publishes readings via `MqttPublisher`
5. Responds to inbound MQTT messages by executing the corresponding action

**Project structure:**

| Project | Role |
|---|---|
| `Samqtt` | Entry point; dual-target (`net8.0` / `net8.0-windows8.0`), registers platform-specific services |
| `Samqtt.Application` | Core `SamqttBackgroundService` orchestrator |
| `Samqtt.Common` | Shared interfaces (`ISensor`, `IAction`, etc.) |
| `Samqtt.Broker.Mqtt2Net` | MQTT connectivity via MQTTNet (`MqttConnector`, `MqttPublisher`, `MqttSubscriber`) |
| `Samqtt.HomeAssistant` | MQTT Discovery message formatting (`HomeAssistantPublisher`, `HomeAssistantSensorValueFormatter`) |
| `Samqtt.SystemSensors` | Cross-platform sensors (CPU, memory, network, timestamp) + multi-sensor base for drive stats |
| `Samqtt.SystemSensors.Windows` | Windows-only sensor: `ComputerInUse` detector |
| `Samqtt.SystemActions` | Cross-platform actions (process management) |
| `Samqtt.SystemActions.Windows` | Windows-only actions: power states (hibernate/suspend/shutdown/reboot), toast notifications, exec |

**MQTT topic schema:** `samqtt/{hostname}/{sensor|action}/{metric}`

**User config locations:**
- Windows: `%LOCALAPPDATA%\SAMQTT\samqtt.appsettings.json`
- Linux: `/etc/mqtt/samqtt.appsettings.json`

Sensors and actions use a factory pattern — both cross-platform and platform-specific implementations register against the same interfaces in DI, with the entry point project conditionally registering Windows services via `#if WINDOWS` or runtime OS checks.

<!-- gitnexus:start -->
# GitNexus — Code Intelligence

This project is indexed by GitNexus as **samqtt** (528 symbols, 1193 relationships, 23 execution flows). Use the GitNexus MCP tools to understand code, assess impact, and navigate safely.

> If any GitNexus tool warns the index is stale, run `npx gitnexus analyze` in terminal first.

## Always Do

- **MUST run impact analysis before editing any symbol.** Before modifying a function, class, or method, run `gitnexus_impact({target: "symbolName", direction: "upstream"})` and report the blast radius (direct callers, affected processes, risk level) to the user.
- **MUST run `gitnexus_detect_changes()` before committing** to verify your changes only affect expected symbols and execution flows.
- **MUST warn the user** if impact analysis returns HIGH or CRITICAL risk before proceeding with edits.
- When exploring unfamiliar code, use `gitnexus_query({query: "concept"})` to find execution flows instead of grepping. It returns process-grouped results ranked by relevance.
- When you need full context on a specific symbol — callers, callees, which execution flows it participates in — use `gitnexus_context({name: "symbolName"})`.

## When Debugging

1. `gitnexus_query({query: "<error or symptom>"})` — find execution flows related to the issue
2. `gitnexus_context({name: "<suspect function>"})` — see all callers, callees, and process participation
3. `READ gitnexus://repo/samqtt/process/{processName}` — trace the full execution flow step by step
4. For regressions: `gitnexus_detect_changes({scope: "compare", base_ref: "main"})` — see what your branch changed

## When Refactoring

- **Renaming**: MUST use `gitnexus_rename({symbol_name: "old", new_name: "new", dry_run: true})` first. Review the preview — graph edits are safe, text_search edits need manual review. Then run with `dry_run: false`.
- **Extracting/Splitting**: MUST run `gitnexus_context({name: "target"})` to see all incoming/outgoing refs, then `gitnexus_impact({target: "target", direction: "upstream"})` to find all external callers before moving code.
- After any refactor: run `gitnexus_detect_changes({scope: "all"})` to verify only expected files changed.

## Never Do

- NEVER edit a function, class, or method without first running `gitnexus_impact` on it.
- NEVER ignore HIGH or CRITICAL risk warnings from impact analysis.
- NEVER rename symbols with find-and-replace — use `gitnexus_rename` which understands the call graph.
- NEVER commit changes without running `gitnexus_detect_changes()` to check affected scope.

## Tools Quick Reference

| Tool | When to use | Command |
|------|-------------|---------|
| `query` | Find code by concept | `gitnexus_query({query: "auth validation"})` |
| `context` | 360-degree view of one symbol | `gitnexus_context({name: "validateUser"})` |
| `impact` | Blast radius before editing | `gitnexus_impact({target: "X", direction: "upstream"})` |
| `detect_changes` | Pre-commit scope check | `gitnexus_detect_changes({scope: "staged"})` |
| `rename` | Safe multi-file rename | `gitnexus_rename({symbol_name: "old", new_name: "new", dry_run: true})` |
| `cypher` | Custom graph queries | `gitnexus_cypher({query: "MATCH ..."})` |

## Impact Risk Levels

| Depth | Meaning | Action |
|-------|---------|--------|
| d=1 | WILL BREAK — direct callers/importers | MUST update these |
| d=2 | LIKELY AFFECTED — indirect deps | Should test |
| d=3 | MAY NEED TESTING — transitive | Test if critical path |

## Resources

| Resource | Use for |
|----------|---------|
| `gitnexus://repo/samqtt/context` | Codebase overview, check index freshness |
| `gitnexus://repo/samqtt/clusters` | All functional areas |
| `gitnexus://repo/samqtt/processes` | All execution flows |
| `gitnexus://repo/samqtt/process/{name}` | Step-by-step execution trace |

## Self-Check Before Finishing

Before completing any code modification task, verify:
1. `gitnexus_impact` was run for all modified symbols
2. No HIGH/CRITICAL risk warnings were ignored
3. `gitnexus_detect_changes()` confirms changes match expected scope
4. All d=1 (WILL BREAK) dependents were updated

## Keeping the Index Fresh

After committing code changes, the GitNexus index becomes stale. Re-run analyze to update it:

```bash
npx gitnexus analyze
```

If the index previously included embeddings, preserve them by adding `--embeddings`:

```bash
npx gitnexus analyze --embeddings
```

To check whether embeddings exist, inspect `.gitnexus/meta.json` — the `stats.embeddings` field shows the count (0 means no embeddings). **Running analyze without `--embeddings` will delete any previously generated embeddings.**

> Claude Code users: A PostToolUse hook handles this automatically after `git commit` and `git merge`.

## CLI

| Task | Read this skill file |
|------|---------------------|
| Understand architecture / "How does X work?" | `.claude/skills/gitnexus/gitnexus-exploring/SKILL.md` |
| Blast radius / "What breaks if I change X?" | `.claude/skills/gitnexus/gitnexus-impact-analysis/SKILL.md` |
| Trace bugs / "Why is X failing?" | `.claude/skills/gitnexus/gitnexus-debugging/SKILL.md` |
| Rename / extract / split / refactor | `.claude/skills/gitnexus/gitnexus-refactoring/SKILL.md` |
| Tools, resources, schema reference | `.claude/skills/gitnexus/gitnexus-guide/SKILL.md` |
| Index, status, clean, wiki CLI commands | `.claude/skills/gitnexus/gitnexus-cli/SKILL.md` |
| Work in the Samqtt.Common area (33 symbols) | `.claude/skills/generated/samqtt-common/SKILL.md` |
| Work in the Samqtt.Application area (19 symbols) | `.claude/skills/generated/samqtt-application/SKILL.md` |
| Work in the Sensors area (12 symbols) | `.claude/skills/generated/sensors/SKILL.md` |
| Work in the Samqtt.HomeAssistant area (12 symbols) | `.claude/skills/generated/samqtt-homeassistant/SKILL.md` |
| Work in the Actions area (10 symbols) | `.claude/skills/generated/actions/SKILL.md` |
| Work in the Samqtt.SystemActions.Windows area (9 symbols) | `.claude/skills/generated/samqtt-systemactions-windows/SKILL.md` |
| Work in the SystemSensors area (5 symbols) | `.claude/skills/generated/systemsensors/SKILL.md` |
| Work in the Samqtt.Broker.Mqtt2Net area (3 symbols) | `.claude/skills/generated/samqtt-broker-mqtt2net/SKILL.md` |

<!-- gitnexus:end -->
