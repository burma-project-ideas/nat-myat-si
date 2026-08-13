# NatMyatSi

NatMyatSi is a bilingual, client-only .NET 10 Blazor WebAssembly app. It runs entirely in the browser and is designed for static hosting on Vercel.

The app has no ASP.NET Core host, API endpoint, account, database, or submitted question. Its 100 questions, 16 symbols, and 1,600 readings are versioned as a local JSON asset.

## Reading flow

1. Choose or search for one of the 100 questions.
2. Choose one position from a shuffled 4 × 4 covered grid.
3. Read the answer matched by the selected question and hidden symbol.
4. Choose a new question or reshuffle the same question.

## Run locally

```powershell
dotnet run
```

Open the HTTPS address shown in the terminal.

## Static data

The application reads `wwwroot/data/nat-myat-si.json` as a same-origin static asset. The catalog is validated in the browser before the reading flow starts:

- exactly 100 unique questions;
- exactly 16 unique symbols;
- one unique answer for every question-and-symbol pair.

## Vercel deployment

`vercel.json` installs the pinned .NET 10 SDK, publishes the standalone WebAssembly project, serves `dist/wwwroot`, and provides a single-page-app route fallback.

Import the repository into Vercel with the project root unchanged. No framework preset, serverless function, or environment variable is required.

## Accessibility and design

The interface uses native controls, bilingual English and Myanmar copy, visible keyboard focus, 44-pixel minimum targets, responsive reflow, reduced-motion support, and a high-contrast warm monochrome palette. The font stack uses San Francisco on Apple platforms and Segoe UI on Windows, with system fallbacks for Myanmar glyphs.
