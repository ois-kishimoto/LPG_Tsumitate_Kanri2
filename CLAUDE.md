# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

LPG積立管理2 (LPG Tsumitate Kanri 2) — an ASP.NET Core 10.0 MVC web application for LPG accumulation management. Currently in early development (scaffolded from the default MVC template).

## Commands

```bash
# Build
dotnet build

# Run (HTTPS on https://localhost:7084, HTTP on http://localhost:5190)
dotnet run --project LPG_Tsumitate_Kanri2/LPG_Tsumitate_Kanri2.csproj

# Publish
dotnet publish
```

No test project exists yet.

## Architecture

Standard ASP.NET Core MVC pattern:

- **[Program.cs](LPG_Tsumitate_Kanri2/Program.cs)** — App entry point. Registers `AddControllersWithViews`, configures HTTPS redirection and HSTS (non-dev), and maps static assets.
- **[Controllers/](LPG_Tsumitate_Kanri2/Controllers/)** — Request handlers. Currently only `HomeController`.
- **[Models/](LPG_Tsumitate_Kanri2/Models/)** — ViewModels and domain models.
- **[Views/](LPG_Tsumitate_Kanri2/Views/)** — Razor (`.cshtml`) templates. Shared layout in `Views/Shared/_Layout.cshtml` uses Bootstrap 5.
- **[wwwroot/](LPG_Tsumitate_Kanri2/wwwroot/)** — Static assets served directly. Third-party libs (Bootstrap, jQuery, jQuery Validation) are vendored under `wwwroot/lib/`.
- **[appsettings.json](LPG_Tsumitate_Kanri2/appsettings.json)** / **[appsettings.Development.json](LPG_Tsumitate_Kanri2/appsettings.Development.json)** — Environment-specific configuration.

## Key Conventions

- Nullable reference types are enabled — avoid `#nullable disable` suppressions.
- Implicit usings are enabled — no need to add `using System;` etc. for standard types.
- Authorization middleware is wired but not yet configured; add policies/roles in `Program.cs` as features are built.
- No NuGet packages have been added yet beyond the SDK defaults.
