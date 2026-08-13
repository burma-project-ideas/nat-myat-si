# NatMyatSi

| Setting | Value |
|---------|-------|
| **Application type** | Standalone Blazor WebAssembly |
| **Target framework** | .NET 10 |
| **Hosting** | Static files on Vercel |

## Rendering configuration

This repository is a client-only Blazor WebAssembly application. It has no ASP.NET Core host, server project, API endpoints, or database connection.

## Project structure

- `Pages/`: Routable pages and their code-behind files.
- `Components/`: Reusable UI components.
- `Models/`: JSON data contracts and application models.
- `Services/`: Browser-side services, including local static data loading.
- `wwwroot/`: Static assets published to Vercel, including the NatMyatSi JSON catalog.

## Data access

- Load NatMyatSi content only from the versioned JSON file in `wwwroot/data/`.
- Use the registered browser `HttpClient` for same-origin static assets only.
- Do not add external API calls, an ASP.NET Core host, or server-side data access.
- Services that load data must accept a `CancellationToken` and present generic user-facing error messages.

## UI guidance

- Use `$a11y-design` and `$minimalist-ui` for every user-facing page or component change.
- Target WCAG 2.2 AA with native HTML semantics, full keyboard access, visible focus, readable contrast, responsive reflow, and reduced-motion support.
- Keep the interface flat and editorial: warm monochrome colors, restrained accents, system fonts, light borders, and no gradients or heavy shadows.
- Use the San Francisco system-font stack on Apple platforms with native Windows system fonts as fallbacks.
- Keep English and Myanmar interface copy equivalent; mark Myanmar copy and source data with `lang="my"`.
- Reuse existing HTML, CSS, SVG assets, and test tooling before adding dependencies.

## Components

- Pass data down with `[Parameter]` and send actions up with `EventCallback<T>`.
- Do not mutate parameter values in child components.
- Use `IReadOnlyList<T>` for collection parameters and `@key` in rendered loops.
- Keep substantial component logic in `.razor.cs` code-behind files.
- Do not add JavaScript interop unless a browser feature cannot be implemented with Blazor and native HTML.

## Deployment

- `vercel.json` is the deployment contract.
- Vercel builds the application with the pinned SDK in `global.json` and publishes `dist/wwwroot`.
- Keep routes compatible with a static single-page application fallback.
