# Copilot Instructions

## Code Comments

- Do not add comments to generated code unless the code does something unintuitive or requires explanation.
- Never remove existing comments from the code, unless they are incorrect or misleading. If you find an existing comment that is wrong, update it to be correct rather than deleting it.

## Terminal

- The terminal is **PowerShell**. Never use bash-only commands such as `grep`, `tail`, `cat` with heredoc syntax, etc. Use PowerShell equivalents (`Select-String`, `Select-Object`, `Set-Content`, etc.).

## CSS

- Prefer scoped `.razor.css` files co-located with the component over adding styles to `app.css`. Only use `app.css` for truly global styles (resets, typography, layout, utilities).
- Use `768px` as the single breakpoint for media queries unless explicitly instructed otherwise.
- Write CSS with desktop/wider view as the default, and treat mobile styles as the exception via `@media (max-width: 768px)`.
- Keep media-query style and breakpoint usage consistent across all CSS files.

## SVG Icons

- Centralize SVG icons in `Shared/IconCatalog.cs`.
- Avoid inline SVG markup in `.razor` files unless there is a clear technical reason not to use `IconCatalog`.

## Development Server

- The application is almost always already running via `run.bat` in a terminal. Avoid starting a new instance unless you have confirmed none is running.

## Language Conventions

- Keep all technical identifiers in English (e.g., variable names, method names, class names, component names, and file names).
- Keep all user-facing display text in Swedish (e.g., labels, headings, button text, validation messages, and other UI content).
